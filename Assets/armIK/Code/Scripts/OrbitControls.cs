using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(DragObjects))]
public class orbitControls : MonoBehaviour {
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 1.0f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxDistance = 5.0f;

    [SerializeField] private float panSensitivity = 1f;
    [SerializeField] private float zoomSensitivity = 0.1f;


    private DragObjects dragObjects;

    private InputAction moveAction;
    private InputAction dragAction;
    private InputAction zoomAction;

    private bool drag = false;

    private void Awake() {
        var playerInput = GetComponent<PlayerInput>();
        dragObjects = GetComponent<DragObjects>();

        moveAction = playerInput.actions["Move"];
        dragAction = playerInput.actions["Drag"];
        zoomAction = playerInput.actions["Zoom"];

        moveAction.performed += MouseMove;
        dragAction.started += dragStart;
        dragAction.canceled += dragEnd;
        zoomAction.performed += Zoom;
    }

    private void OnDisable() {
        moveAction.performed -= MouseMove;
        dragAction.started -= dragStart;
        dragAction.performed -= dragEnd;
    }

    private void MouseMove(InputAction.CallbackContext context) {
        if (!drag || dragObjects.Dragging)
            return;

        var delta = panSensitivity * context.ReadValue<Vector2>();
        transform.RotateAround(target.position, Vector3.up, delta.x);
        transform.RotateAround(target.position, transform.right, -delta.y);
    }

    private void dragStart(InputAction.CallbackContext context) {
        drag = true;
    }

    private void dragEnd(InputAction.CallbackContext context) {
        drag = false;
    }

    private void Zoom(InputAction.CallbackContext context) {
        var delta = zoomSensitivity * -context.ReadValue<float>();
        var easeOutCubic = Mathf.Clamp(1 - Mathf.Pow(1 - (distance - minDistance) / 4, 3), 0, 1);
        distance += delta * easeOutCubic;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void LateUpdate() {
        transform.position = target.position - transform.forward * distance;
    }
}
