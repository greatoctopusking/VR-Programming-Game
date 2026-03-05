using UnityEngine;

public class JudgerDetectCollider : MonoBehaviour
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

        if (GetJudger() != null)
        {
            return;
        }
        
        if (!other.CompareTag("BoolCodeBlock"))
        {
            return;
        }
        
        BoolCode otherCode = other.GetComponent<BoolCode>();
        if (otherCode == null)
        {
            return;
        }
        
        ConnectJudger(otherCode);
    }

    private void OnTriggerExit(Collider other)
    {
        hasConnected = false;
    }
    
    private void ConnectJudger(BoolCode judger)
    {
        Rigidbody thisRb = transform.parent.GetComponent<Rigidbody>();
        Rigidbody otherRb = judger.GetComponent<Rigidbody>();

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

        SetJudger(judger);
        
        judger.transform.SetParent(transform.parent, false);
        judger.transform.localPosition = new Vector3(0, 0, -1);
        judger.transform.localEulerAngles = Vector3.zero;
        judger.transform.localScale = new Vector3(1, 1, 1);
        
        Physics.SyncTransforms();

        if (thisRb != null)
            thisRb.isKinematic = thisWasKinematic;
        if (otherRb != null)
            otherRb.isKinematic = otherWasKinematic;
        
        hasConnected = true;
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
