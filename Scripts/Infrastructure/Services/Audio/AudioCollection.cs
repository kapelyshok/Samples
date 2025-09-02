using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VInspector;

namespace AtomicApps.Infrastructure.Services.Audio
{
    [CreateAssetMenu(menuName = "Game/Audio SFX Collection")]
    public class AudioCollection : ScriptableObject
    {
        [SerializeField] private List<SoundMapping> sounds;
        [SerializeField] private List<AudioClip> soundsBuffer;

        public SoundMapping GetMappingByKey(string key)
        {
            return sounds.FirstOrDefault(s => s.Key == key);
        }

#if UNITY_EDITOR
        [Button]
        private void AddSoundsFromBuffer()
        {
            foreach (var audio in soundsBuffer)
            {
                var mapping = new SoundMapping();
                mapping.Key = NormalizeAudioID(audio.name);
                mapping.Clip = audio;
                sounds.Add(mapping);
            }
            
            soundsBuffer.Clear();
        }
        [Button]
        private void SetAllKeysAsClipsNames()
        {
            foreach (var mapping in sounds)
            {
                mapping.Key = NormalizeAudioID(mapping.Clip.name);
            }
        }
        [Button]
        private void GenerateConstants()
        {
            var folderPath = GetFilePath();
            const string className = "SoundKeys";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-Generated Sound Keys. DO NOT MODIFY");
            sb.AppendLine("public static class " + className);
            sb.AppendLine("{");

            foreach (var sound in sounds)
            {
                if (!string.IsNullOrWhiteSpace(sound.Key))
                {
                    var keyName = ToConstantFormat(NormalizeAudioID(sound.Key));
                    sb.AppendLine($"    public const string {keyName} = \"{sound.Key}\";");
                }
            }

            sb.AppendLine("}");

            var filePath = Path.Combine(folderPath, $"{className}.cs");
            File.WriteAllText(filePath, sb.ToString());
            Debug.Log($"Sound constants class generated at {filePath}");
        }

        private string NormalizeAudioID(string id)
        {
            return id.ToLower().Replace(" ", "_");
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
            var scriptFileName = nameof(AudioCollection) + ".cs";
            var scriptFiles = Directory.GetFiles(Application.dataPath, scriptFileName, SearchOption.AllDirectories);
            if (scriptFiles.Length == 0)
            {
                return "Assets/Code/Scripts/Generated";
            }

            return Path.GetDirectoryName(scriptFiles[0]);
        }
#endif
    }
}