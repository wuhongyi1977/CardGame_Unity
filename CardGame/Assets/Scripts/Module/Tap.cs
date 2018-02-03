using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tap : MonoBehaviour, IPointerClickHandler {
    /*
     * カードのプレハブにTapをアタッチ
     * 
     * タップできるやつ：isselectがtrueのとき
     * 長押ししたら説明が出る：isbackがfalseのとき
     */
    
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerClick(PointerEventData eventData) {
        // もしタップしたカードのisSelectがtrueなら
        Variables.player_tapped_obj = eventData.selectedObject;
        Debug.Log("タップされたやつ: " + this.name);
    }
}
