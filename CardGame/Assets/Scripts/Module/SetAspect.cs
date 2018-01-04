using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAspect : MonoBehaviour {

    //これをカードとかゲームオブジェクトにアタッチしてアスペクト補正をする

	// Use this for initialization
	void Start () {
        //Resizeを取得
        GameObject Resize = GameObject.Find("/Resize");
        //Risizeから変更スケールを取得
        Vector3 scale = Resize.GetComponent<AspectResize>().Resized();
        //スケールを変更
        transform.localScale = scale;

        //ポジションの変更
        //現在のポジションを取得
        float tx = transform.localPosition.x;
        float ty = transform.localPosition.y;
        float tz = transform.localPosition.z;
        //Risizeから変更ポジションを取得
        Vector3 pos = Resize.GetComponent<AspectResize>().Reposition(tx, ty);
        //ポジション変更
        transform.position = new Vector3(pos.x, pos.y, tz);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
