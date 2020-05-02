using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenuManager : MonoBehaviour {
    public Animator facadeAnim;
    public Animator menuAnim, optionsMenuAnim, instructionsMenuAnim;
    public Animator cameraAnim;
    public GameObject SelectPlayerPanel;
    public Button startBtn, optionsBtn, instructionsBtn;
    public Button playBtn;
    private bool starting, left, right, inOptions, inInstructions = false;
    public SpriteRenderer PlayerSpriteRenderer1;
    public SpriteRenderer PlayerSpriteRenderer2;
    int spritesIndex = 0;
    public EventSystem eventSystem;

    [Header("Sounds")]
    public Slider mainThemeSlider;
    public Slider EfectsThemeSlider;

    private void Start() {
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
        eventSystem.SetSelectedGameObject(startBtn.gameObject);
        SetManagertVolumes();
    }
    private void SetManagertVolumes() {
        mainThemeSlider.value = AudioManager.Instance.GetVolume("Theme");
        mainThemeSlider.onValueChanged.RemoveAllListeners();
        mainThemeSlider.onValueChanged.AddListener((System.Single vol) => AudioManager.Instance.ChangeVolumen("Theme", vol)); 
        EfectsThemeSlider.onValueChanged.RemoveAllListeners();
        EfectsThemeSlider.onValueChanged.AddListener((System.Single vol) => AudioManager.Instance.ChangeVolumen("Effects", vol));
    }
    private void Update() {
        if (starting) {
            if (eventSystem.currentSelectedGameObject == null) {
                eventSystem.SetSelectedGameObject(playBtn.gameObject);

            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackMainMenu(0);
            }
            if (Input.GetAxisRaw("Horizontal") > 0) {
                if (!right) {
                    right = true;
                    NextSprite();
                }
            } else {
                right = false;
            }
            if (Input.GetAxisRaw("Horizontal") < 0) {
                if (!left) {
                    left = true;
                    PrevSprite();
                }
            } else {
                left = false;
            }

        } else if (inOptions) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackMainMenu(1);
            }

        } else if (inInstructions) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackMainMenu(2);
            }

        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
            if (eventSystem.currentSelectedGameObject == null) {
                eventSystem.SetSelectedGameObject(startBtn.gameObject);

            }
        }
    }

    private void BackMainMenu(int index) {
        switch (index) {
            case 0:
                starting = false;
                cameraAnim.Play("ZoomOutCameraAnim");
                facadeAnim.Play("FacadeInAnim");
                SelectPlayerPanel.SetActive(false);
                eventSystem.SetSelectedGameObject(startBtn.gameObject);
                break;
            case 1:
                inOptions = false;
                cameraAnim.Play("MoveLeftCamAnim");
                optionsMenuAnim.Play("OtherMenuOutAnim");
                eventSystem.SetSelectedGameObject(optionsBtn.gameObject);
                break;
            case 2:
                inInstructions = false;
                cameraAnim.Play("MoveLeftCamAnim");
                instructionsMenuAnim.Play("OtherMenuOutAnim");
                eventSystem.SetSelectedGameObject(instructionsBtn.gameObject);
                break;
            default:
                break;
        }
        menuAnim.Play("MainMenuInAnim");

    }

    public void LetsStart() {
        starting = true;
        cameraAnim.Play("ZoomInCameraAnim");
        facadeAnim.Play("FacadeOutAnim");
        menuAnim.Play("MainMenuOutAnim");
        Invoke(nameof(ShowPlayerSelector), .5f);
        eventSystem.SetSelectedGameObject(playBtn.gameObject);

    }

    private void ShowPlayerSelector() {
        if (starting) {
            SelectPlayerPanel.SetActive(true);
        }
    }

    public void NextSprite() {
        spritesIndex += 2;
        spritesIndex = spritesIndex % GameController.Instance.spritesCaserxLibres.Count;
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
    }

    public void PrevSprite() {
        spritesIndex -= 2;
        if (spritesIndex < 0)
            spritesIndex = GameController.Instance.spritesCaserxLibres.Count - 2;
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
    }

    public void StartGame() {
        PlayerPrefs.SetInt(GameController.Instance.PLAYER_SELECTED_KEY, spritesIndex);
        SceneManager.LoadScene(1);
    }

    public void GoOptions() {
        inOptions = true;
        cameraAnim.Play("MoveRightCamAnim");
        menuAnim.Play("MainMenuOutAnim");
        optionsMenuAnim.gameObject.SetActive(true);
        optionsMenuAnim.Play("OtherMenuInAnim");
    }
    public void GoCredits() {
        SceneManager.LoadScene("Credits");
    }
    public void GoInstructions() {
        inInstructions = true;
        cameraAnim.Play("MoveRightCamAnim");
        menuAnim.Play("MainMenuOutAnim");
        instructionsMenuAnim.gameObject.SetActive(true);
        instructionsMenuAnim.Play("OtherMenuInAnim");
    }
}
