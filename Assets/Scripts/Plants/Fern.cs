
using UnityEngine;

public class FernPlant : PlantBase
{
    public override PlantKind Kind => PlantKind.Fern;

    public override bool IsMature =>
        Age >= 3 && Age <= 4;

    [SerializeField]
    private float bounceForce = 12f;

    private GameObject body;
    private float nextBounceTime;

    protected override void CreateVisuals()
    {
        body = ObjectFactory.CreateRectangle(
            "Пружинный папоротник",
            transform,
            new Vector2(0, 0.35f),
            new Vector2(1f, 0.7f),
            new Color(0.3f, 0.8f, 0.4f),
            false,
            2
        );

        ConfigureSelection(
            new Vector2(0, 0.35f),
            new Vector2(1f, 0.7f)
        );
    }

    protected override void UpdateAppearance()
    {
        Color color;

        if (Age <= 2)
            color = new Color(0.4f, 0.6f, 0.35f);
        else if (IsMature)
            color = new Color(0.3f, 1f, 0.45f);
        else
            color = new Color(0.6f, 0.5f, 0.3f);

        ObjectFactory.SetColor(body, color);

        body.SetActive(Age < 5);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsMature)
            return;

        if (Time.time < nextBounceTime)
            return;

        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
            return;

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        if (rb.linearVelocity.y > 0.1f)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            bounceForce
        );

        nextBounceTime = Time.time + 0.8f;
    }
}
