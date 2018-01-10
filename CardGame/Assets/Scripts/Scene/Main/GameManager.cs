using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Constants {
    public const int CARD_MAX = 20;
}

public class TurnManager {
    public int turn;

    public TurnManager(int t)
    {
        this.turn = t;
    }

    // もう一方のターンを取得する
    public int getNextTurn()
    {
        if (turn == 0) return 1;
        else if (turn == 1) return 0;
        return -1;
    }
    // ターンを切り替える
    public void changeTurn()
    {
        if (turn == 0){
            turn = 1;
        } else if (turn == 1)
        {
            turn = 0;
        }
    }
}

public class GameManager : MonoBehaviour {
    private int turn;
    private GameObject[] players;
    private TurnManager tm;
    private GameObject deck;
    private GameObject field;
    private GameObject tomb;

	// Use this for initialization
	void Start () {
        turn = 0;
        players = new GameObject[2];
        players[0] = transform.FindChild("Player").gameObject;
        players[1] = transform.FindChild("Enemy").gameObject;
        deck = players[turn].transform.FindChild("Cards/Deck").gameObject;
        field = players[turn].transform.FindChild("Cards/Field").gameObject;
        tomb = players[turn].transform.FindChild("Cards/Tomb").gameObject;
        tm = new TurnManager(turn);
        StartCoroutine(GameLoop());
	}

    // 最初に呼び出され、ゲームの各段階で順に実行される
    private IEnumerator GameLoop()
    {
        // ドロー
        yield return StartCoroutine(RoundDraw());
        // 攻撃側から守備側への攻撃
        yield return StartCoroutine(RoundAttack());
        // ダメージ計算
        yield return StartCoroutine(RoundCalcDamage());
        // 墓地行き判定
        yield return StartCoroutine(RoundTomb());

        if (isFinished())
        {
            // ゲーム終了判定がされれば、結果シーンに遷移する
            SceneManager.LoadScene("Result");
        } else
        {
            // ターン切り替え
            yield return StartCoroutine(RoundTurnChange());

            deck = players[turn].transform.FindChild("Cards/Deck").gameObject;
            field = players[turn].transform.FindChild("Cards/Field").gameObject;
            tomb = players[turn].transform.FindChild("Cards/Tomb").gameObject;

            // ゲーム終了判定がされなければ、コルーチンGameLoopをリスタートする
            // 現時点で行っているGameLoopは終了する
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundDraw()
    {
        // デッキ(山札)から1枚カードを引く
        //Utility.GetSafeComponent<DeckSet>(deck).draw();
        // 手札からフィールドに出せるカードが1枚も無い場合、スキップ
        yield return null;
    }

    private IEnumerator RoundAttack()
    {
        // フィールドにあるカードで攻撃する
        yield return null;
    }

    private IEnumerator RoundCalcDamage()
    {
        // 攻撃側なら攻撃したとき、守備側なら攻撃されたときのダメージを計算する
        // ダメージ＝攻撃力―防御力＊防御力ダウン率＊クリティカル防御率
        yield return null;
    }

    private IEnumerator RoundTomb()
    {
        // HPが0になったカードを墓地に送る
        // フィールドにいるカード(攻撃、防御、イベント)が、墓地に行く可能性有
        /* if (FieldのカードのHP == 0) {
         *     field.parent = tomb;
         * }
         */
        yield return null;
    }

    private IEnumerator RoundTurnChange()
    {
        tm.changeTurn();
        yield return null;
    }

    // 1試合が終わったかどうかを判定する
    bool isFinished()
    {
        /* if (3枚のシールドのどこかから情報を抜き取られたら) {
         *     return true;
         * }
         */
        return false;
    }
}
