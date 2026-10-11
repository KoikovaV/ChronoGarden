
using UnityEngine;

public class VinePlant : PlantBase
{
    public override PlantKind Kind => PlantKind.Vine;

    public override bool IsMature =>
        Age >= 4 && Age <= 7;

    public float Length
    {
        get
        {
            if (Age <= 3) return 2f;
            if (Age <= 7) return 5f;
            return 7f;
        }
    }

    private GameObject vine;
    private BoxCollider2D bridgeCollider;

    private float oldSince = -1f;

    protected override void CreateVisuals()
    {
        vine = ObjectFactory.CreateRectangle(
            "Мост из лианы",
            transform,
            Vector2.zero,
            new Vector2(2f, 0.25f),
            new Color(0.3f, 0.7f, 0.35f),
            true,
            2
        );

        bridgeCollider =
            vine.GetComponent<BoxCollider2D>();
    }

    protected override void UpdateAppearance()
    {
        float length = Length;

        ObjectFactory.Resize(
            vine,
            new Vector2(length / 2f, 0),
            new Vector2(length, 0.25f)
        );

        Color color = Age >= 8
            ? new Color(0.65f, 0.5f, 0.3f)
            : new Color(0.3f, 0.7f, 0.35f);

        ObjectFactory.SetColor(vine, color);

        if (Age >= 8)
        {
            if (oldSince < 0)
                oldSince = Time.time;
        }
        else
        {
            oldSince = -1f;
            bridgeCollider.enabled = true;
        }

        ConfigureSelection(
            new Vector2(length / 2f, 0),
            new Vector2(length, 0.8f)
        );
    }

    private void Update()
    {
        if (Age < 8)
            return;

        if (oldSince < 0)
            return;

        // Старая лиана разрушается
        // через две секунды.
        if (Time.time - oldSince >= 2f)
        {
            bridgeCollider.enabled = false;
        }
    }
}
