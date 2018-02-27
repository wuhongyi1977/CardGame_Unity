using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour {

    public GameObject baseObject;
    
    [SerializeField]
    private GameObject[] shields;
    private bool isCreated;

    //hpの更新もここから
    private Text hp;

    // Use this for initialization
    void Start () {
        CreateShield();

        if (transform.parent.name == "Player") {
            hp = GameObject.Find("Player_hp").GetComponent<Text>();
        }else {
            hp = GameObject.Find("Enemy_hp").GetComponent<Text>();
        }
        hp.text = (shields[0].GetComponent<Shield>().HP
                        + shields[1].GetComponent<Shield>().HP
                        + shields[2].GetComponent<Shield>().HP).ToString();
    }
	
	// Update is called once per frame
	void Update () {
        hp.text = (shields[0].GetComponent<Shield>().HP
                        + shields[1].GetComponent<Shield>().HP
                        + shields[2].GetComponent<Shield>().HP).ToString();
    }

    //シールド生成
    public void CreateShield() {
        if (isCreated) return;

        shields = null;

        if (shields == null) {
            //初期化
            shields = new GameObject[3];

            //生成
            for(int i = 0; i < shields.Length; i++) {
                //生成
                shields[i] = Utility.Instantiate(baseObject);

                //名前変更
                if (i == 0) {
                    shields[i].transform.name = "shield_C";
                } else if (i == 1) {
                    shields[i].transform.name = "shield_I";
                } else if (i == 2) {
                    shields[i].transform.name = "shield_A";
                }

                //変数設定
                Shield shield = shields[i].GetComponent<Shield>();
                if (i == 0) {
                    shield.ATTRIBUTE = "C";
                }else if(i == 1) {
                    shield.ATTRIBUTE = "I";
                } else if (i == 2) {
                    shield.ATTRIBUTE = "A";
                }
                shield.HP = 1000;  //後でゲーム開始前の入力に合わせられるようにしたい
                shield.PARENT = transform.parent.name;

                //座標指定
                float x = 0;
                if (i == 0) {
                    x = (float)-1.25;
                } else if (i == 1) {
                    x = 0;
                } else if (i == 2) {
                    x = (float)1.25;
                }
                float y = 0;
                if (transform.parent.name == "Player") {
                    y = (float)-3.25;
                }else {
                    y = (float)3.25;
                }
                shields[i].transform.localPosition = new Vector3(x, y, 0);

                //親指定
                shields[i].transform.parent = this.transform;
            }
        }
        isCreated = true;

    }
}
