using UnityEngine;
using System.Collections;

public class progressBar : MonoBehaviour {

    Vector3 value = new Vector3(0,1,1);
    float percent = 0;
    float maxValue = 0;
    public Transform bar;
	// Use this for initialization
	void Start () {
        maxValue = bar.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
        if(percent<=100)
        setPercent (percent += 2 *Time.deltaTime);
        //increase (2);
	}

    void setPercent ( float percent )
    {
        value .x= maxValue * percent / 100;
        bar.localScale = value;
    }
 
}
