
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Настройки уровня")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string nextSceneName = "";

    [Header("Объекты сцены")]
    [SerializeField] private PlayerController player;
    [SerializeField] private TimeController timeController;
    [SerializeField] private TutorialManager tutorial;
    [SerializeField] private GameUI gameUI;

    public PlayerController Player => player;
    public TimeController Time => timeController;
    public TutorialManager Tutorial => tutorial;

    public bool IsCompleted { get; private set; }

    public int LevelNumber => levelNumber;

    public bool HasNextLevel =>
        !string.IsNullOrEmpty(nextSceneName) &&
        Application.CanStreamedLevelBeLoaded(nextSceneName);

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameObject obj = new GameObject("GameManager");
            obj.AddComponent<GameManager>();
        }

        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (timeController == null)
            timeController = FindFirstObjectByType<TimeController>();

        if (tutorial == null)
            tutorial = FindFirstObjectByType<TutorialManager>();

        if (gameUI == null)
            gameUI = FindFirstObjectByType<GameUI>();
    }

    private void Start()
    {
        if (tutorial != null)
            tutorial.Initialize(this);

        if (timeController != null)
            timeController.Initialize(this);

        if (gameUI != null)
            gameUI.Initialize(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();

        if (IsCompleted || player == null)
            return;

        if (player.transform.position.y < -6f)
        {
            player.Respawn();

            if (tutorial != null)
            {
                tutorial.ShowHint(
                    "Вы упали. Попробуйте снова."
                );
            }
        }
    }

    public void OnPlantPlaced(PlantBase plant)
    {
        if (tutorial != null)
            tutorial.OnPlantPlaced(plant);
    }

    public void CompleteLevel()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        // Останавливаем персонажа после победы.
        if (player != null)
            player.FreezeAtFinish();

        SaveManager.CompleteLevel(levelNumber);

        if (tutorial != null)
            tutorial.OnLevelCompleted();

        Debug.Log("Пройден уровень " + levelNumber);
    }

    public void RestartLevel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
        else
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
    }

    public void GoToNextLevel()
    {
        if (!IsCompleted || !HasNextLevel)
            return;

        GameManager.Instance.LoadLevel(nextSceneName);
    }
}
