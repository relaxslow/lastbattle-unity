using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABoxFlyer : AMultiColliderObj {

    public override void testCollide ()
    {
        for (int i = 0; i < Groups.bulletAll.Count; i++)
        {
            ABox otherObj = Groups.bulletAll[i] as ABox;

            for (int j = 0; j < collideBoxs.Length; j++)
            {
                ABox collideBox = collideBoxs[j];
                collideBox.collideWithOtherBox ( otherObj );
            }

        }



    }
    public override void update (float time)
    {
        move ( time );
        for (int i = 0; i < collideBoxs.Length; i++)
        {
            ABox collideBox = collideBoxs[i];
            collideBox.updateVertexs();
            collideBox.updateNormal ();
        }
    }
    public override void collideAction ( ContactInfo contactInfo )
    {
        Debug.Log ( "helicopter is underAttack" );
    }
}
