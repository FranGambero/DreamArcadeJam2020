using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
    public GameObject pauseMenu, optionsMenu, lifesPanel, lifePrefab, resumeBtn, rightHandPanel, leftHandPanel;
    public TextMeshProUGUI gameOverMenu;
    public Slider mainThemeSlider, EfectsThemeSlider;
    public TextMeshProUGUI pointstext;
    public Button A1;
    public Button A2;
    public Button A3;
    public Toggle toggleLeftHand;
    public Sprite ToolStay, ToolClicked;
    public Animator MainCameraAnim;
    public Animator CoinAnim;
    public float coinSumSpeed = 100;
    private bool isPaused = false, firstLoop = true, inOptions = false;
    private int moneyTarget;
    private int actualMoney;
    private EventSystem eventSystem;
    private void Awake() {
        //pauseMenu.SetActive(false);
        pointstext.gameObject.SetActive(true);
        eventSystem = FindObjectOfType<EventSystem>();
    }
    private void Start() {
        InitSetStars();
        SetManagertVolumes();
        toggleLeftHand.onValueChanged.AddListener(Action => { ChangeHandPanel(toggleLeftHand); });
    }

    private void SetManagertVolumes() {
        if (AudioManager.Instance != null) {
            mainThemeSlider.value = AudioManager.Instance.GetVolume("Theme");
            mainThemeSlider.onValueChanged.RemoveAllListeners();
            mainThemeSlider.onValueChanged.AddListener((System.Single vol) => AudioManager.Instance.ChangeVolumen("Theme", vol));
            EfectsThemeSlider.value = AudioManager.Instance.GetVolume("Effects");
            EfectsThemeSlider.onValueChanged.RemoveAllListeners();
            EfectsThemeSlider.onValueChanged.AddListener((System.Single vol) => { AudioManager.Instance.ChangeVolumen("Effects", vol); AudioManager.Instance.Play("ClickButton"); });
        }
    }
    private void ChangeHandPanel(Toggle toggle) {
        leftHandPanel.SetActive(toggle.isOn);
        rightHandPanel.SetActive(!toggle.isOn);
    }

    private void Update() {
        if (PlayerStats.Instance.currentLifes > 0) {

            if (firstLoop) {
                firstLoop = false;
                MainCameraAnim.Play("ZoomOutCameraAnim");
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (inOptions) {
                    BackToPause();
                } else {
                    setPause();
                }
            }
            LerpMoney();
        } else {
            if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Space)) {
                Back();
            }
        }


    }
    private void PlaySound(string name, bool oneshot = false, bool randomPeach = false) {
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(name, oneshot, randomPeach);
    }
    public void setPause() {
        PlaySound("ClickButton", false, true);
        eventSystem.SetSelectedGameObject(resumeBtn);

        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
        pauseMenu.SetActive(isPaused);
    }

    public void Restart() {
        PlaySound("ClickButton");

        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void Back() {
        PlaySound("ClickButton");

        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void BackToPause() {
        PlaySound("ClickButton");

        inOptions = false;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void OpenOptions() {
        PlaySound("ClickButton");

        inOptions = true;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
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
        if (index >= 0 && index < lifesPanel.transform.childCount && lifesPanel.transform.GetChild(index) != null) {
            StartCoroutine(HideStar(lifesPanel.transform.GetChild(index).gameObject));
        }
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

    public void showGameOver() {
        Time.timeScale = 0;
        gameOverMenu.transform.parent.gameObject.SetActive(true);
        gameOverMenu.text = RoomManager.Instance.totalPoints.ToString();
    }
}
