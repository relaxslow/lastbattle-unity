using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACombinedObj : ABox {

   
    public List<AMultiColliderObj> collideParts = new List<AMultiColliderObj> ();

   
    public override void initDimension ()
    {
        dimensionFormDirect ();
    
    }
   
    public override void initMeshInfo ()
    {
        initVertexs ();
      
    }

    public override void update ( float time )
    {
        move ( time );
        updateVertexs ();
        updatePartVertexs ();
    }
    public void updatePartVertexs ()
    {
        for (int i = 0; i < collideParts.Count; i++)
        {
            collideParts[i].updateVertexs ();
        }
    }
}
