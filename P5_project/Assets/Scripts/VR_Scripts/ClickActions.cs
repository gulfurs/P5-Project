using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickActions : MonoBehaviour
{

    public InputActionAsset actionAsset;

    private InputAction LeftGrip;
    private InputAction LeftTrigger;
    private InputAction RightGrip;
    private InputAction RightTrigger;

    void OnEnable()
    {
        var actionMap = actionAsset.FindActionMap("XRActionMap"); 
        LeftGrip = actionMap.FindAction("XR_GripButton_Left");  
        LeftTrigger = actionMap.FindAction("XR_TriggerButton_Left");  
        RightGrip = actionMap.FindAction("XR_GripButton_Right");  
        RightTrigger = actionMap.FindAction("XR_TriggerButton_Right");  

        // Subscribe to the actions
        LeftGrip.performed += LeftGripClass;
        LeftTrigger.performed += LeftTriggerClass;
        RightGrip.performed += RightGripClass;
        RightTrigger.performed += GripTriggerClass;

        // Enable the actions
        LeftGrip.Enable();
        LeftTrigger.Enable();
        RightGrip.Enable();
        RightTrigger.Enable();
    }

    void OnDisable()
    {
        LeftGrip.performed -= LeftGripClass;
        LeftTrigger.performed -= LeftTriggerClass;
        RightGrip.performed -= RightGripClass;
        RightTrigger.performed -= GripTriggerClass;

        // Disable the actions
        LeftGrip.Disable();
        LeftTrigger.Disable();
        RightGrip.Disable();
        RightTrigger.Disable();
    }

    public void LeftGripClass(InputAction.CallbackContext context){
        Debug.Log("test-gripLeft");
    }
    public void LeftTriggerClass(InputAction.CallbackContext context){
        Debug.Log("test_trigger_left");
    }
    public void RightGripClass(InputAction.CallbackContext context){
        Debug.Log("test_right_grip");
    }
    public void GripTriggerClass(InputAction.CallbackContext context){
        Debug.Log("test_trigger_right");
    }
}
