using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
	// Use this for initialization

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "MediumBullet" || collision.gameObject.tag == "HeavyBullet")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
