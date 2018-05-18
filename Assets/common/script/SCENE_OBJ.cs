
using System.Collections.Generic;
using UnityEngine;


public class SCENE_OBJ : OBJ, IPoolObj
{
    virtual public void connect ( Transform objTransform )
    {
        transform = objTransform;
    }
    public string __name;//debug use
    //    TYPE type = TYPE.Normal;
    public static List<SCENE_OBJ> ALL = new List<SCENE_OBJ> ();//handle collideTest,preProcess,postProcess ,update ,collideAction
    public Transform transform;
    public IPool<SCENE_OBJ> pool;
    public SCENE_OBJ belongToObj;
    public ABox[] collideBoxs;
    public void addToScene ()
    {

        ALL.Add ( this );
    }
    public void removeFromScene ()
    {
        ALL.Remove ( this );
    }
    public virtual string getClassName ()
    {
        return "SCENEOBJ";
    }

    public void initCommon ( )
    {
     
        __name = transform.name;
        //enableTrack = false;
        canDestroy = false;
        animator = transform.GetComponent<Animator> ();
        init ();//custom init

    }


    public float _dimension;
    public BoxCollider dimensionBox;
    public virtual void initDimension ()
    {


    }

    public void calculateDimension ( BoxCollider boxCollider )
    {
        float wid = boxCollider.size.x * boxCollider.transform.lossyScale.x;
        float hei = boxCollider.size.y * boxCollider.transform.lossyScale.y;
        _dimension = Mathf.Sqrt ( wid * wid + hei * hei );//diagonal is diameter;
    }
    Vector3 _speedVec;
    Vector3 _direct;
    float _speed;

    public float speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            _speedVec = _direct * _speed;
        }
    }

    public Vector3 direct
    {
        get
        {
            return _direct;
        }
        set
        {
            _direct = value;
            _speedVec = _direct * _speed;
        }
    }

    public Vector3 speedVec
    {
        get
        {
            return _speedVec;
        }
        set
        {
            _speedVec = value;
            _direct = _speedVec.normalized;
            _speed = _speedVec.magnitude;
        }
    }


    public float dimension
    {
        get
        {
            return _dimension;/*+ globalSpeedVec.magnitude * Time.deltaTime;*/
        }

    }


    public Vector3 relateVec;



    //public bool enableTrack;



    //public void drawTrackCommon ()
    //{
    //    if (enableTrack)
    //    {
    //        drawTrack ();
    //    }
    //}

    public SURFACE[] _surfaces;
    public VERTEX[] _vertexs;


    //collide will ocur after this time
    public void vertexContactSurface ( VERTEX vertex, SURFACE surface )
    {
        //ignore vertexs which is back to surface
        bool isFacing = false;
        for (int i = 0; i < vertex.normals.Count; i++)
        {
            Vector3 vertexNormal = vertex.normals[i].direct;
            float facingDot = Vector3.Dot ( vertexNormal, surface.normal.direct );
            float threshold = 0.000001f;
            if (facingDot > -threshold && facingDot < threshold)
                facingDot = 0;
            if (facingDot < 0)
            {
                isFacing = true;
                break;
            }

        }
        if (!isFacing)
            return;
        //check if under surface
        Vector3 beginVec = surface.points[0].pos - vertex.pos;
        float dist = Vector3.Dot ( beginVec, surface.normal.direct );
        if (dist > 0)//ignore the surfaces which is back to vertex
            return;

        // relateVec intersect surface
        Vector3 perpendicularVec = surface.normal.direct * dist;
        float angle = Vector3.Angle ( -relateVec, perpendicularVec );
        if (angle >= 90)//parallel
            return;
        float length = Mathf.Abs ( dist ) / Mathf.Cos ( Mathf.Deg2Rad * angle );

        Vector3 intersectPoint = vertex.pos - relateVec.normalized * length;
        //contactsurface
        float collideTime = length / relateVec.magnitude;

        surface.belongToObj.computeGlobalSpeed ();
        Vector3 surfaceGlobalSpeedVec = surface.belongToObj.globalSpeedVec;
        Vector3 collideSurfaceP0 = surface.points[0].pos + surfaceGlobalSpeedVec * collideTime;
        Vector3 collideSurfaceP1 = surface.points[1].pos + surfaceGlobalSpeedVec * collideTime;
        //Vector3 collideSurfaceNormal = surface.normal.direct;
        //check if in contact surface
        Vector3 collidePoint = vertex.pos + globalSpeedVec * collideTime;

        if (Math3D.isProjectInSurface ( collidePoint, collideSurfaceP0, collideSurfaceP1 ))//will contact in future
        {
            CollideInfo info = CollideManager.collideInfo;
            if (info.minCollideTime == -1 || collideTime < info.minCollideTime)
            {
                info.minCollideTime = collideTime;
                info.collidePoint = collidePoint;
                info.collideSurfaceP0 = collideSurfaceP0;
                info.collideSurfaceP1 = collideSurfaceP1;
                info.intersectPoint = intersectPoint;
                info.vertex = vertex;
                info.surface = surface;
                info.node = null;
                info.reachGoalobj = null;
            }

        }
    }

    virtual public bool isObjsMatchCondition ( SCENE_OBJ obj )
    {
        if (obj == this)
            return false;

        if (relateVec == Vector3.zero)
            return false;

        Vector3 distVec = obj.transform.position - transform.position;

        float getAWayOrNearDot = Vector3.Dot ( relateVec, distVec.normalized );
        //if (getAWayOrNearDot >= 0) //is getting away 
        //    return false;

        float distSqr = distVec.sqrMagnitude;
        //float possibleDist = dimension + obj.dimension;
        //if (dist > possibleDist)//too far away
        //    return false;

        float relateSpeed = Mathf.Abs ( getAWayOrNearDot );
        float possibleDist = relateSpeed * MainLoop.lastFrameTime + dimension + obj.dimension;
        if (distSqr > possibleDist * possibleDist)
            return false;



        return true;
    }
    public Vector3 globalSpeedVec;

    public void computeGlobalSpeed ()
    {
        globalSpeedVec = speedVec;
        SCENE_OBJ belongTo = belongToObj;
        while (belongTo != null)
        {
            globalSpeedVec += belongTo.speedVec;
            belongTo = belongTo.belongToObj;
        }

    }
    public void informBelongToObj ( ContactInfo info )
    {
        SCENE_OBJ belongTo = belongToObj;
        while (belongTo != null)
        {
            belongTo.collideAction ( info );
            belongTo = belongTo.belongToObj;
        }

    }
    public List<List<SCENE_OBJ>> referenceGroups = new List<List<SCENE_OBJ>> ();
    public virtual void testCollide ()
    {


    }


    //-1 user controlled obj
    //-2 still obj
    public float mass;
    public List<ContactInfo> contacts;

    public void createContacts ()
    {
        contacts = new List<ContactInfo> ();
        initCheckContactMethod ();
    }
    public void resetContacts ()
    {
        contacts.Clear ();
    }
    public void addContact ( ContactInfo contactInfo )
    {
        ContactInfo newContact = CollideManager.contactPool.create ();
        newContact.copy ( contactInfo );
        contacts.Add ( newContact );
        
    }
    public delegate void CheckContactMethod ( ContactInfo info );
    public CheckContactMethod checkSingleContact;
    virtual public void initCheckContactMethod ()
    {
        checkSingleContact = realityCheck;
    }
    public void contactTest ()
    {

        for (int i = 0; i < contacts.Count; i++)
        {
            checkSingleContact ( contacts[i] );

        }
    }
    void realityCheck ( ContactInfo cantactInfo )
    {
        ContactInfo info = cantactInfo;
        SURFACE otherSurface = info.otherSurface;
        SURFACE selfSurface = info.selfSurface;
        VERTEX vertex = info.selfVertex;


        if (vertex != null)//point contact surface
        {
            bool inSurface = Math3D.isProjectInSurface ( vertex.pos, otherSurface.points[0].pos, otherSurface.points[1].pos );
            bool offContact = offContactSurface ( vertex, otherSurface );
            if (!inSurface || offContact)
            {
                deleteContact ( info );
            }

        }
        else if (selfSurface != null)//surface contact surface
        {

            bool surface0InSurface1 = surfaceInSurface ( otherSurface, selfSurface );
            bool surface1InSurface0 = surfaceInSurface ( selfSurface, otherSurface );
            bool OffContact = offContactSurface ( selfSurface.points[0], otherSurface );
            if (((!surface0InSurface1) && (!surface1InSurface0)) || OffContact)
            {
                deleteContact ( info );
            }


        }
    }
    bool offContactSurface ( VERTEX vertex, SURFACE surface )
    {
        Vector3 beginVec = vertex.pos - surface.points[0].pos;
        float distDot = Vector3.Dot ( beginVec, surface.normal.direct );
        //Debug.Log ( distDot );
        float distLimit = 0.00001f;
        if (distDot > distLimit || distDot < -distLimit)
            return true;
        return false;
    }
    public void deleteContact ( ContactInfo info )
    {
        contacts.Remove ( info );
        offContactAction ( info );
        CollideManager.contactPool.delete ( info );

        //SCENE_OBJ other = info.other;
        //ContactInfo coupleInfo = info.coupleContactInfo;

        //((FF_OBJ)other).contacts.Remove ( coupleInfo );


        //Debug.Log ( "off contact!(" + info.other.transform.name + "," + this.transform.name + ")" );

        //other.offContactAction ( info.coupleContactInfo );


        //CollideManager.contactPool.delete ( coupleInfo );



    }
    protected bool surfaceInSurface ( SURFACE surface0, SURFACE surface1 )
    {
        VERTEX[] contactVertexs = surface0.points;
        for (int j = 0; j < contactVertexs.Length; j++)
        {
            if (Math3D.isProjectInSurface ( contactVertexs[j].pos, surface1.points[0].pos, surface1.points[1].pos ))
            {
                return true;
            }
        }
        return false;
    }



    public void reflect ( ContactInfo info )
    {
        speedVec = Vector3.Reflect ( speedVec, info.blockDirect );

    }



    public virtual void reset ()
    {

        deleteformGroups ();
        transform.parent = Stage.offStage;
        removeFromScene ();
        canDestroy = false;
    }

    public void addToGroup ( List<SCENE_OBJ> group )
    {
        group.Add ( this );
        referenceGroups.Add ( group );

    }
    public void deleteFromGroup ( List<SCENE_OBJ> group )
    {
        group.Remove ( this );
        referenceGroups.Remove ( group );
    }
    void deleteformGroups ()
    {
        for (int i = 0; i < referenceGroups.Count; i++)
        {
            referenceGroups[i].Remove ( this );
        }
    }


    public void move ( float time )
    {
        if (!(speedVec == Vector3.zero))
            transform.Translate ( speedVec * time, Space.World );
    }
    public virtual void initMeshInfo () { }
    public virtual void update ( float time ) { }
    public virtual void updateLast () { }
    public virtual void userInput () { }
    public virtual void drawTrack () { }
    public virtual void collideAction ( ContactInfo contactInfo ) { }
    public virtual void offContactAction ( ContactInfo info ) { }
    public virtual void init ()
    {
        initDimension ();
        initMeshInfo ();// custom meshInfo
        //initCheckContactMethod ();
    }
    public virtual void postProcess () { }
    public virtual void preProcess ()
    {
        userInput ();//response for input,minCollideTime,speedVec may change
    }


    public bool canDestroy = false;

    public virtual void outOfRangeTest ()
    {
        if (isOutOfStage ())
            canDestroy = true;
    }
    public bool isOutOfStage ()
    {
        if (Stage.border == null)
            return false;
        for (int j = 0; j < _vertexs.Length; j++)
        {
            VERTEX point = _vertexs[j];
            if (Stage.isPointInStage ( point.pos ))
                return false;

        }
        return true;
    }

    public Animator animator;
    string _state;
    public string state
    {
        get
        {
            return _state;
        }
        set
        {
            if (!value.Equals ( "" ))
                animator.Play ( value );
            _state = value;
        }
    }

    virtual public void reachAction (PathNode node)
    {

    }
    public int poolIndex = -1;
}


public class SURFACE
{
    public const int vertexNum = 2;
    public VERTEX[] points;
    public NORMAL normal;
    public int index = -1;
    public SCENE_OBJ belongToObj;

    public SURFACE ()
    {
        points = new VERTEX[vertexNum];
    }

    public void init ( VERTEX[] inPoints, int[,] surfaceVertexIndex, int surfaceIndex, SCENE_OBJ obj )
    {
        for (int i = 0; i < points.Length; i++)
        {
            int tt = surfaceVertexIndex[surfaceIndex, i];
            points[i] = inPoints[tt];
            points[i].belongToSurfaces.Add ( this );
        }
        index = surfaceIndex;
        belongToObj = obj;
    }


    public void init ( Vector3 p1, Vector3 p2, int inIndex )
    {

        points[0].pos = p1;
        points[1].pos = p2;
        index = inIndex;
    }



}


public class VERTEX
{
    public Vector3 pos;
    public Vector3 posL;
    public int index;

    public SCENE_OBJ belongToObj;
    const int maxBelongToSurfaceNum = 2;
    public List<SURFACE> belongToSurfaces = new List<SURFACE> ( maxBelongToSurfaceNum );

    public List<NORMAL> normals;
    const int maxNormalNumPerVertex = 2;

    public void initNormal ()
    {
        normals = new List<NORMAL> ( maxNormalNumPerVertex );
    }


    public VERTEX ( SCENE_OBJ belongto = null )
    {
        belongToObj = belongto;
    }

    public void update ( Transform transform, Vector3 offsetVec )
    {
        Vector3 basePoint = transform.position;
        basePoint.z = 0;
        pos = basePoint + transform.rotation * offsetVec;
    }

    public void updateLast ()
    {
        posL = pos;
    }


}

public class NORMAL
{
    public Vector3 original;
    public Vector3 direct;
    public SURFACE surface;

    public void init ( SURFACE s )
    {
        surface = s;
        VERTEX[] surfacePoints = s.points;
        Vector3 right = surfacePoints[1].pos - surfacePoints[0].pos;
        original = Quaternion.AngleAxis ( 90, Vector3.forward ) * right.normalized;
        direct = original;

    }

    public void update ( Quaternion rotation )
    {
        direct = rotation * original;
    }
}

