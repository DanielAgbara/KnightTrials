using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MenuUI : MonoBehaviour
{
    public GameObject helpPanel;
    public GameObject levelPanel;
    public GameObject gameTitle1;
    public GameObject gameTitle2;
    public GameObject playButton;
    public GameObject helpButton;
    public GameObject levelButton;
    public AudioSource audioSource;
    public AudioClip onClickClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowHelp()
    {
        helpPanel.SetActive(true);
        gameTitle1.SetActive(false);
        gameTitle2.SetActive(false);
        playButton.SetActive(false);
        helpButton.SetActive(false);
        levelButton.SetActive(false);
    }

    public void BackToMainMenu()
    {
        helpPanel.SetActive(false);
        levelPanel.SetActive(false);
        gameTitle1.SetActive(true);
        gameTitle2.SetActive(true);
        playButton.SetActive(true);
        helpButton.SetActive(true);
        levelButton.SetActive(true);
    }

    public void ShowLevelOptions()
    {
        levelPanel.SetActive(true);
        gameTitle1.SetActive(false);
        gameTitle2.SetActive(false);
        playButton.SetActive(false);
        helpButton.SetActive(false);
        levelButton.SetActive(false);
    }

    public void LoadGroundMaze()
    {
        levelPanel.SetActive(false);
        SceneManager.LoadScene("GroundMaze", LoadSceneMode.Single);
    }

    public void LoadIceMaze()
    {
        levelPanel.SetActive(false);
        SceneManager.LoadScene("IceMaze", LoadSceneMode.Single);
    }

    public void LoadMudMaze()
    {
        levelPanel.SetActive(false);
        SceneManager.LoadScene("MudMaze", LoadSceneMode.Single);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(onClickClip);
    }
}
