using System.IO;
using System.Text;
using AtomicApps.Tools;
using AtomicApps.UI.Popups;
using Cysharp.Threading.Tasks;
using AtomicApps.Utils;
using VInspector;
using Zenject;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
#endif

using static AtomicApps.UIConstants;


namespace AtomicApps.Infrastructure.Services.Popups.AssetsLoaders
{
    public class AddressablePopupAssetsLoader : MonoBehaviour, IPopupAssetsLoader
    {
        private const string PATH = "Popups/";
        
        [HelpBox(
            @"SETUP INSTRUCTION
1. In Addressable assets system give all popups prefabs address Popups/ActualPopupName.
2. Every popup prefab must contain Popup (or inherited) component.
3. Click Generate Constants to prepare PopupKeys for using. It's a list of popups names stored as constants for using with PopupService. 
4. Every time you add or remove popups click Generate Constants.
5. Assign parent for future popups to content variable.", HelpBoxMessageType.Info)]
        [Space]
        
        [SerializeField]
        private Transform content;
        
        private DiContainer _diContainer;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public void UpdateDiContainer(DiContainer newContainer)
        {
            _diContainer = newContainer;
        }

        public void DisposePopup(BasePopup basePopup)
        {
            var result = Addressables.ReleaseInstance(basePopup.gameObject);
            
            if (!result && gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        
        public async UniTask<TAssetType> LoadPopupAsync<TAssetType>(string popupId, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver) where TAssetType : BasePopup
        {
            var popupComponent = await LoadPopupFromAddressableAsync<TAssetType>(popupId, option);

            if (popupComponent == null)
            {
                return null;
            }
            
            popupComponent.Init(popupId);

            _diContainer.InjectGameObjectWithChildren(popupComponent.gameObject);

            return popupComponent;
        }
        
        private async UniTask<TAssetType> LoadPopupFromAddressableAsync<TAssetType>(string type, UIConstants.PopupShow option = UIConstants.PopupShow.ShowOver) where TAssetType : BasePopup
        {
            string popupAddress = $"{PATH}{type}";

            var instanceHandle = Addressables.InstantiateAsync(popupAddress, Vector3.zero,
                Quaternion.identity, content);
            
            await instanceHandle;

            if (instanceHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject newPopupObject = instanceHandle.Result;
                newPopupObject.transform.localPosition = Vector3.zero;

                if (newPopupObject != null && instanceHandle.Result is GameObject)
                {
                    TAssetType popupComponent = newPopupObject.GetComponent<TAssetType>();
                    if (popupComponent != null)
                    {
                        newPopupObject.name = type.ToString();

                        return popupComponent;
                    }
                    else
                    {
                        Debug.LogError(
                            $"PopupsService can't find Popup component on {popupAddress} prefab",
                            gameObject);
                    }
                }
                else
                {
                    Debug.LogError(
                        $"PopupsService instanceHandle.Result isn't GameObject or null! popupAddress : {popupAddress}",
                        gameObject);
                }
            }
            else
            {
                Debug.LogError(
                    $"PopupsService can't load {popupAddress} from addressable",
                    gameObject);
            }

            return null;
        }
        
        #if UNITY_EDITOR
        [Button]
        [MenuItem("Unavinar/Generate All Popup ID")]
        private static void GenerateConstants()
        {
            var folderPath = GetFilePath();
            const string className = "PopupKeys";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-Generated Popup Keys. DO NOT MODIFY");
            sb.AppendLine("public static class " + className);
            sb.AppendLine("{");
            
            string assetsFolderPath = Application.dataPath + "/Addressables/Popups"; // Full path to the folder

            if (Directory.Exists(assetsFolderPath))
            {
                // Get all .prefab files in the directory
                string[] files = Directory.GetFiles(assetsFolderPath, "*.prefab");

                if (files.Length <= 0)
                {
                    Debug.LogError($"0 popups found in {assetsFolderPath} path");
                }

                foreach (string file in files)
                {
                    // Get the file name without the path and extension
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    
                    var keyName = ToConstantFormat(fileName);
                    sb.AppendLine($"    public const string {keyName} = \"{fileName}\";");
                }

                sb.AppendLine("}");

                var filePath = Path.Combine(folderPath, $"{className}.cs");
                File.WriteAllText(filePath, sb.ToString());
                Debug.Log($"Popup constants class generated at {filePath}");
            }
            else
            {
                Debug.LogError($"Directory not found: {assetsFolderPath}");
            }
            
            AssignPopupKeysToAddressable();
        }
        
        private static string ToConstantFormat(string input)
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
        
        private static string GetFilePath()
        {
            var scriptFileName = nameof(AddressablePopupAssetsLoader) + ".cs";
            var scriptFiles = Directory.GetFiles(Application.dataPath, scriptFileName, SearchOption.AllDirectories);
            if (scriptFiles.Length == 0)
            {
                return "Assets/Code/Scripts/Generated";
            }

            return Path.GetDirectoryName(scriptFiles[0]);
        }
        
        private static void AssignPopupKeysToAddressable()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings not found.");
                return;
            }

            var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");

            foreach (var guid in prefabGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (!assetPath.Contains("Popups")) continue;

                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var newKey = $"Popups/{fileName}";

                var entry = settings.FindAssetEntry(guid);

                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                }

                entry.address = newKey;
                Debug.Log($"Set key: {newKey} for asset at {assetPath}");
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("All Popup keys assigned successfully.");
        }
        #endif
    }
}
