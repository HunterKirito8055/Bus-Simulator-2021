using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BusSpawnAndRotate : MonoBehaviour
{
    public float speed;
    private void Awake()
    {

        //GameManager.UiManager.busProperties = new BusItem[GameManager.UiManager.busProperties.Length];
        //for (int i = 0; i < GameManager.UiManager.busProperties.Length; i++)
        //{
        //    GameObject g = Instantiate(GameManager.UiManager.busProperties[i].busObject, this.transform);
        //    g.transform.localPosition = Vector3.zero;
        //    g.transform.localRotation = Quaternion.identity;

        //    BusItem busItem = new BusItem();
        //    busItem.busObject = g;

        //    busItem.price = GameManager.UiManager.busProperties[i].price;
        //    GameManager.UiManager.busProperties[i] = busItem;

        //}
    }

    private void FixedUpdate()
    {
        //this.transform.RotateAround(new Vector3(0, 1, 0), speed * Time.deltaTime);
        this.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
    }
}
