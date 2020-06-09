using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnyKeyLoadScene : MonoBehaviour
{
    private InputAction anyKeyAction = null;
    private TransitionController transitionController = null;
    [SerializeField] private string nextScene = "";

    private void Awake()
    {
        this.anyKeyAction = new InputAction(binding: "/*/<button>");
        this.anyKeyAction.performed += onAnyKey;
    }

    private void Start()
    {
        this.transitionController = TransitionController.GetInstance();
    }

    private void OnEnable()
    {
        this.anyKeyAction.Enable();
    }

    private void OnDisable()
    {
        this.anyKeyAction.Disable();
    }

    private void OnDestroy()
    {
        this.anyKeyAction.performed -= onAnyKey;
    }

    private void onAnyKey(InputAction.CallbackContext callbackContext)
    {
        this.transitionController.Load(this.nextScene);
        this.anyKeyAction.Disable();
    }
}