using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;

    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isFleeing = false;

    int isRunningHash;
    int isFleeingHash;

    void Awake()
    {
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("isRunning");
        isFleeingHash = Animator.StringToHash("isFleeing");
    }

    public void SetAnimRunning(bool value)
    {
        isRunning = value;

        animator.SetBool(isRunningHash, value);
    }

    public void SetAnimFleeing(bool value)
    {
        isFleeing = value;

        if (value) SetAnimRunning(value);

        animator.SetBool(isFleeingHash, value);
    }

    public void ResetToIdle()
    {
        animator.SetBool(isFleeingHash, false);
        animator.SetBool(isRunningHash, false);
    }

    public bool GetRunningState()
    {
        return isRunning;
    }
    public bool GetFleeingState()
    {
        return isFleeing;
    }
}
