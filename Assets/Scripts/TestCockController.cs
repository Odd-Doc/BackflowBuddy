using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Manipulators;

public class TestCockController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    GameObject playerManager;

    [SerializeField]
    GameObject TestCockValve1;

    [SerializeField]
    GameObject TestCockValve2;

    [SerializeField]
    GameObject TestCockValve3;

    [SerializeField]
    GameObject TestCockValve4;

    GameObject _operableTestCockValve;
    GameObject _operatingTestCock;

    Vector3 _operableTestCockValveScale;

    [SerializeField]
    ZibraLiquidEmitter TestCockEmitter1;

    [SerializeField]
    ZibraLiquidEmitter TestCockEmitter2;

    [SerializeField]
    ZibraLiquidEmitter TestCockEmitter3;

    [SerializeField]
    ZibraLiquidEmitter TestCockEmitter4;

    [SerializeField]
    ZibraLiquidDetector TestCockDetector1;

    [SerializeField]
    ZibraLiquidDetector TestCockDetector2;

    [SerializeField]
    ZibraLiquidDetector TestCockDetector3;

    [SerializeField]
    ZibraLiquidDetector TestCockDetector4;

    [SerializeField]
    ZibraLiquidForceField TestCockFF1;

    [SerializeField]
    ZibraLiquidForceField TestCockFF2;

    [SerializeField]
    ZibraLiquidForceField TestCockFF3;

    [SerializeField]
    ZibraLiquidForceField TestCockFF4;

    OperableComponentDescription operableComponentDescription;
    ZibraLiquidForceField _operableTestCockFF;
    Vector3 _startingTestCockValveScale;
    float testCockClosedYScale;
    public float TestCockValveScaleFactor
    {
        get { return _testCockValveScaleFactor; }
        private set { _testCockValveScaleFactor = value; }
    }
    float _testCockValveScaleFactor;

    public float testCockOpenYScale = 0.005f;

    //Vector3 testCockClosedScale = new Vector3(0.00886263326f,0.0028f,0.000265074486f);
    Vector3 testCockClosedScale;
    public bool TestCockOpenCheck { get; private set; } = false;
    public ZibraLiquidForceField OperableTestCockFF
    {
        get { return _operableTestCockFF; }
        set { _operableTestCockFF = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = playerManager.GetComponent<PlayerController>();
    }

    private void TestCockValveOperationCheck()
    {
            operableComponentDescription =
                playerController.OperableObject.GetComponent<OperableComponentDescription>();
        
        if (playerController.OperableObject.tag == "TestCock")
        {
            switch (operableComponentDescription.componentId)
            {
                case OperableComponentDescription.ComponentId.TestCock1:

                    _operableTestCockValve = TestCockValve1;
                    _operableTestCockFF = TestCockFF1;

                    break;
                case OperableComponentDescription.ComponentId.TestCock2:
                    _operableTestCockValve = TestCockValve2;
                    _operableTestCockFF = TestCockFF2;
                    break;
                case OperableComponentDescription.ComponentId.TestCock3:
                    _operableTestCockValve = TestCockValve3;
                    _operableTestCockFF = TestCockFF3;
                    break;
                case OperableComponentDescription.ComponentId.TestCock4:
                    _operableTestCockValve = TestCockValve4;
                    _operableTestCockFF = TestCockFF4;
                    break;
            }
        }

        //assign the associated test cock valve game object to currently operating test cock;

        _testCockValveScaleFactor = (playerController.OperableObjectRotation.z * 0.01f) + 0.1f;
        _operableTestCockValveScale = _operableTestCockValve.transform.localScale;

        _operableTestCockValveScale.y = Mathf.Lerp(
            testCockClosedYScale,
            testCockOpenYScale,
            _testCockValveScaleFactor
        );
        _operableTestCockValve.transform.localScale = _operableTestCockValveScale;

        /// <summary>
        /// Might revisit this to scale with checkvalve movement for a more realistic
        /// operation (ie. test cocks will not open if the psi upstream is not strong enough to open upstream checkvalve)
        /// </summary>


        //testcock valve should still be closable (via Void) while emitter volume scaled with supply volume

        // manipulate forcefield on test cock


        //_operableTestCockFF.Strength = Mathf.Lerp(0, 1f, testCockValveScaleFactor);
    }

    // Update is called once per frame
    void Update()
    {
        TestCockValveOperationCheck();
    }
}
