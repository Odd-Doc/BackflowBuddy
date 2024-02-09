using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using com.zibra.liquid.Manipulators;
using com.zibra.liquid.Solver;
using JetBrains.Annotations;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RpzTestKitController : MonoBehaviour
{
    public WaterController waterController;
    public PlayerController playerController;
    public ShutOffValveController shutOffValveController;
    public TestCockController testCockController;
    public CheckValveStatus checkValveStatus;
    public Animator openKnobAnimation;

    public ZibraLiquid liquid;
    public GameObject highBleed;
    public GameObject highControl;
    public GameObject lowBleed;
    public GameObject lowControl;
    public GameObject bypassControl;
    public PressureZoneHUDController pressureZoneHUDController;

    public GameObject needle;
    public GameObject digitalKitNeedle;
    public GameObject currentKnob;

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
    ZibraLiquidEmitter bleederHoseEmitter;
    [SerializeField]
    ZibraLiquidDetector LowHoseDetector;

    [SerializeField]
    ZibraLiquidDetector HighHoseDetector;

    [SerializeField]
    ZibraLiquidDetector BypassHoseDetector;

    [SerializeField]
    ZibraLiquidDetector Zone1Detector;
    [SerializeField]
    ZibraLiquidDetector Zone2Detector;
    [SerializeField]
    ZibraLiquidDetector Zone3Detector;
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
    public float MinNeedle_rotation = 135;
    public float MaxNeedle_rotation = -135;
    public float hosePressure;
    public float maxPSID;
    private const float MinKnob_rotation = 0;

    //limit knobs to 4 complete rotations (x1 rotation = 360;)->
    private const float MaxKnob_rotation = 1440;
    private float currentKnobRotCount;

    private float currentKnobRotation;
    private float maxKnobRotation;
    private float minKnobRotation;

    [SerializeField]
    private float currentPSID;

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

    public float highHosePressure;
    float bypasshosePressure;
    float needleSpeedDamp = 0.005f;
    public float knobRotationFactor = 0;

    Coroutine KnobClickOperate;

    Coroutine Check1ClosingPoint;
    float needleVelRef = 0;
    public bool knobOpened = false;

    //ui toolkit
    public UIDocument _root;
    //public VisualElement _root;
    // private VisualElement _gaugeProgressBar;
    // private Length MinFillPos = Length.Percent(0);
    // private Length MaxFillPos = Length.Percent(100);
    private float MinFillPos = 0;
    private float MaxFillPos = 100;
    public float knobRotation;

    public float needleRiseSpeed = 0.25f;
    public List<GameObject> StaticTestCockList;
    // public List<GameObject> TestCockList;
    public List<GameObject> AttachedTestCockList;
    public List<GameObject> AttachedHoseList;
    int rot = 0;
    void OnEnable()
    {


        Actions.onTestCock1Opened += TestCock1Opened;
        Actions.onTestCock1Closed += TestCock1Closed;
        Actions.onTestCock2Opened += TestCoc2Opened;
        Actions.onTestCock2Closed += TestCock2Closed;
        Actions.onTestCock3Opened += TestCock3Opened;
        Actions.onTestCock3Closed += TestCock3Closed;
        Actions.onTestCock4Opened += TestCoc4Opened;
        Actions.onTestCock4Closed += TestCock4Closed;
        Actions.onAddTestCockToList += AddTestCockToList;
        Actions.onRemoveTestCockFromList += RemoveTestCockFromList;
        Actions.onAddHoseToList += AddHoseToList;
        Actions.onRemoveHoseFromList += RemoveHoseFromList;



    }


    void OnDisable()
    {

        Actions.onTestCock1Opened -= TestCock1Opened;
        Actions.onTestCock1Closed -= TestCock1Closed;
        Actions.onTestCock2Opened -= TestCoc2Opened;
        Actions.onTestCock2Opened -= TestCock2Closed;
        Actions.onTestCock3Opened -= TestCock3Opened;
        Actions.onTestCock3Closed -= TestCock3Closed;
        Actions.onTestCock4Opened -= TestCoc4Opened;
        Actions.onTestCock4Closed -= TestCock4Closed;
        Actions.onAddTestCockToList -= AddTestCockToList;
        Actions.onRemoveTestCockFromList -= RemoveTestCockFromList;
        Actions.onAddHoseToList -= AddHoseToList;
        Actions.onRemoveHoseFromList -= RemoveHoseFromList;


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
        // _gaugeProgressBar = _root.rootVisualElement.Q<VisualElement>("Gauge_progress_bar");


        currentPSID = 0;
        minPSID = 0;
        maxPSID = 1;
        currentKnobRotation = 0;
        maxKnobRotation = 1440;
        minKnobRotation = 0;
        highHosePressure = 0;
        hosePressure = 0;
        initLowHosePosition = LowHose.transform.position;
        initHighHosePosition = HighHose.transform.position;
        initBypassHosePosition = BypassHose.transform.position;
    }


    private void AddHoseToList(GameObject @object, OperableComponentDescription description)
    {
        if (!AttachedHoseList.Contains(@object))
        {
            AttachedHoseList.Add(@object);
        }
    }
    private void RemoveHoseFromList(GameObject @object, OperableComponentDescription description)
    {

        if (AttachedHoseList.Contains(@object))
        {
            AttachedHoseList.Remove(@object);
        }
    }


    private void AddTestCockToList(GameObject @object, OperableComponentDescription description)
    {
        if (!AttachedTestCockList.Contains(@object))
        {
            AttachedTestCockList.Add(@object);
        }
    }

    private void RemoveTestCockFromList(GameObject @object, OperableComponentDescription description)
    {

        if (AttachedTestCockList.Contains(@object))
        {
            AttachedTestCockList.Remove(@object);
        }
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

        float normalizedPsid = hosePressure / maxPSID;

        return MinFillPos - normalizedPsid * PsiDiff;


    }


    public float GetKnobRotation()
    {
        // max - min to rotate left while increasing
        float rotationDiff = MaxKnob_rotation - MinKnob_rotation;

        float normalizedRotation = currentKnobRotation / maxKnobRotation;

        knobRotation = MinKnob_rotation + normalizedRotation * rotationDiff;

        return MinKnob_rotation + normalizedRotation * rotationDiff * knobRotationFactor;
    }

    private void OperateControls()
    {
        if (playerController.isOperableObject == true)
        {
            if (
                playerController.operableComponentDescription.partsType
                == OperableComponentDescription.PartsType.TestKitValve
            )
            {
                switch (playerController.operableComponentDescription.componentId)
                {
                    case OperableComponentDescription.ComponentId.HighBleed:
                        currentKnob = highBleed;
                        break;
                    case OperableComponentDescription.ComponentId.LowBleed:
                        currentKnob = lowBleed;
                        break;
                    case OperableComponentDescription.ComponentId.LowControl:
                        currentKnob = lowControl;
                        break;
                    case OperableComponentDescription.ComponentId.HighControl:
                        currentKnob = highControl;
                        break;
                    case OperableComponentDescription.ComponentId.BypassControl:
                        currentKnob = bypassControl;
                        break;
                    default:
                        currentKnob = null;
                        break;
                }

                if (playerController.ClickOperationEnabled == false)
                {



                    //check click operation status

                    //if disabled, use click and drag

                    currentKnobRotation +=
                        (
                            playerController.touchStart.x
                            - Camera.main.ScreenToWorldPoint(Input.mousePosition).x
                        ) / 5;


                    if (currentKnobRotation > maxKnobRotation)
                    {
                        currentKnobRotation = maxKnobRotation;
                    }
                    if (currentKnobRotation < minKnobRotation)
                    {
                        currentKnobRotation = minKnobRotation;
                    }
                    if (currentKnob != null)
                        currentKnob.transform.eulerAngles = new Vector3(0, 0, GetKnobRotation());
                }
                //if enabled, spin to max rotation
                else if (playerController.ClickOperationEnabled == true)
                {
                    if (bleederHoseEmitter.VolumePerSimTime > 0)
                    {

                        KnobClickOperate = StartCoroutine(RotateKnobOpen(currentKnob, new Vector3(0, 0, 180)));
                    }
                    else
                    {
                        KnobClickOperate = StartCoroutine(RotateKnobClosed(currentKnob, new Vector3(0, 0, 180)));


                    }


                }

            }


        }
    }

    IEnumerator RotateKnobOpen(GameObject obj, Vector3 targetRotation)
    {
        Debug.Log($"{obj} rotated OPEN");
        float timeLerped = 0.0f;

        while (timeLerped < 1.0)
        {
            timeLerped += Time.deltaTime;
            obj.transform.eulerAngles = Vector3.Lerp(Vector3.zero, targetRotation, timeLerped) * 10;
            yield return null;
        }
    }
    IEnumerator RotateKnobClosed(GameObject obj, Vector3 targetRotation)
    {
        Debug.Log($"{obj} rotated CLOSED");
        float timeLerped = 0.0f;
        knobOpened = true;
        while (timeLerped < 1.0)
        {
            timeLerped += Time.deltaTime;
            obj.transform.eulerAngles = -Vector3.Lerp(Vector3.zero, targetRotation, timeLerped) * 10;
            yield return null;
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


    private void NeedleControl()
    {
        // For  now, soely using high hose (double check assembly)
        needle.transform.eulerAngles = new Vector3(0, 0, GetPsidNeedleRotation());
    }
    private void DigitalNeedleControl()
    {
        // _gaugeProgressBar.style.width = Length.Percent(GetPsidDigitalNeedle());
    }
    private void PressureControl()
    {



        if (isConnectedToAssembly == true)
        {
            //========================================
            // Begin Test Procedures//========================>
            //========================================


            //========================================
            // Relief Valave Opening Point//========================>
            //========================================


            //========================================
            // END - Relief Valave Opening Point//==================>
            //========================================   


            //========================================
            // Check Valve #2//========================>
            //========================================


            //========================================
            // END - Check Valve #2//==================>
            //========================================


            //========================================
            // Check Valve #1//========================>
            //========================================


            //========================================
            // END - Check Valve #1//==================>
            //========================================






            //========================================
            // End Test Procedures//========================>
            //========================================

            /*
                        //========================================
                        // #1 Check Test//========================>
                        //========================================

                        if (
                            AttachedHoseList.Contains(HighHose)
                            && shutOffValveController.IsSupplyOn == true
                            && isTestCock2Open
                            && TestCock2.GetComponent<HoseDetector>().currentHoseConnection == HighHose
                            && !isTestCock3Open
                        )
                        {
                            //maxed out psid (needle pinned out)
                            hosePressure = Mathf.SmoothStep(
                                hosePressure,
                                maxPSID,
                                needleRiseSpeed
                            );

                        }
                        else if (
                            AttachedHoseList.Contains(HighHose)
                            && isTestCock2Open
                            && isTestCock3Open
                            && waterController.isDeviceInStaticCondition == true
                        )
                        {
                            //best looking psid drop so far is: hosePressure -= 0.3f;
                            // differnce ratio between windows to mac = 1:15
                            //Windows----------------

                            // if (liquid.UseFixedTimestep == true)
                            // {
                            //     hosePressure -= 0.04f;
                            // }
                            // //!Windows----------------
                            // else
                            // {
                            //     hosePressure -= 0.65f;
                            // }

                            hosePressure = Mathf.SmoothStep(
                              hosePressure,
                              waterController.zone1to2PsiDiff,
                              0.1f
                          );




                        }
                        else if (
                            AttachedHoseList.Contains(HighHose)
                            && isTestCock2Open
                            && isTestCock3Open
                            && shutOffValveController.IsSupplyOn == false

                        )
                        {
                            hosePressure += 0;

                        }
                        //========================================
                        // END - #1 Check Test//==================>
                        //========================================
                        //========================================
                        // #2 Check Test//========================>
                        //========================================

                        if (
                            AttachedHoseList.Contains(HighHose)
                            && shutOffValveController.IsSupplyOn == true
                            && isTestCock3Open
                            && TestCock3.GetComponent<HoseDetector>().currentHoseConnection == HighHose
                            && !isTestCock4Open
                        )
                        {

                            //maxed out psid (needle pinned out)
                            hosePressure = Mathf.SmoothStep(
                                hosePressure,
                                maxPSID,
                                needleRiseSpeed
                            );

                        }
                        else if (
                            AttachedHoseList.Contains(HighHose)
                            && isTestCock3Open
                            && isTestCock4Open
                            && waterController.isDeviceInStaticCondition == true
                        )
                        {
                            //best looking psid drop so far is: hosePressure -= 0.3f;
                            // differnce ratio between windows to mac = 1:15
                            //Windows----------------

                            // if (liquid.UseFixedTimestep == true)
                            // {
                            //     hosePressure -= 0.04f;
                            // }
                            // //!Windows----------------
                            // else
                            // {
                            //     hosePressure -= 0.65f;
                            // }

                            hosePressure = Mathf.SmoothStep(
                              hosePressure,
                              waterController.zone2to3PsiDiff,
                              0.1f
                          );




                        }
                        else if (
                            AttachedHoseList.Contains(HighHose)
                            && isTestCock3Open
                            && isTestCock4Open
                            && shutOffValveController.IsSupplyOn == false

                        )
                        {
                            hosePressure += 0;

                        }
                        //========================================
                        // END - #2 Check Test//==================>
                        //========================================
                         */
        }

        //if hose is disconnected, drop pressure on gauge
        if (!AttachedHoseList.Contains(HighHose))
        {
            // hosePressure -= 5;
            hosePressure = Mathf.SmoothStep(
              hosePressure,
              0,
              0.5f
          );
        }
        if (hosePressure <= minPSID)
        {
            hosePressure = minPSID;
        }
        if (hosePressure > maxPSID)
        {
            hosePressure = maxPSID;
        }

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

        if (AttachedHoseList.Count > 0)
        {
            isConnectedToAssembly = true;
        }
        else
        {
            isConnectedToAssembly = false;
        }
        PressureControl();
        OperateControls();
        NeedleControl();
        DigitalNeedleControl();
        knobRotation = highBleed.transform.eulerAngles.z;

    }


}

