using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TankScript : MonoBehaviour {

    public float thrust;
    private Vector3 movement;
    private Transform barrel;
    private GameObject theTarget = null;
    public GameObject[] munition;
    private GameObject bullet;
    private State state;
    private float verticalBorder, horizontalBorder;
    public int tankLife = 100;
    private Text lifeText;
    private string playerId;
    public bool gameStarted;

    //TEST
    public int actionId = 1;
    public string actionType = "Atacar";
    public string movementCords;

    public enum State
    {
        Attack,
        Chase,
        Evade,
        Flee,
    }
    [Serializable]
    public class Player{
        public string playerId;
        public int tankLife;
    }
    // Variables initialization
    void Start() {
        barrel = transform.Find("Barrel");
        getObjective();
        verticalBorder = Camera.main.orthographicSize;
        horizontalBorder = verticalBorder * Screen.width / Screen.height;
    }

    public void Initialize(string playerId, int tankLife, float xPosition,float yPosition)
    {
        this.playerId = playerId;
        this.tankLife = tankLife;
        Vector3 originPosition = new Vector3(xPosition, yPosition, 0);
        transform.position = originPosition;
        
    }

    public void changeState(bool play)
    {
        this.gameStarted = play;
    }

    public Player jSONReady()
    {
        Player player = new Player();
        player.playerId = this.playerId;
        player.tankLife = this.tankLife;
        return player;
    }


    void postData()
    {
        movementCords = transform.position.ToString();
        var test = JsonUtility.ToJson(this);
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/testPost", test);
        www.SetRequestHeader("Content-Type", "application/json");
        www.Send();
        Debug.Log("Enviado : " + test);
    }

    void getData()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.98.131:5000/testGet");
        www.Send();
        Debug.Log("Descargado del servidor: "+ www.downloadHandler.text.ToString());
    }

    // Get the other (s) tank (s) in order to play against them. Can also be used to change objective in case there are many tanks.
   void getObjective()
    {
        var tanks = GameObject.FindGameObjectsWithTag("Enemy");
        do
        {
            theTarget = tanks[UnityEngine.Random.Range(0, tanks.Length)];
        } while (theTarget == transform.gameObject);

    }
    float tChange = 0;
    float randomX;
    float randomY;
    Vector3 movementR;
    // Frame Update
    void Update ()
    {
        if (gameStarted)
        {
            automaticMovement();
            shootEnemy(barrel.transform.position);
            LifeHandler();
        }
        else
        {
            
        }

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
    }

    private void automaticMovement()
    {
        var maxX = 6;
        var minX = -6;
        var maxY = 4.7;
        var minY = -4.7;
        generateRandomMovementRange();

        movementR = new Vector3(randomX, randomY, 0);
        //collisionControlDetection(maxX, minX, maxY, minY);
        float angle = Mathf.Atan2(movementR.y, movementR.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, movementR);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.Translate(movementR * thrust * Time.deltaTime);

        Vector3 direction = theTarget.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, -direction);
        Vector3 rotation = lookRotation.eulerAngles;
        barrel.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
    }

    private void collisionControlDetection(int maxX, int minX, double maxY, double minY)
    {
        if (transform.position.x >= maxX || transform.position.x <= minX)
        {
            movementR = new Vector3(randomX * -1, randomY, 0);
        }
        if (transform.position.y >= maxY || transform.position.y <= minY)
        {
            movementR = new Vector3(randomX, randomY * -1, 0);
        }
    }

    private void generateRandomMovementRange()
    {
        if (Time.time >= tChange)
        {
            randomX = UnityEngine.Random.Range(-1.0f, 2.0f);
            randomY = UnityEngine.Random.Range(-1.0f, 2.0f);
            tChange = Time.time + 2.5f;
        }
    }

    private void ManualMovement()
    {
        var axisX = Input.GetAxis("Horizontal");
        var axisY = Input.GetAxis("Vertical");
        movement.x = axisX;
        movement.y = axisY;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, movement);
        transform.Translate(new Vector3(movement.x, movement.y) * Time.deltaTime * thrust, Space.World);

        transform.position = Vector3.MoveTowards(transform.position, theTarget.transform.position, Time.deltaTime * thrust);
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
    void shootEnemy(Vector3 barrel)
    {
        var barrelPosition = barrel;
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
        Vector3 localOffset = new Vector3(0, -0.5f, 0);
        var worldOffset = barrel.transform.rotation * localOffset;
        var spawnPosition = barrel.transform.position + worldOffset;
        return Instantiate(munitionType, spawnPosition, barrel.transform.rotation);
        
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
           // postData();
           // getData();

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

        if (collision.gameObject.name == "LeftWall" || collision.gameObject.name == "RightWall")
        {
            Debug.Log("Cambiado x");
            movementR.x *= -1;
            transform.Translate(movementR * -1f * thrust * Time.deltaTime);
        }
        else if(collision.gameObject.name == "TopWall" || collision.gameObject.name == "BottomWall")
        {
            Debug.Log("Cambiado y");
            movementR.y *= -1;
            transform.Translate(movementR * - 1f * thrust * Time.deltaTime);
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
