using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static public Player SelectPlayer;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        SelectPlayer = this;
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(string nodeName)
    {
        animator.Play(nodeName, 0, 0);
    }
}
