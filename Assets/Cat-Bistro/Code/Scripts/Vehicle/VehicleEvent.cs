using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VehicleEvent : MonoBehaviour
{
    [Header("References")]

    public Vehicle vehicle;

    public Transform Player;
    
    public Transform exitPoint;
    public GameObject playerCamera;
    public GameObject vehicleCamera;

    [Header("Vehicle Controls")]

    
    public InputActionAsset inputActions;
    public InputAction interact;

    private Rigidbody vehicleRigidBody;
    public bool isInVehicle = false;

    [Header("Temp")]

    public float interactDistance = 3f;
    

    private void Awake()
    {
        interact = InputSystem.actions.FindAction("Interact");

    }

    private void OnEnable()
    {
        inputActions.FindActionMap("Vehicle").Enable();
        inputActions.FindActionMap("Player").Disable();

    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Vehicle").Disable();
        inputActions.FindActionMap("Player").Enable();
    }

    void Start()
    {
        vehicleRigidBody = GetComponent<Rigidbody>();
        if (vehicleRigidBody == null)
        {
            Debug.Log("NO VEHICLE RIGID BODY");
        }



        interact = InputSystem.actions.FindAction("Interact");

        

    }
    
    private void Update()
    {
        float interacting = interact.ReadValue<float>();
        Debug.Log($" interaction button {interacting}");
        Debug.Log($" is in the vehicle { isInVehicle } ");
        Debug.Log($" current action map {interact.actionMap} ");


        if  (interacting == 1f)
        {
            if(!isInVehicle)
            {
                if (Player != null && Vector3.Distance(Player.position, transform.position) < interactDistance)
                {
                    EnterVehicle();
                    
                }
            }
            else
            {
                ExitVehicle();
                
            }
        }
    }


    private void EnterVehicle()
    {
        vehicle.enabled = true;
        inputActions.FindActionMap("Vehicle").Enable();
        inputActions.FindActionMap("Player").Disable();

        Player.gameObject.SetActive(false);
        
        playerCamera.SetActive(false);

        vehicleCamera.SetActive(true);

        
        
    }

    public void ExitVehicle()
    {
        vehicle.enabled = false;
        inputActions.FindActionMap("Vehicle").Disable();
        inputActions.FindActionMap("Player").Enable();



        if (vehicleRigidBody != null)
        {
            vehicleRigidBody.linearVelocity = Vector3.zero;
            vehicleRigidBody.angularVelocity = Vector3.zero;
        }

        
        vehicleCamera.SetActive(false);

        Player.position = exitPoint.position;
        Player.gameObject.SetActive(true);
        playerCamera.SetActive(true);

        isInVehicle = false;



    }

   
}
