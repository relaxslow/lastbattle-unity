using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFloor : ABlock
{

    public static AFloor create ()
    {
        AFloor floor = new AFloor ();
        floor.connect ( GameObject.Find ( "floor" ).transform );
        floor.initCommon ();
        floor.addToScene ();
        return floor;
    }
}
