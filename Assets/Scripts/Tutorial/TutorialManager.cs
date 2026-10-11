
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum Stage
    {
        Free,
        Move,
        ChooseSeed,
        Plant,
        Grow,
        Climb,
        ReachPortal,
        Completed
    }

    [Header("Настройки обучения")]
    [SerializeField] private bool enableTutorial = true;

    [SerializeField]
    private PlantingSpot tutorialSpot;

    private LevelManager level;

    private string temporaryHint;
    private float hintUntil;

    public Stage CurrentStage { get; private set; }

    public bool CanPlant =>
        !enableTutorial ||
        CurrentStage == Stage.Plant;

    public string Message
    {
        get
        {
            if (Time.time < hintUntil)
                return temporaryHint;

            switch (CurrentStage)
            {
                case Stage.Free:
                    return "Найдите путь к выходу.";

                case Stage.Move:
                    return "Шаг 1/6. Идите к грядке: A и D.";

                case Stage.ChooseSeed:
                    return "Шаг 2/6. Выберите бамбук: 1.";

                case Stage.Plant:
                    return "Шаг 3/6. Нажмите на жёлтую грядку.";

                case Stage.Grow:
                    return "Шаг 4/6. Нажмите E три раза.";

                case Stage.Climb:
                    return "Шаг 5/6. Подойдите к бамбуку и нажмите W.";

                case Stage.ReachPortal:
                    return "Шаг 6/6. Прыгайте на платформу к порталу.";

                case Stage.Completed:
                    return "Уровень пройден!";
            }

            return "";
        }
    }

    public void Initialize(LevelManager levelManager)
    {
        level = levelManager;

        if (tutorialSpot == null)
            tutorialSpot =
                FindFirstObjectByType<PlantingSpot>();

        SetStage(
            enableTutorial
                ? Stage.Move
                : Stage.Free
        );
    }

    private void Update()
    {
        if (!enableTutorial || level == null)
            return;

        if (CurrentStage != Stage.Move)
            return;

        if (tutorialSpot == null || level.Player == null)
            return;

        float distance = Mathf.Abs(
            level.Player.transform.position.x -
            tutorialSpot.transform.position.x
        );

        if (distance < 2f)
            SetStage(Stage.ChooseSeed);
    }

    private void SetStage(Stage newStage)
    {
        CurrentStage = newStage;
        temporaryHint = "";
        hintUntil = 0f;
    }

    public void OnSeedChosen(PlantKind kind)
    {
        if (!enableTutorial)
            return;

        if (CurrentStage != Stage.ChooseSeed)
            return;

        if (kind == PlantKind.Bamboo)
            SetStage(Stage.Plant);
        else
            ShowHint("Для начала выберите бамбук.");
    }

    public void OnPlantPlaced(PlantBase plant)
    {
        if (!enableTutorial)
            return;

        if (CurrentStage != Stage.Plant)
            return;

        if (plant.Kind == PlantKind.Bamboo)
            SetStage(Stage.Grow);
    }

    public void OnAgeChanged(PlantBase plant)
    {
        if (!enableTutorial)
            return;

        if (plant is not BambooPlant bamboo)
            return;

        if (CurrentStage == Stage.Grow &&
            bamboo.IsMature)
        {
            SetStage(Stage.Climb);
        }
        else if (CurrentStage == Stage.Climb &&
                 !bamboo.IsMature)
        {
            SetStage(Stage.Grow);
        }

        if (bamboo.Age >= 6)
        {
            ShowHint(
                "Бамбук стал хрупким. Нажмите Q."
            );
        }
    }

    public void OnPlayerClimbed()
    {
        if (!enableTutorial)
            return;

        if (CurrentStage == Stage.Climb)
            SetStage(Stage.ReachPortal);
    }

    public void OnLevelCompleted()
    {
        SetStage(Stage.Completed);
    }

    public void ShowHint(string message)
    {
        temporaryHint = message;
        hintUntil = Time.time + 2.5f;
    }
}
