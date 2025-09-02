using UnityEngine;

public class SpritePreviewAttribute : PropertyAttribute
{
    public float width;
    public float height;

    public SpritePreviewAttribute(float width = 64f, float height = 64f)
    {
        this.width = width;
        this.height = height;
    }
}
