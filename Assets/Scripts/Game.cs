using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    // Use this for initialization
    public GameObject player1;
    public GameObject player2;
	void Start () {
        instantiatePlayers();   
		
	}
	
	// Update is called once per frame
	void Update () {


    }

    void instantiatePlayers()
    {
        TankScript enemy = GameObject.Instantiate(player1, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy.Initialize(GUIDCreator(), 100, 3, 0);

        TankScript enemy2 = GameObject.Instantiate(player2, Vector3.zero, Quaternion.identity).GetComponent<TankScript>();
        enemy.Initialize(GUIDCreator(), 100, -3, 0);
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
