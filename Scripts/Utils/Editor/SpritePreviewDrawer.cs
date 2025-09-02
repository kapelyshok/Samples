using UnityEditor;
using UnityEngine;

namespace AtomicApps.Utils.Editor
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the property field (for the Sprite reference)
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label);

            // Only draw the preview if the property has a valid Sprite
            if (property.objectReferenceValue is Sprite sprite)
            {
                // Get the attribute to determine preview size
                SpritePreviewAttribute previewAttribute = (SpritePreviewAttribute)attribute;

                // Calculate the sprite's aspect ratio
                float spriteAspect = sprite.rect.width / sprite.rect.height;

                // Calculate the preview rectangle dimensions dynamically
                float previewWidth = previewAttribute.width;
                float previewHeight = previewWidth / spriteAspect;

                // Ensure the preview height does not exceed the maximum specified height
                if (previewHeight > previewAttribute.height)
                {
                    previewHeight = previewAttribute.height;
                    previewWidth = previewHeight * spriteAspect;
                }

                // Calculate the position for the sprite preview
                Rect previewRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2,
                    previewWidth,
                    previewHeight
                );

                // Draw the sprite's texture (adjust for padding)
                if (sprite.texture != null)
                {
                    Texture2D texture = sprite.texture;
                    Rect textureRect = sprite.textureRect;
                    Rect uv = new Rect(
                        textureRect.x / texture.width,
                        textureRect.y / texture.height,
                        textureRect.width / texture.width,
                        textureRect.height / texture.height
                    );

                    GUI.DrawTextureWithTexCoords(previewRect, texture, uv);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Default height plus space for the sprite preview
            if (property.objectReferenceValue is Sprite sprite)
            {
                SpritePreviewAttribute previewAttribute = (SpritePreviewAttribute)attribute;

                // Calculate the sprite's aspect ratio
                float spriteAspect = sprite.rect.width / sprite.rect.height;

                // Calculate the dynamic height of the preview
                float previewHeight = previewAttribute.width / spriteAspect;

                // Ensure the preview height does not exceed the maximum specified height
                if (previewHeight > previewAttribute.height)
                {
                    previewHeight = previewAttribute.height;
                }

                return EditorGUIUtility.singleLineHeight + previewHeight + 4; // Extra space for preview
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
