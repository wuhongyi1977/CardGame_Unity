using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour {

    protected string card_id;
    protected string card_name;
    protected string card_attribute;
    protected string card_explain;
    protected Color card_color;
    protected string card_illust;
    //イラストはとりあえずstringにしたけどどう扱う？ID判別でspriteの集まりから呼び出し？

    public Card() {
        this.card_id = "";
        this.card_name = "";
        this.card_attribute = "";
        this.card_explain = "";
        this.card_color = Color.white;
        this.card_illust = "";
    }

    public Card(string id, string name, string attribute, string explain, Color color, string illust){
        this.card_id = id;
        this.card_name = name;
        this.card_attribute = attribute;
        this.card_explain = explain;
        this.card_color = color;
        this.card_illust = illust;
    }
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
