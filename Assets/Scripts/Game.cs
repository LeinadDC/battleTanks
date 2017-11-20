using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;

public class Game : MonoBehaviour {

    // Use this for initialization
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

    void Start () {
        gameBuilder();

    }

    // Update is called once per frame
    void Update () {

    }

    void gameBuilder()
    {
        var activeSession = createGameSession();

        if(activeSession[0].gameId != null)
        {
            instantiatePlayers();
        }
        else
        {
            Debug.Log("No se ha iniciado ninguna partida.");
        }
        

    }

    void getTest()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.98.131:5000/getGameSessions/4fe75d94d8734374831d82358dee2481");
        yield return www.Send();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
        }
    }

    private void ProcessJson(string jsonString)
    {
        JsonData jsonServer = JsonMapper.ToObject(jsonString);
        
    }

    public GameServer[] createGameSession()
    {
        UnityWebRequest www;
        string playersJSON = JsonHelper.ToJson(playersArray);
        GameServer[] gameSession = new GameServer[1];
        gameSession[0] = new GameServer(GUIDCreator(), playersArray, "Begun", "None");
        var gameSessionJSON = JsonHelper.ToJson(gameSession);
        Debug.Log(gameSessionJSON);
        www =UnityWebRequest.Post("http://192.168.98.131:5000/gameSessionInit", gameSessionJSON);
        www.Send();
        return gameSession;
    }

    void instantiatePlayers()
    {
        enemy = GameObject.Instantiate(player1, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy.Initialize(GUIDCreator(), 100, 3, 0);

        enemy2 = GameObject.Instantiate(player2, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy2.Initialize(GUIDCreator(), 100, -3, 0);
        playersArray.SetValue(enemy.jSONReady(),0);
        playersArray.SetValue(enemy2.jSONReady(), 1);
    }

    public string GUIDCreator()
    {
        string playeriD;
        playeriD = Guid.NewGuid().ToString("N");
        return playeriD;
    }
    //Se inicia el juego crenado un nuevo GUID, gameState y gameWinner, ademas de los tanques.
    //No empezar el juego hasta que el estado del servidor (game session sea OK)
    //Esperar a que existan 2 tanques en el juego para que sea OK y existan en el juego.
    //Cada tanque debe tener las siguientes variables inizialibales PUBLICAS
    // Vida y GUID que se le asigna

}
