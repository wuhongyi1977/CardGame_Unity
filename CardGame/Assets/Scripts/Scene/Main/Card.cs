﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Card : MonoBehaviour {

    [SerializeField]
    private int card_id;
    private string card_name;
    private string card_attribute;
    private string card_describe;
    private int card_power;
    private string card_type;
    private bool isBack;  //裏表
    private bool isSelect;  //その時選べるかどうか(ゲームから変更する？)

	// Use this for initialization
	void Start ()
    {
        isBack = true;
        isSelect = true;        
	}
	
	// Update is called once per frame
	void Update ()
    {
        //スプライト変更(裏表の値による,ifうんたらかんたら)
        if (isBack) {
            //裏の時

        } else {
            //表の時

        }

        if (Utility.getOperationUp()) {
            //Debug.Log(card_name);
        }
    }

    public int ID {
        set { card_id = value; }
        get { return card_id;  }
    }

    public string NAME {
        set { card_name = value; }
        get { return card_name; }
    }

    public string ATTRIBUTE {
        set { card_attribute = value; }
        get { return card_attribute; }
    }

    public string DESCRIBE {
        set { card_describe = value; }
        get { return card_describe; }
    }

    public int POWER {
        set { card_power = value; }
        get { return card_power; }
    }

    public string TYPE {
        set { card_type = value; }
        get { return card_type; }
    }

    public bool ISBACK {
        set { isBack = value; }
        get { return isBack; }
    }

    public bool ISSELECT {
        set { isSelect = value; }
        get { return isSelect; }
    }

    // スプライトの変更.(スプライトの格納名は後でいれてください)
    void changeSprite(string fileName, string spriteName) {
        // Resourcesフォルダ内のファイル名, スプライト名.
        Sprite sp = Utility.GetSprite(fileName, spriteName);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sp;
    }
}
