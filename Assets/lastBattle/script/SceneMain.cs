
using UnityEngine;

public class LB_Loop : MainLoop
{
    public override void preProcess ()
    {
        base.preProcess ();

        AHelicopter.createRandom ();
        //ASoldier.createRandom ();
    }
    public override void postProcess ()
    {
        base.postProcess ();
        dealSound ();
        dealScore ();
    }
    void dealSound ()
    {
        if (MainRes.helicopterPool.live.Count == 0)
        {

            AHelicopter.audioObj.stop ();
        }
        else
        {
            AHelicopter.audioObj.play ( true, 0.2f );
        }

    }
    void dealScore ()
    {
        if (Player.score > 100)
            AHelicopter.canCreate = false;
    }

}
public class MainRes
{

    public static ObjPool<SCENE_OBJ> bulletPool = new ObjPool<SCENE_OBJ> ();
    public static ObjPool<SCENE_OBJ> helicopterPool = new ObjPool<SCENE_OBJ> ();
    public static ObjPool<SCENE_OBJ> soldierPool = new ObjPool<SCENE_OBJ> ();

    public void init ()
    {

        initStatic ();

        Groups.init ();
        initAudioRes ();

        Res.initPrefabIntoPool<ABullet> ( "bullet", 20, bulletPool );
        Res.initPrefabIntoPool<AHelicopter> ( "helicopter/helicopter", 5, helicopterPool );
        Res.initPrefabIntoPool<ASoldier> ( "soldier/soldier", 10, soldierPool );
        Res.initEffectIntoPool ( "effect/explode", 20 );

        initUI ();
      
    }
    void initStatic ()
    {
        ActionGroup.initStatic ( 20 );
        Stage.init ();
        ASoldier.initStatic ();
    }




    void initAudioRes ()
    {
        Res.initAudioObjIntoPool ( 20 );
        new Sound ( "cannonShootShort" );
        new Sound ( "explode1" );
        new Sound ( "helicopter" );

        AHelicopter.initSound ();
        
    }


    public static ArtNum scoreNum;
    void initUI ()
    {
        Button.initUICamera (GameObject.Find( "UICamera" ).GetComponent<Camera>() );
        PauseButton pauseButton = UIOBJ.create<PauseButton> ( GameObject.Find ( "pauseButton" ).transform );
        pauseButton.responseForInput =true;

        scoreNum = UIOBJ.create<ArtNum> ( GameObject.Find ( "score" ).transform );





    }




}

public class SceneMain : MonoBehaviour
{
    //test area----------------------------
    public delegate void mydelegate ( string name );
    public delegate int mydelegate2 ( int i );
    public static void func ( string name )
    {
        Debug.Log ( "hello" + name );
    }
    public static void func2 ( string name )
    {
        Debug.Log ( "you are good " + name );
    }
    void test ()
    {
        //mydelegate myde = null;
        //if (myde != null)
        //    myde ( "xuosng" );

        mydelegate myde = new mydelegate ( func );
        Debug.Log ( myde.ToString () );
        mydelegate myde2 = new mydelegate ( func2 );
        mydelegate anonymous = delegate ( string name )
        {
            Debug.Log ( "excellent" + name );
        };
        mydelegate all;
        all = myde + myde2 + anonymous;
        all ( "xusong" );

        //mydelegate2 lambda = i => i+20;

        //Debug.Log ( lambda(2) );

        //Debug.Log ( 123 .ToString()[0] );
        //Debug.Log (Vector3.Dot(Vector3.zero, Vector3.forward));
        //Debug.Log ( Vector3.Project ( new Vector3 ( 12, 12 ), Vector3.zero ) );//vector3.zero
        //Debug.Log ( Vector3.Project (new Vector3(12,12),Vector3.down) );

    }
    //test area end ----------------------------

    MainLoop main = new LB_Loop ();
    MainRes res;
    void Start ()
    {
        //test ();

        res = new MainRes ();
        res.init ();
        Player.init ();

        CollideManager.init ();
        PathManager.init ();


        Debug.Log ( "start end ...." );

    }






    void Update ()
    {
        //testMirrorSoldier.clickCreate ( MainRes.soldierPool );
        main.loop ();
    }



    void OnGUI ()
    {
        main.onGui ();
    }
}
