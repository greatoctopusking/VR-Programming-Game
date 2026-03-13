using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

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

    public void SetHighlight(bool active) //给正在运行的代码块设置高亮显示
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = active ? Color.yellow : Color.white;
    }

}

public abstract class BoolCode : Code
{
    public bool judge = false;

}


