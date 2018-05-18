


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASoldier : ACombinedObj
{

    public override void connect ( Transform objTransform )
    {
        base.connect ( objTransform );
        Transform soldierBodyInstance = transform.Find ( Name_soldierBody );
        Transform ParachuteInstance = transform.Find ( Name_parachute );
        mSoldierBody = createPart<ASoldierBody> ( soldierBodyInstance );
        mParachute = createPart<AParachute> ( ParachuteInstance );




    }

    const string Name_soldierBody = "soldierBody";
    const string Name_parachute = "parachute";
    const string Name_floor = "floor";


    static int total = 1;
    static int num;
    static float coolDown = 2;
    static float time;
    public static void createRandom ()
    {
        if (time < coolDown)
        {
            time += MainLoop.lastFrameTime;
            return;
        }
        else
            time = 0;

        if (num >= total)
            return;

        Vector3 generatePos = Stage.randomPosOnArea ( Stage.middelZone );
        createAndDeploySoldier ( generatePos );
        num++;
    }

    public static ASoldier createSoldier ( Vector3 pos )
    {
        ASoldier soldier = Res.createFromPool ( MainRes.soldierPool ) as ASoldier;
        if (soldier == null)
            return null;
        soldier.transform.position = pos;
        soldier.addToScene ();
        soldier.addToGroup ( Groups.enemyAll );
        return soldier;
    }
    public static bool createAndDeploySoldier ( Vector3 pos )
    {
        if (!canProduceSoldier)
            return false;
        ASoldier soldier = createSoldier ( pos );
        if (soldier == null)
            return false;
        soldier.deploy ();
        return true;
    }

    void Action_Prepare ()
    {
        speedVec = pFallVector;
        nextNode = null;
        rotationInAir ();
    }
    public void Action_PlayerFailure ()
    {
        Player.isFailure = true;
        rGunTower.Action_explode ();
        mSoldierBody.Action_Cheer ();
        allSoldierCheer ();
        stopProduceSoldier ();
        displayGameOver ();
        //displayFailurePanel ();
    }
    void displayGameOver ()
    {
        SceneManager.LoadScene ( "lastBattle/Scene/End", LoadSceneMode.Additive );
    }
    delegate void ReachPointAction ();
    ReachPointAction nextPointAction;
    void allSoldierCheer ()
    {
        for (int i = 0; i < soldierPool.live.Count; i++)
        {
            ASoldier soldier = soldierPool.live[i] as ASoldier;
            if (soldier.mSoldierBody.state == ASoldierBody.State_Walking)
            {
                soldier.mSoldierBody.Action_Cheer ();

            }
            if (soldier.mSoldierBody.state == ASoldierBody.State_Climbing)
            {
                soldier.nextPointAction = soldier.mSoldierBody.Action_Cheer;
            }
        }
    }
    static bool canProduceSoldier;
    void stopProduceSoldier ()
    {
        canProduceSoldier = false;
    }
    void displayFailurePanel ()
    {

    }

    public void Action_success ()
    {

    }
    public const float pFallSpeed = 50;
    public const float pFallSpeedAfterOpenParachute = 8;
    public const float pMoveOnGroundSpeed = 8;
    public const float pJumpSpeed = 15;
    public const float pClimbSpeed = 5;
    public Vector3 pMoveOnGroundDirection;
    public Vector3 pFallVector = new Vector3 ( 0, -pFallSpeed );

    public static int totelNum = 0;




    public ASoldierBody mSoldierBody;
    public AParachute mParachute;




    public override void init ()
    {
        initDimension ();
        initMeshInfo ();
        initPart ();
        //initTestCollideDelegate ();
     
    }



    public void initPart ()
    {
        mSoldierBody.initCommon ();
        mParachute.initCommon ();
    }
    void resetCollidePart ()
    {
        collideParts.Clear ();
        addPart ( mSoldierBody );
        removePart ( mParachute );
        updatePartVertexs ();
    }
    public void deploy ()
    {
        beginTestBlock ();
        Action_Prepare ();
        resetCollidePart ();
        initPathGroup ();
     
        mSoldierBody.Action_Prepare ();
        mParachute.Action_Prepare ();
    }
    public override void reset ()
    {
        transform.position = Vector3.zero;
        resetCollideDelegate ();
        mParachute.canOpen = true;
        base.reset ();
    }
    T createPart<T> ( Transform partTransform ) where T : AMultiColliderObj, new()
    {
        T obj = new T ();
        obj.belongToObj = this;
        obj.connect ( partTransform );
        return obj;
    }



    /// <summary>
    /// use to operate instance before init them 
    /// </summary>



    public override void userInput ()
    {
        inputFreePlace ();
    }
    void pressAndOpenParachute ()
    {
        bool isAPressed = Input.GetKeyDown ( KeyCode.A );
        if (isAPressed)
        {
            mParachute.state = AParachute.State_Open;
            transform.Translate ( new Vector3 ( 0, 1 ) );

        }
    }
    void inputFreePlace ()
    {
        bool BPressed = Input.GetKeyDown ( KeyCode.B );
        if (BPressed)
        {

            Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
            Vector3 normal = Vector3.back;
            Vector3 point = Math3D.intersectPoint ( ray, normal );
            transform.position = point;
        }
    }

    public PathGroup pathGroup;

    private void initPathGroup ()
    {
        if (transform.position.x > 0)
        {
            pathGroup = PathManager.right;

        }
        else
        {
            pathGroup = PathManager.left;

        }
    }
    public void pathFind ()
    {
        if (pathGroup.stepStatus[0] == false)
        {
            pathFollow ( 0 );
        }

        else if (pathGroup.stepStatus[1] == false)
        {
            if (transform.position.x <= pathGroup.getGoal ( 9 ).point.x && transform.position.x >= pathGroup.getGoal ( 12 ).point.x)
                pathFollow ( 1 );
            else
                pathFollow ( 2 );
        }
        else if (pathGroup.stepStatus[2] == false)
        {
            if (transform.position.x <= pathGroup.getGoal ( 9 ).point.x && transform.position.x >= pathGroup.getGoal ( 12 ).point.x)
                pathFollow ( 3 );
            else
                pathFollow ( 4 );
        }
        else if (pathGroup.stepStatus[3] == false)
        {
            if (transform.position.x <= pathGroup.getGoal ( 2 ).point.x && transform.position.x >= pathGroup.getGoal ( 12 ).point.x)
                pathFollow ( 5 );
            else
                pathFollow ( 6 );
        }
    }



    Segment minDistSegment = new Segment ();
    public void findMinDistSegment ( Path path )
    {
        minDistSegment.init ();
        for (int i = 0; i < path.nodes.Length; i++)
        {
            int lastIndex = path.nodes.Length - 1;
            if (i < lastIndex)
            {
                projectPointToSegment ( transform.position, path.nodes[i], path.nodes[i + 1] );
            }
        }
    }
    public void pathFollow ( int pathIndex )
    {
        Path path = pathGroup.paths[pathIndex];
        currentPath = path;
        findMinDistSegment ( path );

        if (minDistSegment.dist != -1)//find Segment to project
        {

            setNextNode ( minDistSegment.n2 );
        }
        else
        {
            PathNode node = findClosestPathNode ();
            setNextNode ( node );
        }
        if (transform.position.y > pathGroup.getGoal ( 1 ).point.y)
            mSoldierBody.Action_Jump ();
        else
            mSoldierBody.Action_Walking ();
    }


    public void rotationToNormal ()
    {
        mSoldierBody.transform.rotation = Quaternion.identity;
    }
    public void rotationInAir ()
    {
        float random = Random.value;
        if (random > 0.5f)
        {

            mSoldierBody.transform.rotation = Quaternion.Euler ( 0, 180, 0 );


        }
        else
        {
            mSoldierBody.transform.rotation = Quaternion.identity;
        }


    }

    public void rotationOnGround ()
    {
        float directDot = Vector3.Dot ( speedVec, Vector3.right );
        if (directDot > 0)
        {
            mSoldierBody.transform.rotation = Quaternion.identity;

        }
        else if (directDot < 0)
        {
            mSoldierBody.transform.rotation = Quaternion.Euler ( 0, 180, 0 );
        }
        else
        {
            if (transform.position.x > 0)
                mSoldierBody.transform.rotation = Quaternion.Euler ( 0, 180, 0 );
            else
                mSoldierBody.transform.rotation = Quaternion.identity;
        }
    }
    void setNextNode ( PathNode node )
    {
        nextNode = node;
        direct = (nextNode.goal.point - transform.position).normalized;

    }

    PathNode findClosestPathNode ()
    {
        float minDistSqr = -1;
        int minDistNodeIndex = -1;
        for (int i = 0; i < currentPath.nodes.Length; i++)
        {
            Vector3 distVec = transform.position - currentPath.nodes[i].goal.point;
            if (minDistSqr == -1 || distVec.sqrMagnitude < minDistSqr)
            {
                minDistSqr = distVec.sqrMagnitude;
                minDistNodeIndex = i;
            }
        }
        return currentPath.nodes[minDistNodeIndex];


    }

    void projectPointToSegment ( Vector3 point, PathNode n1, PathNode n2 )
    {
        bool projectPointIsInSurface = Math3D.isProjectInSurface ( transform.position, n1.goal.point, n2.goal.point );
        if (!projectPointIsInSurface)
        {
            return;
        }
        Vector3 p1 = n1.goal.point;
        Vector3 p2 = n2.goal.point;
        Vector3 lineVec = p1 - p2;
        Vector3 perpendicularVec = Quaternion.AngleAxis ( 90, Vector3.forward ) * lineVec;
        perpendicularVec.Normalize ();
        Vector3 beginVec = p1 - point;

        float perpendicularDot = Vector3.Dot ( beginVec, perpendicularVec );
        if (perpendicularDot < 0)
        {
            perpendicularDot = -perpendicularDot;
            perpendicularVec = -perpendicularVec;
        }

        float dist = perpendicularDot;
        if (minDistSegment.dist == -1 || dist < minDistSegment.dist)
        {
            minDistSegment.n1 = n1;
            minDistSegment.n2 = n2;
            minDistSegment.projectPoint = point + perpendicularVec * perpendicularDot;
            minDistSegment.dist = perpendicularDot;
            minDistSegment.pathDirect = (p2 - p1).normalized;
        }


    }
    public override void preProcess ()
    {

        //userInput ();

        mSoldierBody.preProcess ();
        mParachute.preProcess ();


    }

    public Path currentPath;
    public PathNode nextNode;
    //bool allowTest = false;
    void testPath ()
    {
        //    if (allowTest == false)
        //        return;
        if (nextNode == null)
            return;


        Vector3 distVec = nextNode.goal.point - transform.position;
        float awayOrNearDot = Vector3.Dot ( speedVec, distVec.normalized );
        if (awayOrNearDot < 0) //is away
            return;

        float collideTime = distVec.magnitude / speed;

        CollideInfo info = CollideManager.collideInfo;
        if (info.minCollideTime == -1 || collideTime < info.minCollideTime)
        {

            info.minCollideTime = collideTime;
            info.node = nextNode;
            info.reachGoalobj = this;
            info.collidePoint = Vector3.zero;
            info.collideSurfaceP0 = Vector3.zero;
            info.collideSurfaceP1 = Vector3.zero;
            info.intersectPoint = Vector3.zero;
            info.vertex = null;
            info.surface = null;

        }
        //Debug.Log ( collideTime );

    }


    public void removePart ( AMultiColliderObj obj )
    {
        collideParts.Remove ( obj );
    }
    public void addPart ( AMultiColliderObj obj )
    {
        if (!collideParts.Contains ( obj ))
            collideParts.Add ( obj );
    }

   
    //Action ATestBlock;
    //Action ATestPath;
    ActionGroup AllTestCollideFun = new ActionGroup ();
    //void initTestCollideDelegate ()
    //{
    //    ATestBlock = new Action ();
    //    ATestBlock.init ( testCollideBlock );
    //    ATestPath = new Action ();
    //    ATestPath.init ( testPath );
    //}
    void resetCollideDelegate()
    {
        
        AllTestCollideFun.clear ();
     
    }
    public void beginTestBlock()
    {
        
        AllTestCollideFun.addAction ( testCollideBlock ,transform);
    }
    public void cancelTestBlock ()
    {
        AllTestCollideFun.removeAction ( testCollideBlock,transform );

    }
    public void beginTestPath ()
    {
       
        AllTestCollideFun.addAction ( testPath,transform );

    }
    public void cancelTestPath ()
    {
        AllTestCollideFun.removeAction ( testPath,transform );

    }
    public override void testCollide ()
    {
        AllTestCollideFun.run ();
        //collideTestBullet ();
    }

    public void testCollideASingleBlock ()
    {
        if (testMirrorSoldier.block == null)
            return;
        mSoldierBody.collideTest ( testMirrorSoldier.block );
    }
    void testCollideBullet ()
    {
        if (Groups.bulletAll == null)
            return;

        for (int i = 0; i < collideParts.Count; i++)
        {
            collideParts[i].collideTest ( Groups.bulletAll );
        }
    }
    public static AFloor rFloor;
    public static AGunTower rGunTower;
    void testCollideBlock ()
    {


        mSoldierBody.collideTest ( rFloor );
        mSoldierBody.collideTest ( rGunTower.collideBoxs );

    }








    void postureChange ()
    {
        float angle = Vector3.Angle ( direct, Vector3.up );
        if (angle <= 30)
        {
            mSoldierBody.Action_climb ();
        }
        else
        {
            if (mSoldierBody.state != ASoldierBody.State_Walking)
            {
                mSoldierBody.Action_Walking ();

            }
        }
    }



    public override void reachAction ( PathNode pathNode )
    {
        if (nextPointAction != null)
        {
            nextPointAction ();
            return;
        }
        if (nextNode.next != null)
        {
            nextNode = pathNode.next;
            direct = (nextNode.goal.point - pathNode.goal.point).normalized;
            postureChange ();
        }
        else //reach end
        {

            cancelTestPath ();

            if (currentPath.index == 0) //goal0
            {
                pathGroup.stepStatus[0] = true;
                mSoldierBody.Action_NearTower ();
                allSoldierOnGroundFindPath ();
            }
            else if (currentPath.index == 1 || currentPath.index == 2)//goal1
            {
                pathGroup.stepStatus[1] = true;
                mSoldierBody.Action_NearTower ();
                allSoldierOnGroundFindPath ();
            }
            else if (currentPath.index == 3 || currentPath.index == 4)//goal2
            {
                pathGroup.stepStatus[2] = true;
                mSoldierBody.Action_NearTower ();
                allSoldierOnGroundFindPath ();
            }
            else if (currentPath.index == 5 || currentPath.index == 6)//goal3
            {
                pathGroup.stepStatus[3] = true;
                Action_PlayerFailure ();
            }

        }
    }


    void allSoldierOnGroundFindPath ()
    {
        for (int i = 0; i < soldierPool.live.Count; i++)
        {
            ASoldier soldier = soldierPool.live[i] as ASoldier;
            if (soldier.mSoldierBody.state == ASoldierBody.State_Walking
                || soldier.mSoldierBody.state == ASoldierBody.State_Climbing
                || soldier.mSoldierBody.state == ASoldierBody.State_Jumping)
            {
                soldier.pathFind ();
            }
        }
    }


    static ObjPool<SCENE_OBJ> soldierPool;
    public static void initStatic ()
    {
        soldierPool = MainRes.soldierPool;
        rFloor = Stage.floor;
        rGunTower = Stage.gunTower;
        canProduceSoldier = true;
    }



    public override string getClassName ()
    {
        return "ASoldier";

    }
    public override void postProcess ()
    {

        outOfRangeTest ();
    }

    public void dead ()
    {
        canDestroy = true;
        num--;
    }
    public override void outOfRangeTest ()
    {
        if (isOutOfStage ())
        {
            dead ();
        }
    }


    Vector3 getPosInArray ( Vector3 leftBottomPoint, float cellWid, float cellHei, int col, int index )
    {
        Vector3 pos = leftBottomPoint;
        int verticalOffset = index / col;
        int horizontalOffset = index % col;
        pos.x += cellWid * horizontalOffset;
        pos.y += cellHei * verticalOffset;
        return pos;
    }


}

public class ASoldierBody : AMultiColliderObj
{
    public override void connect ( Transform objTransform )
    {
        base.connect ( objTransform );

        //createContacts ();
    }
    public override void init ()
    {
        getKnowOther ();

        base.init ();

    }
    public override void initDimension ()
    {
        dimensionFormDirect ();

    }

    public override void initMeshInfo ()
    {
        initVertexs ();

    }

    public const string State_Normal = "normal";
    public const string State_DieInAir = "dieInAir";
    public const string State_AliveButFalling = "aliveButFalling";
    public const string State_Walking = "walking";
    public const string State_NearTower = "nearTower";
    public const string State_DieOnGround = "dieOnGround";
    public const string State_Climbing = "climb";
    public const string State_Jumping = "jumping";
    public const string State_Cheer = "cheer";
    public const string State_DeadEnd = "";
    ASoldier soldier;
    AParachute parachute;
    public void getKnowOther ()
    {
        soldier = belongToObj as ASoldier;
        parachute = soldier.mParachute;
    }



    void deadAnimationCompleteTest ()
    {
        AnimatorStateInfo solidierBodyInfo = animator.GetCurrentAnimatorStateInfo ( 0 );
        if (solidierBodyInfo.IsName ( State_DieOnGround ))
        {
            if (solidierBodyInfo.normalizedTime >= 1)
            {
                state = State_DeadEnd;
                soldier.dead ();
            }
        }
    }

    public override void preProcess ()
    {
        deadAnimationCompleteTest ();


    }




    public override void collideAction ( ContactInfo contactInfo )
    {

        if (contactInfo.other is ABullet)
        {
            if (state == State_Normal)
            {
                if (parachute.state == AParachute.State_UnOpen)
                {
                    parachute.Action_fall ();
                }
                Action_dieInAir ();

            }
            else if (state == State_Walking)
            {
                Action_DieOnGround ();
            }
            else if(state==State_Climbing)
            {
                Action_DieOnGround ();
            }
            else if (state == State_AliveButFalling)
            {
                Action_dieInAir ();
            }
            else if (state == State_DieInAir)
            {
                if (parachute.state == AParachute.State_OpenComplete)
                    parachute.Action_fall ();
            }
        }
        else if (contactInfo.other is ABlock)
        {
            soldier.cancelTestBlock ();



            if (state == State_Normal)
            {
                if (Player.isFailure == true)
                {
                    parachute.Action_BackToBag ();
                    Action_Cheer ();
                    return;
                }
                else
                {
                    if (contactInfo.other is AFloor)
                    {
                        parachute.Action_BackToBag ();
                        Action_Walking ();
                        soldier.pathFind ();
                        soldier.beginTestPath ();
                    }
                    else if (contactInfo.other is GunTowerBase)
                    {
                        parachute.Action_BackToBag ();
                        soldier.Action_PlayerFailure ();
                    }

                }

            }
            else if (state == State_DieInAir)
            {
                parachute.Action_BackToBag ();
                Action_LandOnGoundDead ();
            }
            else if (state == State_AliveButFalling)
            {
                Action_LandOnGoundDead ();
            }






        }


    }




    public void Action_Prepare ()
    {
        state = State_Normal;
        soldier.rotationToNormal ();
    }
    public void Action_climb ()
    {
        state = State_Climbing;
        soldier.speed = ASoldier.pClimbSpeed;
        soldier.rotationOnGround ();

    }

    public void Action_dieInAir ()
    {
        state = State_DieInAir;
        soldier.rotationInAir ();
    }
    public void Action_DieOnGround ()
    {
        state = State_DieOnGround;
        soldier.removePart ( this );
        soldier.speed = 0;
    }
    public void Action_Jump ()
    {
        state = State_Jumping;
        soldier.speed = ASoldier.pJumpSpeed;
        soldier.rotationOnGround ();
    }
    public void Action_Walking ()
    {
        state = State_Walking;
        soldier.speed = ASoldier.pMoveOnGroundSpeed;
        soldier.rotationOnGround ();

    }
    public void Action_LandOnGoundDead ()
    {

        state = State_DieOnGround;
        soldier.removePart ( this );
        soldier.speedVec = Vector3.zero;
    }
    public void Action_AliveButFalling ()
    {
        state = State_AliveButFalling;
    }
    public void Action_NearTower ()
    {
        state = State_NearTower;
        transform.rotation = Quaternion.identity;
        soldier.removePart ( this );
        soldier.speed = 0;
    }
    public void Action_Cheer ()
    {
        state = State_Cheer;
        transform.rotation = Quaternion.identity;
        soldier.speed = 0;
    }

}
public class AParachute : AMultiColliderObj
{
    public override void init ()
    {
        getKnowOther ();
        base.init ();

    }
    public override void initDimension ()
    {
        dimensionFormDirect ();
    }

    public override void initMeshInfo ()
    {
        initVertexs ();
    }
    public override void preProcess ()
    {
        randomOpenParachute ();
        AnimatorStateInfo parachuteInfo = animator.GetCurrentAnimatorStateInfo ( 0 );
        if (parachuteInfo.IsName ( State_Open ))
        {
            if (parachuteInfo.normalizedTime >= 1)
            {
                state = State_OpenComplete;
            }

        }
    }



    float minOpenTime = 0;
    float MaxOpenTime;//determined by born height
    public float openTime;
    float timeEscaped;
    void randomOpenParachute ()
    {
        if (!canOpen)
            return;


        if (state == State_UnOpen)
        {
            timeEscaped += Time.deltaTime;
            if (timeEscaped >= openTime)
            {
                state = State_Open;

            }

        }
        else if (state == State_OpenComplete)
        {

            soldier.speed = ASoldier.pFallSpeedAfterOpenParachute;
            soldier.addPart ( this );
            canOpen = false;

        }



    }
    public const string State_UnOpen = "ParachuteUnOpen";
    public const string State_Open = "ParachuteOpening";
    public const string State_OpenComplete = "";

    ASoldier soldier;
    ASoldierBody soldierBody;
    public bool canOpen = true;
    public void getKnowOther ()
    {
        soldier = belongToObj as ASoldier;
        soldierBody = soldier.mSoldierBody;
    }
    public float floorY;
    public float gunTowerY;
    float calculateMinOpenHei ()
    {
        if (ASoldier.rFloor != null && ASoldier.rGunTower != null)
        {
            floorY = ASoldier.rFloor._vertexs[0].pos.y;
            gunTowerY = ASoldier.rGunTower._vertexs[0].pos.y;
            float openDist = 5f;
            float gunTowerHalfWid = ASoldier.rGunTower.wid / 2;
            if (transform.position.x > -gunTowerHalfWid && transform.position.x < gunTowerHalfWid)//block Area
            {
                return gunTowerY + openDist;
            }
            else
            {
                return floorY + openDist;
            }
        }
        else
        {
            return 0;
        }


    }

    float minHeightToOpen = 30;
    void decideParachuteOpenTime ()
    {
        float minOpenHei = calculateMinOpenHei (); ;

        float hei = Mathf.Abs ( minOpenHei - transform.position.y );
        if (hei <= minHeightToOpen)
        {
            openTime = 0;
        }
        else
        {
            MaxOpenTime = (hei - minHeightToOpen) / Mathf.Abs ( soldier.pFallVector.y );
            openTime = minOpenTime + Random.value * MaxOpenTime;
        }
        //Debug.Log ( openTime );
    }
    public void Action_Prepare ()
    {
        state = State_UnOpen;
        decideParachuteOpenTime ();
        timeEscaped = 0;

    }
    public void Action_fall ()
    {
        state = State_UnOpen;
        canOpen = false;
        soldier.removePart ( this );
        soldier.speed = ASoldier.pFallSpeed;
    }
    public void Action_BackToBag ()
    {
        state = State_UnOpen;
        soldier.removePart ( this );
        soldier.speed = 0;
    }

    public override void collideAction ( ContactInfo contactInfo )
    {
        if (contactInfo.other is ABullet)
        {
            if (state == State_OpenComplete)
            {
                if (soldierBody.state == ASoldierBody.State_Normal)
                {
                    soldierBody.Action_AliveButFalling ();
                    Action_fall ();
                }
                else if (soldierBody.state == ASoldierBody.State_DieInAir)
                {
                    Action_fall ();
                }

            }

        }
    }
}