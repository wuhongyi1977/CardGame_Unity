using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Variables { // 外部変数(Tap.csで検知されたオブジェクトを保存する)
    public const int CARD_MAX = 20;
    public static GameObject player_tapped_obj = null; // プレイヤーがタップしたカードを保存する
    public static GameObject player_longtapped_obj = null; // プレイヤーが長押ししたカードを保存する(敵がフィールドに出したカードも含まれる)
    public static GameObject enemy_selected_obj = null; // 敵が選んだカードを保存する
    public static bool player_canSummon = false; // プレイヤーが手札からフィールドにカードを出せるかどうか
    public static bool enemy_canSummon = false; // 敵が手札からフィールドにカードを出せるかどうか
    public static bool player_isSkippable = false; // プレイヤーがスキップできる状態かどうか
    public static bool player_hasSkipped = false; // プレイヤーがスキップボタンが押されたかどうか
    public static bool enemy_hasSkipped = false; // CPUがスキップボタンが押されたかどうか
    public static float def_critical = 1.2f; // クリティカル防御率(防御ダウン率は各カード保有させる？)
    public static int damage = 0; // ダメージ
}

public class TurnManager {
    private int turn; // どちらがその時点で攻撃を行うかを示す。0:プレイヤーが先攻、 1:敵が先攻

    public TurnManager(int t) {
        this.turn = t;
    }

    // もう一方のターンを取得する
    public int getNextTurn() {
        if (turn == 0)
            return 1;
        else if (turn == 1)
            return 0;
        return -1;
    }
    // ターンを切り替える
    public void changeTurn() {
        if (turn == 0) {
            turn = 1;
        } else if (turn == 1) {
            turn = 0;
        }
    }

    public int Turn {
        get { return this.turn; }
    }
}

public class GameManager : MonoBehaviour {
    GameObject[] players;
    GameObject player_deck;
    GameObject player_hand;
    GameObject player_field;
    GameObject player_tomb;
    GameObject enemy_deck;
    GameObject enemy_hand;
    GameObject enemy_field;
    GameObject enemy_tomb;
    TurnManager tm;
    int turn_total;
    string attacker_name;

    // Use this for initialization
    void Start() {
        players = new GameObject[2];
        players[0] = transform.Find("Player").gameObject;
        players[1] = transform.Find("Enemy").gameObject;
        player_deck = players[0].transform.Find("Cards/Deck").gameObject;
        player_hand = players[0].transform.Find("Cards/Hand").gameObject;
        player_field = players[0].transform.Find("Cards/Field").gameObject;
        player_tomb = players[0].transform.Find("Cards/Tomb").gameObject;
        enemy_deck = players[1].transform.Find("Cards/Deck").gameObject;
        enemy_hand = players[1].transform.Find("Cards/Hand").gameObject;
        enemy_field = players[1].transform.Find("Cards/Field").gameObject;
        enemy_tomb = players[1].transform.Find("Cards/Tomb").gameObject;
        tm = new TurnManager(0);
        turn_total = 1;
        attacker_name = players[tm.Turn].name; // "Player"か"Enemy"が入る
        StartCoroutine(GameLoop());
    }

    // 最初に呼び出され、ゲームの各段階で順に実行される
    private IEnumerator GameLoop() {
        Debug.Log("今のターンは、" + tm.Turn);

        // ドロー
        yield return StartCoroutine(RoundDraw());
        // 特殊カードorイベントカードを出す
        yield return StartCoroutine(RoundSpEvent());
        // 攻撃側の操作
        yield return StartCoroutine(RoundAttack());
        // 防御側の操作
        yield return StartCoroutine(RoundDefense());
        // ダメージ計算
        yield return StartCoroutine(RoundCalcDamage());
        // 墓地行き判定
        yield return StartCoroutine(RoundTomb());

        if (isFinished()) {
            // ゲーム終了判定がされれば、結果シーンに遷移する
            SceneManager.LoadScene("Result");
        } else {
            // ターン切り替え
            yield return StartCoroutine(RoundTurnChange());

            // ゲーム終了判定がされなければ、コルーチンGameLoopをリスタートする
            // 現時点で行っているGameLoopは終了する
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundDraw() {
        if (turn_total == 1) {
            // 最初のターンなら、双方とも自分のデッキ(山札)から5枚カードを引いて手札に加える
            Utility.GetSafeComponent<DeckSet>(player_deck).draw5();
            Utility.GetSafeComponent<DeckSet>(enemy_deck).draw5();
        } else {
            // 最初のターンでなければ、双方とも自分のデッキから1枚カードを引く手札に加える
            Utility.GetSafeComponent<DeckSet>(player_deck).draw();
            Utility.GetSafeComponent<DeckSet>(enemy_deck).draw();
        }

        // プレイヤーがカードを出せるかどうかのチェック
        foreach (Transform hand_child in player_hand.transform) {
            Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
            if (hand_child_card.TYPE.Equals("ATK") || hand_child_card.TYPE.Equals("DEF")) {
                Variables.player_canSummon = true;
            }
        }
        // 敵がカードを出せるかどうかのチェック
        foreach (Transform hand_child in enemy_hand.transform) {
            Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
            if (hand_child_card.TYPE.Equals("ATK") || hand_child_card.TYPE.Equals("DEF")) {
                Variables.enemy_canSummon = true;
            }
        }

        if (!Variables.player_canSummon) { // 手札からフィールドに出せるカードが1枚も無い場合、スキップ
            // プレイヤー側のスキップ処理をする
        }
        if (!Variables.enemy_canSummon) {
            // 敵側のスキップ処理をする
        }

        // カードをフィールドに出す処理(summonの引数にタップしたカードを指定する)
        // ※プレイヤーがフィールドに出すカードを決めるまで待機する
        // Utility.GetSafeComponent<HandSet>(player_hand).summon(Variables.player_tapped_obj);
        yield return null;
    }

    private IEnumerator RoundSpEvent() {
        // 特殊カードorイベントカードを出す(イベントカードを使い終わったら、それを使った人の墓地に送らなければならない)

        /*
        
        // 特殊カード、イベントカード以外の手札を選べなくする
        foreach (Transform hand_child in player_hand.transform) {
            Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
            if (!hand_child_card.TYPE.Equals("EV") && !hand_child_card.TYPE.Equals("SP")) {
                hand_child_card.ISSELECTABLE = false;
            }
        }

        // 手札から選んだ(タップした)カードをフィールドに出す
        // プレイヤー側
        string player_tapped_cardtype = Utility.GetSafeComponent<Card>(Variables.player_tapped_obj).TYPE;
        if (player_tapped_cardtype.Equals("EV")) {
            Utility.GetSafeComponent<HandSet>(player_hand).event_summon(Variables.player_tapped_obj);
        } else if (player_tapped_cardtype.Equals("SP")) {
            Utility.GetSafeComponent<HandSet>(player_hand).summon(Variables.player_tapped_obj);
        } else if (Variables.player_hasSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }

        // 敵側
        string enemy_tapped_cardtype = Utility.GetSafeComponent<Card>(Variables.enemy_selected_obj).TYPE;
        if (enemy_tapped_cardtype.Equals("EV")) {
            Utility.GetSafeComponent<HandSet>(enemy_hand).event_summon(Variables.enemy_selected_obj);
        } else if (enemy_tapped_cardtype.Equals("SP")) {
            Utility.GetSafeComponent<HandSet>(enemy_hand).summon(Variables.enemy_selected_obj);
        } else if (Variables.enemy_hasSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        // 特殊カードorイベントカードが発動し終わったら墓地へ移動する

        */
        yield return null;
    }

    private IEnumerator RoundAttack() {
        // 攻撃側が攻撃カードを選んでフィールドに出す

        /*

        if (attacker_name.Equals("Player")) {
            foreach (Transform hand_child in enemy_hand.transform) { // 防御側のカード全てを選べないようにする(turnが0ならplayers[1]のカード選択を制限する)
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
            }
            foreach (Transform hand_child in player_hand.transform) { // 攻撃側の攻撃カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("ATK")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
                }
            }
            Utility.GetSafeComponent<HandSet>(player_hand).summon(Variables.player_tapped_obj);
        } else if (attacker_name.Equals("Enemy")) {
            foreach (Transform hand_child in player_hand.transform) { // 防御側のカード全てを選べないようにする(turnが0ならplayers[1]のカード選択を制限する)
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
            }
            foreach (Transform hand_child in enemy_hand.transform) { // 攻撃側の攻撃カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("ATK")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
                }
            }
            Utility.GetSafeComponent<HandSet>(enemy_hand).summon(Variables.enemy_selected_obj);
        }

        */

        /* この処理はここで必要なのか・・・？
        if (Variables.player_hasSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        */
        yield return null;
    }

    private IEnumerator RoundDefense() {
        // 守備側が防御カードを選ぶ

        /*

        if (attacker_name.Equals("Player")) {
            foreach (Transform hand_child in player_hand.transform) { // 攻撃側のカード全てを選べないようにする
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
            }
            foreach (Transform hand_child in enemy_hand.transform) { // 防御側の防御カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("DEF")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
                }
            }
            Utility.GetSafeComponent<HandSet>(enemy_hand).summon(Variables.enemy_selected_obj);
        } else if (attacker_name.Equals("Enemy")) {
            foreach (Transform hand_child in enemy_hand.transform) { // 攻撃側のカード全てを選べないようにする
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
            }
            foreach (Transform hand_child in player_hand.transform) { // 防御側の防御カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("DEF")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECTABLE = false;
                }
            }
            Utility.GetSafeComponent<HandSet>(player_hand).summon(Variables.player_tapped_obj);
        }

        */
        
        /* この処理はここで必要なのか・・・？
        if (Variables.player_hasSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        */
        yield return null;
    }

    private IEnumerator RoundCalcDamage() {
        // 攻撃側なら攻撃したとき、防御側なら攻撃されたときのダメージを計算する
        // 防御側の防御カードの属性＝攻撃側のカードの属性なら、防御クリティカル発生
        // ダメージ＝攻撃力―防御力＊防御力ダウン率＊クリティカル防御率

        /*

        int atk_power = 0, def_power = 0;
        string field_atk_attr = "", field_def_attr;
        if (attacker_name.Equals("Player")) {
            atk_power = Utility.GetSafeComponent<Card>(player_field.transform.GetChild(0).gameObject).POWER;
            def_power = Utility.GetSafeComponent<Card>(enemy_field.transform.GetChild(0).gameObject).POWER;
            field_atk_attr = Utility.GetSafeComponent<Card>(player_field.transform.GetChild(0).gameObject).ATTRIBUTE;
            field_def_attr = Utility.GetSafeComponent<Card>(enemy_field.transform.GetChild(0).gameObject).ATTRIBUTE;
            if (field_def_attr.Equals(field_atk_attr)) {
                Variables.def_critical = 0.5f;
            }
        } else if (attacker_name.Equals("Enemy")) {
            atk_power = Utility.GetSafeComponent<Card>(enemy_field.transform.GetChild(0).gameObject).POWER;
            def_power = Utility.GetSafeComponent<Card>(player_field.transform.GetChild(0).gameObject).POWER;
            field_atk_attr = Utility.GetSafeComponent<Card>(enemy_field.transform.GetChild(0).gameObject).ATTRIBUTE;
            field_def_attr = Utility.GetSafeComponent<Card>(player_field.transform.GetChild(0).gameObject).ATTRIBUTE;
            if (field_def_attr.Equals(field_atk_attr)) {
                Variables.def_critical = 0.5f;
            }
        }

        */
        
        // 実際のダメージ計算。プレイヤーと敵が持つ3属性の攻撃力・防御力はどこで保持する・・・？
        // Variables.damage = atk_power - (int)(def_power * Variables.def_down * Variables.def_critical);
        yield return null;
    }

    private IEnumerator RoundTomb() {
        // HPが0になったカードを墓地に送る。フィールドにいるカード(攻撃、防御、イベント)が、墓地に行く可能性有
        /* if (FieldのカードのHP == 0) {
         *     field_defside.parent = tomb_defside;
         * }
         */
        yield return null;
    }

    private IEnumerator RoundTurnChange() {
        tm.changeTurn();
        turn_total++;
        attacker_name = players[tm.Turn].name; // 攻守交代
        Debug.Log(attacker_name);
        yield return null;
    }

    private IEnumerator RoundSkip() {
        // ゲーム画面でスキップボタンが押されたらここを実行
        yield return null;
    }

    // 1試合が終わったかどうかを判定する
    bool isFinished() {
        /* if (3枚のシールドのどこかから情報を抜き取られたら) {
         *     return true;
         * }
         */
        return false;
    }
}
