using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSet : MonoBehaviour {

    //メンバ変数(中心位置・移動可能位置・現在位置)
    int count;
    GameObject[] children;

    static Vector3 Center = new Vector3(0, -5, 0);
    Vector3 FrontEdge;
    Vector3 BackEdge;
    Vector3 Current;  //後でデッキのポジと対応させる


	// Use this for initialization
	void Start () {
        count = transform.childCount;
    }
	
	// Update is called once per frame
	void Update () {
        setCards();  //恐らく本当はupdateにあってはいけない関数。ゲームマネジャーがカードからドローした後に整列のために呼び出すべき
	}

    //デッキ内のカードを正しい位置に移動(後でもっと複雑になります)
    public void setCards() {
        int Childcount = transform.childCount;
        if(count != Childcount) {
            count = Childcount;
            GameObject[] children = new GameObject[count];

            Current = Center;  //いる？

            if (count%2 == 0) {  //偶数の時の挙動
                for(int i = 0; count > i; i++) {
                    children[i] = transform.GetChild(i).gameObject;
                    double x = -0.75 - 1.5 * (count/2 - 1) + 1.5 * i;
                    children[i].transform.localPosition = new Vector3((float)x, -5, 0);
                    children[i].GetComponent<Card>().ISBACK = false;  //とりあえず表に
                }
            } else {  //奇数
                for (int i = 0; count > i; i++) {
                    children[i] = transform.GetChild(i).gameObject;
                    double x = -1.5 * ((count - 1)/2) + 1.5 * i;
                    children[i].transform.localPosition = new Vector3((float)x, -5, 0);
                    children[i].GetComponent<Card>().ISBACK = false;  //とりあえず表に
                }

            }
            
        }
        
    }

    //フィールドに出す関数(GameObject引数？)
    public void summon (GameObject card) {
        card.transform.parent = null;
        card.transform.parent = transform.parent.FindChild("Field").gameObject.transform;
    }


    //イベポジに出す関数(GameObject引数？イベポジが詳細未定なので未作成)


}
