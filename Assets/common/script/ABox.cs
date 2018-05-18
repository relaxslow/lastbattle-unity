using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABox : SCENE_OBJ
{
    public ABox ( ) {
        
        createVertexs ();
        createSurfaces ();
        createNormal ();

    }

    public float wid;
    public float hei;
    public int surfaceNum;
    public int vertexNum = 4;
    //vertex


   
    Vector3[] offsetVec;

  
     protected void createVertexs ()
    {
        offsetVec = new Vector3[vertexNum];
       
        for (int i = 0; i < vertexNum; i++)
        {
            offsetVec[i] = Vector3.zero;
            
        }
        _vertexs = new VERTEX[vertexNum];
        for (int i = 0; i < vertexNum; i++)
        {
            _vertexs[i] = new VERTEX ( this );
        }
    }
    Vector3 realCenter;
    protected void initVertexs ()
    {
        wid = dimensionBox.size.x * dimensionBox.transform.lossyScale.x;
        hei = dimensionBox.size.y * dimensionBox.transform.lossyScale.y;
        float Hwid = wid / 2;//half width
        float Hhei = hei / 2;//half height
        realCenter = dimensionBox.center;
        realCenter.x = realCenter.x * dimensionBox.transform.lossyScale.x;
        realCenter.y = realCenter.y * dimensionBox.transform.lossyScale.y;
        realCenter.z = 0;
        offsetVec[0].x = -Hwid; offsetVec[0].y = Hhei;
        offsetVec[1].x = Hwid; offsetVec[1].y = Hhei;
        offsetVec[2].x = Hwid; offsetVec[2].y = -Hhei;
        offsetVec[3].x = -Hwid; offsetVec[3].y = -Hhei;
        for (int i = 0; i < vertexNum; i++)
        {
            Vector3 basePoint = dimensionBox.transform.position;
            basePoint.z = 0;
            _vertexs[i].pos = basePoint + realCenter + offsetVec[i]; ;
            _vertexs[i].index = i;
        }
   

    }
  
  
    void drawVertexSurfaceTrack ()
    {
        for (int i = 0; i < _vertexs.Length; i++)
        {
            int j = i + 1;
            if (j == _vertexs.Length)
                j = 0;
            Debug.DrawLine ( _vertexs[i].pos, _vertexs[j].pos, Color.red, 0 );
        }
    }

    virtual public void updateVertexs ()
    {
        for (int i = 0; i < _vertexs.Length; i++)
        {

            _vertexs[i].update ( dimensionBox.transform, realCenter+offsetVec[i] );

        }
        drawVertexSurfaceTrack ();
    }

    public void updateLastVertexs ()
    {
        for (int i = 0; i < _vertexs.Length; i++)
        {
            _vertexs[i].updateLast ();
        }

    }

    //surfaces
    int[,] surfaceVertexIndex;
    protected void createSurfaces ()
    {
        chooseSurfaceToCreate ();

         surfaceVertexIndex = new int[,]
        {
            { 0, 1 },//up
            { 1, 2 },//right
            { 2, 3 },//down
            { 3, 0 }//left

        };
        surfaceNum = needSurfaceIndex.Length;
        _surfaces = new SURFACE[surfaceNum];
        for (int i = 0; i < surfaceNum; i++)
        {
            _surfaces[i] = new SURFACE ();
        }
    }
    protected void initSurfaces ()
    {
        for (int i = 0; i < surfaceNum; i++)
        {
            _surfaces[i].init ( _vertexs, surfaceVertexIndex, needSurfaceIndex[i], this );
        }
    }
    protected int[] needSurfaceIndex;

    virtual public void chooseSurfaceToCreate ()
    {
        needSurfaceIndex = new int[]
        {
            0, 1, 2, 3
        };
    }




    //normal
    NORMAL[] normals;
    protected void createNormal ()
    {
        normals = new NORMAL[surfaceNum];
        for (int i = 0; i < surfaceNum; i++)
        {
            normals[i] = new NORMAL ();
        }
        for (int i = 0; i < vertexNum; i++)
        {
            _vertexs[i].initNormal ();
        }
    }

    protected void initNormal()
    {
        for (int i = 0; i < surfaceNum; i++)
        {
          
            normals[i].init ( _surfaces[i] );
            _surfaces[i].normal = normals[i];

        }
        for (int i = 0; i < surfaceNum; i++)
        {
            NORMAL normal = _surfaces[i].normal;
            VERTEX[] surfacePoints = _surfaces[i].points;
            for (int j = 0; j < surfacePoints.Length; j++)
            {
                VERTEX vertex = surfacePoints[j];
                vertex.normals.Add ( normal );
            }
        }
    }
  

    public void updateNormal ()
    {
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i].update ( transform.rotation );
        }
    }


    protected void drawVertexTrack ()
    {
        for (int i = 0; i < _vertexs.Length; i++)
        {
            if (_vertexs[i].belongToSurfaces.Count > 0)
                Debug.DrawLine ( _vertexs[i].pos, _vertexs[i].posL, Color.red );
        }
    }

    protected void drawSurfaceTrack ()
    {
        for (int i = 0; i < _surfaces.Length; i++)
        {
            VERTEX[] points = _surfaces[i].points;

            Debug.DrawLine ( points[0].pos, points[1].pos, Color.red, 0 );
            //Debug.DrawLine(points[0].posL, points[1].posL, Color.red,300);

        }

    }



    protected void drawNormalTrack ()
    {
        for (int i = 0; i < _vertexs.Length; i++)
        {
            for (int j = 0; j < _vertexs[i].normals.Count; j++)
            {
                NORMAL normal = _vertexs[i].normals[j];
                Vector3 normalEnd = _vertexs[i].pos + normal.direct * 0.5f;
                Debug.DrawLine ( _vertexs[i].pos, normalEnd, Color.yellow );
            }

        }

    }

    public override void initDimension ()
    {
        dimensionFormDirect ();
    }

    protected void dimensionFormDirect ()
    {
        dimensionBox = transform.GetComponent<BoxCollider> ();
        if (dimensionBox == null)
        {
            Debug.Log ( "noBoxCollider for dimension calculation" );
            return;
        }
        calculateDimension ( dimensionBox );
    }
    protected void dimensionFormObj ()//should not use ,cause collide error
    {
        Transform dimensionObj = transform.Find ( "dimension" );
        dimensionBox = dimensionObj.GetComponent<BoxCollider> ();
        calculateDimension ( dimensionBox );
    }

    public override void initMeshInfo ()
    {

        initVertexs ();
        initSurfaces ();
        initNormal ();

        updateVertexs ();
        updateLastVertexs ();

        updateNormal ();
    }
 

    public override void drawTrack ()
    {
        //        drawVertexTrack();
        drawSurfaceTrack ();
        //        drawNormalTrack();
    }





    public override void update ( float time )
    {
        move ( time );
        updateVertexs ();
        updateNormal ();
    }

    public override void updateLast ()
    {
        updateLastVertexs ();
    }

    public void calculateGlobalspeedAndRelateVec ( SCENE_OBJ one, SCENE_OBJ other )
    {
        one.computeGlobalSpeed ();
        other.computeGlobalSpeed ();
        one.relateVec = other.globalSpeedVec - one.globalSpeedVec;//point to self
        other.relateVec = -one.relateVec;
    }
    public void collideWithOtherBox ( ABox other )
    {
        calculateGlobalspeedAndRelateVec ( this, other );

        //if (!isObjsMatchCondition ( other ))
        //    return;

        for (int k = 0; k < other._surfaces.Length; k++)
        {
            SURFACE surface = other._surfaces[k];
            for (int j = 0; j < _vertexs.Length; j++)
            {
                VERTEX vertex = _vertexs[j];
                vertexContactSurface ( vertex, surface );
            }
        }
        CollideManager.totalCollideObjNumPerFrame++;
    }


    public void collideTest ( ABox[] boxs, ABox obj )
    {
        for (int i = 0; i < boxs.Length; i++)
        {
            ABox collideBox = boxs[i];
            collideBox.collideWithOtherBox ( obj );
        }
    }
 
}
