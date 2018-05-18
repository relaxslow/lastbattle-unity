using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABullet : ABox
{

    public override void init ()
    {
        base.init ();
        initMethod ();
    }
    void initMethod ()
    {
        //initMethodTest ();
        initMethodNormal ();
    }
    public delegate void Method ();

    Method CollideTestRoutine;
    Method CollideActionRoutine;
    void initMethodTest ()
    {
        CollideTestRoutine = collideTestAHelicopter;
        CollideActionRoutine = Action_Test;
    }
    void initMethodNormal ()
    {
        CollideTestRoutine = collideTestAllEnemy ;
        CollideActionRoutine = Action_Normal;
    }
    public override void testCollide ()
    {
        CollideTestRoutine ();

    }
    void collideTestAHelicopter()
    {
        collideTest ( testMirrorHelicopter.helicopter.collideBoxs );

    }
    void collideTest ( ABox[] collideGroup )
    {
        for (int k = 0; k < collideGroup.Length; k++)
        {
            ABox collideBox = collideGroup[k];
            collideWithOtherBox ( collideBox );
        }
    }
    void collideTestAllEnemy()
    {

        if (Groups.enemyAll == null)
            return;
        for (int i = 0; i < Groups.enemyAll.Count; i++)
        {
            if (Groups.enemyAll[i] is AMultiColliderObj)
            {
                AMultiColliderObj enemy = Groups.enemyAll[i] as AMultiColliderObj;
                collideTest ( enemy.collideBoxs );
            }
            else if (Groups.enemyAll[i] is ACombinedObj)
            {
                ACombinedObj enemy = Groups.enemyAll[i] as ACombinedObj;
                for (int j = 0; j < enemy.collideParts.Count; j++)
                {
                    AMultiColliderObj part = enemy.collideParts[j];
                    collideTest ( part.collideBoxs );
                }
            }

        }
    }
    public override void postProcess ()
    {
        outOfRangeTest ();
    }
    ContactInfo contactInfoTempStore=new ContactInfo();
    public override void collideAction ( ContactInfo contactInfo )
    {
        contactInfoTempStore.copy ( contactInfo );
        CollideActionRoutine ();
    }
    void Action_Test()
    {
        speed = 0;
    }
    void Action_Normal ()
    {
        if (contactInfoTempStore.other.belongToObj.__name != "parachute")
            canDestroy = true;
    }

    public static ABullet create ()
    {
        ABullet bullet = Res.createFromPool ( MainRes.bulletPool ) as ABullet;
     
        bullet.addToScene ();
        return bullet;
    }

    public override string getClassName ()
    {
        return "Bullet";
    }

}
