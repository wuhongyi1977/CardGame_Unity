﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSet : MonoBehaviour {

    //メンバ変数
    string player_name;
    int count;
    GameObject[] children;

    //スライダー
    private Slider sld;

	// Use this for initialization
	void Start () {
        // "Player"か"Enemy"が入る。これを用いてsetCards()などの処理を分岐させると楽かも
        player_name = transform.parent.parent.name;
        count = transform.childCount;
        children = new GameObject[20];

        if (player_name == "Player") {
            sld = GameObject.Find("Player_Slider").GetComponent<Slider>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        count = transform.childCount;
        if (player_name == "Player") {
            if (count < 5) {
                sld.interactable = false;
            } else {
                sld.interactable = true;
            }
        }
        setCards();  //恐らく本当はupdateにあってはいけない関数。ゲームマネジャーがカードからドローした後に整列のために呼び出すべき
        if (player_name == "Player") {
            slide();
        }
	}

    //デッキ内のカードを正しい位置に移動
    public void setCards() {
        children = new GameObject[count];

        //スライダーのつまみを中央に戻す必要あり？(ゲームマネージャー内で使用する時にはコメントアウト外してください)
        //if (player_name == "Player") {
            //sld.value = (float)0.5;
        //}

        if (count % 2 == 0) {  //偶数の時の挙動
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                double x = -0.8 - 1.6 * (count / 2 - 1) + 1.6 * i;
                if (player_name == "Player") {
                    children[i].transform.localPosition = new Vector3((float)x, -5, 0);
                }else {
                    children[i].transform.localPosition = new Vector3((float)x, 5, 0);
                }
                children[i].GetComponent<Card>().ISBACK = false;  //とりあえず表に

            }
        } else {  //奇数
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                double x = -1.6 * ((count - 1) / 2) + 1.6 * i;
                if (player_name == "Player") {
                    children[i].transform.localPosition = new Vector3((float)x, -5, 0);
                }else {
                    children[i].transform.localPosition = new Vector3((float)x, 5, 0);
                }
                children[i].GetComponent<Card>().ISBACK = false;  //とりあえず表に
            }
        }
    }

    //スライダーが動いたときに動く関数・カードをずらして位置を合わせる
    public void slide() {
        double value = sld.value;

        if(count%2 == 0) {  //偶数
            for (int i = 0; count > i; i++) {
                double x = -1 * (value - 0.5) * (1.6 * count - 7.5)  +  -0.8 - 1.6 * (count / 2 - 1) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, -5, 0);
            }
        } else {  //奇数
            for (int i = 0; count > i; i++) {
                double x = -1 * (value - 0.5) * (1.6 * count - 7.5) +   -1.6 * ((count - 1) / 2) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, -5, 0);
            }
        }
    }

    //フィールドに出す関数(GameObject引数？)
    public void summon (GameObject card) {
        card.transform.parent = null;
        card.transform.parent = transform.parent.Find("Field").gameObject.transform;
    }


    //イベポジに出す関数(GameObject引数？イベポジが詳細未定なので未作成)
    public void event_summon (GameObject card) {

    }

    //手札からその時出せるカードをランダムで選んで出す関数(Enemy専用)
    public void random_summon() {
        var canSummonCards = new List<GameObject>();
        for (int i = 0; count > i; i++) {
            if (children[i].GetComponent<Card>().ISSELECTABLE) {
                canSummonCards.Add(children[i]);
            }
        }

        //乱数生成
        System.Random Random = new System.Random();
        int RandomI = Random.Next(0,canSummonCards.Count);

        //選んでいるだけ,summoやevent_summonで親子関係の変更はやってください
        Variables.enemy_selected_obj = canSummonCards[RandomI];
    }

}
