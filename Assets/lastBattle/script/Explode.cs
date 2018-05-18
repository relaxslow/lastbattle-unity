using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode :OBJ,IPoolObj {
    public static ObjPool<Explode> pool = new ObjPool<Explode> ();
    public Transform transform;
    public ParticleSystem src;

    public static Explode createFromPool ( )
    {
        Explode explodeObj = pool.create ();
        if (explodeObj == null)
            return null;
        explodeObj.transform.parent = Stage.onStage;
        return explodeObj;
    }
    public void init(Transform objTransform )
    {
        transform = objTransform;
        transform.localScale = new Vector3 ( 0.5f, 0.5f, 0.5f );
        src = transform.GetComponent<ParticleSystem> ();
        ParticleSystem.MainModule main = src.main;
        main.loop = false;
    }
    public void deleteFormScene ()
    {
        transform.parent = Stage.offStage;
        pool.delete ( this );
    }
    public void playAtPos(Vector3 pos)
    {
        transform.position = pos;
        src.Play ();
        Sound.createAndPlay ( "explode1", pos,0.2f );
    }
    public void reset()
    {

    }
}
