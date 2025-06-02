using UnityEngine;

public class MazeExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knight"))
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }
}