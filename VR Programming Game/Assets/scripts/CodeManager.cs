using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Linq;

public class CodeManager : MonoBehaviour
{
    public bool Coding = false;
    public Transform First = null;

    private Coroutine playRoutine = null;
    private Stack<WhileCode> loopStack = new Stack<WhileCode>();

    private bool wasLeftTriggerPressed = false;

    public InputActionAsset inputActions;
    private InputAction leftTriggerAction;

    private void Awake()
    {
        if (inputActions != null)
        {
            leftTriggerAction = inputActions.FindAction("Activate");
            if (leftTriggerAction != null)
            {
                leftTriggerAction.Enable();
            }
        }
    }

    private void OnDestroy()
    {
        if (leftTriggerAction != null)
        {
            leftTriggerAction.Disable();
        }
    }

    private IEnumerator PlayCoroutine()
    {
        Code cur = First?.GetComponent<Code>();
        
        while (cur != null)
        {
            bool completed = false;
            
            cur.OnComplete += () => completed = true;
            
            cur.work();
            
            yield return new WaitUntil(() => completed);
            
            cur.OnComplete -= () => completed = true;
            
            if (cur is WhileCode whileCode)
            {
                if (whileCode.Judger?.judge == true)
                {
                    loopStack.Push(whileCode);
                }
                else
                {
                    if (loopStack.Count > 0)
                    {
                        loopStack.Pop();
                    }
                }
            }
            
            if (cur.next == null && loopStack.Count > 0)
            {
                WhileCode loopStart = loopStack.Peek();
                
                if (loopStart.Judger?.judge == true)
                {
                    if (loopStart.Judger != null)
                    {
                        loopStart.Judger.ResetState();
                    }
                    cur = loopStart.LoopNext;
                    continue;
                }
                else
                {
                    loopStack.Pop();
                    cur = loopStart.next;
                    continue;
                }
            }
            
            cur = cur.next;
        }
        
        playRoutine = null;
    }

    void Update()
    {
        CheckLeftTrigger();
    }

    private void CheckLeftTrigger()
    {
        if (leftTriggerAction == null)
        {
            if (inputActions != null)
            {
                leftTriggerAction = inputActions.FindAction("Activate");
                if (leftTriggerAction != null)
                {
                    leftTriggerAction.Enable();
                }
            }
            return;
        }
        
        bool pressed = leftTriggerAction.IsPressed();

        if (pressed && !wasLeftTriggerPressed)
        {
            ToggleCodeExecution();
        }

        wasLeftTriggerPressed = pressed;
    }

    private void ToggleCodeExecution()
    {
        if (playRoutine == null)
        {
            ResetAllBlocks();
            playRoutine = StartCoroutine(PlayCoroutine());
        }
        else
        {
            StopAllCoroutines();
            playRoutine = null;
            ResetAllBlocks();
        }
    }

    private void ResetAllBlocks()
    {
        if (First == null) return;
        
        Code cur = First.GetComponent<Code>();
        while (cur != null)
        {
            cur.ResetState();
            cur = cur.next;
        }
    }
}
