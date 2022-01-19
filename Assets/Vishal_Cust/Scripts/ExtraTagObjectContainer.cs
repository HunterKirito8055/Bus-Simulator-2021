using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTagObjectContainer : MonoBehaviour
{
    public GameObject frontDoor;
    public GameObject busPathContainer;
    public GameObject seatingContainer;
    public GameObject FPS_cams;

    public Animator animator;

    public void OpenDoor()
    {
        animator.SetTrigger(StringManager.OPENDOOR);
    }
    public void CloseDoor()
    {
        animator.SetTrigger(StringManager.CLOSEDOOR);
    }


}//class
