
using UnityEngine;

public class BambooPlant : PlantBase
{
    public override PlantKind Kind => PlantKind.Bamboo;

    public override bool IsMature =>
        Age >= 3 && Age <= 5;

    public float Height
    {
        get
        {
            if (Age <= 2) return 1f;
            if (Age <= 5) return 3f;
            return 5f;
        }
    }

    private GameObject stem;
    private GameObject top;

    private BoxCollider2D topCollider;

    protected override void CreateVisuals()
    {
        stem = ObjectFactory.CreateRectangle(
            "Стебель бамбука",
            transform,
            Vector2.zero,
            new Vector2(0.4f, 1f),
            new Color(0.2f, 0.7f, 0.3f),
            false,
            2
        );

        top = ObjectFactory.CreateRectangle(
            "Верхушка бамбука",
            transform,
            Vector2.zero,
            new Vector2(1.1f, 0.2f),
            new Color(0.2f, 0.55f, 0.25f),
            true,
            3
        );

        topCollider =
            top.GetComponent<BoxCollider2D>();
    }

    protected override void UpdateAppearance()
    {
        float height = Height;

        ObjectFactory.Resize(
            stem,
            new Vector2(0, height / 2f),
            new Vector2(0.4f, height)
        );

        ObjectFactory.Resize(
            top,
            new Vector2(0, height),
            new Vector2(1.1f, 0.2f)
        );

        Color color = Age >= 6
            ? new Color(0.65f, 0.5f, 0.25f)
            : new Color(0.2f, 0.7f, 0.3f);

        ObjectFactory.SetColor(stem, color);
        ObjectFactory.SetColor(top, color);

        // На старом и молодом бамбуке
        // стоять нельзя.
        topCollider.enabled = IsMature;

        ConfigureSelection(
            new Vector2(0, height / 2f),
            new Vector2(0.9f, height)
        );
    }
}
