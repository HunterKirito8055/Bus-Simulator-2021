using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public DamageType damageType;
    public AnimationCurve damageCurve;
    public float minSpeedforDamage = 25f, maxSpeedforDamage = 60f;

    private void Start()
    {
        gameObject.GetComponent<Collider>().isTrigger =
             damageType == DamageType.Trigger ? true : false;
    }


    private void FixedUpdate()
    {
        gameObject.GetComponent<Collider>().isTrigger =
            damageType == DamageType.Trigger ? true : false;
    }

    public float busSpeedatEnter = 0, busSpeedatExit = 0;

    bool badRoadEntered = false;

    float speedCheckFrequency = 0.2f;
    public float averageSpeed = 0;
    public List<float> speeds = new List<float>();

    float damageToApply;



    //  bool isEnter, isExit;

    //  public bool triggerDamage, collisionDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.damageType == DamageType.Trigger)
            {
                //triggerDamage = true;
                //collisionDamage = false;

                badRoadEntered = true;
                busSpeedatEnter = CurrentBusSpeed;

                if (GameManager.BusSimulation.IsPickUp) // damage passengers only if they are picked
                    StartCoroutine(ICheckForDamages());

            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (badRoadEntered)
            {
                busSpeedatExit = CurrentBusSpeed;


                badRoadEntered = false;
            }

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {

            if (this.damageType == DamageType.Collision)
            {
                busSpeedatEnter = CurrentBusSpeed;

                //collisionDamage = true;
                //triggerDamage = false;
                Debug.Log("enter");
            }

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {

            if (this.damageType == DamageType.Collision)
            {
                busSpeedatExit = CurrentBusSpeed;
                hitCount++;
                StartCoroutine(ICheckForDamages());
                Debug.Log("Exit");

            }

        }
    }


    public float CurrentBusSpeed
    {
        get
        {
            return GameManager.BusSimulation.activeBus.speed;
        }
    }

    public int hitCount;
    IEnumerator ICheckForDamages()
    {
        yield return null;
        if (GameManager.BusSimulation.IsPickUp)
        {
            if (this.damageType == DamageType.Trigger)
            {
                while (badRoadEntered) // if on bad road
                {
                    speedCheckFrequency -= Time.deltaTime;
                    speeds.Add(CurrentBusSpeed);
                    yield return new WaitForSeconds(speedCheckFrequency);
                }
                for (int i = 0; i < speeds.Count; i++)
                {
                    averageSpeed += speeds[i];
                }
                averageSpeed /= speeds.Count;

            }
            if (this.damageType == DamageType.Collision)
            {
                averageSpeed = busSpeedatEnter; /*+ busSpeedatExit) / 2*/
                //damageToApply = Mathf.InverseLerp(this.minSpeedforDamage, this.maxSpeedforDamage, averageSpeed);
                //Debug.Log("Damage Inverse ==>" + damageToApply);
                //damageToApply = this.damageCurve.Evaluate(damageToApply);
            }
            damageToApply = Mathf.InverseLerp(this.minSpeedforDamage, this.maxSpeedforDamage, averageSpeed);
            Debug.Log("Damage Inverse ==>" + damageToApply);
            damageToApply = this.damageCurve.Evaluate(damageToApply) * 0.15f;


            GameManager.BusSimulation.ApplyDamage(damageToApply);
            //isExit = isEnter = false;
        }
    }



}//class

[System.Serializable]
public enum DamageType
{
    Collision,
    Trigger,
    None
}
