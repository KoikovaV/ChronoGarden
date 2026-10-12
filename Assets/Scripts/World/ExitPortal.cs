
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ExitPortal : MonoBehaviour
{
    [Header("Обязательные растения")]
    [SerializeField]
    private PlantingSpot[] requiredPlants;

    private LevelManager level;

    private void Awake()
    {
        BoxCollider2D collider =
            GetComponent<BoxCollider2D>();

        collider.isTrigger = true;
        collider.size = new Vector2(0.9f, 1.6f);
    }

    private void Start()
    {
        level = FindFirstObjectByType<LevelManager>();
    }

    private bool ArePlantsReady()
    {
        if (requiredPlants == null)
            return true;

        foreach (PlantingSpot spot in requiredPlants)
        {
            if (spot == null)
                return false;

            PlantBase plant = spot.PlantedPlant;

            if (plant == null || !plant.IsMature)
                return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (level == null || level.IsCompleted)
            return;

        PlayerController player =
            other.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (!ArePlantsReady())
        {
            if (level.Tutorial != null)
            {
                level.Tutorial.ShowHint(
                    "Для выхода нужны зрелые бамбук и лиана."
                );
            }

            return;
        }

        level.CompleteLevel();
    }
}
