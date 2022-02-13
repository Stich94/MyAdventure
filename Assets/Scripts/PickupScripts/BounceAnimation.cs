using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAnimation : MonoBehaviour
{

    [SerializeField] float bounceSpeed = 8f;
    [SerializeField] float bounceAmplitude = 0.05f;
    [SerializeField] float rotationSpeed = 90f;

    float startingHeight;
    float timeOffset;

    void Start()
    {
        startingHeight = transform.localPosition.y;
        timeOffset = Random.value * Mathf.PI * 2;
    }

    void Update()
    {
        Bounce();
        Spin();
    }

    void Bounce()
    {
        float finalHeight = startingHeight + Mathf.Sin(Time.time * bounceSpeed + timeOffset) * bounceAmplitude;
        Vector3 position = transform.localPosition;
        position.y = finalHeight;
        transform.localPosition = position;
    }

    void Spin()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
}
