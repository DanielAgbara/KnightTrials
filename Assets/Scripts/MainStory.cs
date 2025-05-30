using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainStory : MonoBehaviour
{
    // void Start()
    // {
        
    // }

    // void Update()
    // {
        
    // }

    void OnEnable()
    {
        // Change the name of the scene when we have the main gameplay scene
        // For now, I just leave it as "Intro"
        SceneManager.LoadScene("MudMaze", LoadSceneMode.Single);
    }
}
