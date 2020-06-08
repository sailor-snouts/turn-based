using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    void Update()
    {
        Vector3 mouse = Mouse.current.position.ReadValue();
        mouse.z = 1f;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mouse);
        transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
    }
}
