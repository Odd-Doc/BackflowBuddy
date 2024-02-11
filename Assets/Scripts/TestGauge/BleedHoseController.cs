using System;
using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Manipulators;
using UnityEditor.Rendering;
using UnityEngine;
public class BleedHoseController : MonoBehaviour
{
    ZibraLiquidEmitter bleederHoseEmitter;
    float controlKnobRotation;
    public DoubleCheckTestKitController doubleCheckTestKitController;
    float currentFlow = 0;
    float appliedKnobRotation = 0;
    [SerializeField]
    GameObject highBleedKnob;
    public bool isHighBleedOpen;
    private bool isLowBleedOpen;

    private void OnEnable()
    {

        Actions.onHighBleedOperate += HighBleedKnobOperate;
        Actions.onLowBleedOperate += LowBleedKnobOperate;

    }



    private void OnDisable()
    {
        Actions.onHighBleedOperate -= HighBleedKnobOperate;


    }
    // Start is called before the first frame update
    void Start()
    {

        bleederHoseEmitter = GetComponentInChildren<ZibraLiquidEmitter>();

    }
    void HighBleedKnobOperate()
    {
        if (bleederHoseEmitter.VolumePerSimTime == 0)
        {
            isHighBleedOpen = true;
            bleederHoseEmitter.VolumePerSimTime = 1;
        }
        else
        {
            isHighBleedOpen = false;
            bleederHoseEmitter.VolumePerSimTime = 0;
        }
    }
    private void LowBleedKnobOperate()
    {
        if (bleederHoseEmitter.VolumePerSimTime == 0)
        {
            isLowBleedOpen = true;
            bleederHoseEmitter.VolumePerSimTime = 1;
        }
        else
        {
            isLowBleedOpen = false;
            bleederHoseEmitter.VolumePerSimTime = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
