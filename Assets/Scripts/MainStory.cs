using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStory : MonoBehaviour
{
    void OnEnable()
    {
        // Initialize the LevelManager if it doesn't exist
        if (LevelManager.Instance == null)
        {
            GameObject levelManager = new GameObject("LevelManager");
            levelManager.AddComponent<LevelManager>();
        }
        
        // Load the first level
        SceneManager.LoadScene("GroundMaze", LoadSceneMode.Single);
    }
}