using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOBJ
{

    public static T create<T> (Transform transform) where T:UIOBJ,new()
    {
        T UIObj = new T ();
        UIObj.connect ( transform );
        UIObj.init();
        return UIObj;
    }

    public static ActionGroup AllResponse = new ActionGroup ();
    public static List<UIOBJ> AllResponseForInput = new List<UIOBJ> ();
    public static bool PointUI = false;
    public Transform transform;
    public Area collideArea = new Area ();

    virtual public void connect ( Transform trans )
    {
        transform = trans;
    }


    virtual public void init ()//transform has gained
    {

    }
    virtual public void userInput ()
    {

    }
}
