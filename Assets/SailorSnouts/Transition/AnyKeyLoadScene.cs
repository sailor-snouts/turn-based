using UnityEngine;
using UnityEngine.InputSystem;

public class AnyKeyLoadScene : MonoBehaviour
{
    private InputAction anyKeyAction = null;
    [SerializeField] private string nextScene = "";
    [SerializeField] private TransitionController transitionController = null;
    
    private void Awake()
    {
        this.anyKeyAction = new InputAction(binding: "/*/<button>");
        this.anyKeyAction.performed += onAnyKey;
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