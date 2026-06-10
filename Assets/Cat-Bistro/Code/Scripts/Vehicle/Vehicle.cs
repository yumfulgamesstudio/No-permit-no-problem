using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Vehicle : MonoBehaviour
{
    [Header("References")]
    // Vehicle General

        public Rigidbody rigidBody;
        [SerializeField] private float rigidBodyForceMultiplier = 1f;
    
        [SerializeField] private VehicleInteract vehicleInteract;
        

        [SerializeField] public Transform player;

        public GameObject playerCamera;
        public GameObject vehicleCamera;

        

    [Header("Inputs")]
    //Inputs

        

        public InputActionAsset playerInputActions;
        private InputAction accelerationIA;
        private InputAction steerIA;
        private InputAction brakeIA;

        private InputAction interact;

    [Header("Engine")]
    // Variables for engine power

        public float enginePower = 300f;
        public float maxEnginePower = 300f;
        public float maxSteerAngle = 32f;
        public float brakePower = 1000;

        private Vector2 steerVector;
        private Vector2 accelVector;
        
        private float currentSteerAngle = 0f;
        private float currentBrakeForce = 0f;

        private bool isBraking;
        private float brakeengaged;

    [Header("Wheels")]
    //Variable for wheels

        public WheelCollider FrontLeftWheel, FrontRightWheel, RearLeftWheel, RearRightWheel;
        public Transform FrontLeftWheelMesh , FrontRightWheelMesh, RearLeftWheelMesh, RearRightWheelMesh;
        public bool doWheelTurn;

    [Header("Interaction")]
    //Interaction locations

        [SerializeField] private Transform steeringWheel;
        [SerializeField] public bool isInVehicle = false;    
        [SerializeField] private Transform seat;
        [SerializeField] private Transform exitPoint;

    [Header("DEBUG")]
    //Debug

        public float valued = 0f;
        public bool ActionMapSwitch = false;

    #region ENABLING INPUTS



    private void Awake()
    {
        this.enabled = true;
        
        accelerationIA = InputSystem.actions.FindAction("Accelerate");
        steerIA = InputSystem.actions.FindAction("Steer");
        brakeIA = InputSystem.actions.FindAction("Brake");

        rigidBody = GetComponent<Rigidbody>();
        
        interact = InputSystem.actions.FindAction("VeInteract");

        

    }

    private void OnEnable()
    {
        

        InputSystem.actions["VeInteract"].performed += SwitchActionMap;

        
        
    }

    private void OnDisable()
    {
        InputSystem.actions["VeInteract"].performed -= SwitchActionMap;
    }

    public void SwitchActionMap(InputAction.CallbackContext context)
    {
        //ActionMapSwitch = !ActionMapSwitch;


        //if (!ActionMapSwitch)
        //{
        //    playerInputActions.FindActionMap("Vehicle").Enable();

        //}
        //else
        //{
        //    playerInputActions.FindActionMap("Vehicle").Disable();

        //}

        
        ExitVehicle();
        
    }

    

    public void StartVehicle()
    {
        if (player != null && !isInVehicle)
        {
            vehicleInteract.enabled = false;
            isInVehicle = true;

            if (this.enabled == false)
            {
                this.enabled = true;
            }

            player.transform.position = seat.transform.position;
            player.gameObject.SetActive(false);

            playerCamera.gameObject.SetActive(false);
            vehicleCamera.gameObject.SetActive(true);

            playerInputActions.FindActionMap("Vehicle").Enable();
        }
        //else if (player != null && isInVehicle)
        //{
        //    ExitVehicle();
        //}
    }

    public void ExitVehicle()
    {

        if (player != null && isInVehicle)
        {
            Debug.Log($"PERFORMED ");
            vehicleInteract.enabled = true;
            playerInputActions.FindActionMap("Vehicle").Disable();
            playerInputActions.FindActionMap("Player").Enable();

            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }


            vehicleCamera.SetActive(false);

            player.position = exitPoint.position;
            player.gameObject.SetActive(true);
            playerCamera.SetActive(true);

            isInVehicle = false;
        }
        //else
        //{
        //    isInVehicle = true ;
        //}

        



    }

    #endregion


    private void FixedUpdate()
    {
        Debug.Log($" is in  vehcile {isInVehicle} ");

        

        steerVector = steerIA.ReadValue<Vector2>();
        accelVector = accelerationIA.ReadValue<Vector2>();
        brakeengaged = brakeIA.ReadValue<float>();

        EngineMotor();
        SteeringWheel();
        UpdateWheels();


        if(brakeengaged != 0f)
        {
            isBraking = true;
        }
        else { isBraking = false;}

        
        
    }

    #region ENGINE
    private void EngineMotor()
    {
        //WheelPower
        float power = enginePower * accelVector.y;
        //FrontLeftWheel.motorTorque = power;
       // FrontRightWheel.motorTorque = power;
        RearLeftWheel.motorTorque = power;
        RearRightWheel.motorTorque = power;

        if (accelVector.y > 0f)
        {
            rigidBody.AddForce(transform.forward * power * rigidBodyForceMultiplier);  // Add force to the rigid body
                                                                                       
        }
        else
        {
            rigidBody.AddForce(transform.forward * 0);
        }



        //Braking
        if (isBraking)
        {
            currentBrakeForce = brakePower;
        }
        else
        {
            currentBrakeForce = 0f;
        }

            //Apply brake to brakes
        FrontLeftWheel.brakeTorque = currentBrakeForce;
        FrontRightWheel.brakeTorque = currentBrakeForce;
        RearLeftWheel.brakeTorque = currentBrakeForce;
        RearRightWheel.brakeTorque = currentBrakeForce;
    }
    #endregion

    #region WHEEL ROTATION 
    private void SteeringWheel()
    {
        currentSteerAngle = maxSteerAngle * steerVector.x;


        //Apply rotation to front wheels
        FrontLeftWheel.steerAngle = currentSteerAngle;
        FrontRightWheel.steerAngle = currentSteerAngle;

        if (currentSteerAngle < 0f)
        {
            steeringWheel.transform.localEulerAngles = new Vector3(steeringWheel.transform.localEulerAngles.x, -25, 0);
        }
        else if (currentSteerAngle > 0f)
        {
            steeringWheel.transform.localEulerAngles = new Vector3(steeringWheel.transform.localEulerAngles.x, 25, 0);
        }
        else
        {
            steeringWheel.transform.localEulerAngles = new Vector3(steeringWheel.transform.localEulerAngles.x, 0, 0);
        }
    }



    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FrontLeftWheel, FrontLeftWheelMesh);
        UpdateSingleWheel(FrontRightWheel, FrontRightWheelMesh);
        UpdateSingleWheel(RearLeftWheel, RearLeftWheelMesh);
        UpdateSingleWheel(RearRightWheel, RearRightWheelMesh);
    }

    #endregion

    #region ANGULAR DAMPING

    private void AngularDampingUpdate()
    {
        
    }

    #endregion

}
