using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour {

    public void moveScene()
    {
        // シーン遷移のテスト。
        if (transform.name.Equals("bt_title"))
        {
            SceneManager.LoadScene("Main");
        } else if (transform.name.Equals("bt_main"))
        {
            SceneManager.LoadScene("Result");
        } else if (transform.name.Equals("bt_result"))
        {
            SceneManager.LoadScene("Title");
        } else
        {
            Debug.Log("This isn't a button for moving between scenes.");
        }
    }
}
