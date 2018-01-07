﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// RequireComponent: これをGameObjectにアタッチしたとき、必要なコンポーネントが自動的にそのGameObjectに加えられる。
[RequireComponent(typeof(Rigidbody))]
public class Tap : MonoBehaviour, IPointerClickHandler {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("tapされたで");
    }
}
