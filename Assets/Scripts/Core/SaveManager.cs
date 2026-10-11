
using UnityEngine;

public static class SaveManager
{
    private const string LevelPrefix =
        "ChronoGarden_Level_";

    public static void CompleteLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(
            LevelPrefix + levelNumber,
            1
        );

        int highestLevel = PlayerPrefs.GetInt(
            "ChronoGarden_HighestLevel",
            1
        );

        if (levelNumber + 1 > highestLevel)
        {
            PlayerPrefs.SetInt(
                "ChronoGarden_HighestLevel",
                levelNumber + 1
            );
        }

        PlayerPrefs.Save();
    }

    public static bool IsLevelCompleted(int levelNumber)
    {
        return PlayerPrefs.GetInt(
            LevelPrefix + levelNumber,
            0
        ) == 1;
    }

    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(
            "ChronoGarden_HighestLevel",
            1
        );
    }

    public static void ResetProgress()
    {
        // Удаляем только данные нашей игры.
        int highest = GetHighestUnlockedLevel();

        for (int i = 1; i <= highest; i++)
        {
            PlayerPrefs.DeleteKey(LevelPrefix + i);
        }

        PlayerPrefs.DeleteKey(
            "ChronoGarden_HighestLevel"
        );

        PlayerPrefs.Save();
    }
}
