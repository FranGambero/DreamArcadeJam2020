using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
    public GameObject PauseMenu;
    public Button A1;
    public Button A2;
    public Button A3;
    public Sprite ToolStay, ToolClicked;
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

    public void ClickToolButton(BreakdownType type) {
        switch (type) {
            case BreakdownType.Wrench:
                StartCoroutine(ChangeToolSprite(A1));
                break;
            case BreakdownType.Hammer:
                StartCoroutine(ChangeToolSprite(A2));

                break;
            case BreakdownType.Extinguisher:
                StartCoroutine(ChangeToolSprite(A3));
                break;
            case BreakdownType.Undefined:
                break;
            default:
                break;
        }
    }

    IEnumerator ChangeToolSprite(Button btn) {
        btn.GetComponent<Image>().sprite = ToolClicked;
        yield return new WaitForSeconds(.2f);
        btn.GetComponent<Image>().sprite = ToolStay;
    }
}
