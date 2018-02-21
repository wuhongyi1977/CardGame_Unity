using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skip : MonoBehaviour {

    public Canvas targetCanvas;
    public Canvas CannotCanvas;

    //お試し変数,統合してVaribalesが入ったらコメントアウトor削除で差し替えてください
    private bool isSkippable = true;

    // Use this for initialization
    void Start() {
        if(targetCanvas != null) {
            targetCanvas.enabled = false;
        }
        if(CannotCanvas != null) {
            CannotCanvas.enabled = false;
        }
    }

    //スキップダイアログを表示する関数とYesNoボタン用の関数は分ける

    public void ShowSkipDialog() {

        //スキップできる時、YesNoダイアログを出すように
        //if (Variables.Player_isSkippable) {
        if (isSkippable) {  //統合してVariablesが入ったら上のと差し替えてください
            if(targetCanvas != null) {
                targetCanvas.enabled = true;
            }
        }else {
            //できない時、"スキップできません"ダイアログを出すように
            if(CannotCanvas != null) {
                CannotCanvas.enabled = true;
            }
            
        }
    }

    public void WithdrawSkip() {
        if(targetCanvas != null) {
            targetCanvas.enabled = false;
            //Debug.Log("スキップやめます");
        }
    }

    public void DoSkip() {
        if(targetCanvas != null) {
            targetCanvas.enabled = false;
            //Variables.Player_isSkipped = true;  //統合してVariablesが入ったらコメントアウト外してください
            //Debug.Log("スキップしました");
        }
    }

    public void CannnotSkip() {
        if(CannotCanvas != null) {
            CannotCanvas.enabled = false;
            //Debug.Log("スキップできません");
        }
    }
}
