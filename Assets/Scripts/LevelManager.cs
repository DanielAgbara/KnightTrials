using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    // List of maze scenes in order
    private string[] mazeScenes = { "GroundMaze", "IceMaze", "MudMaze" };
    private int currentLevelIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        
        if (currentLevelIndex >= mazeScenes.Length)
        {
            // All levels completed - return to main menu
            SceneManager.LoadScene("MenuScene");
            Destroy(gameObject); // Remove the LevelManager
            return;
        }
        
        SceneManager.LoadScene(mazeScenes[currentLevelIndex]);
    }

    public string GetCurrentSceneName()
    {
        return mazeScenes[currentLevelIndex];
    }
}