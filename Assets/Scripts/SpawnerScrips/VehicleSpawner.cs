using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] cars = new GameObject[4];

    [SerializeField][Range(370, 550)] float speedRange;
    [SerializeField] float colliiderSize;
    float minSpeed = 370;
    [SerializeField] float spawnDelay = 3f;
    float currentSpawnTime = 0f;

    private void Start()
    {
        SpawnCar();
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0f)
        {
            SpawnCar();
        }



    }

    void SpawnCar()
    {
        float randomSpeed = Random.Range(minSpeed, speedRange);
        int randomCar = Random.Range(0, cars.Length);

        Vector3 spawnPoint = transform.localPosition + Random.insideUnitSphere * colliiderSize;
        GameObject go = Instantiate(cars[randomCar], spawnPoint, transform.localRotation);
        go.GetComponent<Rigidbody>().AddForce((transform.forward) * speedRange);
        currentSpawnTime = spawnDelay;

    }



}
