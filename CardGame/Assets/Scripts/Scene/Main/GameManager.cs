using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Variables { // 外部変数(Tap.csで検知されたオブジェクトを保存する)
    public const int CARD_MAX = 20;
    public static GameObject Player_tapped_obj = null; // プレイヤーがタップしたカードを保存する
    public static GameObject Player_longtapped_obj = null; // プレイヤーが長押ししたカードを保存する(敵がフィールドに出したカードも含まれる)
    public static GameObject Enemy_selected_obj = null; // 敵が選んだカードを保存する
    public static bool Player_isSkippable = false; // プレイヤーがスキップできる状態かどうか
    public static bool Player_isSkipped = false; // プレイヤーがスキップボタンが押されたかどうか
    public static bool Enemy_isSkipped = false; // CPUがスキップボタンが押されたかどうか
    public static float def_critical = 1.2f; // クリティカル防御率(防御ダウン率は各カード保有させる？)
    public static int damage = 0; // ダメージ
}

public class TurnManager {
    private int turn;

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
}

public class GameManager : MonoBehaviour {
    int turn_total;
    int turn_tmp;
    GameObject[] players;
    TurnManager tm;
    GameObject deck_atkside;
    GameObject hand_atkside;
    GameObject field_atkside;
    GameObject tomb_atkside;
    GameObject deck_defside;
    GameObject hand_defside;
    GameObject field_defside;
    GameObject tomb_defside;

    // Use this for initialization
    void Start() {
        turn_total = 1;
        turn_tmp = 0;
        players = new GameObject[2];
        players[0] = transform.Find("Player").gameObject;
        players[1] = transform.Find("Enemy").gameObject;
        tm = new TurnManager(turn_tmp);
        deck_atkside = players[turn_tmp].transform.Find("Cards/Deck").gameObject;
        hand_atkside = players[turn_tmp].transform.Find("Cards/Hand").gameObject;
        field_atkside = players[turn_tmp].transform.Find("Cards/Field").gameObject;
        tomb_atkside = players[turn_tmp].transform.Find("Cards/Tomb").gameObject;
        deck_defside = players[tm.getNextTurn()].transform.Find("Cards/Deck").gameObject;
        hand_defside = players[tm.getNextTurn()].transform.Find("Cards/Hand").gameObject;
        field_defside = players[tm.getNextTurn()].transform.Find("Cards/Field").gameObject;
        tomb_defside = players[tm.getNextTurn()].transform.Find("Cards/Tomb").gameObject;
        StartCoroutine(GameLoop());
    }

    // 最初に呼び出され、ゲームの各段階で順に実行される
    private IEnumerator GameLoop() {
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
        // 最初のターンなら、デッキ(山札)から5枚引いて手札に加える
        // 最初のターンでなければ、デッキから1枚カードを引く
        // 手札からフィールドに出せるカードが1枚も無い場合、スキップ
        if (turn_total == 1) {
            if (turn_tmp == 0) {
                Utility.GetSafeComponent<DeckSet>(deck_atkside).draw5();
            } else if (turn_tmp == 1) {
                Utility.GetSafeComponent<DeckSet>(deck_defside).draw5();
            }
        } else {
            if (turn_tmp == 0) {
                Utility.GetSafeComponent<DeckSet>(deck_atkside).draw();
            } else if (turn_tmp == 1) {
                Utility.GetSafeComponent<DeckSet>(deck_defside).draw();
            }
        }
        
        yield return null;
    }

    private IEnumerator RoundSpEvent() {
        // 特殊カードorイベントカードを出す(イベントカードを使い終わったら、それを使った人の墓地に送らなければならない)
        // 攻撃側(現在のターンのプレイヤー)の手札の特殊カード、イベントカード以外のカードを選べなくする
        
        // 手札から選んだ(タップした)カードをフィールドに出す
        if (Variables.Player_tapped_obj != null) {
            foreach (Transform hand_child in hand_atkside.transform) {
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("EV") && !Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("SP")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
                }
            }

            string taped_card_type = Utility.GetSafeComponent<Card>(Variables.Player_tapped_obj).TYPE;
            if (taped_card_type.Equals("EV")) {
                Utility.GetSafeComponent<HandSet>(hand_atkside).event_summon(Variables.Player_tapped_obj);
            } else if (taped_card_type.Equals("SP")) {
                Utility.GetSafeComponent<HandSet>(hand_atkside).summon(Variables.Player_tapped_obj);
            }
        } else if (Variables.Player_isSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        // 特殊カードorイベントカードが発動し終わったら墓地へ移動する
        yield return null;
    }

    private IEnumerator RoundAttack() {
        // 攻撃側が攻撃カードを選んでフィールドに出す
        if (Variables.Player_tapped_obj != null) {
            foreach (Transform hand_child in hand_defside.transform) { // 防御側のカード全てを選べないようにする(turnが0ならplayers[1]のカード選択を制限する)
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
            }
            foreach (Transform hand_child in hand_atkside.transform) { // 攻撃側の攻撃カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("ATK")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
                }
            }

            Utility.GetSafeComponent<HandSet>(hand_atkside).summon(Variables.Player_tapped_obj);
        } else if (Variables.Player_isSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        yield return null;
    }

    private IEnumerator RoundDefense() {
        // 守備側が防御カードを選ぶ
        if (Variables.Player_tapped_obj != null) {
            foreach (Transform hand_child in hand_atkside.transform) { // 攻撃側のカード全てを選べないようにする
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
            }
            foreach (Transform hand_child in hand_defside.transform) { // 防御側の防御カード以外を選べないようにする
                if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("DEF")) {
                    Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
                }
            }

            Utility.GetSafeComponent<HandSet>(hand_defside).summon(Variables.Player_tapped_obj);
        } else if (Variables.Player_isSkipped) {
            // スキップしたときはコルーチンを終了(この辺の実装は不完全)
            yield break;
        }
        yield return null;
    }

    private IEnumerator RoundCalcDamage() {
        // 攻撃側なら攻撃したとき、防御側なら攻撃されたときのダメージを計算する
        // 防御側の防御カードの属性＝攻撃側のカードの属性なら、防御クリティカル発生
        // ダメージ＝攻撃力―防御力＊防御力ダウン率＊クリティカル防御率
        
        /*
        int atk_power = Utility.GetSafeComponent<Card>(Variables.field_atkside.transform.GetChild(0).gameObject).POWER;
        int def_power = Utility.GetSafeComponent<Card>(Variables.field_defside.transform.GetChild(0).gameObject).POWER;
        string field_atk = Utility.GetSafeComponent<Card>(Variables.field_atkside.transform.GetChild(0).gameObject).ATTRIBUTE;
        string field_def = Utility.GetSafeComponent<Card>(Variables.field_defside.transform.GetChild(0).gameObject).ATTRIBUTE;
        if (field_def.Equals(field_atk)) {
            def_critical = 0.5f;
        }
        damage = atk_power - (int)(def_power * def_down * def_critical);*/
        yield return null;
    }

    private IEnumerator RoundTomb() {
        // HPが0になったカードを墓地に送る
        // フィールドにいるカード(攻撃、防御、イベント)が、墓地に行く可能性有
        /* if (FieldのカードのHP == 0) {
         *     field_defside.parent = tomb_defside;
         * }
         */
        yield return null;
    }

    private IEnumerator RoundTurnChange() {
        tm.changeTurn();
        turn_total++;
        deck_atkside = players[turn_tmp].transform.Find("Cards/Deck").gameObject;
        hand_atkside = players[turn_tmp].transform.Find("Cards/Hand").gameObject;
        field_atkside = players[turn_tmp].transform.Find("Cards/Field").gameObject;
        tomb_atkside = players[turn_tmp].transform.Find("Cards/Tomb").gameObject;
        deck_defside = players[tm.getNextTurn()].transform.Find("Cards/Deck").gameObject;
        hand_defside = players[tm.getNextTurn()].transform.Find("Cards/Hand").gameObject;
        field_defside = players[tm.getNextTurn()].transform.Find("Cards/Field").gameObject;
        tomb_defside = players[tm.getNextTurn()].transform.Find("Cards/Tomb").gameObject;
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
