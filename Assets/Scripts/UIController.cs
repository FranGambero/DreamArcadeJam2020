using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject PauseMenu;
    private bool isPaused = false;

    private void Awake() {
        PauseMenu.SetActive(false);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            setPause();
        }
    }

    private void setPause() {
        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
            PauseMenu.SetActive(isPaused);
    }
}
