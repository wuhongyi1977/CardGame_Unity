using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour {

    [SerializeField]
    private string shield_attribute;
    private int shield_hp;  //符号なしintなので注意

    //スプライトとレンダラ
    private Sprite sp;
    private SpriteRenderer sr;

    //耐久を表示するUI
    private Text TargetText;

	// Use this for initialization
	void Start () {
        if (shield_attribute == "C") {
            sp = Utility.GetSprite("Sprites","shield_C");
            TargetText = GameObject.Find("Player_shield_C").GetComponent<Text>();
        }else if(shield_attribute == "I") {
            sp = Utility.GetSprite("Sprites", "shield_I");
            TargetText = GameObject.Find("Player_shield_I").GetComponent<Text>();
        } else if(shield_attribute == "A") {
            sp = Utility.GetSprite("Sprites", "shield_A");
            TargetText = GameObject.Find("Player_shield_A").GetComponent<Text>();
        }
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sp;

        shield_hp = 1000;
        TargetText.text = shield_hp.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        //shield_hp--;
        if (shield_hp < 0) {
            shield_hp = 0;
        }
        //UIにシールド耐久を伝える操作が必要？
        TargetText.text = shield_hp.ToString();
	}

    public string ATTRIBUTE {
        set { shield_attribute = value; }
        get { return shield_attribute; }
    }

    public int HP {
        set { shield_hp = value; }
        get { return shield_hp; }
    }
}
