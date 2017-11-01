using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    private Card card;
    private Player player;
    private Field field;
    private Shield shield;

    public Game()
    {
        card = new Card();
        player = new Player();
        field = new Field();
        shield = new Shield();
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

    }
}
