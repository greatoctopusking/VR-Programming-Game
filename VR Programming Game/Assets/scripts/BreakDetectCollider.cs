using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakDetectCollider : MonoBehaviour
{
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

        // 1. 获取组件：父物体 WhileCode 和 对方的 Code
        WhileCode parentWhile = transform.parent.GetComponentInParent<WhileCode>();
        Code otherCode = other.GetComponentInParent<Code>();

        if (parentWhile == null || otherCode == null || parentWhile.gameObject == otherCode.gameObject)
        {
            return;
        }

        // 2. 状态检查：如果 next 路径已经有块了，则不再吸附
        if (parentWhile.next != null)
        {
            return;
        }

        // 3. 标签检查
        if (!other.CompareTag("CodeBlock") && !otherCode.CompareTag("CodeBlock"))
        {
            return;
        }

        // 4. 抓取检查：双方都不在被抓取状态时才触发吸附
        XRGrabInteractable myGrab = transform.parent.GetComponent<XRGrabInteractable>();
        if (myGrab != null && myGrab.isSelected) return;

        XRGrabInteractable otherGrab = otherCode.GetComponent<XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected) return;

        // 执行连接
        ConnectNext(parentWhile, otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        // 离开触发区重置标记
        hasConnected = false;
    }

    private void ConnectNext(WhileCode parentWhile, Code otherCode)
    {
        Debug.Log($"[BreakDetect] 连接循环后路径: {parentWhile.name} -> {otherCode.name}");

        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();
        Collider selfCol = parentWhile.GetComponent<Collider>();
        Collider otherCol = otherCode.GetComponent<Collider>();

        // 1. 物理安全：防止重叠导致的物理碰撞冲突
        if (selfCol != null && otherCol != null)
        {
            Physics.IgnoreCollision(selfCol, otherCol, true);
        }

        // 2. Rigidbody 处理：禁用物理模拟和碰撞检测
        if (otherRb != null)
        {
            otherRb.isKinematic = true;
            otherRb.useGravity = false;
            otherRb.detectCollisions = false;
        }

        // 3. 建立逻辑和层级关系
        parentWhile.next = otherCode;
        
        Transform backTransform = parentWhile.transform.Find("Back"); // 找到 Back 这个子物体

        if (backTransform != null)
        {
            // 将父级设为 Back，这样它就会对齐到 Back 的坐标系
            otherCode.transform.SetParent(backTransform, true);
        }
        else
        {
            // 如果没找到 Back，降级拼到根部（防止出错）
            otherCode.transform.SetParent(parentWhile.transform, true);
        }

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

        // 6. 同步物理引擎变换
        Physics.SyncTransforms();

        hasConnected = true;
    }
}