using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    // Clase interna usada para manejar las sesiones.
    [Serializable]
    public class GameServer
    {
        public string gameId;
        public TankScript.Player[] players;
        public string gameState;
        public string gameWinner;

        public GameServer(string gameId, TankScript.Player[] players,string gameState,string gameWinner)
        {
            this.gameId = gameId;
            this.players = players;
            this.gameState = gameState;
            this.gameWinner = gameWinner;
        }
    }

    
    public GameObject player1;
    public GameObject player2;
    TankScript enemy;       
    TankScript enemy2;
    TankScript.Player[] playersArray = new TankScript.Player[2];
    GameServer[] activeServer = new GameServer[1];
    public GameObject count;
    Text countdown;
    private GameObject theTarget = null;
    public GameObject winner;
    Text winnerText;
    public GameObject objectButton;
    public Button playAgain;

    //Inicia un nuevo servidor de juego.
    private void Start()
    {
        countdown = count.GetComponent<Text>();
        winnerText = winner.GetComponent<Text>();
        playAgain = objectButton.GetComponent<Button>();
        activeServer = createGameSession();
        playAgain.onClick.AddListener(restartGame);
    }

    private void restartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    float timeLeft = 3.0f;
    //Construye los datos del servidor.
    void Update() {
        timeLeft -= Time.deltaTime;
        countdown.text = Mathf.Round(timeLeft).ToString();

        if(timeLeft < 0)
        {
            count.SetActive(false);
            gameBuilder();
            activateAndPlay();
            if(enemy == null || enemy2 == null)
            {
                if (enemy == null)
                {
                    deactivateAndStop();
                    winner.SetActive(true);
                    objectButton.SetActive(true);
                    winnerText.text = "Ganó el jugador 2";
                    activeServer[0].gameWinner = activeServer[0].players[1].playerId;
                    activeServer[0].gameState = "Finished";
                    putData(activeServer[0].gameId, jsonConverter(activeServer));
                    enabled = false;

                }
                else if(enemy2 == null){
                    deactivateAndStop();
                    winner.SetActive(true);
                    objectButton.SetActive(true);
                    winnerText.text = "Ganó el jugador 1";
                    activeServer[0].gameWinner = activeServer[0].players[0].playerId;
                    activeServer[0].gameState = "Finished";
                    putData(activeServer[0].gameId, jsonConverter(activeServer));
                    enabled = false;
                }

            }
            else
            {
               
            }

        }

    }

    // Get the other (s) tank (s) in order to play against them. Can also be used to change objective in case there are many tanks.
    void activateAndPlay()
    {
        var tanks = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var tank in tanks)
        {
            tank.SetActive(true);
            var test = tank.GetComponent<TankScript>();
            test.changeState(true);
        }

    }

    void deactivateAndStop()
    {
        var tanks = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var tank in tanks)
        {
            tank.SetActive(false);
            var test = tank.GetComponent<TankScript>();
            test.changeState(false);
        }

    }


    void gameBuilder()
    {
  
        //Verifica que todo está bien.
            if(activeServer[0].gameState  == "Waiting")
            {
            //Instancia jugadores, cambia de estado y actualiza información en servidor.
                instantiatePlayers(activeServer);
                activeServer[0].gameState = "Begun";
                putData(activeServer[0].gameId, jsonConverter(activeServer));
                getData(activeServer[0].gameId);
                Debug.Log(jsonConverter(activeServer));

            }
            //Les señala a los tanques que ya pueden iniciar.
            else if(activeServer[0].gameState == "Begun")
            {
            enemy.changeState(true);
            enemy2.changeState(true);
            }
            else
            {
                Debug.Log("Error");
            }
        
    }

    //Wrapper para el get.
    void getData(string gameId)
    {
        Debug.Log("get data");
        string requestUrl = requestUrlBuilder(gameId);
        UnityWebRequest www = UnityWebRequest.Get(requestUrl);
        string auth = "Bearer " + LoginManager.userToken;
        www.SetRequestHeader("Authorization", auth);
        StartCoroutine(getSession(www));
    }

    //Obtiene información de sesión desde el servidor.
    IEnumerator getSession(UnityWebRequest www)
    {
        www.Send();
        yield return www;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //activeServer[0].gameState = ProcessJson(www.downloadHandler.text);
            Debug.Log(ProcessJson(www.downloadHandler.text));
        }
    }

    //Wrapper para el post. Revisar este método ya que un wrapper no es necesario en post. Creo.
    void putData(string gameId,string jsonGameSession)
    {
        Debug.Log("Put data");
        string requestUrl = putRequestUrlBuilder(gameId);
        UnityWebRequest www = UnityWebRequest.Post(requestUrl,jsonGameSession);
        Debug.Log(jsonGameSession);
        string auth = "Bearer " + LoginManager.userToken;
        www.SetRequestHeader("Authorization", auth);
        StartCoroutine(putSession(www));
    }

    IEnumerator putSession(UnityWebRequest www)
    {
        www.Send();
        yield return www;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }
    }

    private static string requestUrlBuilder(string gameId)
    {
        string id = gameId;
        string baseUrl = "http://192.168.98.131:5000/getGameSessions/";
        string requestUrl = baseUrl + gameId;
        return requestUrl;
    }

    private static string putRequestUrlBuilder(string gameId)
    {
        string id = gameId;
        string baseUrl = "http://192.168.98.131:5000/sessionUpdate/";
        string requestUrl = baseUrl + gameId;
        return requestUrl;
    }

    //Procesa respuesta del servidor y lo convierte en JSON.
    private string ProcessJson(string jsonString)
    {
        JsonData jsonServer = JsonMapper.ToObject(jsonString);
        string gameStateJson = jsonServer["gameState"].ToString();
        return gameStateJson;
    }

    //Creador de la sesión de juego
    public GameServer[] createGameSession()
    {
        playersToJSON();
        GameServer[] gameSession = new GameServer[1];
        string gameSessionJSON = sessionCreator(gameSession);
        Debug.Log(gameSessionJSON);
        sessionInit(gameSessionJSON);
        return gameSession;
    }

    //Crea una nueva sesión en un array y luego a JSON para ser enviado al servidor.
    private string sessionCreator(GameServer[] gameSession)
    {
        gameSession[0] = new GameServer(GUIDCreator(), playersArray, "Waiting", "None");
        return jsonConverter(gameSession);
    }

    private static string jsonConverter(GameServer[] gameSession)
    {
        return JsonHelper.ToJson(gameSession);
    }

    //Convertir objetos de jugadores a JSON para ser enviados al servidor.
    private void playersToJSON()
    {
        string playersJSON = JsonHelper.ToJson(playersArray);
    }

    //Envia POST request para iniciar una sesión de juego en el servidor.
    private void sessionInit(string gameSessionJSON)
    {
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/gameSessionInit", gameSessionJSON);
        string auth = "Bearer " + LoginManager.userToken;

        Debug.Log("Este es el auth " + auth);
        www.SetRequestHeader("Authorization", auth);
        www.Send();
        Debug.Log("Este es el estado del init" + www.responseCode);
    }

    //Envia PUT request para agregar los jugadores de juego en el servidor.
    private void sessionUpdate(string gameId, string gameSessionJSON)
    {
        string requestUrl = putRequestUrlBuilder(gameId);
        UnityWebRequest www = UnityWebRequest.Put(requestUrl, gameSessionJSON);
        www.Send();
    }

    //Instanciador de jugadores AI para probar el juego.
    void instantiatePlayers(GameServer[] gameServer)
    {
        Debug.Log("Instanciando jugadores");
        //Instanciando jugador 1.
        enemy = GameObject.Instantiate(player1, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy.Initialize(GUIDCreator(), 100, 3, 0);
        

        //Instanciando jugador 2-
        enemy2 = GameObject.Instantiate(player2, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy2.Initialize(GUIDCreator(), 100, -3, 0);

        //Agregado jugadores al arreglo de los jugadores activos.
        gameServer[0].players.SetValue(enemy.jSONReady(),0);
        gameServer[0].players.SetValue(enemy2.jSONReady(),1);
    }

    //Crear de GUID para la sesión.
    public string GUIDCreator()
    {
        string playeriD;
        playeriD = Guid.NewGuid().ToString("N");
        return playeriD;
    }


   
    //Post GameWinner
}
