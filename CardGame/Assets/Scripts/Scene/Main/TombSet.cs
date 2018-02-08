using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombSet : MonoBehaviour {

    //メンバ変数
    int count;
    GameObject[] children;

    // Use this for initialization
    void Start () {
        count = transform.childCount;
    }

    // Update is called once per frame
    void Update () {
        setCards();
    }

    //墓地内のカードを正しい位置に移動(以前とカード枚数が違ったら)
    public void setCards() {
        int Childcount = transform.childCount;
        if (count != Childcount) {
            count = Childcount;
            children = new GameObject[count];
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                children[i].transform.localPosition = new Vector3(-3, (float)-2.5, 0);
                children[i].GetComponent<Card>().ISSELECTABLE = false;  //選択不可に
            }
        }
    }
}
