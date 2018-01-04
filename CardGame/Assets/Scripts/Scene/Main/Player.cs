using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //プレイヤーIDはゲーム起動時に割り当て,CPUなら0に
    public int playerID;

    public Shield[] Shields;

    //カード用、通し番号を入れて管理
    //それぞれ最大サイズは20・デッキシャッフルはどのタイミングでする？
    private List<string> Deck;
    public List<string> Hand;
    private List<string> Tomb;

    //カード置くとこも宣言,これでFieldクラスはいらない？イベントカードだけ置き場未定(プレイヤーの中央？)
    private List<string> Field;
    private List<string> SPField;
    

    public bool isPlayable;

    //詳しいコンストラクタは未定義ですごめん
   public  Player() {}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //select card:hand(string) from Hand(List<string>)
    public string HandSelect(){
        string hand= "";
        //選べるカードをここでハイライトした方がよさそう
        //それ用にどのカード種類が選べるかを判別する方法はどうしよう？int引数を設定してターン毎にゲームマネージャーで引数いれてコール？
        for(int i = 0; i < Hand.Count; i++) {
            
        }

        if (playerID != 0){
            //プレイヤーが操作してカード選ぶ


        }else {
            //CPUならランダム
            
        }


        return hand;
    }

    //draw card:hand(string) from Deck(List<string>)
    public string Draw(){

        string hand=Deck[1];
        Deck.RemoveAt(0);

        return hand;
    }

    //send card:hand(string) to Tomb(List<string>)
    public string Discard() {
        string hand="";


        return hand;
    }
}
