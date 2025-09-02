using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AtomicApps.Tools;
using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VInspector;
using Zenject;
using static AtomicApps.UIConstants;


namespace AtomicApps.Infrastructure.Services.Popups.AssetsLoaders
{
    public class InScenePopupAssetsLoader : MonoBehaviour, IPopupAssetsLoader
    {
        [HelpBox(
            @"SETUP INSTRUCTION
1. Assign all popups to popups serialized list or click isAutoFillPopups and it will be done automatically.
2. Every popup prefab must contain Popup (or inherited) component.
3. Click Generate Constants to prepare PopupKeys for using. It's a list of popups names stored as constants for using with PopupService. 
4. Every time you add or remove popups click Generate Constants."
            , HelpBoxMessageType.Info)]
        [Space]
        
        [SerializeField]
        private List<BasePopup> popups;
        [SerializeField]
        private bool isAutoFillPopups = true;

        private DiContainer _diContainer;

        public void UpdateDiContainer(DiContainer newContainer)
        {
            _diContainer = newContainer;
        }

        public void DisposePopup(BasePopup basePopup)
        {
            basePopup.gameObject.SetActive(false);
        }

        public async UniTask<TAssetType> LoadPopupAsync<TAssetType>(string popupId, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver) where TAssetType : BasePopup
        {
            var popupComponent = await GetRequiredPopupFromScene<TAssetType>(popupId, option);
            
            if (popupComponent == null)
            {
                return null;
            }
            
            popupComponent?.Init(popupId);
            return popupComponent;
        }

        private async UniTask<TAssetType> GetRequiredPopupFromScene<TAssetType>(string type, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver) where TAssetType : BasePopup
        {
            var popup = popups.FirstOrDefault(p => p.name == type);
            
            if (popup != default)
            {
                TAssetType popupComponent = popup.GetComponent<TAssetType>();
                if (popupComponent != null)
                {
                    if (option == UIConstants.PopupShow.ShowOver)
                    {
                        popup.transform.SetSiblingIndex(popup.transform.parent.childCount - 1);
                    }
                    return popupComponent;
                }
                else
                {
                    Debug.LogError(
                        $"PopupsService can't find {typeof(TAssetType)} component on {type} prefab",
                        gameObject);
                }
            }
            else
            {
                Debug.LogError(
                    $"PopupsService can't load {type} from scene",
                    gameObject);
            }

            return null;
        }

        #region Editor
        #if UNITY_EDITOR
        [Button]
        public void FindAllPopups()
        {
            popups = transform.root.gameObject.GetComponentsInChildren<BasePopup>(true).ToList();
            GenerateConstants();
        }
        
        private void OnValidate()
        {
            if (isAutoFillPopups)
            {
                popups = transform.root.gameObject.GetComponentsInChildren<BasePopup>(true).ToList();
            }
        }
        
        [Button]
        private void GenerateConstants()
        {
            var folderPath = GetFilePath();
            const string className = "PopupKeys";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-Generated Sound Keys. DO NOT MODIFY");
            sb.AppendLine("public static class " + className);
            sb.AppendLine("{");

            foreach (var popup in popups)
            {
                if (!string.IsNullOrWhiteSpace(popup.name))
                {
                    var keyName = ToConstantFormat(popup.name);
                    sb.AppendLine($"    public const string {keyName} = \"{popup.name}\";");
                }
            }

            sb.AppendLine("}");

            var filePath = Path.Combine(folderPath, $"{className}.cs");
            File.WriteAllText(filePath, sb.ToString());
            Debug.Log($"Sound constants class generated at {filePath}");
        }
        
        private string ToConstantFormat(string input)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                {
                    result.Append('_');
                }
                result.Append(char.ToUpper(input[i]));
            }
            return result.ToString();
        }
        
        private string GetFilePath()
        {
            var scriptFileName = nameof(InScenePopupAssetsLoader) + ".cs";
            var scriptFiles = Directory.GetFiles(Application.dataPath, scriptFileName, SearchOption.AllDirectories);
            if (scriptFiles.Length == 0)
            {
                return "Assets/Code/Scripts/Generated";
            }

            return Path.GetDirectoryName(scriptFiles[0]);
        }
        #endif
        #endregion
    }
}
