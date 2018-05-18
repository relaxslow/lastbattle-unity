using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI  {
    public static Vector3 UIupLeftPoint;
    public static float UICamHWid;
    public static float UICamHHei;


    public static void init ()
    {
        float aspectRatio = 1920f / 1080f;
        GameObject UICam = GameObject.Find ( "UICamera" );
        UICamHHei = UICam.GetComponent<Camera> ().orthographicSize;
        UICamHWid = UICamHHei * aspectRatio;
        UIupLeftPoint = UICam.transform.position + new Vector3 ( -UICamHWid, UICamHHei );
        UIupLeftPoint.z = 0;

       
    }

  
}
