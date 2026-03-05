using UnityEngine;
using System;

public abstract class Code : MonoBehaviour
{
    public bool working = false;
    public Code next = null;
    public event Action OnComplete;

    public abstract void work();

    protected void Complete()
    {
        OnComplete?.Invoke();
    }

    public void ResetState()
    {
        working = false;
    }
}

public abstract class BoolCode : Code
{
    public bool judge = false;

}
