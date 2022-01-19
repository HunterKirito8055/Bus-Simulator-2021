using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickDropArea : MonoBehaviour
{
    public bool fl, fr, rr, rl;


    public BusStopMode busStopMode;

    void Start()
    {
    }
    //private void OnEnable()
    //{
    //    if (defaultMat)
    //        meshRenderer.material = defaultMat;
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringManager.FL))
        {
            fl = true;
        }
        if (other.CompareTag(StringManager.FR))
        {
            fr = true;
        }
        if (other.CompareTag(StringManager.RR))
        {
            rr = true;
        }
        if (other.CompareTag(StringManager.RL))
        {
            rl = true;
        }

        CheckStatus();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringManager.FL))
        {
            fl = false;
        }
        if (other.CompareTag(StringManager.FR))
        {
            fr = false;
        }
        if (other.CompareTag(StringManager.RR))
        {
            rr = false;
        }
        if (other.CompareTag(StringManager.RL))
        {
            rl = false;
        }
        CheckStatus();
    }

    bool IsinArea
    {
        get
        {
            return GameManager.BusSimulation.isInArea;
        }
        set
        {
            GameManager.BusSimulation.isInArea = value;
        }
    }
    void CheckStatus()
    {

        IsinArea = this.rl & this.rr & this.fl & this.fr;

        if (IsinArea)
        {
            //meshRenderer.material = GameManager.BusSimulation.greenMat;

            GameManager.BusSimulation.WaitTillPickUp(this.busStopMode);

        }
        else
        {
            StopCoroutine(GameManager.BusSimulation.IWaitTillBusStops());
            //meshRenderer.material = GameManager.BusSimulation.redMat;
            IsinArea = false;
        }
        //if (!this.rl & !this.rr & !this.fl & !this.fr)
        //{
        //    meshRenderer.material = defaultMat;
        //}

    }

    public void EnableArea()
    {
        this.gameObject.SetActive(true);
    }
    public void DisableArea()
    {
        this.gameObject.SetActive(false);
    }
}
[System.Serializable]
public enum BusStopMode
{
    PICKUP,
    DROPOFF,
    NONE
}