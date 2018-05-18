using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stage
{



    public static Area border;
    public static Area leftBornZone;
    public static Area rightBornZone;
    public static Area middelZone;
    public static Transform onStage;
    public static Transform offStage;




    public static Transform audios;

    public static void init ()
    {

        UI.init ();
        onStage = GameObject.Find ( "onStage" ).transform;
        offStage = GameObject.Find ( "offStage" ).transform;
        offStage.gameObject.SetActive ( false );

        border = new Area ();
        border.init ( GameObject.Find ( "backGround" ).transform );

        leftBornZone = new Area ();
        leftBornZone.init ( GameObject.Find ( "leftZone" ).transform );
        rightBornZone = new Area ();
        rightBornZone.init ( GameObject.Find ( "rightZone" ).transform );

        middelZone = new Area ();
        middelZone.init ( GameObject.Find ( "middleZone" ).transform );

        audios = GameObject.Find ( "audios" ).transform;

        floor = AFloor.create ();
        gunTower =AGunTower.create ();


    }


    public static AFloor floor;
    public static AGunTower gunTower;



    public static bool isPointInStage ( Vector3 point )
    {
        Vector3[] cornerPoints = border.cornerPoints;
        for (int i = 0; i < cornerPoints.Length; i++)
        {

            int k = i + 1;
            if (k == cornerPoints.Length)
                k = 0;
            bool isInSurface = Math3D.isProjectInSurface ( point, cornerPoints[i], cornerPoints[k] );
            if (!isInSurface)
                return false;
        }
        return true;
    }

    static Vector3 randomOffset;
    public static Vector3 randomPosOnArea ( Area area )
    {
        Vector3 randomPos;
        randomOffset.x = Random.value * area.wid;
        randomOffset.y = Random.value * area.hei;
        randomPos = area.leftBottomPoint + randomOffset;
        return randomPos;
    }
}


public class Area
{
    public Transform transform;
    public Vector3 leftBottomPoint;
    public float wid;
    public float hei;
    public float hWid;
    public float hHei;
    public Vector3[] cornerPoints;

    public void init ( ABox sceneObj )
    {
        wid = sceneObj.wid;
        hei = sceneObj.hei;
        hWid = wid / 2;
        hHei = hei / 2;
        cornerPoints = new Vector3[4];
        for (int i = 0; i < cornerPoints.Length; i++)
        {
            cornerPoints[i] = sceneObj._vertexs[i].pos;
        }
        leftBottomPoint = cornerPoints[3];
    }
    public void init ( Transform trans )
    {
        transform = trans;
        wid = transform.lossyScale.x;
        hei = transform.lossyScale.y;
        hWid = wid / 2;
        hHei = hei / 2;
        Vector3[] offset = new Vector3[] {
            new Vector3(-hWid,hHei),
            new Vector3(hWid,hHei),
            new Vector3(hWid,-hHei),
            new Vector3(-hWid,-hHei),
        };
        cornerPoints = new Vector3[4];
        Vector3 ZToZeroPoint = transform.position;
        ZToZeroPoint.z = 0;

        for (int i = 0; i < cornerPoints.Length; i++)
        {

            cornerPoints[i] = ZToZeroPoint + offset[i];
        }

        leftBottomPoint = cornerPoints[3];
    }
    public bool isPointInArea ( Vector3 point )
    {
        Vector3 originalVec = point - leftBottomPoint;
        float projectUpDot = Vector3.Dot ( originalVec, Vector3.up );
        float projectRightDot = Vector3.Dot ( originalVec, Vector3.right );
        if (projectUpDot >= 0 && projectUpDot <= hei && projectRightDot >= 0 && projectRightDot <= wid)
        {
            return true;
        }

        return false;
    }
}