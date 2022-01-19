using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger
{
    public int seatNum;
    public Mode mode;
    public float WalkSpeed
    {
        get
        {
            return GameManager.PassengerManager.walkSpeed;
        }
    }
    public bool isPicked, isDroped;

    public Transform seatLocation;
    public Transform startLocationTransform;
    [SerializeField] Animator animator;
    Vector3 heightOffset;
    string sit = "Sit", walk = "Walk", stairs = "Stairs", idle = "Idle";

    public Transform busPathObject;
    public Transform[] path;
    //public int totalPaths = 0;
    public Vector3 nextPosition;
    // Vector3 startPosition;
    public GameObject thisObject;
    public Transform thisTransform;
    bool isBusy = false;

    public Transform currentPoint;
    public void InitializePassenger(GameObject _gameObject, Transform _seatTransform, Transform[] _path, Transform _startTransform)
    {
        thisObject = _gameObject;
        thisTransform = thisObject.transform;
        seatLocation = _seatTransform;
        path = new Transform[_path.Length];
        path = _path;
        animator = thisObject.GetComponent<Animator>();
        mode = Mode.idle;
        nextPosition = path[0].localPosition;
        startLocationTransform = _startTransform;
    }


    public void ResetAgain()
    {
        SwitchToMode(Mode.idle);
        thisTransform.position = startLocationTransform.position;
    }

    public IEnumerator IGetInBus()
    {
        isPicked = false;

        thisTransform.SetParent(GameManager.BusSimulation.activeBus.transform);
        //GameManager.PassengerManager.SetParentsAttributes(GameManager.PickDropManager.activeBus.transform);
        yield return null;
        int i = 0;
        // startPosition = thisTransform.position;
        SwitchToMode(Mode.walking);
        while (i != GameManager.PassengerManager.totalPathCount - 1)
        {
            //  heightOffset =
            currentPoint = GameManager.PassengerManager.busPath[i];
            nextPosition = GameManager.PassengerManager.busPath[i].position;
            if (i > 0 && i < 4)
            {
                SwitchToMode(Mode.stairs);

            }
            else
            {
                SwitchToMode(Mode.walking);
            }
            while (Vector3.Distance(nextPosition, thisTransform.position) > 0.2f)
            {
                Vector3 rotDir = nextPosition - thisTransform.position;
                rotDir.y = 0;
                Quaternion rot = Quaternion.LookRotation(rotDir);
                thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
                thisTransform.position = Vector3.MoveTowards(thisTransform.position, nextPosition, Time.deltaTime * WalkSpeed);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            i++;
        }

        SwitchToMode(Mode.walking);

        nextPosition = seatLocation.position;

        while (Vector3.Distance(nextPosition, thisTransform.position) > 0.2f)
        {

            Vector3 rotDir = nextPosition - thisTransform.position;
            rotDir.y = 0;
            Quaternion rot = Quaternion.LookRotation(rotDir);
            thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, nextPosition, Time.deltaTime * WalkSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        SwitchToMode(Mode.idle);

        float sittingTime = 1f;
        while (sittingTime > 0)
        {
            sittingTime -= Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(seatLocation.forward);
            thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
        }

        SwitchToMode(Mode.sit);
        isPicked = true;

    }
    public IEnumerator IGetOffBus()
    {
        isDroped = false;
        yield return null;
        this.thisTransform.SetParent(GameManager.PassengerManager.transform);

        int i = GameManager.PassengerManager.totalPathCount - 1;
        while (i > -1)
        {
            currentPoint = GameManager.PassengerManager.busPath[i];

            nextPosition = GameManager.PassengerManager.busPath[i].position;
            if (i > 2 && i < 4)
            {
                SwitchToMode(Mode.stairs);
            }
            else
            {
                SwitchToMode(Mode.walking);
            }
            while (Vector3.Distance(nextPosition, thisTransform.position) > 0.05f && !GameManager.BusSimulation.isLevelDone)
            {
                //print("while " + i);

                Vector3 rotDir = nextPosition - thisTransform.position;
                rotDir.y = 0;
                Quaternion rot = Quaternion.LookRotation(rotDir);
                thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
                thisTransform.position = Vector3.MoveTowards(thisTransform.position, nextPosition, Time.deltaTime * WalkSpeed);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            i--;
        }
        SwitchToMode(Mode.walking);

        nextPosition = startLocationTransform.position;
        while (Vector3.Distance(nextPosition, thisTransform.position) > 0.2f && !GameManager.BusSimulation.isLevelDone)
        {

            Vector3 rotDir = nextPosition - thisTransform.position;
            rotDir.y = 0;
            Quaternion rot = Quaternion.LookRotation(rotDir);
            thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
            thisTransform.position = Vector3.MoveTowards(thisTransform.position, nextPosition, Time.deltaTime * WalkSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isDroped = true;
        SwitchToMode(Mode.idle);
        float sittingTime = 1f;
        while (sittingTime > 0)
        {
            sittingTime -= Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(seatLocation.forward);
            thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, rot, Time.deltaTime * 10f);
        }
        SwitchToMode(Mode.idle);
        thisTransform.forward = startLocationTransform.forward;
        //  print("done");
    }

    void SwitchToMode(Mode _mode)
    {
        mode = _mode;
        bool _walk = false;
        bool _sit = false;
        bool _stairs = false;
        bool _idle = false;
        switch (mode)
        {
            case Mode.idle:
                _idle = true;
                break;
            case Mode.walking:
                _walk = true;
                break;
            case Mode.stairs:
                _stairs = true;
                break;
            case Mode.sit:
                _sit = true;
                break;
            default:
                break;
        }

        animator.SetBool(walk, _walk);
        animator.SetBool(sit, _sit);
        animator.SetBool(stairs, _stairs);
        animator.SetBool(idle, _idle);
    }
}

[System.Serializable]
public enum Mode
{
    idle,
    walking,
    stairs,
    sit
}
