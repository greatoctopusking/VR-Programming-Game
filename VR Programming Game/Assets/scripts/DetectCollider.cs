using UnityEngine;

public class DetectCollider : MonoBehaviour
{
    public Collider Detect;
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected)
        {
            return;
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = transform.parent.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            return;
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable otherGrab = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected)
        {
            return;
        }

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

        ConnectCode(otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        hasConnected = false;
    }

    private void ConnectCode(Code otherCode)
    {
        Debug.Log($"[Detect] 连接: {transform.parent.name} -> {otherCode.name}");

        Rigidbody thisRb = transform.parent.GetComponent<Rigidbody>();
        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();

        bool thisWasKinematic = false;
        bool otherWasKinematic = false;

        if (thisRb != null)
        {
            thisWasKinematic = thisRb.isKinematic;
            thisRb.isKinematic = true;
        }
        if (otherRb != null)
        {
            otherWasKinematic = otherRb.isKinematic;
            otherRb.isKinematic = true;
        }

        transform.parent.GetComponent<Code>().next = otherCode;
        Transform nextCodeTrans = otherCode.transform;

        nextCodeTrans.SetParent(transform.parent, false);
        nextCodeTrans.localPosition = new Vector3(-1, 0, 0);
        nextCodeTrans.localEulerAngles = Vector3.zero;
        //nextCodeTrans.localScale = new Vector3(1, 1, 1);

        Physics.SyncTransforms();

        if (thisRb != null)
            thisRb.isKinematic = thisWasKinematic;
        if (otherRb != null)
            otherRb.isKinematic = otherWasKinematic;

        hasConnected = true;
    }
}
