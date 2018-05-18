using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTowerBase : ABlock
{

}
public class AGunTower : AMultiColliderObj
{
    const string State_Damage = "demage";

    public static AGunTower create ()
    {
        AGunTower gunTower;
        gunTower = new AGunTower ();
        gunTower.connect ( GameObject.Find ( "gunTower" ).transform );
        gunTower.initCommon ();
        gunTower.addToScene ();

        return gunTower;
    }

    public override void createCustomBox ( ref ABox box )
    {
        box = new GunTowerBase ();
    }

    Transform gunRoot;
    Transform rotateCenter;
    float coolDownTime;
    public override void init ()
    {
        base.init ();
        gunRoot = transform.Find ( "gunRoot" ).transform;
        rotateCenter = gunRoot.Find ( "rotateCenter" );
    }

    public override void preProcess ()
    {
        if (AnimationGunDamage != null)
            AnimationGunDamage ();
        userInput ();
    }
    float bulletSpeed = 70;
    public override void userInput ()
    {
        if (Player.isFailure)
            return;
        bool isAPressed = Input.GetKey ( KeyCode.A );
        Vector3 gunVec = Vector3.zero;
        if (isAPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
            Vector3 normal = Vector3.back;
            Vector3 point = Math3D.intersectPoint ( ray, normal );

            gunVec = point - gunRoot.position;
            Debug.DrawLine ( point, gunRoot.position, Color.red );

            float angle = Vector3.Angle ( Vector3.up, gunVec.normalized );

            //LastBattle.debugText = angle.ToString ();
            float angleLimit = 106.2f;
            if (angle > angleLimit)
                angle = angleLimit;
            float leftOrRightDot = Vector3.Dot ( gunVec, Vector3.left );
            if (leftOrRightDot < 0)
                angle = -angle;

            Quaternion rotate = Quaternion.AngleAxis ( angle, Vector3.forward );
            gunRoot.transform.rotation = rotate;
            Vector3 shootVec = rotate * Vector3.up;

            bool isMouseLeftBtnPressed = Input.GetButtonDown ( "Fire1" );
            if (isMouseLeftBtnPressed)
            {
                shootBullet ( shootVec * bulletSpeed );
                //Debug.Log ( angle );
            }
            //Cursor.visible = false;
        }
        else
        {
            //Cursor.visible = true;
        }

    }
    void shootBullet ( Vector3 direct )
    {


        ABullet bullet = ABullet.create ();
        if (bullet == null)
            return;
        bullet.addToGroup ( Groups.bulletAll );
        bullet.transform.position = gunRoot.position;
        bullet.speedVec = direct;
        Sound.createAndPlay ( "cannonShootShort", bullet.transform.position,0.1f );
    }

    FUN AnimationGunDamage;

    void initFunDelegate ()
    {
        AnimationGunDamage = Ani_GunDamage;
    }
    Vector3 minAngle = new Vector3 ( 1, 1 ).normalized;
  void gunBeginDamage ()
    {
      
        float randomAngle = UnityEngine.Random.value * 90;
        damageBeginDirection = Quaternion.AngleAxis ( randomAngle, Vector3.forward ) * minAngle;

        AnimationGunDamage = Ani_GunDamage;
    }
    Vector3 damageBeginDirection;
    float damageSpeed = 40;
    float damageTime = 0.5f;
    float rotateSpeed = 360;
    float var = 0;
    float varSpeed = 1;
    void Ani_GunDamage ()
    {
       
        Vector3 finalFlyDirect = Vector3.Slerp ( damageBeginDirection, Vector3.down, var );
        rotateCenter.Translate ( finalFlyDirect * damageSpeed * MainLoop.lastFrameTime ,Space.World);
        rotateCenter.Rotate (Vector3.forward, rotateSpeed * MainLoop.lastFrameTime,Space.World );

        var += (varSpeed * MainLoop.lastFrameTime);
        if (var >= 1)
            var = 1;
       
        damageTime -= MainLoop.lastFrameTime;
        if (damageTime <= 0)
        {
            AnimationGunDamage = null;
            gunRoot.transform.gameObject.SetActive ( false );
        }
    }
    public void Action_explode ()
    {
        gunBeginDamage ();
        
        state = State_Damage ;

        Explode explode = Explode.createFromPool ();

        explode.playAtPos ( gunRoot.position );
    }

    public override void testCollide ()
    {
        for (int i = 0; i < Groups.enemybulletAll.Count; i++)
        {

        }
    }


}
