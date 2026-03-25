using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class JudgerDetectCollider : MonoBehaviour
{
    public Collider Detect; // �����������
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        // 1. �����ǰ��λ�Ѿ������˶�����ֱ�ӷ���
        if (hasConnected) return;

        // 2. ��ȡ��λ�ĸ����壨IfCode �� WhileCode��
        Code parentBlock = transform.parent.GetComponent<Code>();
        if (parentBlock == null) return;

        // 3. ����λ�Ƿ��Ѿ�ռ���ˣ�ͨ��֮ǰд�� GetJudger �߼���
        if (GetJudger() != null) return;

        // 4. ��ȡ�Է������Ƿ�Ϊ�����ж��� (BoolCode)
        // ʹ�� GetComponentInParent ��Ϊ�˷�ֹץ�������ϵ�����ײ��
        BoolCode incomingJudger = other.GetComponentInParent<BoolCode>();
        // ����Է����� BoolCode�����߶Է��������Լ��ĸ����壬�����
        if (incomingJudger == null || incomingJudger.gameObject == transform.parent.gameObject) return;

        // 5. ץȡ��飺����ҡ��ɿ����ж���ʱ��ִ������������
        XRGrabInteractable judgerGrab = incomingJudger.GetComponent<XRGrabInteractable>();
        if (judgerGrab != null && judgerGrab.isSelected)
        {
            return;
        }

        // 6. ��ǩ��飺ȷ���Է����� CodeBlock ��ǩ
        if (!other.CompareTag("CodeBlock") && !incomingJudger.CompareTag("CodeBlock")) return;

        // 7. ִ���ж������е������߼�
        ConnectJudger(incomingJudger);
    }


    private void ConnectJudger(BoolCode judger)
    {
        // 1. ��ȡ����������
        Transform parentTransform = transform.parent;
        Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();
        Rigidbody judgerRb = judger.GetComponent<Rigidbody>();

        Collider parentCol = parentTransform.GetComponent<Collider>();
        Collider judgerCol = judger.GetComponent<Collider>();

        // 2. ��������֮���������ײ����ֹ����˲�䱬ը��
        if (parentCol != null && judgerCol != null)
        {
            Physics.IgnoreCollision(parentCol, judgerCol, true);
        }

        if (judgerRb != null)
        {
            judgerRb.isKinematic = true;         // ��Ϊ�˶�ѧ����������Ӱ��
            judgerRb.useGravity = false;          // �ر�����
            judgerRb.detectCollisions = false;    // �����޸ģ����ӿ鲻�ٲ���������ײ���㣬��ֹ���Ÿ�����
        }

        // 3. �߼����� (����ԭ���߼�)
        SetJudger(judger);

        // 4. �����߼��Ͳ㼶
        judger.transform.SetParent(parentTransform, true);

        // ��ȷ���뵽 Judger ��λ
        judger.transform.localPosition = new Vector3(0, 0, -1);
        judger.transform.localEulerAngles = Vector3.zero;

        Physics.SyncTransforms();
        hasConnected = true;

        // ע�⣺���ﲻ�ٽ� parentRb �Ļض���ѧ����Ϊ��Ϊ��̿飬ͨ�������Ӻ󱣳� Kinematic ����ȶ���
        // ������������������Ӱ�죬���Ը�����Ҫ�ֶ����� parentRb.isKinematic = false;
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
