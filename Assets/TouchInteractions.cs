using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// System to handle pointer interactions with the new Input System.
/// based on <see href="https://www.youtube.com/watch?v=HfqRKy5oFDQ"/>
/// </summary>
public class TouchInteractions : MonoBehaviour
{
    [SerializeField] private InputActionReference click;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
        click.action.performed += Click;
    }

    private void Click(InputAction.CallbackContext context)
    {
        Ray ray = cam.ScreenPointToRay(Pointer.current.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        Debug.Log(hit2D);
        if (hit2D.collider != null)
        {
            Debug.Log(hit2D.collider.gameObject.name);
        }
    }
}
