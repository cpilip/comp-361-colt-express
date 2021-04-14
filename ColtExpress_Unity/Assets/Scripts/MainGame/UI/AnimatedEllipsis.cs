using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEllipsis : MonoBehaviour
{
    private string text;
    private int numEllipsis = 1;
    private int textLength;

    private bool isCompleted = false;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TMPro.TextMeshProUGUI>().text;
        textLength = text.Length;

        StartCoroutine("AnimateEllipsis");
    }

    public IEnumerator AnimateEllipsis()
    {
        while (isCompleted == false)
        {
            if (this.GetComponent<TMPro.TextMeshProUGUI>().text.Length > (textLength + numEllipsis))
            {
                this.GetComponent<TMPro.TextMeshProUGUI>().text = this.GetComponent<TMPro.TextMeshProUGUI>().text.Substring(0, textLength);
            }
            else
            {
                this.GetComponent<TMPro.TextMeshProUGUI>().text += ".";
            }

            yield return new WaitForSeconds(1);

            Debug.Log("HERE");
        }
    }

}
