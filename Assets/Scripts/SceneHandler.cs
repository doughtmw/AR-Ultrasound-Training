using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneHandler : MonoBehaviour
{
    public WebcamTexture WebcamTexture;
    public GameObject UltrasoundProbe;
    public GameObject Arm;
    public GameObject ButtonStart;
    public GameObject ButtomArmRegistration1;
    public GameObject ButtomArmRegistration2;
    public GameObject ButtomArmRegistration3;
    public GameObject ButtomArmRegistration4;
    public GameObject ButtomArmRegistration1completed;
    public GameObject ButtomArmRegistration2completed;
    public GameObject ButtomArmRegistration3completed;
    public GameObject ButtomArmRegistration4completed;
    public GameObject ButtomArmGO2;
    public GameObject ButtonList;
    public GameObject Ultrasound;
    public GameObject VideoScreen;
    public VNectModel VNectModel;

    // public UnityEvent
    //public enum EventStatus
    //{
    //    markerPlacement = 0,
    //    ultrasoundTracking = 1
    //}

    // STAGE 0
    public void BeginRegistration1()
    {
        // Stop tracking the button arm go, lock it's pose
        VNectModel.registration1 = true;

        // Hide the buttom arm go 
        UltrasoundProbe.SetActive(false);
        Arm.SetActive(false);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(true);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
    }



    // STAGE 1
    public void HideRegistration1AndBeginRegistration2()
    {
        // Stop tracking the button arm go, lock it's pose
        VNectModel.registration1 = false;
        VNectModel.registration2 = true;

        // Hide the buttom arm go 
        UltrasoundProbe.SetActive(false);
        Arm.SetActive(false);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(true);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
    }

    // STAGE 2
    public void HideRegistration2AndBeginRegistration3()
    {
        // Stop tracking the button arm go, lock it's pose
        VNectModel.registration2 = false;
        VNectModel.registration3 = true;

        // Hide the buttom arm go 
        UltrasoundProbe.SetActive(false);
        Arm.SetActive(false);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(true);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
    }

    // STAGE 3
    public void HideRegistration3AndBeginRegistration4()
    {
        // Stop tracking the button arm go, lock it's pose
        VNectModel.registration3 = false;
        VNectModel.registration4 = true;

        // Hide the buttom arm go 
        UltrasoundProbe.SetActive(false);
        Arm.SetActive(false);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(true);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
    }


    // STAGE 4
    public void HideMenuAndBeginMarkerTracking()
    {
        // Stop tracking the button arm go, lock it's pose
        VNectModel.registration4 = false;
        VNectModel.registration = true;
        Debug.Log("New matrice");
        VideoScreen.SetActive(false);

        // Hide the buttom arm go 
        VNectModel.ShowSkeleton = false;
        UltrasoundProbe.SetActive(true);
        Arm.SetActive(true);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(true);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
    }

    // STAGE 5
    public void HideMenuAndBeginUltrasound()
    {
        // Hide the ultrasound menu and show 
        // ultrasound display options
        UltrasoundProbe.SetActive(true);
        Arm.SetActive(true);
        ButtonStart.SetActive(false);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(true);
        Ultrasound.SetActive(true);
       
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Hide all items except for the first registration instructions
        UltrasoundProbe.SetActive(false);
        Arm.SetActive(false);
        ButtonStart.SetActive(true);
        ButtomArmRegistration1.SetActive(false);
        ButtomArmRegistration2.SetActive(false);
        ButtomArmRegistration3.SetActive(false);
        ButtomArmRegistration4.SetActive(false);
        ButtomArmRegistration1completed.SetActive(false);
        ButtomArmRegistration2completed.SetActive(false);
        ButtomArmRegistration3completed.SetActive(false);
        ButtomArmRegistration4completed.SetActive(false);
        ButtomArmGO2.SetActive(false);
        ButtonList.SetActive(false);
        Ultrasound.SetActive(false);
        VideoScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        

    }
}
