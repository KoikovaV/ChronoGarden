
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

        string[] buttons =
        {
            "Бамбук [1]",
            "Лиана [2]",
            "Папоротник [3]",
            "-1 [Q]",
            "+1 [E]",
            "Сброс [R]"
        };

        float gap = 5f;
        float buttonWidth = (width - 24f - 5f * gap) / 6f;

        for (int i = 0; i < buttons.Length; i++)
        {
            float x = 12 + i * (buttonWidth + gap);

            if (GUI.Button(
                new Rect(x, 14, buttonWidth, 38),
                buttons[i]))
            {
                HandleTopButton(i);
            }
        }

        TimeController controller = level.Time;

        if (controller != null)
        {
            string selectedSeed =
                PlantName(controller.SelectedSeed);

            string plantInfo = "Выбрано семя: " +
                selectedSeed;

            if (controller.SelectedPlant != null)
            {
                PlantBase plant =
                    controller.SelectedPlant;

                plantInfo += " | Растение: " +
                    PlantName(plant.Kind) +
                    " | Возраст: " + plant.Age +
                    " сек.";
            }

            GUI.Label(
                new Rect(18, 58, width - 36, 25),
                plantInfo
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

    private void HandleTopButton(int index)
    {
        TimeController controller = level.Time;

        if (controller == null)
            return;

        switch (index)
        {
            case 0:
                controller.SelectSeed(PlantKind.Bamboo);
                break;

            case 1:
                controller.SelectSeed(PlantKind.Vine);
                break;

            case 2:
                controller.SelectSeed(PlantKind.Fern);
                break;

            case 3:
                controller.ChangeSelectedAge(-1);
                break;

            case 4:
                controller.ChangeSelectedAge(1);
                break;

            case 5:
                level.RestartLevel();
                break;
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
