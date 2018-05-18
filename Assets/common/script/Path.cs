using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathGroup
{
    public string name;
    public  Goal[] goals;
   public  Path[] paths;
    public  bool[] stepStatus;
 


    
    
    public  Goal getGoal ( int index )
    {
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].index == index)
            {
                return goals[i];
            }
        }
        return null;
    }
    public  void constructPath (Transform pathRoot,int[][]pathDefine)
    {
        name = pathRoot.name;
        stepStatus = new bool[] {
            false ,
            false,
            false,
            false
        };

        Transform[] goalsTransform = pathRoot.GetComponentsInChildren<Transform> ();
        int goalsNum = goalsTransform.Length - 1;
        goals = new Goal[goalsNum];
        for (int i = 0; i < goalsNum; i++)
        {
            int j = i + 1;
            goals[i] = new Goal ();
            goals[i].point = goalsTransform[j].position;
            goals[i].index = j;
        }

        paths = new Path[pathDefine.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i] = new Path ();
            paths[i].create ( pathDefine[i] ,this);
            paths[i].initColor ();
            paths[i].index = i;
        }



    }
    //static List<Goal> searchResult = new List<Goal> ();
    public  void showPath ( int index )
    {

        paths[index].showPath ();


    }
}

public class PathManager
{
    static int[][] pathDefine;
   public static PathGroup left;
    public static PathGroup right;
    public static void init ()
    {

        pathDefine = new int[][]{
         new int[] { 1, 2, 9, 10 },

         new int[] { 12 , 10, 9, 4, 11 },
         new int[] { 1, 2, 9, 4, 11 },

         new int[] { 12, 10, 9 },
         new int[] { 1, 2, 9 },

         new int[] {12, 10, 9, 2, 3, 4, 5, 6, 7, 8 },
         new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
      
        };

        left = new PathGroup ();
        left.constructPath ( GameObject.Find ( "leftPath" ).transform, pathDefine );
        right = new PathGroup ();
        right.constructPath ( GameObject.Find ( "rightPath" ).transform, pathDefine );



    }
   

    //static List<Segment> allSegments = new List<Segment> ();

  

}
public class Segment 
{
    public PathNode n1;
    public PathNode n2;


    public float dist;
    public Vector3 projectPoint;
    public Vector3 pathDirect;
    public void init()
    {
        n1 = null;
        n2 = null;
        dist = -1;
        projectPoint = Vector3.zero;
        pathDirect = Vector3.zero;
    }

}
public class Goal
{
    public int index = -1;
    public Vector3 point=Vector3.zero;
 
}
public class PathNode
{
    public void create ()
    {
        goal = new Goal ();
        
    }
    public void init(Goal goal)
    {
        this.goal = goal;
    }
    public Goal goal;
    public PathNode next;
    public Path belongToPath;
}
public class Path
{
    //string groupName;
    public int index;
    public PathNode[] nodes;
    //public bool active;

    public void create2NodePath ()
    {
        nodes = new PathNode[2];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new PathNode ();
            nodes[i].goal = new Goal ();
            
        }
        nodes[0].next = nodes[1];
    }
    public void init2NodePath (Vector3 begin,Vector3 end)
    {
        nodes[0].goal.point = begin;
        nodes[1].goal.point = end;
         
    }
    public void initColor ()
    {


        float R = randomInRange ( 0.5f, 0.8f );
        float G = randomInRange ( 0.5f, 0.8f );
        float B = randomInRange ( 0.5f, 0.8f );
        float A = 0.4f;
        color = new Color ( R, G, B, A );


    }
    float randomInRange ( float min, float max )
    {
        return min + Random.value * max;
    }
    public void create ( int[] pointsIndex ,PathGroup group)
    {
        //groupName = group.name;
        nodes = new PathNode[pointsIndex.Length];
        for (int i = 0; i < pointsIndex.Length; i++)
        {
       
            int index = pointsIndex[i];
            nodes[i] = buildPathNode ( group,index );


        }
        //connectNodes
        for (int i = 0; i < nodes.Length; i++)
        {
            int j = i + 1;
            if (j < nodes.Length)
            {
                nodes[i].next =nodes[j];

            }
       
        }

    }
    PathNode buildPathNode (PathGroup group, int index )
    {
        Goal goal = group.getGoal ( index );
      
        PathNode node = new PathNode ();
        node.init ( goal );
        return node;
    
       
    }

    Color color;
    public void showPath ()
    {

        for (int i = 0; i < nodes.Length - 1; i++)
        {
            Debug.DrawLine ( nodes[i].goal.point, nodes[i + 1].goal.point, color );
        }
    }
}
