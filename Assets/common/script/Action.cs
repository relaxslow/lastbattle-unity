using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void FUN ();
public class Action : IPoolObj
{

    public FUN function;
    public System.Text.StringBuilder name = new System.Text.StringBuilder ( 1024 );
    public int indexInGroup;
    public void init ( FUN fun )
    {
        function = fun;
        name.Append ( fun.Target.ToString () + "." + fun.Method.ToString () );
    }
    public void init ( FUN fun, Transform objTrans )
    {
        function = fun;
        name.Append ( objTrans.name.ToString () + "." + fun.Method.ToString () );
    }
    public void run ()
    {
        if (function != null)
            function ();
        else
        {
            Debug.Log ( "function is null" );
        }
    }
    public void reset ()
    {
        function = null;
        name.Remove ( 0, name.Length );
        indexInGroup = -1;
    }
}

public class ActionGroup
{
    public static ObjPool<Action> pool = new ObjPool<Action> ();
    public System.Text.StringBuilder actionNames = new System.Text.StringBuilder ( 1024 );
    List<Action> All = new List<Action> ();
    FUN allFunPrepareToRun;
    public static void initStatic ( int funNum ) //when use multiAdd mode ,this is neccessary
    {
        Action[] actions = new Action[funNum];
        for (int i = 0; i < funNum; i++)
        {
            actions[i] = new Action ();
            actions[i].reset ();
        }
        pool.init ( actions, "actionPool" );
    }
    public void run ()
    {
        if (allFunPrepareToRun != null)
        {
            allFunPrepareToRun ();
        }
        else
        {
        }
    }

    public void addAction ( FUN fun, Transform transform = null )//can add a function multi times
    {

        Action action = pool.create ();
        if (action == null)
        {
            Debug.Log ( "no more action in pool" );
            return;

        }
        
        action.init ( fun, transform );
        action.indexInGroup = All.Count;
        All.Add ( action );
        mergeFunctions ();

    }
    System.Text.StringBuilder removeFunName = new System.Text.StringBuilder ( 1024 );
    public void removeAction ( FUN fun, Transform transform = null )
    {

        removeFunName.Remove ( 0, removeFunName.Length );
        if (transform != null)
        {
            removeFunName.Append ( transform.name.ToString () + "." + fun.Method.ToString () );
        }
        else
        {
            removeFunName.Append ( fun.Target.ToString () + "." + fun.Method.ToString () );

        }
        Action action = All.Find ( x => x.name.ToString ().Equals ( removeFunName.ToString () ) );
        if (action != null)
        {
            All.Remove ( action );
            pool.delete ( action );
        }

        mergeFunctions ();

    }


    public void addAction ( Action action )//each action can add only once
    {
        if (!All.Contains ( action ))
        {
            action.indexInGroup = All.Count;
            All.Add ( action );
        }

        mergeFunctions ();

    }
    public void removeAction ( Action action )
    {
        All.Remove ( action );
        mergeFunctions ();
    }
    public void clear ()
    {
        for (int i = 0; i < All.Count; i++)
        {
            pool.delete ( All[i] );
        }
        All.Clear ();
        mergeFunctions ();
    }
    void mergeFunctions ()
    {

        allFunPrepareToRun = null;
        for (int i = 0; i < All.Count; i++)
        {
            allFunPrepareToRun += All[i].function;
        }
        recordName ();

    }
    void recordName ()
    {
        actionNames.Remove ( 0, actionNames.Length );
        for (int i = 0; i < All.Count; i++)
        {
            actionNames.Append ( All[i].name + "|" );
        }
    }
}
