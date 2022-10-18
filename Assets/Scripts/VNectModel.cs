using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Position index of joint points
/// </summary>
using Unity.Mathematics;
 
public enum PositionIndex : int
{
    rShldrBend,
    rForearmBend,
    rHand,
    rThumb2,
    rMid1,

    lShldrBend,
    lForearmBend,
    lHand,
    lThumb2,
    lMid1,

    lEar,
    lEye,
    rEar,
    rEye,
    Nose,

    rThighBend,
    rShin,
    rFoot,
    rToe,

    lThighBend,
    lShin,
    lFoot,
    lToe,

    abdomenUpper,

    //Calculated coordinates
    hip,
    head,
    neck,
    spine,

    Count,
    None,
}

public static partial class EnumExtend
{
    public static int Int(this PositionIndex i)
    {
        return (int)i;
    }
}

public class VNectModel : MonoBehaviour
{

    public class JointPoint
    {
        public Vector2 Pos2D = new Vector2();
        public float score2D;

        public Vector3 Pos3D = new Vector3();
        public Vector3 Now3D = new Vector3();
        public Vector3[] PrevPos3D = new Vector3[6];
        public float score3D;

        // Bones
        public Transform Transform = null;
        public Quaternion InitRotation;
        public Quaternion Inverse;
        public Quaternion InverseRotation;

        public JointPoint Child = null;
        public JointPoint Parent = null;

        // For Kalman filter
        public Vector3 P = new Vector3();
        public Vector3 X = new Vector3();
        public Vector3 K = new Vector3();

        

    }

    public class Skeleton
    {
        public GameObject LineObject;
        public LineRenderer Line;

        public JointPoint start = null;
        public JointPoint end = null;
    }

    public List<Skeleton> Skeletons = new List<Skeleton>();
    public Material SkeletonMaterial;
    public Material SkeletonMaterial2;

    public bool ShowSkeleton =true;
    private bool useSkeleton;
    private float SkeletonX;
    private float SkeletonY;
    private float SkeletonZ;
    public WebcamTexture webcamTexture;
    private float SkeletonScaleX;
    private float SkeletonScaleY;
    private float SkeletonScaleZ;
    public Vector3 MemoryCalibrationPosition;
    public Vector3 MemoryArmPosition;
    private Vector3 ShoulderWidthMarker;
    private Vector3 ShoulderWidthBodyTracking;
    public Vector3 MatricePassage1;
    public Vector3 MatricePassage2;
    public Vector3 MatricePassage3;
    public bool registration= false;
    public bool registration1 = false;
    public bool registration2 = false;
    public bool registration3 = false;
    public bool registration4 = false;
    // Calibration
    public Vector3 PmMemory = Vector3.zero;
    public Vector3 PbtMemory = Vector3.zero;
    public Vector3 Pm1 = new Vector3();
    public Vector3 Pm2 = new Vector3();
    public Vector3 Pm3 = new Vector3();
    public Vector3 Pm4 = new Vector3();
    public Vector3 Pbt1 = new Vector3();
    public Vector3 Pbt2 = new Vector3();
    public Vector3 Pbt3 = new Vector3();
    public Vector3 Pbt4 = new Vector3();
    public Vector3 matricePm1;
    public Vector3 matricePm2;
    public Vector3 matricePm3;
    public Vector3 matricePbt1;
    public Vector3 matricePbt2;
    public Vector3 matricePbt3;
    public Vector3 matricePbtinv1;
    public Vector3 matricePbtinv2;
    public Vector3 matricePbtinv3;
    private float determinant;

    public GameObject ButtomArmRegistration1completed;
    public GameObject ButtomArmRegistration2completed;
    public GameObject ButtomArmRegistration3completed;
    public GameObject ButtomArmRegistration4completed;



    // Joint position and bone
    public JointPoint[] jointPoints;
    public JointPoint[] JointPoints { get { return jointPoints; } }


    private void Update()
    {
        if (jointPoints != null)
        {
            PoseUpdate();
        }
    }

    /// <summary>
    /// Initialize joint points
    /// </summary>
    /// <returns></returns>
    public JointPoint[] Init()
    {
        jointPoints = new JointPoint[PositionIndex.Count.Int()];
        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new JointPoint();

        useSkeleton = ShowSkeleton;
        if (useSkeleton)
        {
            // Line Child Settings
            // Right Arm
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.rForearmBend);
            AddSkeleton(PositionIndex.rForearmBend, PositionIndex.rHand);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rThumb2);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rMid1);

            // Left Arm
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.lForearmBend);
            AddSkeleton(PositionIndex.lForearmBend, PositionIndex.lHand);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lThumb2);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lMid1);

            // Fase
            AddSkeleton(PositionIndex.lEar, PositionIndex.Nose);
            AddSkeleton(PositionIndex.rEar, PositionIndex.Nose);

            // Right Leg
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShin);
            AddSkeleton(PositionIndex.rShin, PositionIndex.rFoot);
            AddSkeleton(PositionIndex.rFoot, PositionIndex.rToe);

            // Left Leg
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShin);
            AddSkeleton(PositionIndex.lShin, PositionIndex.lFoot);
            AddSkeleton(PositionIndex.lFoot, PositionIndex.lToe);

            // etc
            AddSkeleton(PositionIndex.spine, PositionIndex.neck);
            AddSkeleton(PositionIndex.neck, PositionIndex.head);
            AddSkeleton(PositionIndex.head, PositionIndex.Nose);
            AddSkeleton(PositionIndex.neck, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.neck, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.rThighBend);
        }

        return JointPoints;
    }

    public void PoseUpdate()
    {
        // caliculate movement range of z-coordinate from height
        var t1 = Vector3.Distance(jointPoints[PositionIndex.head.Int()].Pos3D, jointPoints[PositionIndex.neck.Int()].Pos3D);
        var t2 = Vector3.Distance(jointPoints[PositionIndex.neck.Int()].Pos3D, jointPoints[PositionIndex.spine.Int()].Pos3D);
        var pm = (jointPoints[PositionIndex.rThighBend.Int()].Pos3D + jointPoints[PositionIndex.lThighBend.Int()].Pos3D) / 2f;
        var t3 = Vector3.Distance(jointPoints[PositionIndex.spine.Int()].Pos3D, pm);
        var t4r = Vector3.Distance(jointPoints[PositionIndex.rThighBend.Int()].Pos3D, jointPoints[PositionIndex.rShin.Int()].Pos3D);
        var t4l = Vector3.Distance(jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.lShin.Int()].Pos3D);
        var t4 = (t4r + t4l) / 2f;
        var t5r = Vector3.Distance(jointPoints[PositionIndex.rShin.Int()].Pos3D, jointPoints[PositionIndex.rFoot.Int()].Pos3D);
        var t5l = Vector3.Distance(jointPoints[PositionIndex.lShin.Int()].Pos3D, jointPoints[PositionIndex.lFoot.Int()].Pos3D);
        var t5 = (t5r + t5l) / 2f;
        var t = t1 + t2 + t3 + t4 + t5;
        
        
        if (registration1 == true)
        {
            MemoryCalibrationPosition = webcamTexture.MemoryTransducerPosition;

            if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.5 )
            {

                if (MemoryCalibrationPosition.magnitude > 0)
                {
                    PmMemory = MemoryCalibrationPosition;
                    MemoryCalibrationPosition = Vector3.zero;
                }
                MemoryCalibrationPosition = Vector3.zero;

            }
            else if ((PbtMemory - jointPoints[PositionIndex.lShldrBend.Int()].Pos3D).magnitude > 5)
            {
                if (jointPoints[PositionIndex.lShldrBend.Int()].Pos3D.magnitude > 0)
                {
                    PbtMemory = jointPoints[PositionIndex.lShldrBend.Int()].Pos3D;
                }

                jointPoints[PositionIndex.lShldrBend.Int()].Pos3D = Vector3.zero;
                
            }
            else if ((PmMemory - MemoryCalibrationPosition).magnitude >0.01)
            {

                Pm1 = MemoryCalibrationPosition;
                Pbt1 = jointPoints[PositionIndex.lShldrBend.Int()].Pos3D;
                Debug.Log("Registration 1 completed");
                ButtomArmRegistration1completed.SetActive(true);
                registration1 = false;
                PmMemory = Vector3.zero;
                PbtMemory = Vector3.zero;
            }
        }
        if (registration2 == true)
        {
        
            MemoryCalibrationPosition = webcamTexture.MemoryTransducerPosition;

            if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.5)
            {

                if (MemoryCalibrationPosition.magnitude > 0)
                {
                    PmMemory = MemoryCalibrationPosition;
                }
                MemoryCalibrationPosition = Vector3.zero;

            }
            else if ((PbtMemory - jointPoints[PositionIndex.lForearmBend.Int()].Pos3D).magnitude > 5)
            {
                if (jointPoints[PositionIndex.lForearmBend.Int()].Pos3D.magnitude > 0)
                {
                    PbtMemory = jointPoints[PositionIndex.lForearmBend.Int()].Pos3D;
                }

                jointPoints[PositionIndex.lForearmBend.Int()].Pos3D = Vector3.zero;

            }
            else if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.01)
            {


                Pm2 = MemoryCalibrationPosition;
                Pbt2 = jointPoints[PositionIndex.lForearmBend.Int()].Pos3D;
                Debug.Log("Registration 2 completed");
                ButtomArmRegistration2completed.SetActive(true);
                registration2 = false;
                PmMemory = Vector3.zero;
                PbtMemory = Vector3.zero;
            }
        }
        if (registration3 == true)
        {
            MemoryCalibrationPosition = webcamTexture.MemoryTransducerPosition;

            if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.5)
            {

                if (MemoryCalibrationPosition.magnitude > 0)
                {
                    PmMemory = MemoryCalibrationPosition;
                }
                MemoryCalibrationPosition = Vector3.zero;

            }
            else if ((PbtMemory - jointPoints[PositionIndex.lHand.Int()].Pos3D).magnitude > 5)
            {
                if (jointPoints[PositionIndex.lHand.Int()].Pos3D.magnitude > 0)
                {
                    PbtMemory = jointPoints[PositionIndex.lHand.Int()].Pos3D;
                }

                jointPoints[PositionIndex.lHand.Int()].Pos3D = Vector3.zero;

            }
            else if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.01)
            {


                Pm3 = MemoryCalibrationPosition;
                Pbt3 = jointPoints[PositionIndex.lHand.Int()].Pos3D;
                Debug.Log("Registration 3 completed");
                ButtomArmRegistration3completed.SetActive(true);
                registration3 = false;
                PmMemory = Vector3.zero;
                PbtMemory = Vector3.zero;
            }
        }
        if (registration4 == true)
        {
            MemoryCalibrationPosition = webcamTexture.MemoryTransducerPosition;

            if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.5)
            {

                if (MemoryCalibrationPosition.magnitude > 0)
                {
                    PmMemory = MemoryCalibrationPosition;
                }
                MemoryCalibrationPosition = Vector3.zero;

            }
            else if ((PbtMemory - jointPoints[PositionIndex.rHand.Int()].Pos3D).magnitude > 5)
            {
                if (jointPoints[PositionIndex.rHand.Int()].Pos3D.magnitude > 0)
                {
                    PbtMemory = jointPoints[PositionIndex.rHand.Int()].Pos3D;
                }

                jointPoints[PositionIndex.rHand.Int()].Pos3D = Vector3.zero;

            }
            else if ((PmMemory - MemoryCalibrationPosition).magnitude > 0.01)
            {


                Pm4 = MemoryCalibrationPosition;
                Pbt4 = jointPoints[PositionIndex.rHand.Int()].Pos3D;
                Debug.Log("Registration 4 completed");
                ButtomArmRegistration4completed.SetActive(true);
                registration4 = false;

                matricePm1[0]= Pm1[0] - Pm4[0];
                matricePm1[1]= Pm1[1] - Pm4[1];
                matricePm1[2]= Pm1[2] - Pm4[2];
                matricePm2[0]= Pm2[0] - Pm4[0];
                matricePm2[1]= Pm2[1] - Pm4[1];
                matricePm2[2]= Pm2[2] - Pm4[2];
                matricePm3[0]= Pm3[0] - Pm4[0];
                matricePm3[1]= Pm3[1] - Pm4[1];
                matricePm3[2]= Pm3[2] - Pm4[2];
                matricePbt1[0]= Pbt1[0] - Pbt4[0];
                matricePbt1[1]= Pbt1[1] - Pbt4[1];
                matricePbt1[2]= Pbt1[2] - Pbt4[2];
                matricePbt2[0]= Pbt2[0] - Pbt4[0];
                matricePbt2[1]= Pbt2[1] - Pbt4[1];
                matricePbt2[2]= Pbt2[2] - Pbt4[2];
                matricePbt3[0]= Pbt3[0] - Pbt4[0];
                matricePbt3[1]= Pbt3[1] - Pbt4[1];
                matricePbt3[2]= Pbt3[2] -Pbt4[2];

                determinant = matricePbt1[0] * (matricePbt2[1] * matricePbt3[2] - matricePbt2[2] * matricePbt3[1]) - matricePbt2[0] * (matricePbt1[1] * matricePbt3[2] - matricePbt1[2] * matricePbt3[1]) + matricePbt3[0] * (matricePbt2[2] * matricePbt1[1] - matricePbt1[2] * matricePbt2[1]);
                matricePbtinv1[0] = (matricePbt2[1] * matricePbt3[2] - matricePbt3[1] * matricePbt2[2]) / determinant;
                matricePbtinv1[1] = -(matricePbt1[1] * matricePbt3[2] - matricePbt3[1] * matricePbt1[2]) / determinant;
                matricePbtinv1[2] = (matricePbt1[1] * matricePbt2[2] - matricePbt2[1] * matricePbt1[2]) / determinant;
                matricePbtinv2[0] =-(matricePbt2[0] * matricePbt3[2] - matricePbt3[0] * matricePbt2[2]) / determinant;
                matricePbtinv2[1] = (matricePbt1[0] * matricePbt3[2] - matricePbt3[0] * matricePbt1[2]) / determinant;
                matricePbtinv2[2] =-(matricePbt1[0] * matricePbt2[2] - matricePbt2[0] * matricePbt1[2]) / determinant;
                matricePbtinv3[0] = (matricePbt2[0] * matricePbt3[1] - matricePbt3[0] * matricePbt2[1]) / determinant;
                matricePbtinv3[1] = -(matricePbt1[0] * matricePbt3[1] - matricePbt3[0] * matricePbt1[1]) / determinant;
                matricePbtinv3[2] = (matricePbt1[0] * matricePbt2[1] - matricePbt2[0] * matricePbt1[1]) / determinant;

                MatricePassage1[0] = matricePm1[0] * matricePbtinv1[0] + matricePm2[0] * matricePbtinv1[1] + matricePm3[0] * matricePbtinv1[2];
                MatricePassage1[1] = matricePm1[1] * matricePbtinv1[0] + matricePm2[1] * matricePbtinv1[1] + matricePm3[1] * matricePbtinv1[2];
                MatricePassage1[2] = matricePm1[2] * matricePbtinv1[0] + matricePm2[2] * matricePbtinv1[1] + matricePm3[2] * matricePbtinv1[2];
                MatricePassage2[0] = matricePm1[0] * matricePbtinv2[0] + matricePm2[0] * matricePbtinv2[1] + matricePm3[0] * matricePbtinv2[2];
                MatricePassage2[1] = matricePm1[1] * matricePbtinv2[0] + matricePm2[1] * matricePbtinv2[1] + matricePm3[1] * matricePbtinv2[2];
                MatricePassage2[2] = matricePm1[2] * matricePbtinv2[0] + matricePm2[2] * matricePbtinv2[1] + matricePm3[2] * matricePbtinv2[2];
                MatricePassage3[0] = matricePm1[0] * matricePbtinv3[0] + matricePm2[0] * matricePbtinv3[1] + matricePm3[0] * matricePbtinv3[2];
                MatricePassage3[1] = matricePm1[1] * matricePbtinv3[0] + matricePm2[1] * matricePbtinv3[1] + matricePm3[1] * matricePbtinv3[2];
                MatricePassage3[2] = matricePm1[2] * matricePbtinv3[0] + matricePm2[2] * matricePbtinv3[1] + matricePm3[2] * matricePbtinv3[2];
                
                SkeletonX = Pm4[0] - (MatricePassage1[0] * Pbt4[0] + MatricePassage2[0] * Pbt4[1] + MatricePassage3[0] * Pbt4[2]);
                SkeletonY = Pm4[1] - (MatricePassage1[1] * Pbt4[0] + MatricePassage2[1] * Pbt4[1] + MatricePassage3[1] * Pbt4[2]);
                SkeletonZ = Pm4[2] - (MatricePassage1[2] * Pbt4[0] + MatricePassage2[2] * Pbt4[1] + MatricePassage3[2] * Pbt4[2]);

            }
        }


        foreach (var sk in Skeletons)
        {

            var s = sk.start;
            var e = sk.end;
            if (registration == false)
            {
                sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * 0.008f, s.Pos3D.y * 0.008f + 0.5f, s.Pos3D.z * 0.008f ));
                sk.Line.SetPosition(1, new Vector3(e.Pos3D.x *0.008f, e.Pos3D.y * 0.008f +  0.5f,  e.Pos3D.z * 0.008f));
            }

            else
            {
                
                sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * MatricePassage1[0] + s.Pos3D.y * MatricePassage2[0] + s.Pos3D.z * MatricePassage3[0] + SkeletonX, s.Pos3D.x * MatricePassage1[1] + s.Pos3D.y * MatricePassage2[1] + s.Pos3D.z * MatricePassage3[1] + SkeletonY, s.Pos3D.x * MatricePassage1[2] + s.Pos3D.y * MatricePassage2[2] + s.Pos3D.z * MatricePassage3[2] + SkeletonZ));
                sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * MatricePassage1[0] + e.Pos3D.y * MatricePassage2[0] + e.Pos3D.z * MatricePassage3[0] + SkeletonX, e.Pos3D.x * MatricePassage1[1] + e.Pos3D.y * MatricePassage2[1] + e.Pos3D.z * MatricePassage3[1] + SkeletonY, e.Pos3D.x * MatricePassage1[2] + e.Pos3D.y * MatricePassage2[2] + e.Pos3D.z * MatricePassage3[2] + SkeletonZ));
                sk.Line.material = SkeletonMaterial2;
            }

        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }

    /// <summary>
    /// Add skelton from joint points
    /// </summary>
    /// <param name="s">position index</param>
    /// <param name="e">position index</param>
    public void AddSkeleton(PositionIndex s, PositionIndex e)
    {
        var sk = new Skeleton()
        {
            LineObject = new GameObject("Line"),
            start = jointPoints[s.Int()],
            end = jointPoints[e.Int()],
        };

        sk.Line = sk.LineObject.AddComponent<LineRenderer>();
        sk.Line.startWidth = 0.04f;
        sk.Line.endWidth = 0.01f;

        // define the number of vertex
        sk.Line.positionCount = 2;
        sk.Line.material = SkeletonMaterial;

        Skeletons.Add(sk);
    }
}

