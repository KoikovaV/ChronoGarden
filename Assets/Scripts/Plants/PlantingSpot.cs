
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantingSpot : MonoBehaviour
{
    [Header("Какое растение можно посадить")]
    public PlantKind plantKind = PlantKind.Bamboo;

    [Header("Условие открытия точки")]
    public PlantingSpot requiredSpot;
    public int requiredMinimumAge = 3;
    public int requiredMaximumAge = 5;

    public PlantBase PlantedPlant { get; private set; }

    private GameObject indicator;

    public bool IsAvailable
    {
        get
        {
            if (PlantedPlant != null)
                return false;

            if (requiredSpot == null)
                return true;

            PlantBase requiredPlant =
                requiredSpot.PlantedPlant;

            if (requiredPlant == null)
                return false;

            return requiredPlant.Age >= requiredMinimumAge &&
                   requiredPlant.Age <= requiredMaximumAge;
        }
    }

    private void Awake()
    {
        BoxCollider2D collider =
            GetComponent<BoxCollider2D>();

        collider.isTrigger = true;
        collider.size = new Vector2(1.2f, 0.8f);
        collider.offset = new Vector2(0, 0.3f);

        indicator = ObjectFactory.CreateRectangle(
            "Место посадки",
            transform,
            new Vector2(0, 0.08f),
            new Vector2(0.9f, 0.15f),
            new Color(0.95f, 0.75f, 0.25f),
            false,
            4
        );
    }

    private void Update()
    {
        if (indicator != null)
            indicator.SetActive(IsAvailable);
    }

    public bool TryPlant(
        PlantKind selectedKind,
        out PlantBase createdPlant)
    {
        createdPlant = null;

        if (!IsAvailable)
            return false;

        if (selectedKind != plantKind)
            return false;

        GameObject plantObject = new GameObject(
            selectedKind.ToString()
        );

        plantObject.transform.position =
            transform.position;

        switch (selectedKind)
        {
            case PlantKind.Bamboo:
                createdPlant =
                    plantObject.AddComponent<BambooPlant>();
                break;

            case PlantKind.Vine:
                createdPlant =
                    plantObject.AddComponent<VinePlant>();
                break;

            case PlantKind.Fern:
                createdPlant =
                    plantObject.AddComponent<FernPlant>();
                break;
        }

        if (createdPlant == null)
        {
            Destroy(plantObject);
            return false;
        }

        PlantedPlant = createdPlant;
        return true;
    }
}
