using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakDetectCollider : MonoBehaviour
{
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

        // 1. ��ȡ����������� WhileCode �� �Է��� Code
        WhileCode parentWhile = transform.parent.GetComponentInParent<WhileCode>();
        Code otherCode = other.GetComponentInParent<Code>();

        if (parentWhile == null || otherCode == null || parentWhile.gameObject == otherCode.gameObject)
        {
            return;
        }

        // 2. ״̬��飺��� next ·���Ѿ��п��ˣ���������
        if (parentWhile.next != null)
        {
            return;
        }

        // 3. ��ǩ���
        if (!other.CompareTag("CodeBlock") && !otherCode.CompareTag("CodeBlock"))
        {
            return;
        }

        // 4. ץȡ��飺˫�������ڱ�ץȡ״̬ʱ�Ŵ�������
        XRGrabInteractable myGrab = transform.parent.GetComponent<XRGrabInteractable>();
        if (myGrab != null && myGrab.isSelected) return;

        XRGrabInteractable otherGrab = otherCode.GetComponent<XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected) return;

        // ִ������
        ConnectNext(parentWhile, otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        // �뿪���������ñ��
        hasConnected = false;
    }

    private void ConnectNext(WhileCode parentWhile, Code otherCode)
    {
        Debug.Log($"[BreakDetect] ����ѭ����·��: {parentWhile.name} -> {otherCode.name}");

        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();
        Collider selfCol = parentWhile.GetComponent<Collider>();
        Collider otherCol = otherCode.GetComponent<Collider>();

        // 1. ������ȫ����ֹ�ص����µ�������ײ��ͻ
        if (selfCol != null && otherCol != null)
        {
            Physics.IgnoreCollision(selfCol, otherCol, true);
        }

        // 2. Rigidbody ��������������ģ�����ײ���
        if (otherRb != null)
        {
            otherRb.isKinematic = true;
            otherRb.useGravity = false;
            otherRb.detectCollisions = false;
        }

        // 3. �����߼��Ͳ㼶��ϵ
        parentWhile.next = otherCode;
        
        Transform backTransform = parentWhile.transform.Find("Back"); // �ҵ� Back ���������

        if (backTransform != null)
        {
            // ��������Ϊ Back���������ͻ���뵽 Back ������ϵ
            otherCode.transform.SetParent(backTransform, true);
        }
        else
        {
            // ���û�ҵ� Back������ƴ����������ֹ������
            otherCode.transform.SetParent(parentWhile.transform, true);
        }

        // 4. ��ȷ����λ��
        otherCode.transform.localPosition = new Vector3(-1, 0, 0);
        otherCode.transform.localEulerAngles = Vector3.zero;

        // 5. ˢ�� UI ��������
        ConnectionController controller = parentWhile.GetComponentInChildren<ConnectionController>();
        if (controller != null)
        {
            controller.Refresh();
        }

        // 6. ͬ����������任
        Physics.SyncTransforms();

        hasConnected = true;
    }
}