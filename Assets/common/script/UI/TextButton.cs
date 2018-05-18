using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextButton : Button {

    public override void userInput ()
    {
        testClick ();
    }
    public override void onClicked ()
    {
        SceneManager.LoadScene ( "lastBattle/Scene/Game", LoadSceneMode.Single );
        //SceneManager.UnloadSceneAsync ( "lastBattle/Scene/Game" );
        //Debug.Log ( "clickBegin" );
    }

}
