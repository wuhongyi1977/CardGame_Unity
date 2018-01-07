using System;
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

    // Use this for initialization
    void Start() {
        createCard();
    }

    //カード生成
    public void createCard() {
        if (isCreated)
            return;
        cards = null;

        if (cards == null) {
            //初期化
            cards = new GameObject[20];

            //生成
            for (int i = 0; i < cards.Length; i++) {
                //baseObjectから生成
                cards[i] = Utility.Instantiate(baseObject);

                //名前変更
                cards[i].transform.name = "card" + (i + 1);

                //変数設定(CSV読み込んでカードの値設定)
                Card card = cards[i].GetComponent<Card>();
                CsvReader csv = new CsvReader();
                string[] data = csv.Readcsv("deck", i + 1);

                card.ID = Convert.ToInt32(data[0]);
                card.NAME = data[1];
                card.ATTRIBUTE = data[2];
                card.DESCRIBE = data[3];
                card.POWER = Convert.ToInt32(data[4]);
                card.TYPE = data[5];

                //座標指定(デッキの位置に)
                float x = 0;
                float y = 0;
                cards[i].transform.localPosition = new Vector3(x, y, 0);
            }

            //カードのシャッフル
            System.Random rng = new System.Random();
            int n = cards.Length;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                GameObject tmp = cards[k];
                cards[k] = cards[n];
                cards[n] = tmp;
            }

            for (int i = 0; i < cards.Length; i++) {
                //親設定
                cards[i].transform.parent = null;
                cards[i].transform.parent = transform.FindChild("Deck").gameObject.transform;
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
    //public void setActive(bool flag) {
    //    foreach (Transform child in transform)
    //        child.gameObject.SetActive(flag);
    //}


}
