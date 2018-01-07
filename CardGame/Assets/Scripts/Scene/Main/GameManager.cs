using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Constants { // 定数
    public const int CARD_MAX = 20;
}

public static class Variables { // 外部変数
    internal static int tmp_turn;
    internal static GameObject[] players;
    internal static TurnManager tm;
    internal static GameObject deck_atkside;
    internal static GameObject hand_atkside;
    internal static GameObject field_atkside;
    internal static GameObject tomb_atkside;
    internal static GameObject deck_defside;
    internal static GameObject hand_defside;
    internal static GameObject field_defside;
    internal static GameObject tomb_defside;
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

    // Use this for initialization
    void Start() {
        Variables.tmp_turn = 0;
        Variables.players = new GameObject[2];
        Variables.players[0] = transform.FindChild("Player").gameObject;
        Variables.players[1] = transform.FindChild("Enemy").gameObject;
        Variables.tm = new TurnManager(Variables.tmp_turn);
        Variables.deck_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Deck").gameObject;
        Variables.hand_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Hand").gameObject;
        Variables.field_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Field").gameObject;
        Variables.tomb_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Tomb").gameObject;
        Variables.deck_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Deck").gameObject;
        Variables.hand_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Hand").gameObject;
        Variables.field_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Field").gameObject;
        Variables.tomb_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Tomb").gameObject;
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

            Variables.deck_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Deck").gameObject;
            Variables.hand_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Hand").gameObject;
            Variables.field_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Field").gameObject;
            Variables.tomb_atkside = Variables.players[Variables.tmp_turn].transform.FindChild("Cards/Tomb").gameObject;
            Variables.deck_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Deck").gameObject;
            Variables.hand_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Hand").gameObject;
            Variables.field_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Field").gameObject;
            Variables.tomb_defside = Variables.players[Variables.tm.getNextTurn()].transform.FindChild("Cards/Tomb").gameObject;

            // ゲーム終了判定がされなければ、コルーチンGameLoopをリスタートする
            // 現時点で行っているGameLoopは終了する
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundDraw() {
        // 最初のターンなら、デッキ(山札)から5枚引いて手札に加える
        // 最初のターンでなければ、デッキから1枚カードを引く
        // 手札からフィールドに出せるカードが1枚も無い場合、スキップ
        if (Variables.tmp_turn == 0) {
            // draw5();
        } else if (Variables.tmp_turn == 1) {
            // Utility.GetSafeComponent<DeckSet>(deck).draw();
        }
        yield return null;
    }

    private IEnumerator RoundSpEvent() {
        // 特殊カードorイベントカードを出す(イベントカードを使い終わったら、それを使った人の墓地に送らなければならない)
        foreach (Transform hand_child in Variables.hand_atkside.transform) {
            if (!Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("EV") && !Utility.GetSafeComponent<Card>(hand_child.gameObject).TYPE.Equals("SP")) {
                // 手札のカードがイベントカードでも特殊カードでもない場合
                Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
            }
        }
        // 手札から選んだ(タップした)カードをフィールドに出す
        //event_summon();
        yield return null;
    }

    private IEnumerator RoundAttack() {
        /* 攻撃側が攻撃カードを選んでフィールドに出す
         * 防御側のカード全てを選べないようにする(turnが0ならplayers[1]のカード選択を制限する)
         */
        foreach (Transform hand_child in Variables.hand_defside.transform) {
            Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
        }
        yield return null;
    }

    private IEnumerator RoundDefense() {
        /* 守備側が防御カードを選ぶ
         * 攻撃側のカード全てを選べないようにする
         */
         foreach (Transform hand_child in Variables.hand_atkside.transform) {
            Utility.GetSafeComponent<Card>(hand_child.gameObject).ISSELECT = false;
        }
        yield return null;
    }

    private IEnumerator RoundCalcDamage() {
        // 攻撃側なら攻撃したとき、防御側なら攻撃されたときのダメージを計算する
        // 防御側の防御カードの属性＝攻撃側のカードの属性なら、防御クリティカル発生
        // ダメージ＝攻撃力―防御力＊防御力ダウン率＊クリティカル防御率
        float def_down = 0.7f;
        float def_critical = 1.0f;
        int damage = 0;
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
        Variables.tm.changeTurn();
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
