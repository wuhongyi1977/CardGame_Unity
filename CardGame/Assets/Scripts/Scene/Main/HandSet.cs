using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSet : MonoBehaviour {

    //メンバ変数
    int count;
    GameObject[] children;

    //スライダー
    private Slider sld;

	// Use this for initialization
	void Start () {
        count = transform.childCount;
        children = new GameObject[20];
        sld = GameObject.Find("Player_Slider").GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update () {
        count = transform.childCount;
        if (count < 5) {
            sld.interactable = false;
        }else {
            sld.interactable = true;
        }
        setCards();  //恐らく本当はupdateにあってはいけない関数。ゲームマネジャーがカードからドローした後に整列のために呼び出すべき
        slide();
	}

    //デッキ内のカードを正しい位置に移動
    public void setCards() {
        children = new GameObject[count];

        //スライダーのつまみをずらす必要あり？(ゲームマネージャー内で使用する時にはコメントアウト外してください)
        //sld.value = (float)0.5;

        if (count % 2 == 0) {  //偶数の時の挙動
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                double x = -0.8 - 1.6 * (count / 2 - 1) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, -5, 0);
                children[i].GetComponent<Card>().ISBACK = false;  //とりあえず表に
            }
        } else {  //奇数
            for (int i = 0; count > i; i++) {
                children[i] = transform.GetChild(i).gameObject;
                double x = -1.6 * ((count - 1) / 2) + 1.6 * i;
                children[i].transform.localPosition = new Vector3((float)x, -5, 0);
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
        card.transform.parent = transform.parent.FindChild("Field").gameObject.transform;
    }


    //イベポジに出す関数(GameObject引数？イベポジが詳細未定なので未作成)
    public void event_summon (GameObject card) {

    }

}
