using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKitController : MonoBehaviour
{
    public WaterController waterController;
    public PlayerController playerController;
    public GameObject lowBleed;
    public GameObject lowControl;

    public GameObject highBleed;
    public GameObject highControl;

    public GameObject bypassControl;

    public GameObject needle;
    private GameObject currentKnob;
    private const float MinNeedle_rotation = 11;
    private const float MaxNeedle_rotation = -101;
    private const float MinKnob_rotation = 0;

    //limit knobs to 4 complete rotations (x1 rotation = 360;)->
    private const float MaxKnob_rotation = 1440;

    private float currentKnobRotation;
    private float maxKnobRotation;

    private float currentPSID;
    private float maxPSID;

    // Start is called before the first frame update
    void Start()
    {
        currentPSID = 0;
        maxPSID = 15;
        currentKnobRotation = 0;
        maxKnobRotation = 1440;
    }

    private float GetPsidNeedleRotation()
    {
        float PsidDiff = MinNeedle_rotation - MaxNeedle_rotation;

        float normalizedPsid = currentPSID / maxPSID;

        return MinNeedle_rotation - normalizedPsid * PsidDiff;
    }

    private float GetKnobRoation()
    {
        // max - min to rotate left while increasing
        float rotationDiff = MaxKnob_rotation - MinKnob_rotation;

        float normalizedRotation = currentKnobRotation / maxKnobRotation;

        return MinKnob_rotation + normalizedRotation * rotationDiff;
    }

    private void DetectKnob() { }

    // Update is called once per frame
    void Update()
    {
        DetectKnob();
        /*
        currentPSID += 1 * Time.deltaTime;
        if (currentPSID > maxPSID)
        {
            currentPSID = maxPSID;
        }
        needle.transform.eulerAngles = new Vector3(0, 0, GetPsidNeedleRotation());
        */
    }
}
