using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player  {
    public static bool isFailure;
    public static bool isSuccess;
    public static  int score;
    public static int killHelicopterNum;
    public static void init ()
    {
        score = 0;
        killHelicopterNum = 0;
    }
}
