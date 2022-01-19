using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PassengerManager : MonoBehaviour
{

    public GameObject[] PassengersPrefabs;
    public GameObject[] passengersGameObjects;


    public Passenger[] passengersList;


    public Transform PassengersWaitingTransform;
    public Transform busPathTransform;
    public Transform seatLocationTransform;

    public float walkSpeed = 1.5f;
    Transform[] PassengersWaitingPoints;

    Transform[] busSeats;

    public Transform[] busPath;
    [Range(1, 8)]
    public int noOfPassengerToCreate = 1;


    public int totalPathCount = 0;
    public IEnumerator InitiateBusPoints()
    {
        noOfPassengerToCreate = UnityEngine.Random.Range(3, 8);
        yield return new WaitForSeconds(0.2f);
        PassengersWaitingTransform = GameObject.FindGameObjectWithTag(StringManager.PASSENGERPARENT).transform;

        busPathTransform = GameManager.BusSimulation.extraTag.busPathContainer.transform;
        seatLocationTransform = GameManager.BusSimulation.extraTag.seatingContainer.transform;

        InitializChildObjects(out PassengersWaitingPoints, PassengersWaitingTransform);// create waiting points
        InitializChildObjects(out busPath, busPathTransform); // create bus path
        InitializChildObjects(out busSeats, seatLocationTransform);  //create Seat Locations
        totalPathCount = busPath.Length;


        SetParentsAttributes(GameManager.BusSimulation.CurrentLevel.pickUp.transform);
        StartCoroutine(ICreatePassengers());
    }

    public void SetParentsAttributes(Transform toParent, bool reposition = false)
    {
        if (toParent != null)
        {
            PassengersWaitingTransform.forward = toParent.forward;
            PassengersWaitingTransform.position = new Vector3(toParent.position.x, toParent.position.y, toParent.position.z);
            if (toParent == GameManager.BusSimulation.activeBus.transform)
            {

                PassengersWaitingTransform.position = new Vector3(toParent.position.x, toParent.position.y - 1.08f, toParent.position.z);

            }
            if (reposition)
            {
                PassengersReposition();
            }

        }
    }


    //public void CallInPassengers()
    //{
    //    StartCoroutine(ICallGetInPassengers());
    //}
    //public void CallOutPassengers()
    //{

    //    StartCoroutine(ICallGetOffPassengers());
    //}
    public IEnumerator ICallGetInPassengers()
    {
        yield return null;
        foreach (var item in passengersList)
        {
            StartCoroutine(item.IGetInBus());

            float waitTimebyDistance = Vector3.Distance(item.thisTransform.position, busPath[0].position);
            waitTimebyDistance = Mathf.InverseLerp(minDistance, maxDistance, waitTimebyDistance);

            yield return new WaitForSeconds(waitTimebyDistance * 1.9f);
        }
        yield return StartCoroutine(CheckPassengerStatus(checkPick: true));
    }
    public IEnumerator ICallGetOffPassengers()
    {
        yield return null;
        foreach (var item in passengersList)
        {
            StartCoroutine(item.IGetOffBus());
            yield return new WaitForSeconds(1.2f);
        }
        yield return StartCoroutine(CheckPassengerStatus(checkPick: false));
    }



    float minDistance, maxDistance;
    IEnumerator ICreatePassengers() // create Passengers
    {
        yield return new WaitForSeconds(0.2f);
        if (passengersGameObjects.Length > 0)
        {
            for (int i = 0; i < passengersGameObjects.Length; i++)
            {
                Destroy(passengersGameObjects[i]);
            }
        }
        passengersList = new Passenger[noOfPassengerToCreate];
        passengersGameObjects = new GameObject[noOfPassengerToCreate];


        minDistance = Mathf.Infinity;
        maxDistance = 0;

        for (int i = 0; i < noOfPassengerToCreate; i++)
        {
            GameObject newPassenger = Instantiate(PassengersPrefabs[UnityEngine.Random.Range(0, PassengersPrefabs.Length)]);
            newPassenger.transform.position = PassengersWaitingPoints[i].position;
            newPassenger.transform.rotation = PassengersWaitingPoints[i].rotation;
            passengersGameObjects[i] = newPassenger;//passengerGameobjects

            Passenger newPassengerObject = new Passenger();
            newPassengerObject.InitializePassenger(newPassenger, busSeats[i], busPath, PassengersWaitingPoints[i]);
            passengersList[i] = newPassengerObject;//passengerScriptList
            yield return new WaitForSeconds(0.2f);

            float dis = Vector3.Distance(PassengersWaitingPoints[i].position, busPath[0].position);
            if (maxDistance < dis)
            {
                maxDistance = dis;
            }
            if (minDistance > dis)
            {
                minDistance = dis;
            }
        }
    }
    public void DestroyPassengers()
    {
        if (passengersGameObjects.Length > 0)
            foreach (var item in passengersGameObjects)
            {
                if (item != null)
                    Destroy(item);
            }
    }

    void PassengersReposition()
    {
        for (int i = 0; i < noOfPassengerToCreate; i++)
        {
            passengersList[i].thisTransform.position = PassengersWaitingPoints[i].position;
            passengersList[i].thisTransform.rotation = PassengersWaitingPoints[i].rotation;
        }

    }
    void InitializChildObjects(out Transform[] arrayTransform, Transform trans)
    {
        arrayTransform = new Transform[trans.childCount];
        for (int i = 0; i < trans.childCount; i++)
        {
            arrayTransform[i] = trans.GetChild(i);
        }
    }



    public List<Transform> startpositions;

    public bool everyOnePicked = false, everyOneDropped = false;






    public IEnumerator CheckPassengerStatus(bool checkPick)
    {
        if (checkPick)
            everyOnePicked = false;
        else
        {
            everyOneDropped = false;
        }

        ////// until the waituntil has true, the control execution of this ienumerator suspends
        yield return new WaitUntil(() =>
        {
            new WaitForSeconds(Time.fixedDeltaTime);

            foreach (var item in passengersList)
            {
                new WaitForSeconds(1f);
                if (checkPick ? !item.isPicked : !item.isDroped)
                {
                    return false;
                }
                return true;
            }
            return false;
        });

        if (checkPick)
            everyOnePicked = true;
        else
        {
            everyOneDropped = true;
        }

    }
    //public IEnumerator IsAllDropped()
    //{
    //    //// until the waituntil has true, the control execution of this ienumerator suspends
    //    yield return new WaitUntil(() =>
    //    {
    //        new WaitForSeconds(Time.deltaTime);

    //        foreach (var item in passengersList)
    //        {
    //            new WaitForSeconds(1f);
    //            if (!item.isDroped)
    //            {
    //                return false;
    //            }
    //            return true;
    //        }
    //        return false;
    //    });

    //    everyOneDropped = true;
    //}

    public void DisableColliders()
    {
        Debug.Log("Collider disable");
        foreach (var item in passengersGameObjects)
        {
            item.GetComponent<Collider>().enabled = false;
        }
    }

    public void EnableColliders()
    {
        Debug.Log("Collider enable");

        foreach (var item in passengersGameObjects)
        {
            item.GetComponent<Collider>().enabled = true;
        }
    }

}//class

//[System.Serializable]
//public class SeatContainer
//{
//    public Transform seat;
//    public bool isOccupy;
//}

/*
   public IEnumerator IPickPassengers()
    {
        yield return null;



        foreach (var item in passengersList)
        {
            int seatNum = UnoccupiedSeat();
            item.seatLocation = seating[seatNum].seat;
            seating[seatNum].isOccupy = true;

            item.newStart = startpositions[passengersList.IndexOf(item)];
            item.newStart.position = item.transform.position;
            item.newStart.rotation = item.transform.rotation;
            item.newStart.parent = RCC_SceneManager.Instance.activePlayerVehicle.transform;

            item.PickUpPassengers();
            item.seatNum = seatNum;
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    public IEnumerator IDropPassengers()
    {
        yield return null;
        foreach (var item in passengers)
        {

            //item.newStart = startpositions[passengers.IndexOf(item)];
            //item.newStart.position = item.transform.position;
            item.newStart.parent = RCC_SceneManager.Instance.activePlayerVehicle.transform;

            item.DropOffPassengers();
            seating[item.seatNum].isOccupy = false;
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
 */