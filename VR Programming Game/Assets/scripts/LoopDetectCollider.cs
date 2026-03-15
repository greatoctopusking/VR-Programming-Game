using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LoopDetectCollider : MonoBehaviour
{
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

        // 1. 获取组件：父物体必须是 WhileCode，对方必须是 Code
        WhileCode parentWhile = transform.parent.GetComponent<WhileCode>();
        Code otherCode = other.GetComponentInParent<Code>();

        if (parentWhile == null || otherCode == null || parentWhile.gameObject == otherCode.gameObject)
        {
            return;
        }

        // 2. 状态检查：如果 LoopNext 已存在，不执行
        if (parentWhile.LoopNext != null)
        {
            return;
        }

        // 3. 标签检查
        if (!other.CompareTag("CodeBlock") && !otherCode.CompareTag("CodeBlock"))
        {
            return;
        }

        // 4. 抓取检查：如果任意一方正被抓着，不进行吸附
        XRGrabInteractable myGrab = transform.parent.GetComponent<XRGrabInteractable>();
        if (myGrab != null && myGrab.isSelected) return;

        XRGrabInteractable otherGrab = otherCode.GetComponent<XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected) return;

        // 执行连接逻辑
        ConnectLoopNext(parentWhile, otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        // 当物体离开触发区，重置连接标记，允许下次连接
        hasConnected = false;
    }

    private void ConnectLoopNext(WhileCode parentWhile, Code otherCode)
    {
        Debug.Log($"[LoopDetect] 连接: {parentWhile.name} -> {otherCode.name}");

        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();
        Collider selfCol = parentWhile.GetComponent<Collider>();
        Collider otherCol = otherCode.GetComponent<Collider>();

        // 1. 物理安全：禁用两者之间的碰撞，防止吸附瞬间产生物理排斥弹飞
        if (selfCol != null && otherCol != null)
        {
            Physics.IgnoreCollision(selfCol, otherCol, true);
        }

        // 2. Rigidbody 处理：彻底禁用子块的物理模拟
        if (otherRb != null)
        {
            otherRb.isKinematic = true;
            otherRb.useGravity = false;
            otherRb.detectCollisions = false; // 关键：让子块不再参与物理碰撞计算
        }

        // 3. 逻辑和层级设置
        parentWhile.LoopNext = otherCode;

        // 使用 true 保持世界坐标，然后下一步再设置 localPosition 确保过渡平滑
        otherCode.transform.SetParent(parentWhile.transform, true);

        // 4. 精确对齐位置
        otherCode.transform.localPosition = new Vector3(-1, 0, 0);
        otherCode.transform.localEulerAngles = Vector3.zero;
        otherCode.transform.localScale = Vector3.one;

        // 5. 刷新 UI 或连接线
        ConnectionController controller = parentWhile.GetComponentInChildren<ConnectionController>();
        if (controller != null)
        {
            controller.Refresh();
        }

        // 6. 同步物理变换
        Physics.SyncTransforms();

        hasConnected = true;
    }
}