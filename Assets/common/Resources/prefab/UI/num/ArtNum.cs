using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtNum :UIOBJ
{
    public override void connect ( Transform trans )
    {
        base.connect ( trans );
        MaxDigitNum = transform.childCount - 1;
        str = new System.Text.StringBuilder ( MaxDigitNum );
        numbers = new SingleNum[MaxDigitNum];

        for (int i = 0; i < MaxDigitNum; i++)
        {
            numbers[i] = new SingleNum ();
           

        }
    }
    SingleNum[] numbers;
    int MaxDigitNum;
    override public void init()
    {
        
   
        for (int i = 0; i < MaxDigitNum; i++)
        {
            numbers[i].connect ( transform.GetChild ( i ).transform );
            numbers[i].init ();
            
        }

    }
    System.Text.StringBuilder str;
    public void display(int num)
    {
        str.Remove ( 0, str.Length );
        str.Append ( num );
        int digit = str.Length;
        int index = MaxDigitNum-digit;
        while(digit>0)
        {
           
            int divider = (int)Mathf.Pow ( 10, digit-1 );
            int numCurrent = num / divider;
            numbers[index].display (numCurrent);
            num %= divider;
            index++;
            digit--;
        }
    }
}

public class SingleNum :UIOBJ
{
    public float scale = 1;
    Transform[] zeroToNine=new Transform[10];

   override public void init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            zeroToNine[i] = transform.GetChild ( i );
            if(i!=displayNow)
                zeroToNine[i].gameObject.SetActive ( false );
        }

    }
    Vector3 scaleVector;
    public void setScale(float factor)
    {
        scaleVector.x = factor;
        scaleVector.y = factor;
        scaleVector.z = factor;
        transform.localScale = scaleVector;
    }
    int displayNow = 0;
    public void display(int num)
    {
        zeroToNine[displayNow].gameObject.SetActive ( false );
        zeroToNine[num].gameObject.SetActive ( true );
        displayNow = num;
    }
}
