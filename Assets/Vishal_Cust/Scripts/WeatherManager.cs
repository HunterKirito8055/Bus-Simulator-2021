using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public Transform rainSystem, snowSystem;
    public Weather_Controller weather_Controller;

    private void Start()
    {
        if (weather_Controller == null)
        {
            weather_Controller = GetComponentInChildren<Weather_Controller>();
        }
        weather_Controller.en_CurrWeather = Weather_Controller.WeatherType.SUN;
        weather_Controller._bChangeWeather = true;
    }
    private void LateUpdate()
    {
        if (GameManager.BusSimulation.activeBus)
        {
            rainSystem.position = Vector3.Lerp(rainSystem.position, new Vector3(GameManager.BusSimulation.activeBus.transform.position.x, 50, GameManager.BusSimulation.activeBus.transform.position.z), Time.deltaTime * 5f);// new Vector3(GameManager.BusSimulation.activeBus.transform.position.x, 50, GameManager.BusSimulation.activeBus.transform.position.z);
            snowSystem.position = new Vector3(GameManager.BusSimulation.activeBus.transform.position.x, 50, GameManager.BusSimulation.activeBus.transform.position.z);
        }
    }
}
