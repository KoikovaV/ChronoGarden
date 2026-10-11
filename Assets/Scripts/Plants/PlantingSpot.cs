
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantingSpot : MonoBehaviour
{
    [Header("Вид растения")]
    public PlantKind plantKind = PlantKind.Bamboo;

    [Header("Префабы растений")]
    [SerializeField]
    private BambooPlant bambooPrefab;

    [SerializeField]
    private VinePlant vinePrefab;

    [SerializeField]
    private FernPlant fernPrefab;

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

        // Создаём растение из нужного префаба.
        switch (selectedKind)
        {
            case PlantKind.Bamboo:
                if (bambooPrefab != null)
                {
                    createdPlant = Instantiate(
                        bambooPrefab,
                        transform.position,
                        Quaternion.identity
                    );
                }
                break;

            case PlantKind.Vine:
                if (vinePrefab != null)
                {
                    createdPlant = Instantiate(
                        vinePrefab,
                        transform.position,
                        Quaternion.identity
                    );
                }
                break;

            case PlantKind.Fern:
                if (fernPrefab != null)
                {
                    createdPlant = Instantiate(
                        fernPrefab,
                        transform.position,
                        Quaternion.identity
                    );
                }
                break;
        }

        if (createdPlant == null)
        {
            Debug.LogWarning(
                "Для растения " + selectedKind +
                " не назначен префаб в PlantingSpot."
            );

            return false;
        }

        PlantedPlant = createdPlant;
        return true;
    }
}
