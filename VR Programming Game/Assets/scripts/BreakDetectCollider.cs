using UnityEngine;

public class BreakDetectCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        WhileCode parentWhile = transform.parent.GetComponent<WhileCode>();
        if (parentWhile == null) return;
        if (!other.CompareTag("CodeBlock")) return;
        
        Code otherCode = other.GetComponent<Code>();
        if (otherCode == null) return;
        
        parentWhile.next = otherCode;
        
        otherCode.transform.SetParent(transform.parent, false);
        otherCode.transform.localPosition = new Vector3(-1, 0, 0);
        otherCode.transform.localEulerAngles = Vector3.zero;
        otherCode.transform.localScale = new Vector3(1, 1, 1);
        
        Physics.SyncTransforms();
    }
}
