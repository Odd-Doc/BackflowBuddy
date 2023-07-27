using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Solver;
using com.zibra.liquid.Manipulators;

public class FillButton : MonoBehaviour
{
    public ZibraLiquid liquid;
    public GameObject Check1;
    public GameObject Check2;
    public ShutOffValveController shutOffValveController;
    public TestCockController testCockController;
    public ZibraLiquidForceField check1HousingFF;
    public ZibraLiquidForceField check2HousingFF;

    private float checkffVelref;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake() { }

    // Start is called before the first frame update
    void Start() { }

    public void FillDevice()
    {
        liquid.ReleaseSimulation();
        liquid.InitialState = ZibraLiquid.InitialStateType.BakedLiquidState;
        liquid.enabled = false;
        liquid.enabled = true;
        liquid.InitializeSimulation();

        Check1.transform.localPosition = new Vector3(-0.101f, 0, -0.08f);
        Check2.transform.localPosition = new Vector3(-0.201f, -2.25f, -0.17f);

        //shutOffValveController.ShutOffValve1.transform.eulerAngles = new Vector3(0, 0, 0);
        foreach (GameObject testCock in testCockController.TestCockList)
        {
            testCock.GetComponent<AssignTestCockManipulators>().testCockVoid.enabled = false;
            testCock.GetComponent<AssignTestCockManipulators>().testCockCollider.enabled = true;
        }
    }

    // Update is called once per frame
    void Update() { }
}
