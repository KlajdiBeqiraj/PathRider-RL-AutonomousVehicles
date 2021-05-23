using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;


    [SerializeField] private float currentSteerAngle;
    [SerializeField] private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] public float acceleratorPedal;
    [SerializeField] public float currentSteeringAngle;
    [SerializeField] public float brakePedal;
    [SerializeField] public bool reverse;


    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    //private void FixedUpdate()
    //{
    //    GetInput();
    //    HandleMotor();
    //    HandleSteering();
    //    UpdateWheels();
    //}



    public void updateController()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        ApplyBreaking();
    }

    private void HandleMotor()
    {
        //frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        //frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        if (reverse)
        {
            frontLeftWheelCollider.motorTorque = -motorForce * acceleratorPedal;
            frontRightWheelCollider.motorTorque = -motorForce * acceleratorPedal;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = motorForce * acceleratorPedal;
            frontRightWheelCollider.motorTorque = motorForce * acceleratorPedal;
        }

        //currentbreakForce = isBreaking ? breakForce : 0f;
        //ApplyBreaking();

    }

    private void ApplyBreaking()
    {
        frontLeftWheelCollider.brakeTorque = breakForce* brakePedal;
        frontRightWheelCollider.brakeTorque = breakForce * brakePedal;
        rearLeftWheelCollider.brakeTorque = breakForce * brakePedal;
        rearRightWheelCollider.brakeTorque = breakForce * brakePedal;
    }

    private void HandleSteering()
    {
        //currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteeringAngle*maxSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteeringAngle*maxSteerAngle;

    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);

    }

    public  void GetInput(float accp, float steerAngle,float b_pedal, int r )
    {
        //horizontalInput = hInput;
        //verticalInput = vInput;
        //isBreaking = isBreakBool;

        acceleratorPedal = accp;
        currentSteeringAngle = Mathf.Clamp(steerAngle, -1f, 1f);
        brakePedal = b_pedal <= 0.3f ? 0f : b_pedal;

        if(r==1)
        {
            reverse = !reverse;
        }

    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);

    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
