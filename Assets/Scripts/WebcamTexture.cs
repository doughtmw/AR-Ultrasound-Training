using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WebcamTexture : MonoBehaviour
{
    #region DllImport
    [DllImport("opencv-dll")]
    private static extern bool InitializeDll(
        float mS,
        float fx, float fy,
        float px, float py,
        float rx, float ry, float rz,
        float tx, float ty,
        int imgWidth, int imgHeight);

    [DllImport("opencv-dll")]
    private static extern bool StartMarkerTracking(IntPtr texData, int width, int height);

    [DllImport("opencv-dll")]
    private static extern int GetDetectedMarkerCount();

    [DllImport("opencv-dll")]
    private static extern bool GetDetectedMarkerIds(int[] detectedIds, int size);

    [DllImport("opencv-dll")]
    private static extern bool GetDetectedMarkerPose(int detectedId, float[] position, float[] quaternion);

    #endregion

    public bool isProcessVideoFrames = false;
    public GameObject transducerPrefab;
    public int transducerPrefabArUcoId = 2;
    public GameObject forearmPrefab;
    public bool isArmTracking = true;
    public VNectModel vnectmodel;
    public float scale;

    public Vector3 MemoryForearmPosition { get; set; }
    public Vector3 MemoryTransducerPosition { get; set; }
    public Quaternion MemoryForeArmRotation { get; set; }
    public Quaternion MemoryTransducerRotation { get; set; }
    public UltrasoundDisplay.CurrentImageSelection CurrentImageSelection { get; set; }


    // Camera parameters from local webcam 
    // Captured from scene 1
    // Computed in Python project
    public float markerSize = 0.08f;
    public Vector2 focalLength;
    public Vector2 principalPoint;
    public Vector3 radialDistortion;
    public Vector2 tangentialDistortion;

    private bool _isDllLoaded = false;
    private WebCamTexture _webcamTexture;
    private Thread WebcamTextureThread;
    private int _width = 0;
    private int _height = 0;
   // private GameObject _armPrefabChild;
    //private GameObject _calibrationPrefabChild;
   // private GameObject _forearmPrefabChild;
    public VideoCapture videoCapture;


    private void Start()
    {
        _webcamTexture = videoCapture.CameraPlayStart();
        // Initialize the opencv dll
        InitializeCvDll();

        CurrentImageSelection = UltrasoundDisplay.CurrentImageSelection.None;

    }


    private void InitializeCvDll()
    {
        _isDllLoaded = InitializeDll(
            markerSize,
            focalLength.x, focalLength.y,
            principalPoint.x, principalPoint.y,
            radialDistortion.x, radialDistortion.y, radialDistortion.z,
            tangentialDistortion.x, tangentialDistortion.y,
            _width, _height);

        if (isProcessVideoFrames)
        {
            UnityMainThread.wkr.AddJob(() =>
            {
                WebCamTextureToTexture2D(_webcamTexture, _webcamTexture.width, _webcamTexture.height);
            });
        }
    }


    private void Update()
    {

        if (isProcessVideoFrames)
        {
            UnityMainThread.wkr.AddJob(() =>
            {
                WebCamTextureToTexture2D(_webcamTexture, _webcamTexture.width, _webcamTexture.height);
            });

            UnityMainThread.wkr.AddJob(() =>
            {

                GetArUcoMarkerPose();

            });
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

        }
    }
  

    // Cast int to current image selection, tie to button event
    public void HandleButtonSelectionOfUltrasoundMode(int i)
    {
        CurrentImageSelection = (UltrasoundDisplay.CurrentImageSelection)i;
    }

    private void WebCamTextureToTexture2D(WebCamTexture webCamTexture, int width, int height)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        // Instantiate texture 2d, graphics copy from webcam texture to texture 2D
        var texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture2D.Apply(false);
        Graphics.CopyTexture(webCamTexture, texture2D);

        // Convert texture to opencv matrix
        if (_isDllLoaded)
        {
            StartArUcoMarkerTracking(texture2D);
        }

        // Do something with texture 2D
        Destroy(texture2D);

        sw.Stop();
        //Debug.Log($"WebCamTextureToTexture2D: in {sw.ElapsedMilliseconds} ms");
    }

    // https://stackoverflow.com/questions/50925614/convert-texture2d-to-opencv-mat
    unsafe void StartArUcoMarkerTracking(Texture2D texData)
    {
        Color32[] texDataColor = texData.GetPixels32();

        // Pin Memory
        fixed (Color32* p = texDataColor)
        {
            var isMarkerTracking = StartMarkerTracking((IntPtr)p, texData.width, texData.height);
        }
    }

    void GetArUcoMarkerPose()
    {
        if (vnectmodel.jointPoints != null)
        {
            foreach (var sk in vnectmodel.Skeletons)
            {
                scale = 1f;
                var s = sk.start;
                var e = sk.end;
                if (s == vnectmodel.jointPoints[PositionIndex.lForearmBend.Int()])
                {
                    Vector3 VectorrForearmBend = sk.Line.GetPosition(0);
                    Vector3 VectorrHand = sk.Line.GetPosition(1);
                    // try to set the virtual arm
                    Vector3 Forearm = - VectorrForearmBend + VectorrHand;
                    Quaternion Angle = Quaternion.LookRotation(Forearm, Vector3.forward);
                    //Angle.w *= -1f;
                    forearmPrefab.transform.SetPositionAndRotation(
                      VectorrForearmBend,
                      Angle);
                    
                    forearmPrefab.transform.localScale = new Vector3(scale, scale, scale);
                    MemoryForearmPosition = VectorrForearmBend;
                    MemoryForeArmRotation = Utils.RotationQuatFromRodrigues(Forearm);
                    //Debug.Log($"arm{VectorrForearmBend}");
                    //Debug.Log($"hand{VectorrHand}");
                }

            }
        }

        var count = GetDetectedMarkerCount();
        if (count > 0)

        {
            int[] ids = new int[count];
            if (GetDetectedMarkerIds(ids, ids.Length))
            {
                foreach (var id in ids)
                {
                    var positionOpenCv = new float[3];
                    var rodriguesRotation = new float[3];
                    if (GetDetectedMarkerPose(id, positionOpenCv, rodriguesRotation))
                    {
                        Vector3 position = Utils.Vec3FromFloatArr(positionOpenCv);
                        position.y *= -1f;

                        Quaternion rotation = Utils.RotationQuatFromRodrigues(Utils.Vec3FromFloatArr(rodriguesRotation));

                        var marker = new Marker(id, position, rotation);
                        //Debug.Log("Marker detected: " + marker.ToString());

                        // Separate by marker id
                        if (marker.Id == transducerPrefabArUcoId)
                        {

                            //transducerPrefab.transform.localScale = new Vector3(markerSize, markerSize, markerSize);
                            transducerPrefab.transform.SetPositionAndRotation(
                                marker.Position,
                                marker.Rotation);

                            transducerPrefab.transform.localScale = new Vector3(scale, scale, scale);
                            MemoryTransducerPosition = marker.Position;
                            MemoryTransducerRotation = marker.Rotation;
                            //Debug.Log($"marker{marker.Position}");
                        }
                        

                    }
                }
            }
        }
    }
}


public class Marker
{
    public int Id { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public Marker(int id, Vector3 position, Quaternion rotation)
    {
        Id = id;
        Position = position;
        Rotation = rotation;
    }
}