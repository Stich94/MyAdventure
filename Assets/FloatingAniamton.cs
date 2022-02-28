using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAniamton : MonoBehaviour
{
    [SerializeField] float bounceSpeed = 1.5f;
    [SerializeField] float bounceAmplitude = 0.05f;


    float startingHeight;
    float timeOffset;

    void Start()
    {
        startingHeight = transform.localPosition.y;
        timeOffset = Random.value * Mathf.PI * 2;
    }


    // Update is called once per frame
    void Update()
    {
        Bounce();
    }

    void Bounce()
    {
        float finalHeight = startingHeight + Mathf.Sin(Time.time * bounceSpeed + timeOffset) * bounceAmplitude;
        Vector3 position = transform.localPosition;
        position.y = finalHeight;
        transform.localPosition = position;
    }
}
