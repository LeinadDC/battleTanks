using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    // Use this for initialization

    public GameObject[] bullets;
    GameObject bulletSpawned;
    void Start()
    {
        InvokeRepeating("Spawn", 3, 3);
    }

    void Spawn()
    {
        // Find a random index between zero and one less than the number of spawn points.
        int bulletToSpawn = Random.Range(0, bullets.Length);
        var x = Random.Range(-5, 5);
        var y = Random.Range(-5, 5);
        var z = 0;
        var pos = new Vector3(x, y, z);
        transform.position = pos;

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        bulletSpawned = Instantiate(bullets[bulletToSpawn], pos, transform.rotation);
    }

     void FixedUpdate()
    {
        bulletSpawned.transform.RotateAround(transform.position, new Vector3(0, 0, 1), 5);
    }
}
