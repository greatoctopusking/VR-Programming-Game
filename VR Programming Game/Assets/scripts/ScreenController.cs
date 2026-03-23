using UnityEngine;
using TMPro;

public class ScreenController : MonoBehaviour
{
    public TMP_Text screenText;
    
    public void UpdateText(string message)
    {
        if (screenText != null)
        {
            screenText.text = message;
        }
    }
    
    public void ClearScreen()
    {
        if (screenText != null)
        {
            screenText.text = "";
        }
    }
    
    public void AppendText(string message)
    {
        if (screenText != null)
        {
            screenText.text += message + "\n";
        }
    }
}
