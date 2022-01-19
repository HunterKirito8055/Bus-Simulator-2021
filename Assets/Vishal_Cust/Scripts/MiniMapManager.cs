using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject miniCam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LateUpdate()
    {
        if (GameManager.BusSimulation.activeBus)
        {
            Transform pos = GameManager.BusSimulation.activeBus.transform;
            miniCam.transform.position = new Vector3(pos.position.x, miniCam.transform.position.y, pos.position.z);
            miniCam.transform.eulerAngles = new Vector3(90, pos.eulerAngles.y, 0);
        }
    }
}
