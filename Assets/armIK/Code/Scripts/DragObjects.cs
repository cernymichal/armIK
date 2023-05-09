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

    private Vector2 mousePosition;
    private bool drag = false;
    private Transform draggedObject;

    private void Awake() {
        var playerInput = GetComponent<PlayerInput>();

        mouseAction = playerInput.actions["MousePosition"];
        moveAction = playerInput.actions["Move"];
        dragAction = playerInput.actions["Drag"];

        mouseAction.performed += MousePosition;
        moveAction.performed += MouseMove;
        dragAction.started += DragStart;
        dragAction.canceled += DragEnd;
    }

    private void OnDisable() {
        mouseAction.performed -= MousePosition;
        moveAction.performed -= MouseMove;
        dragAction.started -= DragStart;
        dragAction.performed -= DragEnd;
    }

    private void MousePosition(InputAction.CallbackContext context) {
        mousePosition = context.ReadValue<Vector2>();
    }

    private void MouseMove(InputAction.CallbackContext context) {
        if (!drag)
            return;

        var delta = context.ReadValue<Vector2>();
        var screenPosition = Camera.main.WorldToScreenPoint(draggedObject.position);
        draggedObject.position = Camera.main.ScreenToWorldPoint(screenPosition + new Vector3(delta.x, delta.y, 0));

    }

    private void DragStart(InputAction.CallbackContext context) {
        var ray = Camera.main.ScreenPointToRay(mousePosition);
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
