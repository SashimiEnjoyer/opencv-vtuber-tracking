using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OpenCVForUnity.UnityUtils.Helper;
using CVVTuber;
using UnityEngine.UI;
using Leap.Unity;
using RootMotion.FinalIK;
using CVVTuberExample;

public class UIManager : CVVTuberProcess
{
    [Header("Script References")]
    [SerializeField] WebCamTextureToMatHelper WebCamTextureToMatHelper;
    [SerializeField] CameraTouchController cameraController;
    [SerializeField] DlibFaceLandmarkGetter faceLandmarkGetter;
    [SerializeField] HandModelManager handModelManager;
    [SerializeField] HeadRotationController posRotManager;
    [SerializeField] FaceBlendShapeController faceController;

    [Header("UI")]
    [SerializeField] TMP_Dropdown dropdownListOfCamera;
    [SerializeField] TMP_Dropdown dropdownLeapServices;
    [SerializeField] Toggle toggleIsFaceTracking;
    [SerializeField] Toggle toggleActiveLeap;
    [SerializeField] Toggle toggleWebCamView;
    [SerializeField] Toggle toggleMouseInput;
   // [SerializeField] Toggle toggleUseHeadMove;
    [SerializeField] Slider sliderLeapZOffset;
    [SerializeField] Slider sliderLeapYOffset;
    [SerializeField] Slider sliderHeadPosSmoothing;
    [SerializeField] Slider sliderHeadRotSmoothing;
    [SerializeField] Slider sliderEyeSmoothing;
    [SerializeField] Slider sliderMouthSmoothing;
    [SerializeField] Slider OffsetPosX;
    [SerializeField] Slider OffsetPosY;
    [SerializeField] Slider OffsetPosZ;


    [Header("Other")]
    [SerializeField] Transform handOffset;
    [SerializeField] LeapServiceProvider leapServiceProvider;
    [SerializeField] LeapXRServiceProvider leapXRServiceProvider;
    [SerializeField] GameObject UI;
    [SerializeField] List<string> listOfCamera = new List<string>();

    public Leap.Controller leapController;

    Vector3 handOffsetDefaultPos;
    Vector3 offsetHeadRef;
    int leapProviderIndex = 0;
    bool isLeapActive;

    private void Start()
    {
        toggleMouseInput.onValueChanged.AddListener(EnableDisableMouseInput);
        toggleActiveLeap.onValueChanged.AddListener(EnableDisableLeap);
        toggleWebCamView.onValueChanged.AddListener(EnableDisableWebCamView);
        toggleIsFaceTracking.onValueChanged.AddListener(SetActiveFaceTracking);
        //toggleUseHeadMove.onValueChanged.AddListener(EnableDisableHeadPosMovement);
        dropdownListOfCamera.onValueChanged.AddListener(SetCameraIndex);
        dropdownLeapServices.onValueChanged.AddListener(SetLeapProvider);
        sliderLeapZOffset.onValueChanged.AddListener(SetPosZLeap);
        sliderLeapYOffset.onValueChanged.AddListener(SetPosYLeap);
        sliderHeadPosSmoothing.onValueChanged.AddListener(SetHeadPosSmoothing);
        sliderHeadRotSmoothing.onValueChanged.AddListener(SetHeadRotSmoothing);
        sliderEyeSmoothing.onValueChanged.AddListener(SetEyeSmoothing);
        sliderMouthSmoothing.onValueChanged.AddListener(SetMouthSmoothing);
        OffsetPosX.onValueChanged.AddListener(SetOffsetXHead);
        OffsetPosY.onValueChanged.AddListener(SetOffsetYHead);
        OffsetPosZ.onValueChanged.AddListener(SetOffsetZHead);

    }

    public override string GetDescription()
    {
        return "Set up main UI";
    }

    // Start is called before the first frame update
    public override void Setup()
    {
        if(dropdownListOfCamera.options != null)
            dropdownListOfCamera.ClearOptions();

        for(int i = 0; i < WebCamTextureToMatHelper.devices.Length + 1; i++)
        {
            if (i == 0)
                listOfCamera.Add("--Choose Camera--");
            else
                listOfCamera.Add(WebCamTextureToMatHelper.devices[i - 1].name);
        }
        
        dropdownListOfCamera.AddOptions(listOfCamera);

        handOffsetDefaultPos = handOffset.position;

        SetLeapController(leapProviderIndex);

        //leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_ALLOW_PAUSE_RESUME);

        //Disable some feature on start app
        toggleActiveLeap.isOn = false;
        isLeapActive = toggleActiveLeap.isOn;
        EnableDisableLeap(toggleActiveLeap.isOn);

        if (toggleWebCamView != null)
        {
            toggleWebCamView.isOn = false;
        }

        //EnableDisableWebCamView(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.SetActive(UI.activeInHierarchy ? false : true);
        }
    }
    
    void SetOffsetXHead(float _value)
    {
        offsetHeadRef.x = _value;
        posRotManager.offset = offsetHeadRef;
    }

    void SetOffsetYHead(float _value)
    {
        offsetHeadRef.y = _value;
        posRotManager.offset = offsetHeadRef;
    }
    void SetOffsetZHead(float _value)
    {
        offsetHeadRef.z = _value;
        posRotManager.offset = offsetHeadRef;
    }

    public void SetCameraIndex(int _camIndex)
    {
        if (_camIndex == 0 || listOfCamera[_camIndex].Contains("Leap"))
            return;

        StartCoroutine(WebCamTextureToMatHelper.SetWebCam(_camIndex - 1));
    }

    public void SetLeapProvider(int _providerIndex)
    {
        //if (!isLeapActive)
        //    return;

        //Set index for global variable
        leapProviderIndex = _providerIndex;

        switch (_providerIndex)
        {
            case 0:

                //Set active service provider game object needed
                if (!leapServiceProvider.gameObject.activeInHierarchy)
                {
                    leapServiceProvider.gameObject.SetActive(true);
                    leapXRServiceProvider.gameObject.SetActive(false);
                }

                //Set new leap controller
                SetLeapController(leapProviderIndex);

                //Assign new leap provider to Hand Model Manager Script
                handModelManager.leapProvider = leapServiceProvider;

                leapServiceProvider.ChangeTrackingMode(LeapServiceProvider.TrackingOptimizationMode.Desktop);
                break;

            case 1:

                leapServiceProvider.CopySettingsToLeapXRServiceProvider(leapXRServiceProvider);

                //Set active service provider game object needed
                if (!leapXRServiceProvider.gameObject.activeInHierarchy)
                {
                    leapXRServiceProvider.gameObject.SetActive(true);
                    leapServiceProvider.gameObject.SetActive(false);
                }

                //Set new leap controller
                SetLeapController(leapProviderIndex);

                //Assign new leap provider to Hand Model Manager Script
                handModelManager.leapProvider = leapXRServiceProvider;

                leapServiceProvider.ChangeTrackingMode(LeapServiceProvider.TrackingOptimizationMode.HMD);
                break;
        }
        
        
    }

    

    public void SetPosZLeap(float _offset)
    {
        handOffset.position = new Vector3(handOffset.position.x, handOffset.position.y, handOffsetDefaultPos.z + _offset);
    }

    public void SetPosYLeap(float _offset)
    {
        handOffset.position = new Vector3(handOffset.position.x, handOffsetDefaultPos.y + _offset, handOffset.position.z);
    }

    public void EnableDisableLeap(bool _isActive)
    {
        if (leapController == null)
            return;

        isLeapActive = _isActive;

        if (_isActive)
        {
            leapController.StartConnection();
        }
        else
        {
            leapController.StopConnection();
        }
    }

    public void EnableDisableMouseInput(bool _isActive)
    {
        cameraController.enabled = _isActive;
    }

    public void EnableDisableWebCamView(bool _isActive)
    {
        faceLandmarkGetter.isDebugMode = _isActive;
    }

    //public void EnableDisableHeadPosMovement(bool _isActive)
    //{
    //    posRotManager.canMovePos = _isActive;
    //}

    public void EnableDisableDefaultPos(bool _isActive)
    {

    }

    void SetActiveFaceTracking(bool _isActive)
    {
        FaceLandmarkConnector.instance.isFaceTracking = _isActive;
    }

    public void SetMouthSmoothing(float _value)
    {
        faceController.mouthSmoothing = _value;
    }

    public void SetEyeSmoothing(float _value)
    {
        faceController.eyeSmoothing = _value;
    }

    public void SetHeadPosSmoothing(float _value)
    {
        posRotManager.positionSmoothing = _value;
    }

    public void SetHeadRotSmoothing(float _value)
    {
        posRotManager.rotationSmoothing = _value;
    }

    void SetLeapController(int _index)
    {
        switch (_index)
        {
            case 0:
                leapController = leapServiceProvider.GetLeapController();
                break;
            case 1:
                leapController = leapXRServiceProvider.GetLeapController();
                break;
        }
    }
}
