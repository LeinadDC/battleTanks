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
    private State state;
    private float verticalBorder, horizontalBorder;
    public float tankLife = 100;

    public enum State
    {
        Attack,
        Chase,
        Evade,
        Flee,
    }

    // Variables initialization
    void Start() {
        //UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/position",enemyTank.transform.position.ToString());
        //www.Send();
        getObjective();
        verticalBorder = Camera.main.orthographicSize;
        horizontalBorder = verticalBorder * Screen.width / Screen.height;
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
    float tChange = 0;
    float randomX;
    float randomY;
    Vector3 movementR;
    // Frame Update
    void Update () {
    var axisX = Input.GetAxis("Horizontal");
    var axisY = Input.GetAxis("Vertical");
    movement.x = axisX;
    movement.y = axisY;
        var maxX = 6.9;
        var minX = -6.9;
        var maxY = 5.2;
        var minY = -5.2;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
        //transform.Translate(new Vector3(movement.x, movement.y) * Time.deltaTime * thrust, Space.World);

        //transform.position = Vector3.MoveTowards(transform.position, theTarget.transform.position, Time.deltaTime * thrust);
        // change to random direction at random intervals
        if (Time.time >= tChange)
        {
            randomX = Random.Range(-1.0f, 2.0f);
            randomY = Random.Range(-1.0f, 2.0f); 
            tChange = Time.time + 2f;
        }
        // if object reached any border, revert the appropriate direction
        if (transform.position.x >= maxX || transform.position.x <= minX)
        {
            movementR = new Vector3(-randomX, randomY, 0);
        }
        if (transform.position.y >= maxY || transform.position.y <= minY)
        {
            movementR = new Vector3(randomX, -randomY, 0);
        }
        movementR = new Vector3(randomX, randomY, 0);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, movementR);
        transform.Translate(movementR * thrust * Time.deltaTime);

        Vector3 direction = theTarget.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, -direction);
        Vector3 rotation = lookRotation.eulerAngles;
        barrel.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);

        //    Collider2D hit = Physics2D.OverlapCircle(transform.position,0.6f);
        //    var xPosition = hit.transform.position.x;
        //    var yPosition = hit.transform.position.x;
        //    if (hit.tag == "Bullet")
        //        {
        //            Debug.Log("Objeto detectado" + xPosition + yPosition);
        //        state = State.Chase;
        //        }

        //        if(state == State.Chase)
        //    {
        //        evadeEnemy(xPosition, yPosition);

        //    }
        shootEnemy();
        LifeHandler();
    }

    float basicTimer;
    float basicWaitingTime = 1f;
    int counterTest = 0;
    
    void evadeEnemy(float xPosition, float yPosition)
    {
        Vector3 moveDestination = new Vector3(-4.50f, 2.37f);
        if(xPosition <= 0)
        {
            //transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * thrust);
           transform.Translate(new Vector3(moveDestination.x, moveDestination.y) * Time.deltaTime * thrust, Space.World);
        }
        else
        {
            //transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * thrust);
            transform.Translate(new Vector3(moveDestination.x, moveDestination.y) * Time.deltaTime * thrust, Space.World);
        }
        //do
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, moveDestination, Time.deltaTime * thrust);

        //} while (transform.position != moveDestination);
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

            return Instantiate(munitionType, new Vector3(barrelPosition.x - 0.4f, barrelPosition.y, barrelPosition.z),barrel.transform.rotation);
        }
        else
        {
            return Instantiate(munitionType, new Vector3(barrelPosition.x + 0.4f, barrelPosition.y, barrelPosition.z), barrel.transform.rotation);
        }
    }

    int basicCounter = 0;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            basicCounter++;
            Debug.Log("Contador basico " + basicCounter);
            if(basicCounter == 50)
            {
                tankLife -= 25;
            }

        }else if(collision.gameObject.tag == "MediumBullet")
        {
            tankLife -= 50;
            Debug.Log("Impacto medio");
        }else if(collision.gameObject.tag == "HeavyBullet")
        {
            tankLife -= 100;
            Debug.Log("Impacto pesado");
        }
        else
        {
            Debug.Log("Impacto con otro objeto");
        }
    }

    void LifeHandler()
    {
        if(tankLife <= 0)
        {
            Destroy(gameObject);
        }
    }

}
