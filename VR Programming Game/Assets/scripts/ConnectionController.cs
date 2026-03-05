using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [Header("组件引用")]
    public WhileCode whileCode;
    public Transform backModule;
    
    [Header("尺寸设置")]
    public float baseLength = 0.5f;
    public float lengthPerCode = 0.5f;
    
    [Header("初始位置")]
    public float initialBackX = -2f;
    
    private void Start()
    {
        if (whileCode == null)
        {
            whileCode = GetComponentInParent<WhileCode>();
        }
        
        Refresh();
    }
    
    public void Refresh()
    {
        if (whileCode == null)
        {
            return;
        }
        
        int codeCount = CountCodeBlocks(whileCode.LoopNext);
        float newLength = baseLength + codeCount * lengthPerCode;
        
        transform.localScale = new Vector3(
            transform.localScale.x,
            newLength,
            transform.localScale.z
        );
        
        float newConnectionX = -1f - codeCount * 0.5f;
        
        transform.localPosition = new Vector3(
            newConnectionX,
            transform.localPosition.y,
            transform.localPosition.z
        );
        
        if (backModule != null)
        {
            float newBackX = initialBackX - codeCount;
            
            backModule.localPosition = new Vector3(
                newBackX,
                backModule.localPosition.y,
                backModule.localPosition.z
            );
        }
    }
    
    private int CountCodeBlocks(Code start)
    {
        int count = 0;
        Code current = start;
        
        while (current != null)
        {
            count++;
            current = current.next;
            if (count > 100) break;
        }
        
        return count;
    }
}
