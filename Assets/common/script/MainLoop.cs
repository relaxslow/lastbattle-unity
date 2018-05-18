using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainLoop
{
    public static float MaxFrameTime = 0.04f;//estimate
    public static bool Pause = false;

    public static System.Text.StringBuilder debugText = new System.Text.StringBuilder ( "debugtxt", 1024 );
    public static float lastFrameTime;
    public void loop ()
    {
        //if (Time.deltaTime >= MaxFrameTime)
            //Debug.Log ( Time.deltaTime );
            loopUI ();
        if (Pause)
            return;

        float remainTime = Time.deltaTime;
        lastFrameTime = remainTime;
        while (remainTime > 0)
        {
        
            preProcess ();
            CollideManager.collideTest ();//update relateVec
            CollideManager.drawCollideInfoTrack ();

            updateAllLast ();//for Debug use
            float collideTime = CollideManager.collideInfo.minCollideTime;

            //debugText = collideTime.ToString ();
            if (collideTime > 0 && collideTime < remainTime)//is collide
            {
                updateAll ( collideTime );
                //CollideManager.drawCollideInfoTrack();
                CollideManager.collideActionAll ();
            }
            else
            {
                collideTime = remainTime;
                updateAll ( collideTime );
            }
            postProcess ();
            lastFrameTime = collideTime;
            remainTime -= collideTime;
        }

    }

    public void loopUI ()
    {
        UIOBJ.PointUI = false;
        for (int i = 0; i < UIOBJ.AllResponseForInput.Count; i++)
        {
            UIOBJ.AllResponseForInput[i].userInput ();
        }
    }
    public virtual void preProcess ()
    {
        if (UIOBJ.PointUI == true)
            return;
        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {
            SCENE_OBJ.ALL[i].preProcess ();// possible change
        }
    }
    public virtual void postProcess ()
    {
        //run obj postprocess First
        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {
            SCENE_OBJ obj = SCENE_OBJ.ALL[i];
            obj.postProcess ();
        }

        clearDeath ();
    }
    void clearDeath ()
    {

        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {
            SCENE_OBJ obj = SCENE_OBJ.ALL[i];
            if (obj.canDestroy)
            {

                obj.pool.delete ( obj );
            }
        }
        for (int i = 0; i < AudioObj.globalAudio.Count; i++)
        {
            AudioObj obj = AudioObj.globalAudio[i];
            if (!obj.audioSrc.isPlaying)
            {
                obj.deleteFromScene ();
            }
        }

        if (Explode.pool.live != null)
        {
            for (int i = 0; i < Explode.pool.live.Count; i++)
            {
                Explode obj = Explode.pool.live[i];
                if (!obj.src.isPlaying)
                {
                    obj.deleteFormScene ();
                }
            }
        }



    }



    void updateAll ( float time )
    {
        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {
            SCENE_OBJ.ALL[i].update ( time );

        }


    }



    //void drawTrackAll ()
    //{
    //    for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
    //    {
    //        SCENE_OBJ.ALL[i].drawTrackCommon ();

    //    }
    //}

    void updateAllLast ()
    {
        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {
            SCENE_OBJ.ALL[i].updateLast ();
        }
    }
    public void onGui ()
    {

        //displayALabel ();
    }
    Rect rect = new Rect ( 500, 10, 400, 200 );
    Rect buttonRect = new Rect ( 10, 40, 100, 30 );
    Action clickButtonAction=new Action();
    void displayALabel ()
    {

        GUIStyle fontStyle = new GUIStyle ();
        fontStyle.normal.background = null;
        fontStyle.normal.textColor = new Color ( 1, 0, 0 );
        fontStyle.fontSize = 70;
        GUI.Label ( rect, debugText.ToString (), fontStyle );


        if (GUI.Button ( buttonRect, "button" ))
        {
        
                clickButtonAction.run ();
        }
    }
    //public static void stopPlay ()
    //{
    //    EditorApplication.ExecuteMenuItem ( "Edit/Pause" );
    //}
}
