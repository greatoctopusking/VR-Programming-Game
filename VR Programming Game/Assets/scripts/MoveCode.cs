using UnityEngine;

public class MoveCode : Code
{
    public float speed = 2f;
    private bool isMoving = false;
    public Transform target;
    private float movedDistance = 0f;
    public Vector3 direction = Vector3.right;
    public float targetDistance = 10f;

    override public void work()
    {
        if (isMoving) return;
        isMoving = true;
        movedDistance = 0f;
    }

    private void Update()
    {
        if (!isMoving) return;

        direction = direction.normalized;
        float step = speed * Time.deltaTime;
        step = Mathf.Min(step, targetDistance - movedDistance);
        target.Translate(direction * step, Space.World);
        movedDistance += step;

        if (movedDistance >= targetDistance)
        {
            isMoving = false;
            Complete();
        }
    }
}
