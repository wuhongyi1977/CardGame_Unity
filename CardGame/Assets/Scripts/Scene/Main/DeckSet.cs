using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSet : MonoBehaviour {

    //メンバ変数
    string player_name;
    int count;
    GameObject[] children;

    //試しドロー用
    int a = 0;
    int b = 0;

    // Use this for initialization
    void Start () {
        // "Player"か"Enemy"が入る。これを用いてsetCards()などの処理を分岐させると楽かも
        player_name = transform.parent.parent.name;
        count = transform.childCount;
	}
	
	// Update is called once per frame
	void Update () {
        setCards();

        /*
        a++;
        if (a > 5 && b<20) {
            draw();
            a = 0;
            b++;
        }
        */
	}

    //デッキ内のカードを正しい位置に移動(以前とカード枚数が違ったら)
    public void setCards() {
        int Childcount = transform.childCount;
        if (count != Childcount) {
            count = Childcount;
            children = new GameObject[count];
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                children[i].transform.localPosition = new Vector3(3, (float)-2.5, 0);
            }
        }
    }

    //手札を引く・カードを一枚手札に送る(山札にカードがなかったらスルー)
    public void draw() {
        if(transform.childCount == 0) {
            return;
        }
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.parent = null;
        child.transform.parent = transform.parent.Find("Hand").gameObject.transform;

        Debug.Log(transform.parent.parent.name + " " + child.name);
    }

    //手札を５枚引く(ゲーム開始時に使う)
    public void draw5() {
        for (int i = 0; i < 5; i++) {
            GameObject child = transform.GetChild(0).gameObject;
            child.transform.parent = null;
            child.transform.parent = transform.parent.Find("Hand").gameObject.transform;
        }
    }

}
