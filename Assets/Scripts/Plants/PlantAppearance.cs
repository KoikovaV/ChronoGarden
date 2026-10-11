
using UnityEngine;

public class PlantAppearance : MonoBehaviour
{
    [Header("Изображения растения")]
    public Sprite sproutSprite;
    public Sprite youngSprite;
    public Sprite matureSprite;
    public Sprite oldSprite;

    [Header("Смещение изображения")]
    public Vector2 bambooOffset = Vector2.zero;

    [Header("Порядок отображения")]
    public int sortingOrder = 5;

    private PlantBase plant;
    private SpriteRenderer artwork;

    private int previousAge = -1;

    private void Start()
    {
        plant = GetComponent<PlantBase>();

        if (plant == null)
        {
            Debug.LogError(
                "На объекте отсутствует PlantBase!"
            );

            enabled = false;
            return;
        }

        if (sproutSprite == null &&
            youngSprite == null &&
            matureSprite == null &&
            oldSprite == null)
        {
            Debug.LogWarning(
                "Не назначены изображения растения."
            );

            enabled = false;
            return;
        }

        // Скрываем старые прямоугольные изображения.
        // Физические коллайдеры при этом сохраняются.
        SpriteRenderer[] oldRenderers =
            GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in oldRenderers)
        {
            renderer.enabled = false;
        }

        // Создаём отдельный объект с новой графикой.
        GameObject artObject =
            new GameObject("PlantArtwork");

        artObject.transform.SetParent(
            transform,
            false
        );

        artwork =
            artObject.AddComponent<SpriteRenderer>();

        artwork.sortingOrder = sortingOrder;

        UpdateArtwork();
    }

    private void LateUpdate()
    {
        if (plant == null || artwork == null)
            return;

        if (previousAge != plant.Age)
        {
            UpdateArtwork();
        }
    }

    private Sprite GetSpriteForAge()
    {
        int age = plant.Age;

        switch (plant.Kind)
        {
            case PlantKind.Bamboo:
                if (age == 0)
                    return sproutSprite != null
                        ? sproutSprite : youngSprite;

                if (age <= 2)
                    return youngSprite;

                if (age <= 5)
                    return matureSprite;

                return oldSprite;

            case PlantKind.Vine:
                if (age <= 3)
                    return youngSprite;

                if (age <= 7)
                    return matureSprite;

                return oldSprite;

            case PlantKind.Fern:
                if (age <= 2)
                    return youngSprite;

                if (age <= 4)
                    return matureSprite;

                return oldSprite;
        }

        return youngSprite;
    }

    private void UpdateArtwork()
    {
        if (plant == null || artwork == null)
            return;

        previousAge = plant.Age;

        Sprite selected = GetSpriteForAge();

        if (selected == null)
        {
            selected = matureSprite != null
                ? matureSprite : youngSprite;
        }

        if (selected == null)
            return;

        artwork.sprite = selected;

        Vector2 spriteSize = selected.bounds.size;

        if (spriteSize.x <= 0 || spriteSize.y <= 0)
            return;

        Transform art = artwork.transform;

        // Бамбук растёт вертикально.

        if (plant is BambooPlant bamboo)
        {
            // Высота физической верхушки бамбука.
            float targetHeight = bamboo.Height + 0.5f;

            // Реальные границы выбранного спрайта.
            Bounds bounds = selected.bounds;

            // Масштабируем изображение до нужной высоты.
            float scale = targetHeight / bounds.size.y;

            art.localScale = new Vector3(
                scale,
                scale,
                1f
            );

            // Совмещаем низ изображения с точкой посадки.
            // Работает как с Pivot Center, так и с Pivot Bottom.
            float bottomOffset = -bounds.min.y * scale;

            art.localPosition = new Vector3(
                bambooOffset.x,
                bottomOffset + bambooOffset.y,
                0f
            );
        }

        // Лиана растёт горизонтально.
        else if (plant is VinePlant vine)
        {
            float length = vine.Length;
            float scale = length / spriteSize.x;

            art.localPosition =
                new Vector3(length / 2f, 0, 0);

            art.localScale =
                new Vector3(scale, scale, 1);
        }
        // Папоротник сохраняет небольшой размер.
        else if (plant is FernPlant)
        {
            float height = 1f;
            float scale = height / spriteSize.y;

            art.localPosition =
                new Vector3(0, height / 2f, 0);

            art.localScale =
                new Vector3(scale, scale, 1);
        }
    }
}
