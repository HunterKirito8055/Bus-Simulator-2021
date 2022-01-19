using System.Collections;
using UnityEngine;
using TMPro;

public class PopUpMenu : MonoBehaviour
{
    public void PopUpMessage(string _message)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(IPopUpMessage(_message));
    }
    IEnumerator IPopUpMessage(string _message)
    {

        this.GetComponentInChildren<TextMeshProUGUI>().text = _message;
        yield return new WaitForSecondsRealtime(1f);
        this.gameObject.SetActive(false);
    }
}
