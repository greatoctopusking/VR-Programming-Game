using UnityEngine;

public class IfCode : Code
{
    public BoolCode Judger = null;
    private Code BreakNext = null;
    public Code NoBreakNext = null;

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
                next = NoBreakNext;
            }
            else
            {
                next = BreakNext;
            }
        }
        
        Complete();
    }
}
