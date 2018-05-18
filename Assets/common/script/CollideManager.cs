using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideManager  {

    public static CollideInfo collideInfo = new CollideInfo ();
    public static ObjPool<ContactInfo> contactPool = new ObjPool<ContactInfo> ();
    
    public static void init ()
    {
        ContactInfo[] contacts = new ContactInfo[20];
        for (int i = 0; i < contacts.Length; i++)
        {
            contacts[i] = new ContactInfo ();
        }
        contactPool.init ( contacts,"ContactInfo" );
    }
    public static void drawCollideInfoTrack ()
    {
        if (collideInfo.minCollideTime == -1)
            return;
        if(collideInfo.node!=null)
        {
            Debug.DrawLine ( collideInfo.reachGoalobj.transform.position, collideInfo.node.goal.point, Color.green );
        }
        else
        {
            //Debug.Log ( collideInfo.minCollideTime+","+collideInfo.vertex.belongToObj+collideInfo.vertex.index+","+collideInfo.surface.belongToObj );

            //        Debug.DrawLine(collideInfo.vertex.pos, collideInfo.relateVecIntersectSurface, Color.blue);

            Debug.DrawLine ( collideInfo.vertex.pos, collideInfo.collidePoint, Color.green );
            Debug.DrawLine ( collideInfo.collideSurfaceP0, collideInfo.collideSurfaceP1, Color.cyan );
            //        Debug.Log(collideInfo.minCollideTime);
            //        Debug.Log(collideInfo.vertex.index);
        }

    }
    static ContactInfo newContactA=new ContactInfo();
    static ContactInfo newContactB=new ContactInfo();
    public static void collideActionAll ()
    {
        if (collideInfo.minCollideTime == -1)
            return;
        if(collideInfo.node==null)
        {
            collideSceneObjProcess ();
        }
        else
        {
            collideGoalProcess ();
        }
     


    }
    public static void collideSceneObjProcess ()
    {
        SCENE_OBJ A = collideInfo.vertex.belongToObj;//vertex
        SCENE_OBJ B = collideInfo.surface.belongToObj;//surface
        //Debug.Log ( "collide occur!!(" + A.transform.name + "," + B.transform.name + ")" + collideInfo.minCollideTime );
        newContactA.reset ();
        newContactB.reset ();
        newContactA.other = B;
        newContactB.other = A;
        newContactA.self = A;
        newContactB.self = B;
        //newContactA.coupleContactInfo = newContactB;
        //newContactB.coupleContactInfo = newContactA;
        newContactA.blockDirect = collideInfo.surface.normal.direct;
        newContactB.blockDirect = -newContactA.blockDirect;


        //record surface
        newContactA.otherSurface = newContactB.otherSurface = collideInfo.surface;
        //if surface contact surface record surface
        bool isSurfaceContact = false;
        for (int i = 0; i < collideInfo.vertex.normals.Count; i++)
        {
            NORMAL normal = collideInfo.vertex.normals[i];
            if (normal.direct == -collideInfo.surface.normal.direct)
            {
                newContactA.selfSurface = newContactB.selfSurface = normal.surface;
                isSurfaceContact = true;
                break;
            }
        }
        //otherwise record vertex
        if (!isSurfaceContact)
            newContactA.selfVertex = newContactB.selfVertex = collideInfo.vertex;

        A.collideAction ( newContactA );
        B.collideAction ( newContactB );

        A.informBelongToObj ( newContactA );
        B.informBelongToObj ( newContactB );

    }
    public static void collideGoalProcess()
    {
        collideInfo.reachGoalobj.reachAction (collideInfo.node);
    }

   //public static void drawCollideObjTrack ()
   // {
        
   //     CollideInfo info = collideInfo;
   //     if(info.goal!=null)
   //     {
            
   //     }
   //     else
   //     {
   //         info.vertex.belongToObj.drawTrackCommon ();
   //         info.surface.belongToObj.drawTrackCommon ();
   //     }
       
   // }
    public static int totalCollideObjNumPerFrame;//debug
    public static void collideTest ()
    {
        collideInfo.init ();//init

        totalCollideObjNumPerFrame = 0;
        for (int i = 0; i < SCENE_OBJ.ALL.Count; i++)
        {

            SCENE_OBJ.ALL[i].testCollide ();
           
           
        }
        //if (totalCollideObjNumPerFrame > 0)
            //Debug.Log ( totalCollideObjNumPerFrame );
    }
}
public class CollideInfo
{
    public float minCollideTime;
    public VERTEX vertex;
    public SURFACE surface;


    public Vector3 collidePoint;
    public Vector3 collideSurfaceP0;
    public Vector3 collideSurfaceP1;
    public Vector3 intersectPoint;


    public SCENE_OBJ reachGoalobj;
    public PathNode node;


    public CollideInfo ()
    {
        init ();
    }
    public void init ()
    {
        minCollideTime = -1;
        collidePoint = Vector3.zero;
        collideSurfaceP0 = Vector3.zero;
        collideSurfaceP1 = Vector3.zero;
        intersectPoint = Vector3.zero;
        vertex = null;
        surface = null;
        node = null;
        reachGoalobj = null;
    }


}
public class ContactInfo : IPoolObj
{
    public SCENE_OBJ self;
    public SCENE_OBJ other;
    public SCENE_OBJ extraObj;//used for extraContactInfo;
    public ContactInfo coupleContactInfo;
    public Vector3 blockDirect;//contact surface normal

    public VERTEX selfVertex;
    public SURFACE selfSurface;//possible surface contact surface;

    public SURFACE otherSurface;//correspond to the surface of CollideInfo

    public void reset ()
    {

        other = null;
        extraObj = null;
        coupleContactInfo = null;
        blockDirect = Vector3.zero;
        selfVertex = null;
        selfSurface = null;
        otherSurface = null;

    }
    public void copy(ContactInfo otherInfo)
    {
        self = otherInfo.self;
        other = otherInfo.other;
        extraObj = otherInfo.extraObj;
        coupleContactInfo = otherInfo.coupleContactInfo;
        blockDirect = otherInfo.blockDirect;
        selfVertex = otherInfo.selfVertex;
        otherSurface = otherInfo.otherSurface;


    }


}