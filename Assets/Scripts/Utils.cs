using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public enum WebcamResolutions
    {
        _1920x1080,
        _1280x720,
        _720x480,
        _640x360
    }

    public static Vector3 Vec3FromFloatArr(float[] f)
    {
        return new Vector3()
        {
            x = f[0],
            y = f[1],
            z = f[2]
        };
    }

    public static Quaternion RotationQuatFromRodrigues(Vector3 v)
    {
        var angle = Mathf.Rad2Deg * v.magnitude;
        var axis = v.normalized;
        Quaternion q = Quaternion.AngleAxis(angle, axis);

        // Ensure: 
        // Positive x axis is in the left direction of the observed marker
        // Positive y axis is in the upward direction of the observed marker
        // Positive z axis is facing outward from the observed marker
        // Convert from rodrigues to quaternion representation of angle
        q = Quaternion.Euler(
            -1.0f * q.eulerAngles.x,
            q.eulerAngles.y,
            -1.0f * q.eulerAngles.z) * Quaternion.Euler(0, 0, 180);

        return q;
    }

    public static Tuple<Vector3, Quaternion> CalcAverageMarker(List<Marker> markers)
    {
        var count = (float)markers.Count;
        var averagePos = Vector3.zero;
        int id = -1;
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (var marker in markers)
        {
            averagePos += marker.Position / count;
            rotations.Add(marker.Rotation);
            id = marker.Id;
        }

        var averageRot = CalcAverageQuaternion(rotations.ToArray());

        return new Tuple<Vector3, Quaternion>(averagePos, averageRot);
        //return new Marker(id, averagePos, averageRot);
    }
    public static Quaternion CalcAverageQuaternion(Quaternion[] quaternions)
    {
        Quaternion mean = quaternions[0];
        var text = "Quaternions: ";
        for (int i = 1; i < quaternions.Length; i++)
        {
            text += "{" + quaternions[i].x + ", " + quaternions[i].y + ", " + quaternions[i].z + ", " + quaternions[i].w + "}";
            float weight = 1.0f / (i + 1);
            mean = Quaternion.Slerp(mean, quaternions[i], weight);
        }
        //Debug.Log(text);
        //Debug.Log("Mean Quaternion: " + mean.x + ", " + mean.y + ", " + mean.z + ", " + mean.w);
        return mean;
    }
}