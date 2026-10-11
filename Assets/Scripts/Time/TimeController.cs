
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public PlantKind SelectedSeed { get; private set; }
        = PlantKind.Bamboo;

    public PlantBase SelectedPlant { get; private set; }

    private LevelManager level;

    public void Initialize(LevelManager levelManager)
    {
        level = levelManager;
    }

    private void Update()
    {
        if (level == null || level.IsCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSeed(PlantKind.Bamboo);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSeed(PlantKind.Vine);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSeed(PlantKind.Fern);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeSelectedAge(-1);

        if (Input.GetKeyDown(KeyCode.E))
            ChangeSelectedAge(1);

        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
    }

    public void SelectSeed(PlantKind kind)
    {
        SelectedSeed = kind;

        if (level.Tutorial != null)
            level.Tutorial.OnSeedChosen(kind);
    }

    public void SelectPlant(PlantBase plant)
    {
        SelectedPlant = plant;
    }

    public void ChangeSelectedAge(int amount)
    {
        if (level == null || level.IsCompleted)
            return;

        if (SelectedPlant == null)
        {
            ShowHint("Сначала выберите растение.");
            return;
        }

        SelectedPlant.ChangeAge(amount);

        if (level.Tutorial != null)
            level.Tutorial.OnAgeChanged(SelectedPlant);
    }

    private void HandleMouseClick()
    {
        if (IsPointerOverInterface())
            return;

        Camera camera = Camera.main;

        if (camera == null)
            return;

        Vector3 mouseWorld =
            camera.ScreenToWorldPoint(
                Input.mousePosition
            );

        Vector2 point = new Vector2(
            mouseWorld.x,
            mouseWorld.y
        );

        Collider2D[] hits =
            Physics2D.OverlapPointAll(point);

        // Сначала проверяем места посадки.
        foreach (Collider2D hit in hits)
        {
            PlantingSpot spot =
                hit.GetComponent<PlantingSpot>();

            if (spot == null || !spot.IsAvailable)
                continue;

            if (level.Tutorial != null &&
                !level.Tutorial.CanPlant)
            {
                return;
            }

            if (spot.TryPlant(
                SelectedSeed,
                out PlantBase plant))
            {
                SelectPlant(plant);
                level.OnPlantPlaced(plant);
            }
            else
            {
                ShowHint("Выбрано неподходящее семя.");
            }

            return;
        }

        // Если это не грядка, выбираем растение.
        foreach (Collider2D hit in hits)
        {
            PlantBase plant =
                hit.GetComponentInParent<PlantBase>();

            if (plant != null)
            {
                SelectPlant(plant);
                return;
            }
        }
    }

    private bool IsPointerOverInterface()
    {
        float scale = Mathf.Clamp(
            Mathf.Min(
                Screen.width / 960f,
                Screen.height / 540f
            ),
            0.65f,
            1.5f
        );

        float mouseY = Input.mousePosition.y;

        return mouseY > Screen.height - 140f * scale ||
               mouseY < 95f * scale;
    }

    private void ShowHint(string message)
    {
        if (level != null && level.Tutorial != null)
            level.Tutorial.ShowHint(message);
    }
}
