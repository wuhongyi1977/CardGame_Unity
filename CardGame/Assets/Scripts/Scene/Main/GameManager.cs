using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Variables { // 外部変数(Tap.csで検知されたオブジェクトを保存する)
    public const int CARD_MAX = 20;
    public static GameObject player_tapped_obj = null; // プレイヤーがタップしたカードを保存する
    public static GameObject player_longtapped_obj = null; // プレイヤーが長押ししたカードを保存する(敵がフィールドに出したカードも含まれる)
    public static GameObject enemy_selected_obj = null; // 敵が選んだカードを保存する
    public static bool attacker_canSummon = false; // 攻撃側が手札からフィールドにカードを出せるかどうか
    public static bool defender_canSummon = false; // 防御側が手札からフィールドにカードを出せるかどうか
    public static bool attacker_hasSkipped = false; // 攻撃側がスキップをしたかどうか
    public static bool defender_hasSkipped = false; // 防御側がスキップをしたかどうか
    public static bool player_isSkippable = false; // プレイヤーがスキップできる状態かどうか(使わないかも)
    public static float defdown_C = 1.0f; // 機密性の防御ダウン率(30％)
    public static float defdown_I = 1.0f; // 完全性の防御ダウン率(30％)
    public static float defdown_A = 1.0f; // 可用性の防御ダウン率(30％)
    public static float defdown_O = 1.0f; // 3属性全体の防御ダウン率(10％)
    public static float def_critical = 1.0f; // クリティカル防御率(防御ダウン率は各カード保有させる？)
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
    Card field_atk_card;
    Card field_def_card;
    int field_atk_power;
    int field_def_power;
    string field_atk_attr;
    string field_def_attr;
    string tmp_attacker_name;
    GameObject[] defender_shields;
    GameObject defender_shield_C;
    GameObject defender_shield_I;
    GameObject defender_shield_A;

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
        defender_shields = Utility.GetSafeComponent<ShieldManager>(transform.Find(tmp_attacker_name + "/Shields").gameObject).SHIELDS;
        foreach (GameObject shield in defender_shields) {
            if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("C")) {
                defender_shield_C = shield;
            } else if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("I")) {
                defender_shield_I = shield;
            } else if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("A")) {
                defender_shield_A = shield;
            }
        }
        StartCoroutine(GameLoop());
    }

    // 最初に呼び出され、ゲームの各段階で順に実行される
    private IEnumerator GameLoop() {
        Debug.Log("今のターンは、" + tm.Turn);

        // 攻撃側と防御側双方が、それぞれの山札から手札にカードを加える
        yield return StartCoroutine(RoundDraw());
        // 攻撃側が手札から特殊カードorイベントカードを出す
        yield return StartCoroutine(RoundSpEvent());
        // 攻撃側が手札から攻撃カードをフィールドに出す
        yield return StartCoroutine(RoundAttack());
        // 防御側が手札から防御カードをフィールドに出す
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
        Debug.Log("Now is RoundDraw.");

        if (turn_total == 1) {
            // 最初のターンなら、双方とも自分のデッキ(山札)から5枚カードを引いて手札に加える
            Utility.GetSafeComponent<DeckSet>(attacker_deck).draw5();
            Utility.GetSafeComponent<DeckSet>(defender_deck).draw5();
        } else {
            // 最初のターンでなければ、双方とも自分のデッキから1枚カードを引いて手札に加える
            Utility.GetSafeComponent<DeckSet>(attacker_deck).draw();
            Utility.GetSafeComponent<DeckSet>(defender_deck).draw();
        }
        yield return null;
    }

    private IEnumerator RoundSpEvent() {
        Debug.Log("Now is RoundSpEvent.");

        // 攻撃側の手札で、特殊カードとイベントカードを選べるようにし、それ以外を選べないようにする
        change_isSelectable(attacker_hand, 1);
        // 攻撃側が手札から特殊カードorイベントカードを出せるかどうかのチェック
        check_canSummon(attacker_hand, 1);
        Debug.Log(Variables.attacker_canSummon);

        // 攻撃側がここでスキップしたらこのコルーチンを停止する
        if (Variables.attacker_hasSkipped) {
            Variables.attacker_hasSkipped = false; // 攻撃側で勝手にスキップ判定されるのを防ぐ
            yield break;
        }

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        // 手札からタップしたカードをフィールドに出す
        if (tmp_attacker_name.Equals("Player")) {
            // 敵(防御)側のカード(手札以外も)を全て選べないようにする
            change_isSelectable(defender_hand, 0);
            // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
            Debug.Log("selected correctly.");
        } else if (tmp_attacker_name.Equals("Enemy")) {
            if (!Variables.attacker_canSummon) {
                // 敵が攻撃側の時にイベントカードor特殊カードを出せない場合、コルーチン終了
                yield break;
            }
            Utility.GetSafeComponent<HandSet>(attacker_hand).random_summon();
            Debug.Log("Enemy has selected SP or EV card.");
            yield return new WaitForSeconds(1.0f); // 1秒待つ
        }
        summon_to_field(attacker_hand, 1);

        yield return null;
    }

    private IEnumerator RoundAttack() {
        Debug.Log("Now is RoundAttack.");

        // 攻撃側の手札で、攻撃カードを選べるようにし、それ以外を選べないようにする
        change_isSelectable(attacker_hand, 2);

        // 攻撃側が手札からカードを出せるかどうかのチェック
        check_canSummon(attacker_hand, 2);

        // 攻撃側がここでスキップしたらこのコルーチンを停止する
        if (Variables.attacker_hasSkipped) {
            yield break;
        }

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        // 手札からタップしたカードをフィールドに出す
        if (tmp_attacker_name.Equals("Player")) {
            // 敵(防御)側のカード(手札以外も)を全て選べないようにする
            change_isSelectable(defender_hand, 0);
            // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
            Debug.Log("selected correctly.");
        } else if (tmp_attacker_name.Equals("Enemy")) {
            if (!Variables.attacker_canSummon) {
                // 敵が攻撃側の時に攻撃カードを出せない場合、コルーチン終了
                yield break;
            }
            Utility.GetSafeComponent<HandSet>(attacker_hand).random_summon();
            Debug.Log("Enemy has selected ATK card.");
            yield return new WaitForSeconds(1.0f); // 1秒待つ
        }
        summon_to_field(attacker_hand, 2);

        yield return null;
    }

    private IEnumerator RoundDefense() {
        Debug.Log("Now is RoundDefense.");

        // 攻撃側(Player or Enemy)がスキップしていたら、防御側が手札を出すフェーズをスキップさせる
        if (Variables.attacker_hasSkipped) {
            Variables.attacker_hasSkipped = false; // 次のターンの攻撃側が勝手にスキップされるのを防ぐ
            yield break;
        }

        // 防御側の手札で、防御カードを選べるようにし、それ以外を選べないようにする
        change_isSelectable(defender_hand, 3);

        // 防御側が手札からカードを出せるかどうかのチェック
        check_canSummon(defender_hand, 3);

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        // 手札からタップしたカードをフィールドに出す
        if (tmp_attacker_name.Equals("Enemy")) { // プレイヤーが防御側のとき
            // プレイヤー(攻撃)側のカード(手札以外も)を全て選べないようにする
            change_isSelectable(attacker_hand, 0);
            // yield return new WaitUntil(): ()内の再開条件で指定した関数がtrueを返すまで処理を中断する(() => ...は、引数無しのラムダ式)
            yield return new WaitUntil(() => Variables.player_tapped_obj != null);
            Debug.Log("selected correctly.");
        } else if (tmp_attacker_name.Equals("Player")) { // 敵が防御側のとき
            if (!Variables.defender_canSummon) {
                // 敵が防御側の時に防御カードを出せない場合、コルーチン終了
                yield break;
            }
            Utility.GetSafeComponent<HandSet>(defender_hand).random_summon();
            Debug.Log("Enemy has selected DEF card.");
            yield return new WaitForSeconds(1.0f); // 1秒待つ
        }
        summon_to_field(defender_hand, 3);

        yield return null;
    }

    private IEnumerator RoundCalcDamage() {
        Debug.Log("Now is RoundCalcDamage.");

        if (!Variables.defender_canSummon) {
            // フィールドに出ているカードの情報を保存
            field_atk_card = Utility.GetSafeComponent<Card>(attacker_field.transform.GetChild(0).gameObject);
            field_def_card = Utility.GetSafeComponent<Card>(defender_field.transform.GetChild(0).gameObject);
            field_atk_power = field_atk_card.POWER;
            field_def_power = field_def_card.POWER;
            field_atk_attr = field_atk_card.ATTRIBUTE;
            field_def_attr = field_atk_card.ATTRIBUTE;

            // 防御側の防御カードの属性=攻撃側の攻撃カードの属性ならば、防御クリティカル発生
            if (field_def_attr.Equals(field_atk_attr)) {
                Variables.def_critical = 0.5f; // 防御クリティカルにより、ダメージ半減
            } else {
                Variables.def_critical = 1.0f;
            }
        } else {
            // 防御側が防御カードを出していなければ防御パワーは無い
            field_def_power = 0;
        }

        // 実際のダメージ計算。ダメージ = 攻撃力 - 防御力 * 防御力ダウン率 * クリティカル防御率
        // ※イベントカードで防御が下がっている時、その倍率は1属性のみの場合30％、3属性全体の場合10％
        // ※Card.csの特殊・イベントカードの処理内で防御ダウン率を更新してもらえるとありがたいです(Variables.defdown_C = 0.7f;みたいな感じで)

        if (field_atk_attr.Equals("Confidentiality")) { // 機密性の攻撃のとき
            Variables.damage = field_atk_power - (int)(field_def_power * Variables.defdown_C * Variables.def_critical);
            Utility.GetSafeComponent<Shield>(defender_shield_C).HP -= Variables.damage;
        } else if (field_atk_attr.Equals("Integrity")) { // 完全性の攻撃のとき
            Variables.damage = field_atk_power - (int)(field_def_power * Variables.defdown_I * Variables.def_critical);
            Utility.GetSafeComponent<Shield>(defender_shield_I).HP -= Variables.damage;
        } else if (field_atk_attr.Equals("Availability")) { // 可用性の攻撃のとき
            Variables.damage = field_atk_power - (int)(field_def_power * Variables.defdown_A * Variables.def_critical);
            Utility.GetSafeComponent<Shield>(defender_shield_A).HP -= Variables.damage;
        } else if (field_atk_attr.Equals("Overall")) { // 全体的な攻撃のとき
            Variables.damage = field_atk_power - (int)(field_def_power * Variables.defdown_O * Variables.def_critical);
            Utility.GetSafeComponent<Shield>(defender_shield_C).HP -= Variables.damage;
            Utility.GetSafeComponent<Shield>(defender_shield_I).HP -= Variables.damage;
            Utility.GetSafeComponent<Shield>(defender_shield_A).HP -= Variables.damage;
        }

        yield return null;
    }

    private IEnumerator RoundTomb() {
        Debug.Log("Now is RoundTomb.");
        // 特殊・イベントカードを含めた、フィールドに出ているカードを、ターン交代直前に墓地に移動させる
        // HPが0になったシールドは、そのまま残しておく

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        if (attacker_field.transform.childCount > 0) {
            GameObject attacker_field_child = attacker_field.transform.GetChild(0).gameObject;
            attacker_field_child.transform.parent = attacker_tomb.transform;
        }

        yield return new WaitForSeconds(1.0f); // 1秒待つ

        if (defender_field.transform.childCount > 0) {
            GameObject defender_field_child = defender_field.transform.GetChild(0).gameObject;
            defender_field_child.transform.parent = defender_tomb.transform;
        }
        // ※イベントエリア内のカード移動は、HandSet.csのメソッドevent_summonの実装ができてから。
        // 防御ダウン率のリセット(イベントカードの持続時間が1ターンの場合しか対応できないので、ターンカウンタとか用意して対応させようかと思います)
        Variables.defdown_C = 1.0f;
        Variables.defdown_I = 1.0f;
        Variables.defdown_A = 1.0f;
        Variables.defdown_O = 1.0f;

        yield return null;
    }

    private IEnumerator RoundTurnChange() {
        Debug.Log("Now is RoundTurnChange.");
        tm.changeTurn();
        turn_total++;
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
        defender_shields = Utility.GetSafeComponent<ShieldManager>(transform.Find(tmp_attacker_name + "/Shields").gameObject).SHIELDS;
        foreach (GameObject shield in defender_shields) {
            if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("C")) {
                defender_shield_C = shield;
            } else if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("I")) {
                defender_shield_I = shield;
            } else if (Utility.GetSafeComponent<Shield>(shield).ATTRIBUTE.Equals("A")) {
                defender_shield_A = shield;
            }
        }
        yield return null;
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
            case 1: // 特殊カードとイベントカードをtrue、それ以外をfalseにする
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("EV") && !hand_child_card.TYPE.Equals("SP")) {
                        hand_child_card.ISSELECTABLE = false;
                    } else {
                        hand_child_card.ISSELECTABLE = true;
                    }
                }
                break;
            case 2: // 攻撃カードをtrue、それ以外をfalseにする
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("ATK")) {
                        hand_child_card.ISSELECTABLE = false;
                    } else {
                        hand_child_card.ISSELECTABLE = true;
                    }
                }
                break;
            case 3: // 防御カードをtrue、それ以外をfalseにする
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (!hand_child_card.TYPE.Equals("DEF")) {
                        hand_child_card.ISSELECTABLE = false;
                    } else {
                        hand_child_card.ISSELECTABLE = true;
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
                        break;
                    } else {
                        Variables.attacker_canSummon = false;
                    }
                }
                break;
            case 2: // 攻撃フェーズで攻撃側がカードを出せるかどうか
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (hand_child_card.TYPE.Equals("ATK")) {
                        Variables.attacker_canSummon = true;
                        break;
                    } else {
                        Variables.attacker_canSummon = false;
                    }
                }
                break;
            case 3: // 防御フェーズで防御側がカードを出せるかどうか
                foreach (Transform hand_child in hand.transform) {
                    Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
                    if (hand_child_card.TYPE.Equals("DEF")) {
                        Variables.defender_canSummon = true;
                        break;
                    } else {
                        Variables.defender_canSummon = false;
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
                    Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.player_tapped_obj);
                    // 操作系の変数の中身をリセット
                    Variables.player_tapped_obj = null;
                    Variables.player_isSkippable = false;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    Utility.GetSafeComponent<HandSet>(attacker_hand).summon(Variables.enemy_selected_obj);
                    // 操作系の変数の中身をリセット
                    Variables.enemy_selected_obj = null;
                }
                Variables.attacker_canSummon = false;
                break;
            case 3: // RoundDefenseのフェーズで、防御側がカードを出す
                if (tmp_attacker_name.Equals("Player")) {
                    Utility.GetSafeComponent<HandSet>(defender_hand).summon(Variables.enemy_selected_obj);
                    // 操作系の変数の中身をリセット
                    Variables.enemy_selected_obj = null;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    Utility.GetSafeComponent<HandSet>(defender_hand).summon(Variables.player_tapped_obj);
                    // 操作系の変数の中身をリセット
                    Variables.player_tapped_obj = null;
                    Variables.player_isSkippable = false;
                }
                Variables.defender_canSummon = false;
                break;
            default:
                break;
        }
    }

    // 1試合が終わったかどうかを判定する
    bool isFinished() {
        // プレイヤーと敵のシールドの総HPを計算
        GameObject[] player_shields = Utility.GetSafeComponent<ShieldManager>(transform.Find("Player/Shields").gameObject).SHIELDS;
        GameObject[] enemy_shields = Utility.GetSafeComponent<ShieldManager>(transform.Find("Enemy/Shields").gameObject).SHIELDS;
        int player_hp_total = 0, enemy_hp_total = 0;
        foreach (GameObject player_shield in player_shields) {
            player_hp_total += Utility.GetSafeComponent<Shield>(player_shield).HP;
        }
        foreach (GameObject enemy_shield in enemy_shields) {
            enemy_hp_total += Utility.GetSafeComponent<Shield>(enemy_shield).HP;
        }

        // プレイヤーと敵それぞれが攻撃カードを持っているかどうかを確認
        bool player_hasATK = false, enemy_hasATK = false;
        foreach (Transform hand_child in attacker_hand.transform) {
            Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
            if (hand_child_card.TYPE.Equals("ATK")) {
                if (tmp_attacker_name.Equals("Player")) {
                    player_hasATK = true;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    enemy_hasATK = true;
                }
                break;
            }
        }
        foreach (Transform hand_child in defender_hand.transform) {
            Card hand_child_card = Utility.GetSafeComponent<Card>(hand_child.gameObject);
            if (hand_child_card.TYPE.Equals("ATK")) {
                if (tmp_attacker_name.Equals("Player")) {
                    enemy_hasATK = true;
                } else if (tmp_attacker_name.Equals("Enemy")) {
                    player_hasATK = true;
                }
                break;
            }
        }

        // // 3枚のシールドの総HPが0になったら or 攻撃側と防御側の双方に、出せる攻撃カードが無くなったら、ゲーム終了判定
        return (player_hp_total == 0 || enemy_hp_total == 0) || (!player_hasATK && !enemy_hasATK);
    }
}
