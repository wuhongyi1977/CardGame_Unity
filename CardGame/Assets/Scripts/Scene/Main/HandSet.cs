using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        setCards();

	}

    //デッキ内のカードを正しい位置に移動(後でもっと複雑になります)
    public void setCards() {
        int count = transform.childCount;
        GameObject[] children = new GameObject[count];
        for (int i = 0; i < count; i++) {
            children[i] = transform.GetChild(i).gameObject;
            children[i].transform.localPosition = new Vector3(0, -5, 0);
        }
    }

}
