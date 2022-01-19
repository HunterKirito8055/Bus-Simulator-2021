using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public GameObject notificationTxtPrefab;
    List<GameObject> poolingList = new List<GameObject>();

    private void Start()
    {
        createPool(6);
    }
    void createPool(int _size)
    {
        for (int i = 0; i < _size; i++)
        {
            var newGobj = Instantiate(notificationTxtPrefab, transform);
            newGobj.SetActive(false);
            poolingList.Add(newGobj);
        }
    }
    GameObject UseObjFromList()
    {
        foreach (var item in poolingList)
        {
            if (!item.activeSelf)
            {
                return item;
            }
        }
        createPool(4);
        return UseObjFromList();
    }
    NotificationText notificationTxt;
    public void DisplayNotification(string _messeage)
    {
        GameObject txtObj = UseObjFromList();
        txtObj.SetActive(true);
        notificationTxt = new NotificationText(txtObj);
        StartCoroutine(notificationTxt.NotificationMesseage(_messeage));

    }

    ////

}//class



public class NotificationText
{
    Text notificationText;
    GameObject txtGameObj;
    public NotificationText(GameObject _txtGameObj)
    {

        notificationText = _txtGameObj.GetComponent<Text>();
        txtGameObj = _txtGameObj;
    }
    public IEnumerator NotificationMesseage(string messeage)
    {
        txtGameObj.transform.SetAsLastSibling();
        notificationText.text = messeage;


        yield return new WaitForSecondsRealtime(1f);
        float alpha = 1f;
        float a = 1.3f;
        while (alpha > 0)
        {
            a -= Time.unscaledDeltaTime;
            alpha = Mathf.InverseLerp(0, 1f, a);
            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, alpha);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        txtGameObj.SetActive(false);
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1);
    }

}
