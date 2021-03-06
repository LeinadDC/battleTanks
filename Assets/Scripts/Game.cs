﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;

public class Game : MonoBehaviour {

    // Clase interna usada para manejar las sesiones.

    #region Clase seriazable para manejar sesiones

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
    #endregion

    #region Variables de sesión

    public GameObject player1;
    public GameObject player2;
    TankScript enemy;
    TankScript enemy2;
    TankScript.Player[] playersArray;
    GameServer[] activeServer = new GameServer[1];
    #endregion

    //Inicia una nueva sesión.
    private void Start()
    {
        activeServer = createGameSession();
    }

    //Construye la sesión.
    void Update   () {
        gameBuilder();
    }

    void gameBuilder()
    {
            //Verifica el estado del juego.
            if(activeServer[0].gameState  == "Waiting")
            {
            //Instancia jugadores, cambia de estado y actualiza información en servidor.
                Debug.Log("Instanciando jugadores");
                instantiatePlayers(activeServer);
                Debug.Log("Ya hay jugadores");
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
            activeServer[0].gameState = ProcessJson(www.downloadHandler.text);
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
        Debug.Log(gameStateJson);
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
    private static void sessionInit(string gameSessionJSON)
    {
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.98.131:5000/gameSessionInit", gameSessionJSON);
        www.Send();
    }

    //Envia PUT request para agregar los jugadores de juego en el servidor.
    private static void sessionUpdate(string gameId, string gameSessionJSON)
    {
        string requestUrl = putRequestUrlBuilder(gameId);
        UnityWebRequest www = UnityWebRequest.Post(requestUrl, gameSessionJSON);
        www.Send();
    }

    //Instanciador de jugadores AI para probar el juego.
    void instantiatePlayers(GameServer[] gameServer)
    {
        playersArray = new TankScript.Player[2];
        activeServer[0].players = new TankScript.Player[2];
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

    //Se inicia el juego crenado un nuevo GUID, gameState y gameWinner, ademas de los tanques.
    //No empezar el juego hasta que el estado del servidor (game session sea OK)
    //Esperar a que existan 2 tanques en el juego para que sea OK y existan en el juego.
    //Cada tanque debe tener las siguientes variables inizialibales PUBLICAS
    // Vida y GUID que se le asigna

}
