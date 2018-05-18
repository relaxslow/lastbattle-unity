using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : UIOBJ
{
    static  Camera uiCamera;
    public static  void initUICamera(Camera camera)
    {
        uiCamera = camera;
    }
    override public void init ()
    {
        Transform collideObj = transform.GetChild ( 0 );
        //Debug.Log ( "pauseButton" + collideObj.name );

        collideArea.init ( collideObj );

        clickAction.init (onClicked);
    }
   protected Action clickAction =new Action();
    protected void testClick ()
    {
        if (!_responseForInput)
            return;
        Ray ray = uiCamera.ScreenPointToRay ( Input.mousePosition );
        Vector3 normal = Vector3.back;
        Vector3 point = Math3D.intersectPoint ( ray, normal );
        if (collideArea.isPointInArea ( point ))
        {
            //Debug.Log ( "overButton" );
            PointUI = true;
            bool pressKey = Input.anyKeyDown;
            if (pressKey)
            {

                clickAction.run ();
            }

        }
       
    }
    virtual public void onClicked ()
    {

    }
    public bool _responseForInput;
    public bool responseForInput
    {
        get
        {
            return _responseForInput;
        }
        set
        {
            _responseForInput = value;
            if(value)
            {
                AllResponseForInput.Add ( this );
            }
            else
            {
                AllResponseForInput.Remove ( this );
            }
        }
    }
   
}
