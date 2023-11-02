using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Manipulators;
using UnityEditor.Rendering;
using UnityEngine;
public class BleedHoseController : MonoBehaviour
{
    ZibraLiquidEmitter bleederHoseEmitter;
    float controlKnobRotation;
    public TestKitController testKitController;
    float currentFlow = 0;
    float appliedKnobRotation = 0;
    [SerializeField]
    GameObject highBleedKnob;



    // Start is called before the first frame update
    void Start()
    {

        bleederHoseEmitter = GetComponentInChildren<ZibraLiquidEmitter>();

    }



    // Update is called once per frame
    void Update()
    {
        //reset cached knob rotation to not reset after rotating 180 degress--> zRot is rotating from 0 -> 180 -> -180 -> 0 -> 180..and so on
        if (testKitController.currentKnob == highBleedKnob)
        {
            appliedKnobRotation = testKitController.knobRotation;

            bleederHoseEmitter.VolumePerSimTime = appliedKnobRotation / 10000;

        }





    }
}
