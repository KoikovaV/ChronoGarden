
using UnityEngine;

public static class ObjectFactory
{
    private static Sprite squareSprite;

    private static Sprite GetSquareSprite()
    {
        if (squareSprite != null)
            return squareSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        squareSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        return squareSprite;
    }

    public static GameObject CreateRectangle(
        string objectName,
        Transform parent,
        Vector2 localPosition,
        Vector2 size,
        Color color,
        bool solid = false,
        int sortingOrder = 0)
    {
        GameObject obj = new GameObject(objectName);

        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale =
            new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer =
            obj.AddComponent<SpriteRenderer>();

        renderer.sprite = GetSquareSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        if (solid)
        {
            obj.AddComponent<BoxCollider2D>();
        }

        return obj;
    }

    public static void Resize(
        GameObject obj,
        Vector2 localPosition,
        Vector2 size)
    {
        obj.transform.localPosition = localPosition;
        obj.transform.localScale =
            new Vector3(size.x, size.y, 1f);
    }

    public static void SetColor(
        GameObject obj,
        Color color)
    {
        SpriteRenderer renderer =
            obj.GetComponent<SpriteRenderer>();

        if (renderer != null)
            renderer.color = color;
    }
}
