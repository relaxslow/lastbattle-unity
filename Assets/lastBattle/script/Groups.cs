using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Groups  {
    public static List<SCENE_OBJ> bulletAll;
    public static List<SCENE_OBJ> enemybulletAll;
    public static List<SCENE_OBJ> enemyAll;
    public static void init ()
    {
        bulletAll = new List<SCENE_OBJ> ();
        enemybulletAll = new List<SCENE_OBJ> ();
        enemyAll = new List<SCENE_OBJ> ();
    }

}
