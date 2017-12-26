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

	// Use this for initialization
	void Start () {
        turn = 0;
        players = new GameObject[2];
        players[0] = transform.FindChild("Player").gameObject;
        players[1] = transform.FindChild("Enemy").gameObject;
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
        // ターン切り替え
        yield return StartCoroutine(RoundTurnChange());

        if (isFinished())
        {
            // ゲーム終了判定がされれば、結果シーンに遷移する
            SceneManager.LoadScene("Result");
        } else
        {
            // ゲーム終了判定がされなければ、コルーチンGameLoopをリスタートする
            // 現時点で行っているGameLoopは終了する
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator RoundDraw()
    {
        GameObject deck = players[turn].transform.FindChild("Cards/Deck").gameObject;
        // デッキ(山札)から1枚カードを引く
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
        yield return null;
    }

    private IEnumerator RoundTomb()
    {
        // HPが0になったカードを墓地に送る
        // フィールドにいるカード(攻撃、防御、イベント)が、墓地に行く可能性有
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
        return false;
    }
}
