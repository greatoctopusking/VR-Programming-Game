using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DetectCollider : MonoBehaviour
{
    public Collider Detect; // 检测区触发器
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

        // 获取自己的根物体（父物体）和对方的根物体
        Code myCode = transform.parent.GetComponent<Code>();
        Code otherCode = other.GetComponentInParent<Code>(); // 使用InParent更安全

        if (myCode == null || otherCode == null || myCode == otherCode) return;

        // 1. 状态检查：如果我已经有下一个了，就不再吸附
        if (myCode.next != null) return;

        // 2. 抓取检查：如果任意一方正被抓着，通常不吸附
        XRGrabInteractable myGrab = transform.parent.GetComponent<XRGrabInteractable>();
        if (myGrab != null && myGrab.isSelected) return;

        XRGrabInteractable otherGrab = otherCode.GetComponent<XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected) return;

        // 3. 标签检查
        if (!other.CompareTag("CodeBlock") && !otherCode.CompareTag("CodeBlock")) return;

        // 执行连接
        ConnectCode(myCode, otherCode);
    }

    private void ConnectCode(Code self, Code otherCode)
    {
        // Debug.Log($"[Detect] 永久连接: {self.name} -> {otherCode.name}");

        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();
        Collider selfCol = self.GetComponent<Collider>();
        Collider otherCol = otherCode.GetComponent<Collider>();

        // 1. 禁用两者之间的物理碰撞（最重要，防止松手瞬间爆炸）
        if (selfCol != null && otherCol != null)
        {
            Physics.IgnoreCollision(selfCol, otherCol, true);
        }

        if (otherRb != null)
        {
            // 彻底禁用子块 Rigidbody，甚至可以考虑直接 Destroy(otherRb)
            // 但设为 Kinematic 并关闭 Detect Collisions 通常足够
            otherRb.isKinematic = true;
            otherRb.useGravity = false;
            otherRb.detectCollisions = false; // 让子块的刚体不再参与物理碰撞计算
        }

        // 2. 不要把子块设为 Trigger，否则你以后抓不到它
        if (otherCol != null)
        {
            otherCol.isTrigger = false;
        }

        // 3. 建立逻辑和层级
        self.next = otherCode;
        otherCode.transform.SetParent(self.transform, true); // 改为 true，保持世界坐标尝试

        // 4. 精确对齐
        otherCode.transform.localPosition = new Vector3(-1, 0, 0);
        otherCode.transform.localEulerAngles = Vector3.zero;

        hasConnected = true;
        Physics.SyncTransforms();
    }
}

