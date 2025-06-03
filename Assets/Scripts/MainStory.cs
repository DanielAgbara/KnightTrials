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
        SceneManager.LoadScene("GroundMaze", LoadSceneMode.Single);
    }
}
