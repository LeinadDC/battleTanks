using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;

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

    void Start () {
        gameBuilder();
  
    }

    void gameBuilder()
    {
        var activeSession = createGameSession();

        if(activeSession[0].gameState  == "Waiting")
        {
            Debug.Log("Esperando jugadores");
            //Instancia jugadores
            //Envia jugadores
            //Revisa de nuevo
            //Inicia Juego
            StartCoroutine(getSession(activeSession[0].gameId));
        }
        else
        {
            Debug.Log("Juego iniciado");
        }
        
    }

    //Obtiene información de sesión desde el servidor.
    IEnumerator getSession(string gameId)
    {
        string requestUrl = requestUrlBuilder(gameId);
        UnityWebRequest www = UnityWebRequest.Get(requestUrl);
        yield return www.Send();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Imprime información recibida.
            Debug.Log(www.downloadHandler.text);
        }
    }

    private static string requestUrlBuilder(string gameId)
    {
        string id = gameId;
        string baseUrl = "http://192.168.98.131:5000/getGameSessions/";
        string requestUrl = baseUrl + gameId;
        return requestUrl;
    }

    //Procesa respuesta del servidor y lo convierte en JSON.
    private void ProcessJson(string jsonString)
    {
        JsonData jsonServer = JsonMapper.ToObject(jsonString);
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
        var gameSessionJSON = JsonHelper.ToJson(gameSession);
        return gameSessionJSON;
    }

    //Convertir objetos de jugadores a JSON para ser enviados al servidor.
    private void playersToJSON()
    {
        string playersJSON = JsonHelper.ToJson(playersArray);
    }

    //Envia POST request para iniciar una sesión de juego en el servidor.
    private static void sessionInit(string gameSessionJSON)
    {
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/gameSessionInit", gameSessionJSON);
        www.Send();
    }

    //Instanciador de jugadores AI para probar el juego.
    void instantiatePlayers()
    {
        //Instanciando jugador 1.
        enemy = GameObject.Instantiate(player1, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy.Initialize(GUIDCreator(), 100, 3, 0);

        //Instanciando jugador 2-
        enemy2 = GameObject.Instantiate(player2, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy2.Initialize(GUIDCreator(), 100, -3, 0);

        //Agregado jugadores al arreglo de los jugadores activos.
        playersArray.SetValue(enemy.jSONReady(),0);
        playersArray.SetValue(enemy2.jSONReady(),1);
    }

    //Crear de GUID para la sesión.
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
