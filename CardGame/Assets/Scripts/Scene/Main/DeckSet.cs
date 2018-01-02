﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSet : MonoBehaviour {
    
    //メンバ変数
    int a = 0;
    int count;
    GameObject[] children;

    // Use this for initialization
    void Start () {
        count = transform.childCount;
	}
	
	// Update is called once per frame
	void Update () {
        setCards();

        a++;
        if (a > 60) {
            draw();
            a = 0;
        }
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
            return ;
        }
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.parent = null;
        child.transform.parent = transform.parent.FindChild("Hand").gameObject.transform;
        Debug.Log(child.name);
    }

}
