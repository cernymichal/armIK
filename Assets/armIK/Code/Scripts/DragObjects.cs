using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class DragObjects : MonoBehaviour {
    [SerializeField] private string draggableTag;

    public bool Dragging { get { return drag; } }

    private InputAction mouseAction;
    private InputAction moveAction;
    private InputAction dragAction;

    private bool drag = false;
    private Transform draggedObject;

    private void OnEnable() {
        var playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dragAction = playerInput.actions["Drag"];

        moveAction.performed += MouseMove;
        dragAction.started += DragStart;
        dragAction.canceled += DragEnd;
    }

    private void OnDisable() {
        moveAction.performed -= MouseMove;
        dragAction.started -= DragStart;
        dragAction.performed -= DragEnd;
    }

    private void MouseMove(InputAction.CallbackContext context) {
        if (!drag)
            return;

        var objectScreenPosition = Camera.main.WorldToScreenPoint(draggedObject.position);
        var mousePosition = Mouse.current.position.ReadValue();
        draggedObject.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, objectScreenPosition.z));
    }

    private void DragStart(InputAction.CallbackContext context) {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(draggableTag)) {
            draggedObject = hit.transform;
            drag = true;
        }
    }

    private void DragEnd(InputAction.CallbackContext context) {
        drag = false;
    }
}
