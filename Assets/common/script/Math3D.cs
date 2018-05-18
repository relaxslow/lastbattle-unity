using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math3D  {
    public static Vector3 intersectPoint(Ray ray,Vector3 normal){
        Vector3 beginVec = Vector3.zero - ray.origin;
        float dist = Vector3.Dot(beginVec, normal);
        //        if (dist > 0)
        //            return Vector3.zero;
        Vector3 perpendicularVec = normal * dist;
        float angle = Vector3.Angle(ray.direction, perpendicularVec);
        //        if (angle >= 90)
        //            return;
        float length=Mathf.Abs(dist) / Mathf.Cos(Mathf.Deg2Rad * angle);
        Vector3 point = ray.origin + ray.direction * length;
        return point;
    } 
    public static Vector3 AverageNormal(List<NORMAL> normals){
        Vector3 A = Vector3.zero;
        for (int i = 0; i < normals.Count; i++)
        {
            Vector3 B = normals[i].direct;
          
            A = ((A - B) / 2 + B).normalized;
        }
        return A;
    }
    public static Vector3 AveragePos ( VERTEX[] points )
    {

        Vector3 average = points[1].pos + (points[0].pos - points[1].pos) / 2;
        return average;
    }

    public static bool isProjectInSurface ( Vector3 P, Vector3 SP0, Vector3 SP1)
    {
        Vector3 vec1 = SP0 - P;
        Vector3 vec2 = SP1 - P;
        Vector3 any = SP1 - SP0;
        float vec1Dot = Vector3.Dot ( vec1, any.normalized );
        float vec2Dot = Vector3.Dot ( vec2, any.normalized );
        float inSurface = vec1Dot * vec2Dot;
        if (inSurface < 0)
            return true;
        return false;
    }
}
