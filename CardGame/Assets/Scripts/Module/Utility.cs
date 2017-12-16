using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility{

    private static Vector2 tempVector2 = Vector2.zero;
    private static Vector3 tempVector3 = Vector3.zero;

    //Resourceからファイル読み込み
    public static GameObject ResourceLoad( string path) {
        return (GameObject)Resources.Load(path);
    }

    //破棄
    public static void UnLoad() {
        Resources.UnloadUnusedAssets();
    }

    //GameObject生成
    public static GameObject Instantiate(GameObject baseObj) {
        return GameObject.Instantiate(baseObj,Vector3.zero,Quaternion.identity) as GameObject;
    }

    // 破棄.
    public static void Destroy(GameObject obj) {
        GameObject.Destroy(obj);
    }

    // 2D用コンポーネントを使用していた時.
    // タップしたオブジェクトを取得する.
    public static bool findObject2DWithTag(Camera camera, Vector2 tapPos, string target) {
        Vector2 tapPoint = camera.ScreenToWorldPoint(tapPos);
        Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
        if (collition2d) {
            RaycastHit2D hitObject = Physics2D.Raycast(tapPoint, -Vector2.up);
            if (hitObject) {
                return (hitObject.collider.gameObject.tag.Equals(target));
            }
        }

        return false;
    }

    // 安全にGetComponentを呼ぶ.
    public static T GetSafeComponent<T>(GameObject obj) where T : Component {
        T component = obj.GetComponent<T>();

        if (component == null) {
            Debug.LogError("Expected to find component of type " + typeof(T) + " but found none", obj);
        }

        return component;
    }

    // 操作...クリック or タップした時.
    public static bool getOperationDown() {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetMouseButtonDown(0);
#else
		foreach ( Touch touch in Input.touches )
		{
			if ( touch.phase == TouchPhase.Began )
			{
				flag = true;
				break;
			}
		}
#endif
        return flag;
    }

    // 操作...クリック or タップ中.
    public static bool getOperationStay() {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetMouseButton(0);
#else
		foreach ( Touch touch in Input.touches )
		{
			if ( touch.phase == TouchPhase.Moved )
			{
				flag = true;
				break;
			}
		}
#endif
        return flag;
    }

    // 操作...クリック or タップを離した時.
    public static bool getOperationUp() {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetMouseButtonUp(0);
#else
		foreach ( Touch touch in Input.touches )
		{
			if ( touch.phase == TouchPhase.Ended )
			{
				flag = true;
				break;
			}
		}
#endif
        return flag;
    }

    public static bool getKeyDown(string keyName) {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetKeyDown(keyName);
#endif
        return flag;
    }

    public static bool getKey(string keyName) {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetKey(keyName);
#endif
        return flag;
    }

    public static bool getKeyUp(string keyName) {
        bool flag = false;
#if UNITY_EDITOR || UNITY_WEBPLAYER
        flag = Input.GetKeyUp(keyName);
#endif
        return flag;
    }
}

