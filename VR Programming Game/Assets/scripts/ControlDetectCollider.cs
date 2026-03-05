using UnityEngine;

public class ControlDetectCollider : MonoBehaviour
{
    public Collider Detect;
    private void OnTriggerEnter(Collider other)
    {
        if (transform.parent.GetComponent<Code>().next != null)
        {
            return;
        }

        if (!other.CompareTag("CodeBlock"))
        {
            return;
        }

        Code otherCode = other.GetComponent<Code>();
        if (otherCode == null)
        {
            return;
        }

        transform.parent.GetComponent<Code>().next = otherCode;
        Transform nextCodeTrans = otherCode.transform;

        nextCodeTrans.SetParent(transform.parent, false);
        nextCodeTrans.localPosition = new Vector3(-1, 0, 0);
        nextCodeTrans.localEulerAngles = Vector3.zero;
        nextCodeTrans.localScale = new Vector3(1, 1, 1);

        Physics.SyncTransforms();
    }
}
