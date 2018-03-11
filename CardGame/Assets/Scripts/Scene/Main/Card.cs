using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour {

    [SerializeField]
    private int card_id;
    private string card_name;
    private string card_attribute;
    private string card_describe;
    private int card_power;
    private string card_type;
    private string card_parent;  //playerかenemy所属どちらか,CardManagerで値いれる
    private bool isBack;  //裏表(trueで裏)
    private bool isSelectable;  //その時選べるかどうか(ゲームから変更する？)

    //スプライト一覧(update内で毎回スプライト生成すると負荷がやばいのでメンバ変数に・startで値代入とレンダラに適用)
    private Sprite sp_back;
    private Sprite sp_face;
    private Sprite sp_child;

    //レンダラ一覧
    private SpriteRenderer sr;
    private SpriteRenderer sr_child;

    //テキスト一覧
    private GameObject text_card_name;
    private GameObject text_card_power;

	// Use this for initialization
	void Start ()
    {
        isBack = true;
        isSelectable = true;

        sp_back = Utility.GetSprite("Sprites", "card_back");

        if (card_type == "ATK") {
            //後でAttribute分岐も
            sp_face = Utility.GetSprite("Sprites","ATK");

        } else if(card_type == "DEF") {
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

        text_card_name = transform.Find("card_name").gameObject;
        text_card_power = transform.Find("card_power").gameObject;
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

        //きっちりカード生成後に初期化できるならいらない(特殊イベントはどうするか)
        //if (text_card_name.GetComponent<TextMesh>().text.Equals(card_name)) {
            text_card_name.GetComponent<TextMesh>().text = card_name;
        //}
        //if (text_card_power.GetComponent<TextMesh>().text.Equals(card_power.ToString())) {
            text_card_power.GetComponent<TextMesh>().text = card_power.ToString();
        //}

        Vector3 currentPos = transform.localPosition;
        float name_y = currentPos.y + (float)0.9;
        float power_y = currentPos.y - (float)0.75;
        text_card_name.transform.position= new Vector3(currentPos.x,name_y,currentPos.z);
        text_card_power.transform.position = new Vector3(currentPos.x, power_y, currentPos.z);

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

    public string PARENT {
        set { card_parent = value; }
        get { return card_parent;  }
    }

    public bool ISBACK {
        set { isBack = value; }
        get { return isBack; }
    }

    public bool ISSELECTABLE {
        set { isSelectable = value; }
        get { return isSelectable; }
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

    //追加部分(特殊カード)
    public void specialCard(){
        int count;
        GameObject[] children;
        GameObject eneHand;

        eneHand = transform.Find("Enemy/Hand").gameObject;
        count = eneHand.transform.childCount;
        children = new GameObject[count];
        
        if (count % 2 == 0)
        {  //偶数の時の挙動
            for (int i = 0; count > i; i++)
            {
                children[i] = eneHand.transform.GetChild(i).gameObject;
                double x = -0.8 - 1.6 * (count / 2 - 1) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, 5, 0);
                children[i].GetComponent<Card>().ISBACK = false;  
                children[i].GetComponent<Card>().ISSELECTABLE = true;  

            }
        } else{  //奇数

            for (int i = 0; count > i; i++)
            {
                children[i] = eneHand.transform.GetChild(i).gameObject;
                double x = -1.6 * ((count - 1) / 2) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, 5, 0);
                children[i].GetComponent<Card>().ISBACK = false;  
                children[i].GetComponent<Card>().ISSELECTABLE = true;  
            }
        }



    }
    //追加部分(イベントカード)
    public void eventCard(){
        if (card_id == 17){

        }else if (card_id == 18){

        }else if (card_id == 19){

        }
    }

}
