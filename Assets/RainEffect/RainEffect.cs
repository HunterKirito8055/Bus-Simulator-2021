using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainEffect : MonoBehaviour
{
    public Sprite[] sprite;
    public Color color;
    public int particlePoolSize = 10;
    [Range(0.01f, 1f)]
    public float rainRate = 0.05f;

    float tempRate = 0f;
    List<Image> particles;
    int height, width;


    void Start()
    {
        height = Screen.height;
        width = Screen.width;
        particles = new List<Image>();
        Create();

    }
    private void OnEnable()
    {
        StartCoroutine(Play());
    }



    IEnumerator Play()
    {
        yield return null;
        while (true)
        {
            tempRate += Time.deltaTime;
            if (tempRate >= rainRate)
            {
                StartCoroutine(GetImageFromPool().gameObject.GetComponent<RainParticle>().CrossFade());
                tempRate = 0f;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    Image GetImageFromPool()
    {
        foreach (var item in particles)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                item.gameObject.transform.position = GetRandomPosition();
                StartCoroutine(item.gameObject.GetComponent<RainParticle>().CrossFade());
                return item;
            }
        }
        Create();

        return GetImageFromPool();
    }

    void Create()
    {
        for (int i = 0; i < particlePoolSize; i++)
        {
            GameObject newParticle = new GameObject("particle");
            newParticle.transform.parent = transform;
            newParticle.transform.position = GetRandomPosition();
            newParticle.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
            newParticle.transform.Rotate(Vector3.forward * Random.Range(0f, 180f));
            newParticle.AddComponent<Image>().sprite = sprite[Random.Range(0, sprite.Length)];
            newParticle.GetComponent<Image>().color = color;
            newParticle.AddComponent<RainParticle>();
            newParticle.SetActive(false);

            particles.Add(newParticle.GetComponent<Image>());
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(0, width), Random.Range(0, height), 0);
    }
}
