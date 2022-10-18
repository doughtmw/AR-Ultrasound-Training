using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UltrasoundDisplay : MonoBehaviour
{
    public enum CurrentImageSelection
    {

        C = 0,
        D = 1,
        E = 2,
        F = 3,
        None = 4
    }

    public WebcamTexture webcamTexture;
    public Texture2D ultrasoundTexture2D;
    public Texture2D popupTexture2D;
    public VNectModel vnectmodel;

    public float minDistance;
    private Renderer _renderer;

    #region ImageCountPerFolder
    // Total number of images in each folder bin
    public float start_time;
    public float memory_time;
    public float start_time2;
    public float memory_time2;
    #endregion

    private void Start()
    {
        _renderer = gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        minDistance = (vnectmodel.Pm2).magnitude/12;
        HandleUltrasoundImages(webcamTexture.CurrentImageSelection);
    }

    public void LoadJPGToTexture2D(string additionalPath, string fileName)
    {
        var filePath = Application.streamingAssetsPath + "/ultrasound/";
        filePath = filePath + additionalPath + fileName;

        Texture2D tex = null;
        byte[] fileData;
        //Debug.Log($"FilePath: {filePath}");


        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        // Set the webcam texture to the main texture
        _renderer.material.mainTexture = tex;
    }

    private int ComputeDistance(Vector3 tArm, Vector3 tTrans, float minDist, int totalCount)
    {
        return Mathf.RoundToInt((tArm - tTrans).sqrMagnitude / minDist * totalCount);
    }

    private Tuple<int, int> ComputeAngle(Quaternion rArm, Quaternion rTrans, int angleArm, int angleTrans)
    {
        float angleBF = (rArm[0] - rTrans[0]) / 0.7f;
        float angleLR = (rArm[1] - rTrans[1]) / 1.03f;
        var k = Mathf.RoundToInt(angleBF * angleArm);
        var l = Mathf.RoundToInt(angleLR * angleTrans);
        //Debug.Log($"angleBF{angleBF}");
        //Debug.Log($"angleLR{angleLR}");

        return new Tuple<int, int>(k, l);
    }


    private void HandleUltrasoundImages(CurrentImageSelection currImageSelection)
    {
        switch (currImageSelection)
        {
            case CurrentImageSelection.C:
                memory_time = 0;
                memory_time2 = 0;
                start_time = 0;
                start_time2 = 0;
                var tC = ComputeAngle(
                           webcamTexture.MemoryForeArmRotation,
                           webcamTexture.MemoryTransducerRotation,
                           40,
                           8);

                // k is t.Item1, l is t.Item2
                
                var kC = (tC.Item1 + 40) / 2+1;
                var lC = (Mathf.Abs(tC.Item2))+1;
                var distC = (ComputeDistance(
                    webcamTexture.MemoryForearmPosition,
                    webcamTexture.MemoryTransducerPosition,
                    minDistance,
                    34)) / 2 +1;
                Debug.Log(distC + "/" + lC + "/" + string.Concat(kC, ".jpg"));
                if (distC < 34 && distC > 0)
                {
                    if (lC < 10 && lC > 0)
                    {
                        if (kC < 40 && kC > 0)
                        {


                            LoadJPGToTexture2D(
                                distC + "/" + lC + "/",
                                string.Concat(kC, ".jpg"));
                        }

                    }
                }
                break;

            case CurrentImageSelection.D:
                memory_time = 0;
                memory_time2 = 0;
                start_time = 0;
                start_time2 = 0;
                var distD = (ComputeDistance(
                    webcamTexture.MemoryForearmPosition,
                    webcamTexture.MemoryTransducerPosition,
                    minDistance,
                    197)) / 2;
                //Debug.Log(distD);
                if (distD < 197 && distD > -1)
                {

                    LoadJPGToTexture2D(
                        "dynamic/",
                        string.Concat(distD, ".jpg"));

                }
                break;

            case CurrentImageSelection.E:
                memory_time2 = 0;
                if ( memory_time == 0)
                {
                    memory_time = start_time;
                    start_time = Time.time;
                    
                }

                if (Time.time > memory_time)
                {
                    var Current_time = Time.time - memory_time;
                    
                    if (Current_time / 10 < 1 )
                    {
                        var id = Mathf.Round(Current_time / 10 * 76);
                        LoadJPGToTexture2D(
                           "static/",
                          string.Concat(id, ".jpg"));

                    }
                    break;
                }
                break;
            case CurrentImageSelection.F:
                memory_time = 0;
                if (memory_time2 == 0)
                {
                    memory_time2 = start_time2;
                    start_time2 = Time.time;

                }

                if (Time.time > memory_time2)
                {
                    var Current_time2 = Time.time - memory_time2;

                    if (Current_time2 / 30 < 1)
                    {
                        var id2 = Mathf.Round(Current_time2 / 30 * 125);
                        LoadJPGToTexture2D(
                           "static 2/",
                          string.Concat(id2, ".jpg"));

                    }
                    break;
                }
                break;

            case CurrentImageSelection.None:
                break;
            default:
                break;

        }
    }
}


