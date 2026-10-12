
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Русский шрифт")]
    public Font russianFont;

    private LevelManager level;

    public void Initialize(LevelManager levelManager)
    {
        level = levelManager;
    }

    private string PlantName(PlantKind kind)
    {
        switch (kind)
        {
            case PlantKind.Bamboo:
                return "Бамбук";

            case PlantKind.Vine:
                return "Лиана";

            case PlantKind.Fern:
                return "Папоротник";
        }

        return "Растение";
    }

    private void OnGUI()
    {
        if (level == null)
            return;

        float scale = Mathf.Clamp(
            Mathf.Min(
                Screen.width / 960f,
                Screen.height / 540f
            ),
            0.65f,
            1.5f
        );

        GUI.matrix = Matrix4x4.Scale(
            new Vector3(scale, scale, 1f)
        );

        float width = Screen.width / scale;
        float height = Screen.height / scale;

        if (russianFont != null)
        {
            GUI.skin.font = russianFont;
            GUI.skin.button.font = russianFont;
            GUI.skin.label.font = russianFont;
            GUI.skin.box.font = russianFont;
        }

        GUI.skin.button.fontSize = 15;
        GUI.skin.label.fontSize = 15;
        GUI.skin.box.fontSize = 18;

        DrawTopPanel(width);
        DrawControls(width, height);

        if (level.IsCompleted)
            DrawVictoryWindow(width, height);
    }


    private void DrawTopPanel(float width)
    {
        GUI.Box(
            new Rect(8, 8, width - 16, 125),
            ""
        );

        TimeController controller = level.Time;

        // Количество кнопок зависит от растений уровня.
        int buttonCount = 3; // -1, +1, сброс

        if (level.IsPlantAvailable(PlantKind.Bamboo))
            buttonCount++;

        if (level.IsPlantAvailable(PlantKind.Vine))
            buttonCount++;

        if (level.IsPlantAvailable(PlantKind.Fern))
            buttonCount++;

        float gap = 5f;
        float buttonWidth =
            (width - 24f - (buttonCount - 1) * gap)
            / buttonCount;

        float x = 12f;

        if (level.IsPlantAvailable(PlantKind.Bamboo))
        {
            if (GUI.Button(
                new Rect(x, 14, buttonWidth, 38),
                "Бамбук [1]"))
            {
                controller?.SelectSeed(PlantKind.Bamboo);
            }

            x += buttonWidth + gap;
        }

        if (level.IsPlantAvailable(PlantKind.Vine))
        {
            if (GUI.Button(
                new Rect(x, 14, buttonWidth, 38),
                "Лиана [2]"))
            {
                controller?.SelectSeed(PlantKind.Vine);
            }

            x += buttonWidth + gap;
        }

        if (level.IsPlantAvailable(PlantKind.Fern))
        {
            if (GUI.Button(
                new Rect(x, 14, buttonWidth, 38),
                "Папоротник [3]"))
            {
                controller?.SelectSeed(PlantKind.Fern);
            }

            x += buttonWidth + gap;
        }

        if (GUI.Button(
            new Rect(x, 14, buttonWidth, 38),
            "-1 [Q]"))
        {
            controller?.ChangeSelectedAge(-1);
        }

        x += buttonWidth + gap;

        if (GUI.Button(
            new Rect(x, 14, buttonWidth, 38),
            "+1 [E]"))
        {
            controller?.ChangeSelectedAge(1);
        }

        x += buttonWidth + gap;

        if (GUI.Button(
            new Rect(x, 14, buttonWidth, 38),
            "Сброс [R]"))
        {
            level.RestartLevel();
        }

        if (controller != null)
        {
            string info = "Выбрано семя: " +
                PlantName(controller.SelectedSeed);

            if (controller.SelectedPlant != null)
            {
                PlantBase plant = controller.SelectedPlant;

                info += " | Растение: " +
                    PlantName(plant.Kind) +
                    " | Возраст: " +
                    plant.Age + " сек.";
            }

            GUI.Label(
                new Rect(18, 58, width - 36, 25),
                info
            );
        }

        if (level.Tutorial != null)
        {
            GUI.Label(
                new Rect(18, 89, width - 36, 30),
                level.Tutorial.Message
            );
        }
    }

    private void DrawControls(float width, float height)
    {
        if (level.Player == null)
            return;

        float bottom = height - 80f;

        bool left = GUI.RepeatButton(
            new Rect(10, bottom, 80, 65),
            "Влево"
        );

        bool right = GUI.RepeatButton(
            new Rect(100, bottom, 80, 65),
            "Вправо"
        );

        level.Player.SetTouchMovement(left, right);

        if (GUI.Button(
            new Rect(width - 180, bottom, 80, 65),
            "Прыжок"))
        {
            level.Player.Jump();
        }

        if (GUI.Button(
            new Rect(width - 90, bottom, 80, 65),
            "Вверх"))
        {
            level.Player.TryClimb();
        }
    }

    private void DrawVictoryWindow(float width, float height)
    {
        float x = width / 2f - 170f;
        float y = height / 2f - 110f;

        GUI.Box(
            new Rect(x, y, 340, 220),
            "УРОВЕНЬ ПРОЙДЕН!"
        );

        GUI.Label(
            new Rect(x + 25, y + 65, 300, 30),
            "Сад становится ближе к жизни!"
        );

        if (level.HasNextLevel)
        {
            if (GUI.Button(
                new Rect(x + 65, y + 115, 210, 40),
                "Следующий уровень"))
            {
                level.GoToNextLevel();
            }
        }

        if (GUI.Button(
            new Rect(x + 65, y + 165, 210, 40),
            "Играть снова"))
        {
            level.RestartLevel();
        }
    }
}
