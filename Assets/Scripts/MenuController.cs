using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        // Load the gameplay scene when the player clicks the "Start Game" button
        SceneManager.LoadScene("MainStory", LoadSceneMode.Single);
    }
}
