using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenuManager : MonoBehaviour {
    public Animator facadeAnim;
    public Animator menuAnim;
    public Animator cameraAnim;
    public GameObject SelectPlayerPanel;
    public Button startBtn;
    public Button playBtn;
    private bool starting, left, right = false;
    public SpriteRenderer PlayerSpriteRenderer1;
    public SpriteRenderer PlayerSpriteRenderer2;
    int spritesIndex = 0;
    public EventSystem eventSystem;

    private void Start() {
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
        eventSystem.SetSelectedGameObject(startBtn.gameObject);
    }

    private void Update() {
        if (starting) {
            if (eventSystem.currentSelectedGameObject == null) {
                eventSystem.SetSelectedGameObject(playBtn.gameObject);

            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackMainMenu();
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

        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
            if (eventSystem.currentSelectedGameObject == null) {
                eventSystem.SetSelectedGameObject(startBtn.gameObject);

            }
        }
    }

    private void BackMainMenu() {
        starting = false;
        cameraAnim.Play("ZoomOutCameraAnim");
        facadeAnim.Play("FacadeInAnim");
        menuAnim.Play("MainMenuInAnim");
        SelectPlayerPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(startBtn.gameObject);

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
}
