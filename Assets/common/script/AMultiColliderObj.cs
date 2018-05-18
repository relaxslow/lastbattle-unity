using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMultiColliderObj : ABox
{
    public override void connect ( Transform objTransform )
    {
        base.connect ( objTransform );
        createCollideGroup ();
    }
   
    virtual public void createCustomBox (ref ABox box)
    {
       box = new ABox ();
    }
  void createCollideGroup ()
    {
        
        Transform collideGroup = transform.Find ( "collideGroup" );
        Transform[]  childs = collideGroup.transform.GetComponentsInChildren<Transform> ();
        collideBoxs = new ABox[childs.Length - 1];
        for (int i = 1; i < childs.Length; i++)
        {
            int j = i - 1;
            createCustomBox ( ref collideBoxs[j] );
            collideBoxs[j].connect ( childs[i]);
            collideBoxs[j].belongToObj = this;
        }
    }
    public override void init ()
    {
        initDimension ();
        initMeshInfo ();
        initChildColliders ();
    }
    public override void initDimension ()
    {
        dimensionFormDirect ();
    }
    public override void initMeshInfo ()
    {

        base.updateVertexs ();
        updateLastVertexs ();
       
    
    }

    void initChildColliders ()
    {
        for (int i = 0; i < collideBoxs.Length; i++)
        {
            
            collideBoxs[i].initCommon (  );
        }
    }
    public override void update ( float time )
    {
        move ( time );
        updateVertexs ();
    }
    public override void updateVertexs ()
    {
        base.updateVertexs ();
        for (int i = 0; i < collideBoxs.Length; i++)
        {
            ABox collideBox = collideBoxs[i];
            collideBox.updateVertexs ();
        }

    }

    public void collideTest ( List<SCENE_OBJ> bulletGroup )
    {
        if (bulletGroup == null)
            return;
        for (int i = 0; i < bulletGroup.Count; i++)
        {
            ABullet otherObj = bulletGroup[i] as ABullet;

            calculateGlobalspeedAndRelateVec ( this, otherObj );

            if (!isObjsMatchCondition ( otherObj ))
                return;

            collideTest ( collideBoxs, otherObj );

        }
    }
    public void collideTest ( ABox obj )
    {
        collideTest ( collideBoxs, obj );
    }
    public void collideTest ( ABox[] objs )
    {
        for (int i = 0; i < objs.Length; i++)
        {
            collideTest ( objs[i] );
        }
    }

  
}
