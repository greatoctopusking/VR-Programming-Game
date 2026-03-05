using UnityEngine;

public class WhileCode : Code
{
    public BoolCode Judger = null;
    private Code BreakNext = null;
    public Code LoopNext = null;

    private void Start()
    {
        BreakNext = next;
    }

    public override void work()
    {
        // 先执行条件判断
        if (Judger != null)
        {
            Judger.work();
            
            // 根据条件结果设置下一步
            if (Judger.judge)
            {
                next = LoopNext;
            }
            else
            {
                next = BreakNext;
            }
        }
        else
        {
            next = BreakNext;
        }
        
        Complete();
    }

    public void ResetLoop()
    {
        if (Judger != null)
        {
            Judger.ResetState();
        }
    }
}
