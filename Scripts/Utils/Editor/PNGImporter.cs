#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AtomicApps.Utils
{
    #if UNITY_EDITOR
    public class PNGImporter : AssetPostprocessor
    #else
    public class PNGImporter
    #endif
    {
        #if UNITY_EDITOR
        void OnPreprocessTexture()
        {
            if (assetPath.EndsWith(".png"))
            {
                TextureImporter importer = (TextureImporter)assetImporter;

                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.SaveAndReimport();
                }
            }
        }
        #endif
    }
}