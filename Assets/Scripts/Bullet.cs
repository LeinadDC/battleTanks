using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    public Rigidbody2D rigid2d;
	// Use this for initialization
	void Start () {

       rigid2d = GetComponent<Rigidbody2D>();
		
	}

	
	// Update is called once per frame
	void Update () {
        //rigid2d.velocity = new Vector2(speed, rigid2d.velocity.y);




    }

    void OnCollisionEnter2D()
    {
            Destroy(gameObject);
    }
}
