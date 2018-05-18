using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PauseButton :Button {
  
    public override void userInput ()
    {
        testClick ();
    }
    override public void onClicked ()
    {
        if (MainLoop.Pause)
            MainLoop.Pause = false;
        else
            MainLoop.Pause = true;


    }
    
	
}
