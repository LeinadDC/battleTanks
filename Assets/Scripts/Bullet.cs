using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
	// Use this for initialization

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
