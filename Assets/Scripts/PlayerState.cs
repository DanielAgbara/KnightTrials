using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool IsBlocking { get; private set; }

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        IsBlocking = Input.GetMouseButton(1); // Right mouse button
    }
}
