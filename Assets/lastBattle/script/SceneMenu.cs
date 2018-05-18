using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMenu : MonoBehaviour {
    TextButton button;
    // Use this for initialization
    MainLoop loop = new MainLoop ();
	void Start () {

        Button.initUICamera ( GameObject.Find("UICamera").GetComponent<Camera>() );
        button = UIOBJ.create<TextButton> ( GameObject.Find ( "TextButton" ).transform );
        button.responseForInput =true;
	}
	
	// Update is called once per frame
	void Update () {
        loop.loopUI ();
	}
}
