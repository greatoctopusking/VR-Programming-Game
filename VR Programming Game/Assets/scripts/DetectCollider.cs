using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DetectCollider : MonoBehaviour
{
    public Collider Detect;
    private bool hasConnected = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasConnected) return;

        Code myCode = transform.parent.GetComponent<Code>();
        Code otherCode = other.GetComponentInParent<Code>();

        if (myCode == null || otherCode == null || myCode == otherCode) return;

        if (myCode.next != null) return;

        XRGrabInteractable myGrab = transform.parent.GetComponent<XRGrabInteractable>();
        if (myGrab != null && myGrab.isSelected) return;

        XRGrabInteractable otherGrab = otherCode.GetComponent<XRGrabInteractable>();
        if (otherGrab != null && otherGrab.isSelected) return;

        if (!other.CompareTag("CodeBlock") && !otherCode.CompareTag("CodeBlock")) return;

        ConnectCode(myCode, otherCode);
    }

    private void ConnectCode(Code self, Code otherCode)
    {
        Rigidbody otherRb = otherCode.GetComponent<Rigidbody>();
        Collider selfCol = self.GetComponent<Collider>();
        Collider otherCol = otherCode.GetComponent<Collider>();

        if (selfCol != null && otherCol != null)
        {
            Physics.IgnoreCollision(selfCol, otherCol, true);
        }

        if (otherRb != null)
        {
            otherRb.isKinematic = true;
            otherRb.useGravity = false;
            otherRb.detectCollisions = false;
        }

        if (otherCol != null)
        {
            otherCol.isTrigger = false;
        }

        self.next = otherCode;
        otherCode.transform.SetParent(self.transform, false);

        hasConnected = true;
        StartCoroutine(SmoothConnect(otherCode));
    }

    private IEnumerator SmoothConnect(Code otherCode)
    {
        Vector3 targetPos = new Vector3(-1f, 0, 0);
        Vector3 startPos = otherCode.transform.localPosition;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            otherCode.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        otherCode.transform.localPosition = targetPos;
        otherCode.transform.localEulerAngles = Vector3.zero;
        otherCode.transform.localScale = Vector3.one;
        Physics.SyncTransforms();
    }
}
