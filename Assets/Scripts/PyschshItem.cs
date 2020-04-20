using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PyschshItem : MonoBehaviour
{
    public float translationSpeed = 100f;
    public float duration = 4f;
    public float removalOffset = 1f;
    public float translationSpeedLossFactor = 1.5f;
    public Vector3 translationAxis = Vector2.up;


    private Transform mTransform;
    private Vector3 oldPosition;

    public TMP_Text Label;
    public Color c = Color.white;
    private void OnEnable()
    {
        mTransform = gameObject.transform;
        oldPosition = gameObject.transform.position;
        Label = gameObject.GetComponent<TMP_Text>();
        Label.color = Color.white;
        StartCoroutine(PyschshAnimation());
    }

    private void OnDisable()
    {
        mTransform.position = oldPosition;
        Label.alpha = 1.0f;
    }

    private float timer;
    private float currentTranslationSpeed;
    private IEnumerator PyschshAnimation()
    {
        timer = 0f;
        currentTranslationSpeed = translationSpeed;
        while (timer < duration)
        {
            yield return null;
            mTransform.position = mTransform.position + translationAxis * currentTranslationSpeed * Time.deltaTime;
            
            if(timer >= removalOffset)
            {
                Label.alpha -= 0.8f / (duration - removalOffset) * Time.deltaTime;
                
            }
            currentTranslationSpeed = currentTranslationSpeed >= 0 ? currentTranslationSpeed - (currentTranslationSpeed* translationSpeedLossFactor / (duration - removalOffset)) * Time.deltaTime : 0;
            //currentTranslationSpeed = currentTranslationSpeed >= 0 ? currentTranslationSpeed - 1f / (duration - removalOffset) : 0;// * Time.deltaTime;
            timer += Time.deltaTime;
        }

        DestroyImmediate(gameObject);// ameObject.SetActive(false);
    }
}
