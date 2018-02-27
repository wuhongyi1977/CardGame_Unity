using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSet : MonoBehaviour {

    //メンバ変数
    string player_name;
    int count;
    GameObject[] children;  //本来フィールドにはカード一枚だけど一応

    // Use this for initialization
    void Start () {
        player_name = transform.parent.parent.name;
        count = transform.childCount;
    }
	
	// Update is called once per frame
	void Update () {
        setCards();
	}

    //フィールド内のカードを正しい位置に移動(以前とカード枚数が違ったら)
    public void setCards() {
        int Childcount = transform.childCount;
        if (count != Childcount) {
            count = Childcount;
            children = new GameObject[count];
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                if (player_name == "Player") {
                    children[i].transform.localPosition = new Vector3(0, (float)-1.15, 0);
                }else {
                    children[i].transform.localPosition = new Vector3(0, (float)1.15, 0);
                }
                children[i].GetComponent<Card>().ISSELECTABLE = false;  //選択不可に
            }
        }
    }

    //カードを一枚墓地に送る(フィールドにカードがなかったらスルー)
    public void discrad() {
        if (transform.childCount == 0) {
            return;
        }
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.parent = null;
        child.transform.parent = transform.parent.Find("Tomb").gameObject.transform;
        Debug.Log(child.name);
    }
}
