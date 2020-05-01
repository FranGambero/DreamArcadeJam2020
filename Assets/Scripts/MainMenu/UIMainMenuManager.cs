using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenuManager : MonoBehaviour {
    public Animator facadeAnim;
    public Animator menuAnim;
    public Animator cameraAnim;
    public GameObject SelectPlayerPanel;
    private bool starting = false;
    public SpriteRenderer PlayerSpriteRenderer1;
    public SpriteRenderer PlayerSpriteRenderer2;
    int spritesIndex = 0;

    private void Start() {
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
    }

    private void Update() {
        if (starting) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                BackMainMenu();
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }
    }

    private void BackMainMenu() {
        starting = false;
        cameraAnim.Play("ZoomOutCameraAnim");
        facadeAnim.Play("FacadeInAnim");
        menuAnim.Play("MainMenuInAnim");
        SelectPlayerPanel.SetActive(false);
    }

    public void LetsStart() {
        starting = true;
        cameraAnim.Play("ZoomInCameraAnim");
        facadeAnim.Play("FacadeOutAnim");
        menuAnim.Play("MainMenuOutAnim");
        Invoke(nameof(ShowPlayerSelector), .5f);
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
        spritesIndex = spritesIndex % GameController.Instance.spritesCaserxLibres.Count;
        PlayerSpriteRenderer1.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex];
        PlayerSpriteRenderer2.sprite = GameController.Instance.spritesCaserxLibres[spritesIndex + 1];
    }

    public void StartGame() {
        PlayerPrefs.SetInt(GameController.Instance.PLAYER_SELECTED_KEY, spritesIndex);
        SceneManager.LoadScene(1);
    }
}
