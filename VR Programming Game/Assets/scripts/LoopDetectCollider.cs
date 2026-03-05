using UnityEngine;

public class LoopDetectCollider : MonoBehaviour
{
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

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

        WhileCode parentWhile = transform.parent.GetComponent<WhileCode>();
        if (parentWhile == null)
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
        
        ConnectLoopNext(parentWhile, otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        hasConnected = false;
    }

    private void ConnectLoopNext(WhileCode parentWhile, Code otherCode)
    {
        if (parentWhile.LoopNext != null)
        {
            Debug.Log($"[LoopDetect] LoopNext已存在: {parentWhile.LoopNext.name}");
            return;
        }

        Debug.Log($"[LoopDetect] 连接: {transform.parent.name} -> {otherCode.name}");

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

        parentWhile.LoopNext = otherCode;
        
        otherCode.transform.SetParent(transform.parent, false);
        otherCode.transform.localPosition = new Vector3(-1, 0, 0);
        otherCode.transform.localEulerAngles = Vector3.zero;
        otherCode.transform.localScale = new Vector3(1, 1, 1);
        
        Physics.SyncTransforms();

        if (thisRb != null)
            thisRb.isKinematic = thisWasKinematic;
        if (otherRb != null)
            otherRb.isKinematic = otherWasKinematic;
        
        ConnectionController controller = transform.parent.GetComponentInChildren<ConnectionController>();
        if (controller != null)
        {
            controller.Refresh();
        }
        
        hasConnected = true;
    }
}
