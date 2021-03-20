using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateArrows : MonoBehaviour
{
    [SerializeField] private RectTransform arrows;
    [SerializeField] private RectTransform finger;
    public float delta = 1;
    public float freq = 1;
    private Vector3 position;
    private void OnEnable()
    {
        position = finger.localPosition;
    }

    Vector2 eulers;
    private void LateUpdate()
    {
        Vector3 v = position;
        v.x += delta * Mathf.Sin(Time.time * freq);
        finger.localPosition = v;
        //transform.position = v;

        //position.x += Mathf.Cos(Time.fixedTime * Mathf.PI * freq) * amplitude;

        
    }
   
}
