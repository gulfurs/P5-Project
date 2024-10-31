using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickActions : MonoBehaviour
{

    //The actionMap
    public InputActionAsset actionAsset;

    //DialogMan Reference
    public DialogueManager dialogueMan;

    //The different bindings
    private InputAction LeftGrip;
    private InputAction LeftTrigger;
    private InputAction RightGrip;
    private InputAction RightTrigger;

    //ButtonOptinos
    public GameObject ButtonOption_A;
    public GameObject ButtonOption_B;


    void OnEnable() 
    {
        //Connecting them to the code
        var actionMap = actionAsset.FindActionMap("XRActionMap"); 
        LeftGrip = actionMap.FindAction("XR_GripButton_Left");  
        LeftTrigger = actionMap.FindAction("XR_TriggerButton_Left");  
        RightGrip = actionMap.FindAction("XR_GripButton_Right");  
        RightTrigger = actionMap.FindAction("XR_TriggerButton_Right");  

        
        // Subscribe to the actions
        LeftTrigger.performed += OnLeftTriggerPress;
        //LeftGrip.performed += LeftGripClass;
        //LeftTrigger.performed += LeftTriggerClass;

        RightTrigger.performed += OnRightTriggerPress;
        //RightTrigger.performed += GripTriggerClass;


        // Enable the actions
        LeftGrip.Enable();
        LeftTrigger.Enable();
        RightGrip.Enable();
        RightTrigger.Enable();
    }

    void OnDisable() //Oppersite of Enable
    {
        LeftTrigger.performed -= OnLeftTriggerPress;
        //LeftGrip.performed -= LeftGripClass;
        //LeftTrigger.performed -= LeftTriggerClass;

        RightTrigger.performed -= OnRightTriggerPress;
        //RightTrigger.performed -= GripTriggerClass;

        // Disable the actions
        LeftGrip.Disable();
        LeftTrigger.Disable();
        RightGrip.Disable();
        RightTrigger.Disable();
    }

    void Update(){
        ButtonOption_A.SetActive(dialogueMan.ChoicesActive);
        ButtonOption_B.SetActive(dialogueMan.ChoicesActive);
    }

    //----The classes for each button - see if work. ----
    public void LeftGripClass(InputAction.CallbackContext context){
       // Debug.Log("test-gripLeft");
    }
    public void LeftTriggerClass(InputAction.CallbackContext context){
        //Debug.Log("test_trigger_left");
    }
    public void RightGripClass(InputAction.CallbackContext context){
        //Debug.Log("test_right_grip");
    }
    public void GripTriggerClass(InputAction.CallbackContext context){
        //Debug.Log("test_trigger_right");
    }
    //---END of testing----

    //The triggerbutton trigger the button trigger :)
    //For left controller
    public void OnLeftTriggerPress(InputAction.CallbackContext context)
    {
        if (ButtonOption_A != null && dialogueMan.ChoicesActive){
            dialogueMan?.OnChoiceA();
            //delegate(dialogueMan.choiceA);
            Debug.Log("Button A has been pressed!");
        }
        //
    }

    //For right controller
    public void OnRightTriggerPress(InputAction.CallbackContext context)
    {
        if (ButtonOption_B != null && dialogueMan.ChoicesActive){
            dialogueMan?.OnChoiceB();
            //delegate(dialogueMan.choiceB);
            Debug.Log("Button B has been pressed!");
        }
    }

}
