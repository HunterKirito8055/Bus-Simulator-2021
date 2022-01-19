using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainParticle : MonoBehaviour
{
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator CrossFade()
    {
        yield return null;

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        float wait = 1.8f;

        image.CrossFadeAlpha(0, wait, transform);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y-15f, transform.position.z);
        while (wait > 0)
        {
            wait -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
