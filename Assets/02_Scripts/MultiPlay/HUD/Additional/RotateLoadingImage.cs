using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateLoadingImage : MonoBehaviour 
{
    RectTransform rectTransformComponent;
    Image loadingImage;
    float rotateSpeed = -300f;

    void Start() 
    {
        rectTransformComponent = GetComponent<RectTransform>();
        loadingImage = rectTransformComponent.GetComponent<Image>();
    }

    void Update()
    {
        rectTransformComponent.Rotate(0f, 0f, (rotateSpeed * Time.deltaTime));
    }
}
