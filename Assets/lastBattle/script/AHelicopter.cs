using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHelicopter : AMultiColliderObj
{

    public override void init ()
    {
       
        base.init ();
    }
    public  void deploy ()
    {
        Area generateZone;
        float hit = Random.value;
        Vector3 direct = Vector3.zero;
        float angleY = 0;
        if (hit >= 0 && hit < .5f)
        {
            generateZone = Stage.leftBornZone;
            direct = Vector3.right;
        }
        else
        {
            generateZone = Stage.rightBornZone;
            direct = Vector3.left;
            angleY = 180;
        }

        transform.position = Stage.randomPosOnArea ( generateZone );
        transform.rotation = Quaternion.Euler ( 0, angleY, -17 );
        speedVec = direct * (MinSpeed + Random.value * (MaxSpeed - MinSpeed));
        addToGroup ( Groups.enemyAll );
    }
    static int MinGenerateNum = 1;
    static int MaxGenerateNum = 3;
    static float MaxSpeed = 30;
    static float MinSpeed = 10;
    static float coolDownTime = 1;
    static float timeCount;
    public static bool canCreate = true;
    public static void createRandom ()
    {
        if (!canCreate)
            return;
        timeCount += Time.deltaTime;
        if (timeCount < coolDownTime)
            return;
        timeCount = 0;
        int num = (int)(MinGenerateNum + Random.value * (MaxGenerateNum - MinGenerateNum));
        for (int i = 0; i < num; i++)
        {
            AHelicopter helicopter= Res.createFromPool ( MainRes.helicopterPool ) as AHelicopter;
            if(helicopter!=null)
            {
                helicopter.addToScene ();
                helicopter.deploy ();
            }
           
                //Quaternion.AngleAxis ( -17, Vector3.forward );
        }
    }

    float limitMaxRange = 111;//position  on ground
    float limitMinRange = 10;
    float minInterval = 1;
    float maxInterval = 5;
    float timerInterval=-1;
    float timer=0;
    Vector3 soldierOffset=new Vector3(0,-5);
    public  void randomCreateSoldier ()
    {
        Transform createSoldierObj = transform.Find ( "soldierPoint" );
       
        if(transform.position.x<-limitMaxRange || transform.position.x>limitMaxRange ||(transform.position.x>-limitMinRange && transform.position.x<limitMinRange))
           return;
        if (timerInterval!=-1) 
        {
            timer += Time.deltaTime;
            if (timer >= timerInterval)
            {
                timerInterval = -1;
                ASoldier.createAndDeploySoldier ( createSoldierObj.transform.position + soldierOffset );

            }
          

        }
        else//timer restart
        {
            timerInterval = minInterval + Random.value * maxInterval;
            timer = 0;
        }
      
       
        


    }

    public override void preProcess ()
    {
        randomCreateSoldier ();
    }
  
    public static AudioObj audioObj;
    public static void initSound ()
    {
        Transform helicopterAudio = GameObject.Find ( "helicopterAudio" ).transform;

        audioObj = AudioObj.createFromPool ( helicopterAudio );

        audioObj.attachSound ( Sound.findSound ( "helicopter" ) );

    }
    public override void testCollide ()
    {
        collideTest ( Groups.bulletAll );
    }

    public override void postProcess ()
    {
        outOfRangeTest ();
    }

    int scoreContribute = 1;
    public override void collideAction ( ContactInfo contactInfo )
    {
        //Action_Test ( contactInfo );
        Action_Normal ( contactInfo );
    }
    void Action_Test(ContactInfo contactInfo)
    {
        speed = 0;
        Debug.Log ( "bullet hit helicopter" );
    }
    void Action_Normal ( ContactInfo contactInfo )
    {
        Player.killHelicopterNum++;
        Player.score += scoreContribute;
        MainRes.scoreNum.display ( Player.score );

        Explode explode = Explode.createFromPool ();

        explode.playAtPos ( transform.position );
    
        canDestroy = true;
    }
    public override string getClassName ()
    {
        return "Helicopter";
    }

}
