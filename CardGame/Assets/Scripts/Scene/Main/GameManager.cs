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
=======
    public static bool attacker_canSummon = false; // 攻撃側が手札からフィールドにカードを出せるかどうか
    public static bool defender_canSummon = false; // 防御側が手札からフィールドにカードを出せるかどうか
    public static bool attacker_hasSkipped = false; // 攻撃側がスキップをしたかどうか
    public static bool defender_hasSkipped = false; // 防御側がスキップをしたかどうか
    public static bool player_isSkippable = false; // プレイヤーがスキップできる状態かどうか(使わないかも)

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
=======
    TurnManager tm;
    int turn_total;
    GameObject[] players;
    GameObject attacker_deck;
    GameObject attacker_hand;
    GameObject attacker_field;
    GameObject attacker_tomb;
    GameObject defender_deck;
    GameObject defender_hand;
    GameObject defender_field;
    GameObject defender_tomb;
    string tmp_attacker_name;

    // Use this for initialization
    void Start() {
        tm = new TurnManager(0);
        turn_total = 1;
        players = new GameObject[2];
        players[0] = transform.Find("Player").gameObject;
        players[1] = transform.Find("Enemy").gameObject;
        attacker_deck = players[tm.Turn].transform.Find("Cards/Deck").gameObject;
        attacker_hand = players[tm.Turn].transform.Find("Cards/Hand").gameObject;
        attacker_field = players[tm.Turn].transform.Find("Cards/Field").gameObject;
        attacker_tomb = players[tm.Turn].transform.Find("Cards/Tomb").gameObject;
        defender_deck = players[tm.getNextTurn()].transform.Find("Cards/Deck").gameObject;
        defender_hand = players[tm.getNextTurn()].transform.Find("Cards/Hand").gameObject;
        defender_field = players[tm.getNextTurn()].transform.Find("Cards/Field").gameObject;
        defender_tomb = players[tm.getNextTurn()].transform.Find("Cards/Tomb").gameObject;
        tmp_attacker_name = players[tm.Turn].name; // "Player"か"Enemy"が入る

        StartCoroutine(GameLoop());
    }

    // 最初に呼び出され、ゲームの各段階で順に実行される
    private IEnumerator GameLoop() {
        Debug.Log("今のターンは、" + tm.Turn);


        // ドロー
=======
        // 攻撃側と防御側双方がドローする

        yield return StartCoroutine(RoundDraw());
        // 攻撃側が特殊カードorイベントカードを出す
        yield return StartCoroutine(RoundSpEvent());
        // 攻撃側が攻撃カードをフィールドに出す
        yield return StartCoroutine(RoundAttack());
        // 防御側が防御カードをフィールドに出す
        yield return StartCoroutine(RoundDefense());
        // ダメージを計算・反映させる
        if (!Variables.attacker_hasSkipped) {
            yield return StartCoroutine(RoundCalcDamage());
        }
        // そのターンにフィールドに出たカードを墓地に送る
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
=======
            Utility.GetSafeComponent<DeckSet>(attacker_deck).draw5();
            Utility.GetSafeComponent<DeckSet>(defender_deck).draw5();
        } else {
            // 最初のターンでなければ、双方とも自分のデッキから1枚カードを引いて手札に加える
            Utility.GetSafeComponent<DeckSet>(attacker_deck).draw();
            Utility.GetSafeComponent<DeckSet>(defender_deck).draw();
        }
        yield return new WaitForSeconds(1.0f); // 1秒待つ
    }

    private IEnumerator RoundSpEvent() {
        // 防御側のカード全てを選べないようにする
        change_isSelectable(defender_hand, 0);

        // 攻撃側の手札で、特殊カードとイベントカード以外のカードを選べなくする
        change_isSelectable(attacker_hand, 1);

        // 攻撃側が特殊カードorイベントカードを出せるかどうかのチェック
        check_canSummon(attacker_hand, 1);

        // 攻撃側がここでスキップしたらこのコルーチンを停止する
        if (Variables.attacker_hasSkipped) {
            yield break;
        }

        // 手札からタップしたカードをフィールドに出す
        // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
        if (tmp_attacker_name.Equals("Player")) {
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
        } else if (tmp_attacker_name.Equals("Enemy")) {
            yield return new WaitUntil(() => Variables.enemy_selected_obj != null);
        }
        summon_to_field(attacker_hand, 1);
        
        yield return new WaitForSeconds(1.0f); // 1秒待つ
    }

    private IEnumerator RoundAttack() {
        // 防御側のカード全てを選べないようにする
        change_isSelectable(defender_hand, 0);

        // 攻撃側の攻撃カード以外を選べないようにする
        change_isSelectable(attacker_hand, 2);

        // 攻撃側がカードを出せるかどうかのチェック
        check_canSummon(attacker_hand, 2);

        // 攻撃側がここでスキップしたらこのコルーチンを停止する
        if (Variables.attacker_hasSkipped) {
            yield break;
        }

        // 手札からタップしたカードをフィールドに出す
        // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
        if (tmp_attacker_name.Equals("Player")) {
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
        } else if (tmp_attacker_name.Equals("Enemy")) {
            yield return new WaitUntil(() => Variables.enemy_selected_obj != null);
        }
        summon_to_field(attacker_hand, 2);

        yield return new WaitForSeconds(1.0f); // 1秒待つ
    }

    private IEnumerator RoundDefense() {
        // 攻撃側(Player or Enemy)がスキップしていたら、防御側が手札を出すフェーズをスキップさせる
        if (Variables.attacker_hasSkipped) {
            yield break;
        }

        ///*

        // 攻撃側のカード全てを選べないようにする
        change_isSelectable(attacker_hand, 0);

        // 防御側の防御カード以外を選べないようにする
        change_isSelectable(defender_hand, 3);

        // 防御側がカードを出せるかどうかのチェック
        check_canSummon(defender_hand, 3);

        // 手札からタップしたカードをフィールドに出す
        // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
        if (tmp_attacker_name.Equals("Player")) {
            yield return new WaitUntil(() => Variables.enemy_selected_obj != null);
        } else if (tmp_attacker_name.Equals("Enemy")) {
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
        }
        summon_to_field(defender_hand, 3);

        //*/
        yield return new WaitForSeconds(1.0f); // 1秒待つ

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
=======
        atk_power = Utility.GetSafeComponent<Card>(attacker_field.transform.GetChild(0).gameObject).POWER;
        def_power = Utility.GetSafeComponent<Card>(defender_field.transform.GetChild(0).gameObject).POWER;
        field_atk_attr = Utility.GetSafeComponent<Card>(attacker_field.transform.GetChild(0).gameObject).ATTRIBUTE;
        field_def_attr = Utility.GetSafeComponent<Card>(defender_field.transform.GetChild(0).gameObject).ATTRIBUTE;
        // 防御側のフィールドに出ているカードの属性=攻撃側のフィールドに出ているカードならば、防御クリティカル発生
        if (field_def_attr.Equals(field_atk_attr)) {
            Variables.def_critical = 0.5f;
        }

        */

        // 実際のダメージ計算。プレイヤーと敵が持つ3属性の攻撃力・防御力はどこで保持する・・・？
        // Variables.damage = atk_power - (int)(def_power * Variables.def_down * Variables.def_critical);
        yield return new WaitForSeconds(1.0f); // 1秒待つ
    }

    private IEnumerator RoundTomb() {
        // 特殊・イベントカードを含めた、フィールドに出ているカードを、ターン交代直前に墓地に移動させる
        // HPが0になったシールドは、そのまま残しておく

        if (attacker_field.transform.childCount > 0) {
            // 攻撃側と防御側のフィールド双方に
            GameObject attacker_field_child = attacker_field.transform.GetChild(0).gameObject;
            attacker_field_child.transform.parent = attacker_tomb.transform;
        }
        if (defender_field.transform.childCount > 0) {
            GameObject defender_field_child = defender_field.transform.GetChild(0).gameObject;
            defender_field_child.transform.parent = defender_tomb.transform;
        }
        // イベントエリア内のカード移動は、HandSet.csのメソッドevent_summonの実装ができてから。

        yield return new WaitForSeconds(1.0f); // 1秒待つ

    }

    private IEnumerator RoundTurnChange() {
        tm.changeTurn();
        turn_total++;

        attacker_name = players[tm.Turn].name; // 攻守交代
        Debug.Log(attacker_name);
        yield return null;
=======
        tmp_attacker_name = players[tm.Turn].name;
        // 攻守交代のため、攻撃側と防御側のGameObjectをそれぞれ更新
        attacker_deck = players[tm.Turn].transform.Find("Cards/Deck").gameObject;
        attacker_hand = players[tm.Turn].transform.Find("Cards/Hand").gameObject;
        attacker_field = players[tm.Turn].transform.Find("Cards/Field").gameObject;
        attacker_tomb = players[tm.Turn].transform.Find("Cards/Tomb").gameObject;
        defender_deck = players[tm.getNextTurn()].transform.Find("Cards/Deck").gameObject;
        defender_hand = players[tm.getNextTurn()].transform.Find("Cards/Hand").gameObject;
        defender_field = players[tm.getNextTurn()].transform.Find("Cards/Field").gameObject;
        defender_tomb = players[tm.getNextTurn()].transform.Find("Cards/Tomb").gameObject;
        Debug.Log(tmp_attacker_name);
        yield return new WaitForSeconds(1.0f); // 1秒待つ

    }

    // モードごとに、攻撃側or防御側のカードを選べないようにする
    void change_isSelectable(GameObject hand, int mode) {
        switch (mode) {
            case 0: // 全てfalse
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    hand_child_card.ISSELECTABLE = false;
                }
                break;
            case 1: // 特殊カードとイベントカード以外をfalse
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("EV") && !hand_child_card.TYPE.Equals("SP")) {
                        hand_child_card.ISSELECTABLE = false;
                    }
                }
                break;
            case 2: // 攻撃カード以外をfalse
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("ATK")) {
                        hand_child_card.ISSELECTABLE = false;
                    }
                }
                break;
            case 3: // 防御カード以外をfalse
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("DEF")) {
                        hand_child_card.ISSELECTABLE = false;
                    }
                }
                break;
            default: // モードが範囲外なら何もしない
                break;
        }
    }

    // 各フェーズで、攻撃側or防御側がカードを出せるかどうかチェックしてcanSummonを更新する
    void check_canSummon(GameObject hand, int phase) {
        switch (phase) {
            case 1: // 特殊カード・イベントカードを出すフェーズで攻撃側がカードを出せるかどうか
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (hand_child_card.TYPE.Equals("EV") || hand_child_card.TYPE.Equals("SP")) {
                        Variables.attacker_canSummon = true;
                    }
                }
                break;
            case 2: // 攻撃フェーズで攻撃側がカードを出せるかどうか
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("ATK")) {
                        Variables.attacker_canSummon = true;
                    }
                }
                break;
            case 3: // 防御フェーズで防御側がカードを出せるかどうか
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("DEF")) {
                        Variables.defender_canSummon = true;
                    }
                }
                break;
            default: // モードが範囲外なら何もしない
                break;
        }
    }

    // 各フェーズで、攻撃側or防御側がカードを手札からフィールドに出す
    void summon_to_field(GameObject hand, int phase) {
        switch (phase) {
            case 1: // RoundSpEventのフェーズで、攻撃側がカードを出す
                if (tmp_attacker_name.Equals("Player")) {
                    string player_tapped_cardtype = Utility.GetSafeComponent<Card>(Variables.player_tapped_obj).TYPE;
                    if (player_tapped_cardtype.Equals("EV")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).event_summon(Variables.player_tapped_obj);
                    } else if (player_tapped_cardtype.Equals("SP")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.player_tapped_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.player_tapped_obj = null;
                    Variables.player_isSkippable = false;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    string enemy_selected_cardtype = Utility.GetSafeComponent<Card>(Variables.enemy_selected_obj).TYPE;
                    if (enemy_selected_cardtype.Equals("EV")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).event_summon(Variables.enemy_selected_obj);
                    } else if (enemy_selected_cardtype.Equals("SP")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.enemy_selected_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.enemy_selected_obj = null;
                }
                Variables.attacker_canSummon = false;
                break;
            case 2: // RoundAttackのフェーズで、攻撃側がカードを出す
                if (tmp_attacker_name.Equals("Player")) {
                    string player_tapped_cardtype = Utility.GetSafeComponent<Card>(Variables.player_tapped_obj).TYPE;
                    if (player_tapped_cardtype.Equals("ATK")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.player_tapped_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.player_tapped_obj = null;
                    Variables.player_isSkippable = false;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    string enemy_selected_cardtype = Utility.GetSafeComponent<Card>(Variables.enemy_selected_obj).TYPE;
                    if (enemy_selected_cardtype.Equals("ATK")) {
                        Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.enemy_selected_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.enemy_selected_obj = null;
                }
                Variables.attacker_canSummon = false;
                break;
            case 3: // RoundDefenseのフェーズで、防御側がカードを出す
                if (tmp_attacker_name.Equals("Player")) {
                    string enemy_selected_cardtype = Utility.GetSafeComponent<Card>(Variables.enemy_selected_obj).TYPE;
                    if (enemy_selected_cardtype.Equals("DEF")) {
                        Utility.GetSafeComponent<HandSet>(defender_hand).summon(Variables.enemy_selected_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.enemy_selected_obj = null;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    string player_tapped_cardtype = Utility.GetSafeComponent<Card>(Variables.player_tapped_obj).TYPE;
                    if (player_tapped_cardtype.Equals("DEF")) {
                        Utility.GetSafeComponent<HandSet>(defender_hand).summon(Variables.player_tapped_obj);
                    }
                    // 操作系の変数の中身をリセット
                    Variables.player_tapped_obj = null;
                    Variables.player_isSkippable = false;
                }
                Variables.defender_canSummon = false;
                break;
        }
    }

    // 1試合が終わったかどうかを判定する
    bool isFinished() {
        /* if (3枚のシールドの総HPが0になったら || 攻撃側と防御側の双方に、出せる攻撃カードが無くなったら？) {
         *     return true;
         * }
         */
        return false;
    }
}
