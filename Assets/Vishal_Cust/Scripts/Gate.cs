using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject _gate;
    public Animator anim;
    public bool isGateOpen;
    public float speed = 3f;


    private void Start()
    {
        anim = GetComponent<Animator>();
        if (_gate != null)
        {
            return;
        }
        _gate = System.Array.Find(GetComponentsInChildren<Transform>(), x => x.CompareTag(StringManager.GATE)).gameObject;

        isGateOpen = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringManager.PLAYER))
        {
            if (isGateOpen)
            {
                return;
            }
            anim.SetTrigger(StringManager.OPENGATE);
            isGateOpen = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringManager.PLAYER))
        {
            if (!isGateOpen)
            {
                return;
            }
            anim.SetTrigger(StringManager.CLOSEGATE);
            isGateOpen = false;
        }
    }





}
