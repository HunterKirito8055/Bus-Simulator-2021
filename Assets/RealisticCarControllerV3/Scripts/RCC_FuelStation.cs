//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2020 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCC_FuelStation : MonoBehaviour
{

    public RCC_CarControllerV3 targetVehicle;
    public float refillSpeed = 1f;
    public bool canRefuel
    {
        get { return GameManager.BusSimulation.IsFilling; }
        set
        {
            GameManager.BusSimulation.IsFilling = value;
        }
    }
    void OnTriggerStay(Collider col)
    {

        if (targetVehicle == null)
        {

            if (col.gameObject.GetComponentInParent<RCC_CarControllerV3>())
                targetVehicle = col.gameObject.GetComponentInParent<RCC_CarControllerV3>();

        }

        if (targetVehicle)
        {
            GameManager.BusSimulation.fuelToggleBtn.SetActive(true);
            if (canRefuel)
            {
                targetVehicle.FuelTank += refillSpeed * Time.deltaTime;
                if (GameManager.BusSimulation.FuelAmount >= 0.85f)
                {
                    //should send notification  that Refueling is not needed
                    canRefuel = false;
                }
            }
        }

    }

    void OnTriggerExit(Collider col)
    {

        if (col.gameObject.GetComponentInParent<RCC_CarControllerV3>())
        {
            GameManager.BusSimulation.fuelToggleBtn.SetActive(false);
            targetVehicle = null;
            GameManager.BusSimulation.ShowMainHUD(true, true);

        }

    }

}
