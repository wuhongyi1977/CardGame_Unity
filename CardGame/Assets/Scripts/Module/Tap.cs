using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tap : MonoBehaviour, IPointerClickHandler , IPointerDownHandler , IPointerExitHandler, IPointerUpHandler , IDragHandler {
    /*
     * カードのプレハブにTapをアタッチ
     * 
     * タップできるやつ：isselectがtrueのとき
     * 長押ししたら説明が出る：isbackがfalseのとき
     */

    //スライダー
    private Slider sld;

    //タップ判別の為の変数
    private float TimeForTap = 0.2f;  //タップ認識時間
    private float TappedTime;  //この時間までに指を離すとタップ

    //長押し判別の為の変数
    private float TimeForLongTap = 1.0f;  //長押し認識時間
    private float LongTappedTime;  //長押しが完了する時間

    //押しっぱかつ指が一本か
    private bool pressing = false;
    
    // Use this for initialization
    void Start() {
        sld = GameObject.Find("Player_Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {
        if (pressing) {
            if(LongTappedTime < Time.time) {  //長押し完了時の動作
                if (!GetComponent<Card>().ISBACK) {
                    Variables.player_longtapped_obj = gameObject;
                    Debug.Log("長押しされたやつ(表): " + Variables.player_longtapped_obj.name);
                    //ここにカード詳細表示のUIをオンにする動作必要(スクリプトのメンバにCanvasも必要)
                    //アニメーション追加するならゲーム時間停止とかも？
                }
                pressing = false;
            }
        }
    }

    //タップ関数
    public void OnPointerClick(PointerEventData eventData) {
        if(Time.time < TappedTime) {
            if (GetComponent<Card>().ISSELECTABLE) {
                Variables.player_tapped_obj = gameObject;
                Debug.Log("タップされたやつ(選択可): " + Variables.player_tapped_obj.name);
            }
        }
    }

    //ドラッグ・スワイプの関数(実機用にもう少し複雑にしてもよいかも)
    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("ドラッグ中");
        sld.value -= eventData.delta.x * 0.005f;
    }

    //タップ・長押し開始の関数
    public void OnPointerDown(PointerEventData eventData) {
        if (!pressing) {
            pressing = true;
            TappedTime = Time.time + TimeForTap;  //タップ検出用
            LongTappedTime = Time.time + TimeForLongTap;  //長押し検出用
        }else{
            pressing = false;
        }
    }

    public void OnPointerUp(PointerEventData data) {
        if (pressing) {
            pressing = false;
        }
    }

    public void OnPointerExit(PointerEventData data) {
        if (pressing) {
            pressing = false;
        }
    }
}
