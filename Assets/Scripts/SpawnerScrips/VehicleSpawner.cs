using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] cars = new GameObject[4];

    [SerializeField] [Range(1, 10)] float speed;
    float randomSpeed;
    float randomSpawnTime;
    float maxCarToSpawn;

    float spawnDelay = 1.5f;
    float currentSpawnTime = 0f;
    BoxCollider collider;

    Vector3 colliderSize;

    int randomCarCounter = 0;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        colliderSize = collider.size;
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
        float randomSpeed = Random.Range(1, speed);

        Vector3 randomSpawn = new Vector3(Random.Range(-colliderSize.x, colliderSize.x),
        Random.Range(-colliderSize.y, colliderSize.y),
        Random.Range(-colliderSize.z, colliderSize.z)
        );

        int randomCar = Random.Range(0, cars.Length);
        GameObject go = Instantiate(cars[randomCar], transform.position, Quaternion.identity);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * randomSpeed * Time.deltaTime;
        currentSpawnTime = 10f;
        randomCarCounter++;

    }

}
