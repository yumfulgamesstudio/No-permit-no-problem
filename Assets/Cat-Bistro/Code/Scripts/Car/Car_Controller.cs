using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public bool carActive { get; private set; }
    private InputSystem_Actions controls;
    private float moveInput;
    private float steerInput;

    public float speed;

    [Range(30, 60)]
    [SerializeField] private float turnSensetivity = 30;
    [Header("Car Settings")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350, 1000)]
    [SerializeField] private float carMass = 400;
    [Range(20, 80)]
    [SerializeField] private float wheelsMass = 30;
    [Range(.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1;
    [Range(.5f, 2f)]
    [SerializeField] private float backWheelTraction = 1;

    [Header("Steering Wheel Animation")]
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private float steeringWheelMaxRotation = 180f;
    [SerializeField] private float steeringWheelRotationSpeed = 8f;
    [SerializeField] private Vector3 steeringWheelRotationAxis = Vector3.forward;
    private Quaternion steeringWheelDefaultRotation;

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed;
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7;
    [Range(.5f, 50)]
    [SerializeField] private float accleerationSpeed = 2;
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Brakes Settings")]
    [Range(0, 10)]
    [SerializeField] private float frontBrakesSensetivity = 5;
    [Range(0, 10)]
    [SerializeField] private float backBrakesSensetivity = 5;
    [Range(4000, 6000)]
    [SerializeField] private float brakePower = 5000;
    private bool isBraking;

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = .5f;
    [Range(0, 1)]
    [SerializeField] private float backDriftFactor = .5f;
    [SerializeField] private float driftDuration = 1f;
    private float driftTimer;
    private bool isDrifting;


    private Car_Wheel[] wheels;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();

        controls = ControlsManager.instance.controls;

        if (steeringWheel != null)
            steeringWheelDefaultRotation = steeringWheel.localRotation;

        AssignInputEvents();
        SetupDefaultValues();
        ActivateCar(false);
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelsMass;

            if (wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffnes(frontWheelTraction);

            if (wheel.axelType == AxelType.Back)
                wheel.SetDefaultStiffnes(backWheelTraction);
        }

    }

    private void Update()
    {
        if (carActive == false)
            return;


        speed = rb.linearVelocity.magnitude;

        driftTimer -= Time.deltaTime;

        if (driftTimer < 0)
            isDrifting = false;

        ApplyAnimationToSteer();
    }

    private void FixedUpdate()
    {
        if (carActive == false)
            return;

        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();

        if (isDrifting)
            ApplyDrift();
        else
            StopDrift();
    }



    private void ApplyDrive()
    {
        currentSpeed = moveInput * accleerationSpeed * Time.deltaTime;

        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axelType == AxelType.Front)
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axelType == AxelType.Back)
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else
            {
                wheel.cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensetivity;
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, .5f);
            }
        }
    }

    private void ApplyAnimationToSteer()
    {
        if (steeringWheel == null)
            return;

        float targetAngle = -steerInput * steeringWheelMaxRotation;

        Quaternion targetRotation =
            steeringWheelDefaultRotation *
            Quaternion.Euler(steeringWheelRotationAxis * targetAngle);

        steeringWheel.localRotation = Quaternion.Lerp(
            steeringWheel.localRotation,
            targetRotation,
            steeringWheelRotationSpeed * Time.deltaTime
        );
    }

    private void ApplyBrakes()
    {

        foreach (var wheel in wheels)
        {
            bool frontBrakes = wheel.axelType == AxelType.Front;
            float brakeSensetivity = frontBrakes ? frontBrakesSensetivity : backBrakesSensetivity;

            float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0;

            wheel.cd.brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.cd.sidewaysFriction;

            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.cd.sidewaysFriction = sidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffnes();
        }
    }


    private void ApplyAnimationToWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.cd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }
    }

    public void ActivateCar(bool activate)
    {
        carActive = activate;

        //if(!activate)
        //    rb.constraints = RigidbodyConstraints.FreezeAll;
        //else
        //    rb.constraints = RigidbodyConstraints.None;
    }

    public void BrakeTheCar()
    {
        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = .9f;
        backDriftFactor = .9f;
    }

    private void AssignInputEvents()
    {
        controls.Car.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            moveInput = input.y;
            steerInput = input.x;
        };

        controls.Car.Move.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;
    }
}
