using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {

    //カードのベースとなるオブジェクト
    public GameObject baseObject;
    //自動でカードを生成するかどうか
    public bool autoGenerate;
    //カードの配列
    [SerializeField]
    private GameObject[] cards;
    private bool isCreated;

    //void Awake() {
    //    setActive(false);
    //}

    //カード生成
    public void createTarget() {
        if (isCreated) return;
        cards = null;

        if (cards == null) {
            //初期化
            cards = new GameObject[20];

            //生成
            for( int i = 0;i< cards.Length; i++){
                //baseObjectから生成
                cards[i] = Utility.Instantiate(baseObject);

                //名前変更
                cards[i].transform.name = "card"+i;

                //裏表の設定もする

                //スプライト変更(裏表の値による,ifうんたらかんたら)


                //座標指定(デッキの位置に)
                float x = 0;
                float y = 0;
                cards[i].transform.localPosition = new Vector3(x,y,0);

                //親設定
                cards[i].transform.parent = transform;

            }
            isCreated = true;
        }
    }
    // 指定のターゲットを返す.
    public GameObject getcard(int index) {
        return cards[index];
    }

    // カードの個数を返す.
    // 最終的にはConstants内で指定したMAX値.
    public int getcardMaxNumber {
        get { return cards.Length; }
    }

    // 表示 / 非表示の設定.
    public void setActive(bool flag) {
        foreach (Transform child in transform)
            child.gameObject.SetActive(flag);
    }

    // スプライトの変更.(スプライトの格納名は後でいれてください)
    void changeSprite(int index) {
        string name = "" + index;
        // Resourcesフォルダ内のファイル名, スプライト名.
        Sprite sp = Utility.GetSprite("", name);
        SpriteRenderer sr = Utility.GetSafeComponent<SpriteRenderer>(cards[index]);
        sr.sprite = sp;
    }


}
