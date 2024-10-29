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

        // Enable the actions
        LeftGrip.Enable();
        LeftTrigger.Enable();
        RightGrip.Enable();
        RightTrigger.Enable();
    }

    void OnDisable()
    {
        LeftGrip.performed -= LeftGripClass;

        // Disable the actions
        LeftGrip.Disable();
        LeftTrigger.Disable();
        RightGrip.Disable();
        RightTrigger.Disable();
    }

    private void LeftGripClass(InputAction.CallbackContext context){
        Debug.Log("test");
    }
}
