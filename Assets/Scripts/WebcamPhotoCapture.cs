using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

// Adapted from: https://docs.unity3d.com/ScriptReference/Windows.WebCam.PhotoCapture.TakePhotoAsync.html
public class WebcamPhotoCapture : MonoBehaviour
{
    public Utils.WebcamResolutions webcamResolution;
    public int webCamDevice = 0;
    public int frameRate = 30;
    public int totalImagesToCapture = 25;
    public Renderer cameraRenderer;
    public Renderer webCamRenderer;

    private WebCamTexture _webcamTexture;
    int capturedImageCount = 0;
    Texture2D targetTexture;

    // Use this for initialization
    void Start()
    {
        InitializeCameraAndWebcam();
    }

    void InitializeCameraAndWebcam()
    {
        // Get all webcam devices
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"InitializeCameraAndWebcam: device name {devices[i].name}");
        }

        // Select the device by name
        string deviceName = devices[webCamDevice].name;
        Debug.Log($"InitializeCameraAndWebcam: Selecting {deviceName} for initialization.");

        int width = 0;
        int height = 0;

        switch (webcamResolution)
        {
            case Utils.WebcamResolutions._1920x1080:
                width = 1920;
                height = 1080;
                break;
            case Utils.WebcamResolutions._1280x720:
                width = 1280;
                height = 720;
                break;
            case Utils.WebcamResolutions._720x480:
                width = 720;
                height = 480;
                break;
            case Utils.WebcamResolutions._640x360:
                width = 640;
                height = 360;
                break;
            default:
                break;
        }

        // Instantiate webcam texture
        _webcamTexture = new WebCamTexture(deviceName, width, height, frameRate);

        // Set the webcam texture to the main texture
        webCamRenderer.material.mainTexture = _webcamTexture;

        // Begin playing
        _webcamTexture.Play();
        Debug.Log($"StartWebcam: with resolution {_webcamTexture.width} x {_webcamTexture.height}");
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space key was pressed. Taking picture.");
            TakePicture();
        }
        else if (Input.GetKeyDown("space"))
        {
            Debug.Log("Completed picture recording.");
        }
        else { }
    }

    void TakePicture()
    {
        capturedImageCount++;
        string filename = string.Format(@"{0}.png", capturedImageCount);
        string filePath = System.IO.Path.Combine(Application.dataPath, "CapturedImages/raw/");
        filePath = System.IO.Path.Combine(filePath, filename);

        // Set the webcam texture to the main texture
        Texture2D snap = new Texture2D(_webcamTexture.width, _webcamTexture.height);
        cameraRenderer.material.mainTexture = snap;

        snap.SetPixels(_webcamTexture.GetPixels());
        snap.Apply();
        System.IO.File.WriteAllBytes(filePath, snap.EncodeToPNG());

        if (capturedImageCount < totalImagesToCapture)
        {
            Debug.Log($"Captured {capturedImageCount} / {totalImagesToCapture} images to {filePath}.");
        }
        else
        {
            Debug.Log($"Completed photo caputre.");
        }
    }
}