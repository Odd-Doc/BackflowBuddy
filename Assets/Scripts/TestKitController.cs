using System;
using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Manipulators;
using UnityEngine;
using UnityEngine.UIElements;

public class TestKitController : MonoBehaviour
{
    public WaterController waterController;
    public PlayerController playerController;
    public ShutOffValveController shutOffValveController;
    public TestCockController testCockController;
    public CheckValveStatus checkValveStatus;
    public GameObject lowBleed;
    public GameObject lowControl;

    public GameObject highBleed;
    public GameObject highControl;

    public GameObject bypassControl;

    public GameObject needle;
    public GameObject digitalKitNeedle;
    private GameObject currentKnob;

    [SerializeField]
    GameObject LowHose;

    [SerializeField]
    GameObject HighHose;

    [SerializeField]
    GameObject BypassHose;

    [SerializeField]
    GameObject Check1;

    [SerializeField]
    Vector3 Check1Pos;

    Vector3 initLowHosePosition;
    Vector3 initHighHosePosition;
    Vector3 initBypassHosePosition;

    [SerializeField]
    ZibraLiquidDetector LowHoseDetector;

    [SerializeField]
    ZibraLiquidDetector HighHoseDetector;

    [SerializeField]
    ZibraLiquidDetector BypassHoseDetector;

    [SerializeField]
    ZibraLiquidDetector Zone1Detector;

    [SerializeField]
    GameObject TestCock1;

    [SerializeField]
    GameObject TestCock2;

    [SerializeField]
    GameObject TestCock3;

    [SerializeField]
    GameObject TestCock4;

    [SerializeField]
    GameObject connectedTestCock;

    [SerializeField]
    ZibraLiquidDetector TestCock2Detector;

    // private const float MinNeedle_rotation = 55;
    // private const float MaxNeedle_rotation = -55;
    private const float MinNeedle_rotation = 61;
    private const float MaxNeedle_rotation = -58;
    private const float MinKnob_rotation = 0;

    //limit knobs to 4 complete rotations (x1 rotation = 360;)->
    private const float MaxKnob_rotation = 1440;
    private float currentKnobRotCount;

    private float currentKnobRotation;
    private float maxKnobRotation;
    private float minKnobRotation;

    [SerializeField]
    private float currentPSID;
    private float maxPSID;
    private float minPSID;
    float closingPoint = 0;
    public bool isOperableObject;
    public bool isConnectedToAssembly;
    public bool isCheck1Open;
    public bool isCheck2Open;
    float lowHosePressure;

    public bool isConnectedTestCockOpen;
    public bool isTestCock1Open;
    public bool isTestCock2Open;
    public bool isTestCock3Open;
    public bool isTestCock4Open;
    public float hosePressure;
    public float highHosePressure;
    float bypasshosePressure;
    float needleSpeedDamp = 0.005f;

    [SerializeField]
    public List<GameObject> TestCockList;

    [SerializeField]
    public List<ZibraLiquidDetector> TestCockDetectorList;
    Coroutine Check1ClosingPoint;
    float needleVelRef = 0;

    //ui toolkit
    public UIDocument _root;
    //public VisualElement _root;
    private VisualElement _gaugeProgressBar;
    // private Length MinFillPos = Length.Percent(0);
    // private Length MaxFillPos = Length.Percent(100);
    private float MinFillPos = 0;
    private float MaxFillPos = 100;


    void OnEnable()
    {
        Actions.onHoseAttach += AttachHoseBib;
        Actions.onHoseDetach += DetachHoseBib;
        Actions.onTestCock1Opened += TestCock1Opened;
        Actions.onTestCock1Closed += TestCock1Closed;
        Actions.onTestCock2Opened += TestCoc2Opened;
        Actions.onTestCock2Closed += TestCock2Closed;
        Actions.onTestCock3Opened += TestCock3Opened;
        Actions.onTestCock3Closed += TestCock3Closed;
        Actions.onTestCock4Opened += TestCoc4Opened;
        Actions.onTestCock4Closed += TestCock4Closed;

        //Actions.onTestCockOpen += DetectTestCockOpen;
    }

    void OnDisable()
    {
        Actions.onHoseAttach -= AttachHoseBib;
        Actions.onHoseDetach -= DetachHoseBib;
        Actions.onTestCock1Opened -= TestCock1Opened;
        Actions.onTestCock1Closed -= TestCock1Closed;
        Actions.onTestCock2Opened -= TestCoc2Opened;
        Actions.onTestCock2Opened -= TestCock2Closed;
        Actions.onTestCock3Opened -= TestCock3Opened;
        Actions.onTestCock3Closed -= TestCock3Closed;
        Actions.onTestCock4Opened -= TestCoc4Opened;
        Actions.onTestCock4Closed -= TestCock4Closed;

        //Actions.onTestCockOpen -= DetectTestCockOpen;
    }

    // Start is called before the first frame update
    void Start()
    {

        /// <summary>
        /// //ui tool kit for digital gauge
        /// </summary>
        /// <typeparam name="UIDocument"></typeparam>
        /// <returns></returns>
       // _root = GetComponent<UIDocument>().rootVisualElement;
        _gaugeProgressBar = _root.rootVisualElement.Q<VisualElement>("Gauge_progress_bar");


        currentPSID = 0;
        minPSID = 0;
        maxPSID = 55;
        currentKnobRotation = 0;
        maxKnobRotation = 1440;
        minKnobRotation = 0;
        highHosePressure = 0;
        hosePressure = 0;
        initLowHosePosition = LowHose.transform.position;
        initHighHosePosition = HighHose.transform.position;
        initBypassHosePosition = BypassHose.transform.position;
    }

    private float GetPsidNeedleRotation()
    {
        float PsidDiff = MinNeedle_rotation - MaxNeedle_rotation;

        float normalizedPsid = hosePressure / maxPSID;

        return MinNeedle_rotation - normalizedPsid * PsidDiff;
    }
    private float GetPsidDigitalNeedle()
    {
        float PsiDiff = MinFillPos - MaxFillPos;
        float percentChange = hosePressure;
        float normalizedPsid = hosePressure / maxPSID;

        return MinFillPos - normalizedPsid * PsiDiff;
        // Debug.Log($"hosePressure = {hosePressure}|| progressBar = {_gaugeProgressBar.style.width} || MinFillPos - percentChange = {MinFillPos - percentChange}");

    }


    private float GetKnobRotation()
    {
        // max - min to rotate left while increasing
        float rotationDiff = MaxKnob_rotation - MinKnob_rotation;

        float normalizedRotation = currentKnobRotation / maxKnobRotation;

        //return MinKnob_rotation + normalizedRotation * rotationDiff;
        return MinKnob_rotation + normalizedRotation * rotationDiff;
    }

    private void OperateControls()
    {
        if (playerController.isOperableObject == true)
        {
            if (
                playerController.operableComponentDescription.partsType
                == OperableComponentDescription.PartsType.TestKitValve
            )
                currentKnob = playerController.OperableTestGaugeObject;

            currentKnobRotation +=
                (
                    playerController.touchStart.x
                    - Camera.main.ScreenToWorldPoint(Input.mousePosition).x
                ) / 5;

            //currentKnobRotation += 1 * Time.deltaTime;

            if (currentKnobRotation > maxKnobRotation)
            {
                currentKnobRotation = maxKnobRotation;
            }
            if (currentKnobRotation < minKnobRotation)
            {
                currentKnobRotation = minKnobRotation;
            }
            if (currentKnob != null)
            {
                currentKnob.transform.eulerAngles = new Vector3(0, 0, GetKnobRotation());
            }
        }
    }

    private void TestCock4Closed()
    {
        isTestCock4Open = false;
    }

    private void TestCoc4Opened()
    {
        isTestCock4Open = true;
    }

    private void TestCock3Closed()
    {
        isTestCock3Open = false;
    }

    private void TestCock3Opened()
    {
        isTestCock3Open = true;
    }

    private void TestCock2Closed()
    {
        isTestCock2Open = false;
    }

    private void TestCoc2Opened()
    {
        isTestCock2Open = true;
    }

    private void TestCock1Closed()
    {
        isTestCock1Open = false;
    }

    private void TestCock1Opened()
    {
        isTestCock1Open = true;
    }

    public void AttachHoseBib(GameObject testCock, OperableComponentDescription description)
    {
        isConnectedToAssembly = true;

        if (TestCockList.Contains(testCock) != true)
        {
            TestCockList.Add(testCock);
        }
        if (
            TestCockDetectorList.Contains(testCock.GetComponentInChildren<ZibraLiquidDetector>())
            != true
        )
        {
            TestCockDetectorList.Add(testCock.GetComponentInChildren<ZibraLiquidDetector>());
        }
        //  Debug.Log($"{gameObject} attached to assembly");
    }

    public void DetachHoseBib(GameObject testCock, OperableComponentDescription description)
    {
        isConnectedToAssembly = false;

        TestCockList.Remove(testCock);

        //Debug.Log($"{gameObject} detached from assembly");
    }

    private void NeedleControl()
    {
        // For  now, soely using high hose (double check assembly)
        needle.transform.eulerAngles = new Vector3(0, 0, GetPsidNeedleRotation());
    }
    private void DigitalNeedleControl()
    {
        _gaugeProgressBar.style.width = Length.Percent(GetPsidDigitalNeedle());
    }
    private void PressureControl()
    {
        // For  now, soely using high hose (double check assembly testing)


        if (isConnectedToAssembly == true)
        {
            //checking if hose/ test kit is connected to test cock while supply is open

            //test cock #1 will have continuous pressure whether the supply is open or closed, as it sits upstream of #1 shut off valve
            if (TestCockList.Contains(TestCock1) && isTestCock1Open)
            {
                //supply is open and test cock is open
                hosePressure = Mathf.SmoothStep(
                    hosePressure,
                    Zone1Detector.ParticlesInside,
                    needleSpeedDamp
                );
                // Debug.Log($"test cock #1 is connected & open");
            }

            else if (
                TestCockList.Contains(TestCock3)
                && shutOffValveController.IsSupplyOn == true
                && isTestCock3Open
            )
            {
                //supply is open and test cock is open
                hosePressure = Mathf.SmoothStep(
                    hosePressure,
                    Zone1Detector.ParticlesInside,
                    needleSpeedDamp
                );
                // Debug.Log($"supply is open and test cock 3 is connected & open");
            }
            else if (
                TestCockList.Contains(TestCock4)
                && shutOffValveController.IsSupplyOn == true
                && isTestCock4Open
            )
            {
                //supply is open and test cock is open
                hosePressure = Mathf.SmoothStep(
                    hosePressure,
                    Zone1Detector.ParticlesInside,
                    needleSpeedDamp
                );
                // Debug.Log($"supply is open and test cock 4 is connected & open");
            }
            //END CHECKING IS TC IS HOOKED UP TO MOVE GAUGE WHILE DEVICE IS OPEN




            //========================================
            // #1 Check Test//========================>
            //========================================

            else if (
                TestCockList.Contains(TestCock2)
                && shutOffValveController.IsSupplyOn == true
                && isTestCock2Open
                && !isTestCock3Open
            )
            {
                hosePressure = Mathf.SmoothStep(
                    hosePressure,
                    TestCock2Detector.ParticlesInside,
                    0.015f
                );
                // Debug.Log(
                //     $"supply is open & test cock #2 is connected & open & test cock #3 is closed"
                // );
            }
            else if (
                TestCockList.Contains(TestCock2)
                && isTestCock2Open
                && isTestCock3Open
                && shutOffValveController.IsSupplyOn == false
                && checkValveStatus.isCheck1Closed == false
            )
            {
                //best looking psid drop so far is: hosePressure -= 0.3f;
                hosePressure -= 0.5f;

                // Debug.Log($"hosePressure = {hosePressure}");

                // Debug.Log(
                //     $"supply is closed & check1 is open & test cock #2 is connected & open & test cock #3 is open"
                // );
            }
            else if (
                TestCockList.Contains(TestCock2)
                && isTestCock2Open
                && isTestCock3Open
                && shutOffValveController.IsSupplyOn == false
                && checkValveStatus.isCheck1Closed == true
            )
            {
                hosePressure += 0;
                //CaptureCheck1ClosingPoint(hosePressure);

                //Debug.Log($"closingPoint = {closingPoint}");
            }
            //========================================
            // END - #1 Check Test//==================>
            //========================================
            //========================================
            // #2 Check Test//========================>
            //========================================
            else if (
                TestCockList.Contains(TestCock3)
                && shutOffValveController.IsSupplyOn == true
                && isTestCock3Open
                && !isTestCock4Open
            )
            {
                hosePressure = Mathf.SmoothStep(
                    hosePressure,
                    TestCock2Detector.ParticlesInside,
                    0.015f
                );
                // Debug.Log(
                //     $"supply is open & test cock #3 is connected & open & test cock #4 is closed"
                // );
            }
            else if (
                TestCockList.Contains(TestCock3)
                && isTestCock3Open
                && isTestCock4Open
                && shutOffValveController.IsSupplyOn == false
                && checkValveStatus.isCheck2Closed == false
            )
            {
                //best looking psid drop so far is: hosePressure -= 0.3f;
                hosePressure -= 0.6f;

                // Debug.Log($"hosePressure = {hosePressure}");

                // Debug.Log(
                //     $"supply is closed & check2 is open & test cock #3 is connected & open & test cock #4 is open"
                // );
            }
            else if (
                TestCockList.Contains(TestCock3)
                && isTestCock3Open
                && isTestCock4Open
                && shutOffValveController.IsSupplyOn == false
                && checkValveStatus.isCheck2Closed == true
            )
            {
                hosePressure += 0;
                // Debug.Log(
                //     $"supply is closed & check2 is closed & test cock #3 is connected & open & test cock #4 is open"
                // );
            }
            //========================================
            // END - #2 Check Test//==================>
            //========================================
        }
        if (isConnectedToAssembly == false)
        {
            hosePressure -= 5;
        }
        if (hosePressure <= minPSID)
        {
            hosePressure = minPSID;
        }
        if (hosePressure > maxPSID)
        {
            hosePressure = maxPSID;
        }
        /*
        Debug.Log(
            $"hosePressure = {hosePressure}| GetPsidNeedleRotation() = {GetPsidNeedleRotation()}| closingPoint = {closingPoint}"
        );
        */
        // lowHosePressure = LowHoseDetector.ParticlesInside;
        // bypasshosePressure = BypassHoseDetector.ParticlesInside;
        //Debug.Log(hosePressure);
    }

    private float CaptureCheck1ClosingPoint(float psid)
    {
        return psid;
    }

    IEnumerator Check1Test()
    {
        while (true)
        {
            closingPoint += 0.1f * Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isCheck1Open = checkValveStatus.isCheck1Open;

        PressureControl();
        OperateControls();
        NeedleControl();
        DigitalNeedleControl();

    }


}
