using System.Collections;
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
    private bool isBack;  //裏表(trueで裏)
    private bool isSelect;  //その時選べるかどうか(ゲームから変更する？)

    //スプライト一覧(update内で毎回スプライト生成すると負荷がやばいのでメンバ変数に・startで値代入とレンダラに適用)
    private Sprite sp_back;
    private Sprite sp_face;
    private Sprite sp_child;

    //レンダラ一覧
    private SpriteRenderer sr;
    private SpriteRenderer sr_child;


	// Use this for initialization
	void Start ()
    {
        isBack = true;
        isSelect = true;

        sp_back = Utility.GetSprite("Sprites", "card_back");
        if (card_type == "ATK") {
            //後でAttribute分岐も
            sp_face = Utility.GetSprite("Sprites","ATK");

        }else if(card_type == "DEF") {
            //後でAttribute分岐も
            sp_face = Utility.GetSprite("Sprites", "DEF");

        } else if (card_type == "EV") {
            sp_face = Utility.GetSprite("Sprites", "EV");
        } else if (card_type == "SP") {
            sp_face = Utility.GetSprite("Sprites", "SP");
        }
        sp_child = Utility.GetSprite("Sprites", "card_picture");

        sr = GetComponent<SpriteRenderer>();
        sr_child = transform.Find("card_picture").gameObject.GetComponent<SpriteRenderer>();

        sr.sprite = sp_back;
        sr_child.sprite = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //スプライト変更
        if (isBack) {
            //裏の時
            sr.sprite = sp_back;
            sr_child.sprite = null;
        } else {
            //表の時
            sr.sprite = sp_face;
            sr_child.sprite = sp_child;
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

    //子のスプライト変更
    void changeChildSprite(string fileName, string spriteName) {
        // Resourcesフォルダ内のファイル名, スプライト名.
        Sprite sp = Utility.GetSprite(fileName, spriteName);
        GameObject child = transform.Find("card_picture").gameObject;
        SpriteRenderer child_sr = child.GetComponent<SpriteRenderer>();
        child_sr.sprite = sp;
    }
}
