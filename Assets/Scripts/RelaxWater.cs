using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.SDFObjects;
using com.zibra.liquid.Manipulators;
using com.zibra.liquid.DataStructures;
using com.zibra.liquid.Analytics;

public class RelaxWater : MonoBehaviour
{
    [SerializeField]
    ZibraLiquidEmitter supplyEmitter;

    [SerializeField]
    ZibraLiquidForceField checkValve1ForceField;

    [SerializeField]
    ZibraLiquidForceField checkValve2ForceField;

    [SerializeField]
    ZibraLiquidSolverParameters liquidSolverParameters;

    [SerializeField]
    ZibraLiquidForceField testCock1FF;

    [SerializeField]
    ZibraLiquidForceField testCock2FF;

    [SerializeField]
    ZibraLiquidForceField testCock3FF;

    [SerializeField]
    ZibraLiquidForceField testCock4FF;

    [SerializeField]
    ZibraLiquidForceField supplyFF;

    [SerializeField]
    ZibraLiquidVoid supplyVoid;

    float initSupplyVolume;

    [SerializeField]
    ZibraLiquidDetector testCock1Detector;

    [SerializeField]
    ZibraLiquidDetector testCock2Detector;

    [SerializeField]
    ZibraLiquidDetector testCock3Detector;

    [SerializeField]
    ZibraLiquidDetector testCock4Detector;

    [SerializeField]
    ZibraLiquidDetector zone1Detector;

    [SerializeField]
    ZibraLiquidDetector zone2Detector;

    [SerializeField]
    ZibraLiquidDetector zone3Detector;

    [SerializeField]
    ShutOffValveController shutOffValveController;

    [SerializeField]
    ZibraLiquidCollider supplyCollider;

    [SerializeField]
    ZibraLiquidSolverParameters waterMaxVelocity;

    [SerializeField]
    ConfigurableJoint check1Spring;

    [SerializeField]
    ConfigurableJoint check2Spring;

    float initialWaterMaxVelocity;
    Vector3 supplyColliderClosedPos = new Vector3(-15, -0.06f, 0.03f);
    Vector3 initSupplyColliderPos;
    Vector3 supplyColliderTargetPos;

    Vector3 initSupplyVoidScale;
    Vector3 currentSupplyVoidScale;
    Vector3 targetSupplyVoidScale;
    Vector3 supplyVoidRef = Vector3.zero;

    [Range(0, 0.00009f)]
    public float supplyVoidSurfaceDepthLerpFactor;

    [Range(0, 100000)]
    public float zone1MinCompressionThreshold;

    [Range(0, 3000000)]
    public float zone1MaxCompressionThreshold;

    [Range(0, 3000000)]
    public float zone2MinCompressionThreshold;

    [Range(0, 5000000)]
    public float zone2MaxCompressionThreshold;
    PlayerController playerController;
    public GameObject playerManager;

    public float supplyVolume;

    public float currentVelocity = 0;

    private void checkRelax()
    {
        supplyVolume = shutOffValveController.mainSupplyEmitter.VolumePerSimTime;

        //close supply end with collider if shutoff is closed, to keep current volume of water at time of shutoff (protect water from supply void)
        supplyColliderTargetPos.x =
            shutOffValveController.ShutOffValve1.transform.eulerAngles.z / 90;
        supplyCollider.transform.position = initSupplyColliderPos + supplyColliderTargetPos;

        if (shutOffValveController.IsSupplyOn == false)
        {
            supplyFF.enabled = false;
        }
        else if (shutOffValveController.IsSupplyOn == true)
        {
            supplyFF.enabled = true;
        }
        //exists only for easily peeking water velocity in inspector without finding the ZibraLiquid in hierarchy
        currentVelocity = waterMaxVelocity.MaximumVelocity;

        //Debug.Log($"supplyColliderTargetPos.z = {supplyColliderTargetPos.z / 90}");
        //Debug.Log(shutOffValveController.ShutOffValve1.transform.rotation.eulerAngles);
        /*
                if (supplyVolume <= 0 && playerController.isInit == true)
                {
                    supplyCollider.transform.position = supplyColliderClosedPos;
                }
                else if (supplyVolume > 0 && playerController.isInit == false)
                {
                    supplyCollider.transform.position = initSupplyColliderPos;
                }
                */
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = playerManager.GetComponent<PlayerController>();

        initSupplyColliderPos = supplyCollider.transform.position;
        initSupplyVolume = shutOffValveController.supplyVolume;
        initSupplyVoidScale = supplyVoid.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        checkRelax();
    }
}
