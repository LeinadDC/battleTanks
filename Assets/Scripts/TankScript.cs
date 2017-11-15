using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankScript : MonoBehaviour {

    public float thrust;
    private Vector3 movement;
    public GameObject barrel;
    private GameObject theTarget = null;
    public GameObject[] munition;
    private GameObject bullet;

    public enum State
    {
        Attack,
        Chase,
        Evade,
        Flee,
    }

    void StateSwitch(State state)
    {
            switch (state)
            {
                case State.Attack:
                    shootEnemy();
                    break;
                case State.Chase:
                    break;
                case State.Evade:
                    break;
                case State.Flee:
                    Debug.Log("Escapando");
                    break;
            }
    }

    // Variables initialization
    void Start() {
        //UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/position",enemyTank.transform.position.ToString());
        //www.Send();
        getObjective();
    }

    // Get the other (s) tank (s) in order to play against them. Can also be used to change objective in case there are many tanks.
   void getObjective()
    {
        var tanks = GameObject.FindGameObjectsWithTag("Enemy");
        do
        {
            theTarget = tanks[Random.Range(0, tanks.Length)];
        } while (theTarget == transform.gameObject);

    }
    // Frame Update
    void Update () {
    var axisX = Input.GetAxis("Horizontal");
    var axisY = Input.GetAxis("Vertical");
    movement.x = axisX;
    movement.y = axisY;

    
    //transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
    //transform.Translate(new Vector3(movement.x, movement.y) * Time.deltaTime * thrust, Space.World);

    //transform.position = Vector3.MoveTowards(transform.position, theTarget.transform.position, Time.deltaTime * thrust);

    Vector3 direction = theTarget.transform.position - transform.position;
    Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, -direction);
    Vector3 rotation = lookRotation.eulerAngles;
    barrel.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
    shootEnemy();

        Collider2D hit = Physics2D.OverlapCircle(transform.position,0.4f);

            if (hit.tag == "Bullet")
            {
                Debug.Log("Objeto detectado");
                evadeEnemy();
            }

    }

    float basicTimer;
    float basicWaitingTime = 1f;
    int counterTest = 0;
    
    void evadeEnemy()
    {
        Vector3 moveDestination = new Vector3(-4.50f, 2.37f);
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * thrust);

        } while (transform.position != moveDestination);
    }
    void shootEnemy()
    {
        var barrelPosition = barrel.transform.position;
        var bullet = munition[0];
        basicTimer += Time.deltaTime;
        if(basicTimer > basicWaitingTime)
        {
            if(counterTest != 10 && counterTest != 25)
            {
                bullet = CreateBullet(munition[0],barrelPosition);
            }
            else if(counterTest % 10 == 0)
            {
                bullet = CreateBullet(munition[1], barrelPosition);
            }
            else if(counterTest % 25 == 0)
            {
                bullet = CreateBullet(munition[2], barrelPosition);

            }
            else
            {
                counterTest = 0;
            }
        }

    }

    private GameObject CreateBullet(GameObject munitionType,Vector3 barrelPosition)
    {
        GameObject bullet = InstantiateBullet(munitionType, barrelPosition);
        bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(0, -300f));
        basicTimer = 0;
        counterTest++;
        return bullet;
    }

    private GameObject InstantiateBullet(GameObject munitionType,Vector3 barrelPosition)
    {
        if (barrelPosition.x >= 0)
        {

            return Instantiate(munitionType, new Vector3(barrelPosition.x - 0.3f, barrelPosition.y, barrelPosition.z), barrel.transform.rotation);
        }
        else
        {
            return Instantiate(munitionType, new Vector3(barrelPosition.x + 0.3f, barrelPosition.y, barrelPosition.z), barrel.transform.rotation);
        }
    }


}
