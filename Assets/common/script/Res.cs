using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Res : MonoBehaviour
{
    public static void initPrefabIntoPool<T> ( string prefabName, int num, IPool<SCENE_OBJ> pool ) where T : SCENE_OBJ, new()
    {
        GameObject[] prefabs = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            prefabs[i] = Instantiate ( Resources.Load ( "Game/prefab/" + prefabName ), Vector3.zero, Quaternion.identity ) as GameObject;
            prefabs[i].transform.parent = Stage.offStage;

        }

        T[] sceneObjs = new T[num];

        for (int i = 0; i < sceneObjs.Length; i++)
        {
            sceneObjs[i] = new T ();
            sceneObjs[i].connect ( prefabs[i].transform );
            sceneObjs[i].initCommon ();
            sceneObjs[i].poolIndex = i;
        }
        pool.init ( sceneObjs, sceneObjs[0].getClassName () );

    }
    public static SCENE_OBJ createFromPool ( IPool<SCENE_OBJ> sceneObjPool )
    {
        SCENE_OBJ TObj = sceneObjPool.create ();
        if (TObj == null)
            return null;
        TObj.transform.parent = Stage.onStage;
      
        TObj.pool = sceneObjPool;
        return TObj;
    }


    public static void initAudioObjIntoPool ( int num )
    {
        GameObject[] prefabs = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            prefabs[i] = Instantiate ( Resources.Load ( "Game/prefab/audio" ), Vector3.zero, Quaternion.identity ) as GameObject;
            prefabs[i].transform.parent = Stage.offStage;

        }

        AudioObj[] audioObjs = new AudioObj[num];

        for (int i = 0; i < audioObjs.Length; i++)
        {
            audioObjs[i] = new AudioObj ();
            audioObjs[i].init ( prefabs[i].transform, false );
        }
        AudioObj.pool.init ( audioObjs, "AudioObj" );

    }







    public static void initEffectIntoPool ( string prefabName, int num )
    {
        GameObject[] prefabs = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            prefabs[i] = Instantiate ( Resources.Load ( "Game/prefab/" + prefabName ), Vector3.zero, Quaternion.identity ) as GameObject;
            prefabs[i].transform.parent = Stage.offStage;

        }
        Explode[] effectObjs = new Explode[num];
        for (int i = 0; i < effectObjs.Length; i++)
        {
            effectObjs[i] = new Explode ();
            effectObjs[i].init ( prefabs[i].transform );
        }
        Explode.pool.init ( effectObjs, "Explode Effect" );
    }


}
public interface IPool<T>
{
    void init ( T[] obj, string name );
    T create ();
    void delete ( T obj );
}


public interface IPoolObj
{

    void reset ();
}

public class ObjPool<T> : IPool<T> where T : IPoolObj
{
    public string name;
    public Stack<T> stack = new Stack<T> ();
    public List<T> live;

    public void init ( T[] obj, string name )
    {
        this.name = name;
        live = new List<T> ( obj.Length );
        for (int i = 0; i < obj.Length; i++)
        {

            stack.Push ( obj[i] );
        }
    }

    public T create ()
    {
        if (stack.Count > 0)
        {
            T obj = stack.Pop ();
            live.Add ( obj );
            return obj;
        }
        else
        {
            //Debug.Log ( "not enough "+ name +" in stack" );
            return default ( T );
        }



    }

    public void delete ( T obj )
    {
        if (obj == null)
        {
            Debug.Log ( "obj is null" );
            return;
        }
        live.Remove ( obj );
        obj.reset ();
        stack.Push ( obj );
        obj = default ( T );
    }




    public void clearLive ()
    {
        for (int i = 0; i < live.Count; i++)
        {
            delete ( live[i] );
        }
    }
}
