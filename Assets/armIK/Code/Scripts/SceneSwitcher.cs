using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class SceneSwitcher : MonoBehaviour {
    private InputAction previousSceneAction;
    private InputAction nextSceneAction;

    private void OnEnable() {
        var playerInput = GetComponent<PlayerInput>();

        nextSceneAction = playerInput.actions["NextScene"];
        previousSceneAction = playerInput.actions["PreviousScene"];

        nextSceneAction.performed += NextScene;
        previousSceneAction.performed += PreviousScene;
    }

    private void OnDisable() {
        nextSceneAction.performed -= NextScene;
        previousSceneAction.performed -= PreviousScene;
    }

    private void NextScene(InputAction.CallbackContext context) {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int index = (SceneManager.GetActiveScene().buildIndex + 1) % sceneCount;
        SceneManager.LoadScene(index);
    }

    private void PreviousScene(InputAction.CallbackContext context) {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int index = (SceneManager.GetActiveScene().buildIndex - 1 + sceneCount) % sceneCount;
        SceneManager.LoadScene(index);
    }

}
