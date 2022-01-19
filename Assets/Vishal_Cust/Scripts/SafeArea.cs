using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : MonoBehaviour
{
    public RectTransform rectTransform;
    public Vector2 minAnchor, maxAnchor;
    public Rect safeArea;
    public CanvasScaler canvasScaler;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();



        Vector2 vec;
        vec.x = Screen.currentResolution.width;
        vec.y = Screen.currentResolution.height;
        if (canvasScaler)
            canvasScaler.referenceResolution = vec;
        safeArea = Screen.safeArea;
        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;


    }
}
