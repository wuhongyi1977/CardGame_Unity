using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSet : MonoBehaviour {

    int a = 0;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        setCards();

        a++;
        if (a > 300) {
            draw();
            a = 0;
        }
	}

    //デッキ内のカードを正しい位置に移動
    public void setCards() {
        int count = transform.childCount;
        GameObject[] children = new GameObject[count];
        for (int i = 0; i < count; i++) {
            children[i] = transform.GetChild(i).gameObject;
            children[i].transform.localPosition = new Vector3(3, (float)-2.5, 0);
        }
    }

    //手札を引く・カードを一枚手札に送る
    public void draw() {
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.parent = transform.parent.FindChild("Hand").gameObject.transform;
        Debug.Log(child.name);
    }

}
