using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card {

    protected int atk;

    public AttackCard(){
        this.atk = 0;
    }

    public AttackCard(string name, string id, string explain, string illust, int atk){
        this.card_name = name;
        this.card_id = id;
        this.card_explain = explain;
        this.card_color = Color.red;
        this.card_illust = illust;
        this.atk = atk;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
