using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour {

    public float thrust;
    public Rigidbody2D r2d;
    public Vector3 movement;
    public GameObject enemyTank;
    public GameObject barrel;
    private GameObject target;
	// Variables initialization
	void Start () {
        r2d = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Enemy");
	}
	
	// Frame Update
	void Update () {
        var axisX = Input.GetAxis("Horizontal");
        var axisY = Input.GetAxis("Vertical");
        movement.x = axisX;
        movement.y = axisY;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
        transform.Translate(new Vector3(movement.x, movement.y) * Time.deltaTime * thrust, Space.World);
        /***
        Vector3 targetPoint = enemyTank.transform.position - transform.position;

        float rotationZ = Mathf.Atan2(targetPoint.y, targetPoint.x) * Mathf.Rad2Deg;

        barrel.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -rotationZ);
        ***/
        Vector3 direction = enemyTank.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, -direction);
        Vector3 rotation = lookRotation.eulerAngles;
        barrel.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
    }
}
