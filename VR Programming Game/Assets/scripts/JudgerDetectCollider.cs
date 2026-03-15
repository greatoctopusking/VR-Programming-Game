using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class JudgerDetectCollider : MonoBehaviour
{
    public Collider Detect; // 检测区触发器
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        // 1. 如果当前槽位已经连接了东西，直接返回
        if (hasConnected) return;

        // 2. 获取槽位的父物体（IfCode 或 WhileCode）
        Code parentBlock = transform.parent.GetComponent<Code>();
        if (parentBlock == null) return;

        // 3. 检查槽位是否已经占用了（通过之前写的 GetJudger 逻辑）
        if (GetJudger() != null) return;

        // 4. 获取对方物体是否为布尔判定块 (BoolCode)
        // 使用 GetComponentInParent 是为了防止抓到方块上的子碰撞体
        BoolCode incomingJudger = other.GetComponentInParent<BoolCode>();
        // 如果对方不是 BoolCode，或者对方就是我自己的父物体，则忽略
        if (incomingJudger == null || incomingJudger.gameObject == transform.parent.gameObject) return;

        // 5. 抓取检查：当玩家“松开”判定块时，执行吸附动作。
        XRGrabInteractable judgerGrab = incomingJudger.GetComponent<XRGrabInteractable>();
        if (judgerGrab != null && judgerGrab.isSelected)
        {
            return;
        }

        // 6. 标签检查：确保对方带有 CodeBlock 标签
        if (!other.CompareTag("CodeBlock") && !incomingJudger.CompareTag("CodeBlock")) return;

        // 7. 执行判定块特有的连接逻辑
        ConnectJudger(incomingJudger);
    }


    private void ConnectJudger(BoolCode judger)
    {
        // 1. 获取相关组件引用
        Transform parentTransform = transform.parent;
        Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();
        Rigidbody judgerRb = judger.GetComponent<Rigidbody>();

        Collider parentCol = parentTransform.GetComponent<Collider>();
        Collider judgerCol = judger.GetComponent<Collider>();

        // 2. 禁用两者之间的物理碰撞（防止松手瞬间爆炸）
        if (parentCol != null && judgerCol != null)
        {
            Physics.IgnoreCollision(parentCol, judgerCol, true);
        }

        if (judgerRb != null)
        {
            judgerRb.isKinematic = true;         // 设为运动学，不受重力影响
            judgerRb.useGravity = false;          // 关闭重力
            judgerRb.detectCollisions = false;    // 核心修改：让子块不再参与物理碰撞计算，防止干扰父物体
        }

        // 3. 逻辑连接 (保留原有逻辑)
        SetJudger(judger);

        // 4. 建立逻辑和层级
        judger.transform.SetParent(parentTransform, true);

        // 精确对齐到 Judger 槽位
        judger.transform.localPosition = new Vector3(0, 0, -1);
        judger.transform.localEulerAngles = Vector3.zero;
        judger.transform.localScale = Vector3.one; // 确保缩放一致

        // 5. 同步与状态更新
        Physics.SyncTransforms();
        hasConnected = true;

        // 注意：这里不再将 parentRb 改回动力学，因为作为编程块，通常在连接后保持 Kinematic 会更稳定。
        // 如果父物体必须受物理影响，可以根据需要手动开启 parentRb.isKinematic = false;
    }
    private BoolCode GetJudger()
    {
        IfCode ifCode = transform.parent.GetComponent<IfCode>();
        if (ifCode != null) return ifCode.Judger;
        
        WhileCode whileCode = transform.parent.GetComponent<WhileCode>();
        if (whileCode != null) return whileCode.Judger;
        
        return null;
    }
    
    private void SetJudger(BoolCode judger)
    {
        IfCode ifCode = transform.parent.GetComponent<IfCode>();
        if (ifCode != null)
        {
            ifCode.Judger = judger;
            return;
        }
        
        WhileCode whileCode = transform.parent.GetComponent<WhileCode>();
        if (whileCode != null)
        {
            whileCode.Judger = judger;
        }
    }
}
