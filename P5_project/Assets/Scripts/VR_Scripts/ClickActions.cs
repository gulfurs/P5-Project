using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ClickActions : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private InputActionAsset actionAsset;
    public DialogueManager dialogueMan;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject ButtonOption_A;
    [SerializeField] private GameObject ButtonOption_B;

    // Input Actions
    private InputAction LeftGrip;
    private InputAction LeftTrigger;
    private InputAction RightGrip;
    private InputAction RightTrigger;

    public InputAction PrimaryLeft;

    public TextMeshPro targetText;
    public float activeAlpha = 0.3f;
    private float normalAlpha;

    private void Awake()
    {
        // Validate required components
        if (actionAsset == null)
        {
            Debug.LogError("Input Action Asset is not assigned to ClickActions script!", this);
            enabled = false;
            return;
        }

        if (ButtonOption_A == null || ButtonOption_B == null)
        {
            Debug.LogError("Button options are not assigned in ClickActions script!", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable() 
    {
        try
        {
            // Setup input actions
            var actionMap = actionAsset.FindActionMap("XRActionMap");
            if (actionMap == null)
            {
                Debug.LogError("Could not find XRActionMap in the Input Action Asset!", this);
                enabled = false;
                return;
            }

            // Initialize actions
            LeftGrip = actionMap.FindAction("XR_GripButton_Left");
            LeftTrigger = actionMap.FindAction("XR_TriggerButton_Left");
            RightGrip = actionMap.FindAction("XR_GripButton_Right");
            RightTrigger = actionMap.FindAction("XR_TriggerButton_Right");
            PrimaryLeft = actionMap.FindAction("XR_Primary_Left");

            // Validate all actions were found
            if (LeftGrip == null || LeftTrigger == null || RightGrip == null || RightTrigger == null)
            {
                Debug.LogError("One or more input actions could not be found!", this);
                enabled = false;
                return;
            }

            // Subscribe to events
            LeftTrigger.performed += OnLeftTriggerPress;
            RightTrigger.performed += OnRightTriggerPress;
            PrimaryLeft.performed += LeftPrimaryPress;

            // Enable actions
            LeftGrip.Enable();
            LeftTrigger.Enable();
            RightGrip.Enable();
            RightTrigger.Enable();
            PrimaryLeft.Enable();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during input setup: {e.Message}", this);
            enabled = false;
        }
    }

    private void OnDisable() 
    {
        if (LeftTrigger != null)
        {
            LeftTrigger.performed -= OnLeftTriggerPress;
            LeftTrigger.Disable();
        }

        if (RightTrigger != null)
        {
            RightTrigger.performed -= OnRightTriggerPress;
            RightTrigger.Disable();
        }

        if (LeftGrip != null) LeftGrip.Disable();
        if (RightGrip != null) RightGrip.Disable();

        if (PrimaryLeft != null) 
        {
            PrimaryLeft.performed -= LeftPrimaryPress;
            PrimaryLeft.Disable();
        }
    }

    void Start() {
        if (targetText != null)
        {
            // Save the original alpha value
            normalAlpha = targetText.color.a;
        }
        else
        {
            Debug.LogError("TextMesh is not assigned!");
        }
    }

    private void Update()
    {
        if (ButtonOption_A != null && ButtonOption_B != null && dialogueMan != null)
        {
            ButtonOption_A.SetActive(dialogueMan.ChoicesActive);
            ButtonOption_B.SetActive(dialogueMan.ChoicesActive);
        }

        if (dialogueMan != null && targetText != null)
        {
            if (dialogueMan.ChoicesActive)
            {
                SetMaterialAlpha(activeAlpha);
            }
            else
            {
                SetMaterialAlpha(normalAlpha);
            }
        }
    }

    void SetMaterialAlpha(float alpha)
    {
        Color color = targetText.color; // Get current color
        color.a = alpha; // Modify alpha
        targetText.color = color; // Apply new color
    }

    public void OnLeftTriggerPress(InputAction.CallbackContext context)
    {
        //LeftPrimaryPress(new InputAction.CallbackContext());
        //LeftPrimaryPress(context);   
        if (dialogueMan != null && ButtonOption_A != null && dialogueMan.ChoicesActive)
        {
            
          //  dialogueMan.SimulatePress();
            PrimaryLeft.Enable();
            LeftPrimaryPress(context); 
            dialogueMan.OnChoiceA();
            Debug.Log("Button A has been pressed!");

        }
    }

    public void OnRightTriggerPress(InputAction.CallbackContext context)
    {
        //LeftPrimaryPress(new InputAction.CallbackContext());   
        //LeftPrimaryPress(context);   
        if (dialogueMan != null && ButtonOption_B != null && dialogueMan.ChoicesActive)
        {
            //
           // dialogueMan.SimulatePress();
            PrimaryLeft.Enable();
            LeftPrimaryPress(context); 
            dialogueMan.OnChoiceB();
            Debug.Log("Button B has been pressed!");

        }
    }

    public void LeftPrimaryPress(InputAction.CallbackContext context)
    {
        dialogueMan.PlayNextLine();
        dialogueMan.SimulatePress();
        Debug.Log("second Tower is hit");
        
    }

}