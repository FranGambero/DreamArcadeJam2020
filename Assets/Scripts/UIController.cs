using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
    public GameObject pauseMenu, lifesPanel, lifePrefab;

    public TextMeshProUGUI pointstext;
    public Button A1;
    public Button A2;
    public Button A3;
    public Sprite ToolStay, ToolClicked;
    public Animator MainCameraAnim;
    public Animator CoinAnim;
    public float coinSumSpeed = 100;
    private bool isPaused = false, firstLoop = true;
    private int moneyTarget;
    private int actualMoney;
    private void Awake() {
        //pauseMenu.SetActive(false);
        pointstext.gameObject.SetActive(true);
    }
    private void Start() {
        InitSetStars();
    }
    private void Update() {
        if (firstLoop) {
            firstLoop = false;
            MainCameraAnim.Play("ZoomOutCameraAnim");
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            setPause();
        }
        LerpMoney();

    }

    public void setPause() {
        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
        pauseMenu.SetActive(isPaused);
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void Back() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void OpenOptions() {

    }
    public void Quit() {
        Time.timeScale = 1;
        Application.Quit();
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
    private void LerpMoney() {
        CoinAnim.SetFloat("Speed", Mathf.Clamp((float)moneyTarget - (float)actualMoney, 0, 20) / 20);
        //Debug.Log("Speed: " + Mathf.Clamp((float)moneyTarget - (float)actualMoney, 0, 20) / 20);
        moneyTarget = RoomManager.Instance.totalPoints;
        actualMoney = (int)Mathf.LerpUnclamped(actualMoney, moneyTarget, Time.deltaTime * coinSumSpeed);
        pointstext.text = actualMoney.ToString();
    }
    IEnumerator ChangeToolSprite(Button btn) {
        btn.GetComponent<Image>().sprite = ToolClicked;
        yield return new WaitForSeconds(.2f);
        btn.GetComponent<Image>().sprite = ToolStay;
    }

    public void InitSetStars() {
        for (int i = 0; i < PlayerStats.Instance.currentLifes; i++) {
            if (lifesPanel.transform.GetChild(i) != null) {
                StartCoroutine(InitStart(lifesPanel.transform.GetChild(i).gameObject));
            }
        }
    }
    public void HideLife(int index) {
        StartCoroutine(HideStar(lifesPanel.transform.GetChild(index).gameObject));
    }
    IEnumerator HideStar(GameObject star) {

        for (int i = 0; i < 3; i++) {
            star.SetActive(false);
            yield return new WaitForSeconds(.2f);
            star.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }
        star.SetActive(false);

    }
    IEnumerator InitStart(GameObject star) {

        for (int i = 0; i < 3; i++) {
            star.SetActive(true);
            yield return new WaitForSeconds(.2f);
            star.SetActive(false);
            yield return new WaitForSeconds(.2f);
        }
        star.SetActive(true);

    }
}
