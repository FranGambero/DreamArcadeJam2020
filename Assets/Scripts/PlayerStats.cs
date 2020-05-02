using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Singleton<PlayerStats>
{
    public int maxLifes, currentLifes;
    public TextMeshProUGUI lifesText;
    public List<GameObject> pilaVidas;
    public GameObject LifesPanel;
    public GameObject Corazao;

    private void Awake() {
        pilaVidas = new List<GameObject>();

        maxLifes = 5;
        currentLifes = 5;

        assignLifes();
    }

    private void assignLifes() {
        // Cambiar por asignacion de imagenes segun nivel dificultad
        lifesText.text = currentLifes.ToString();

        Debug.Log("Tienes vidas: " + pilaVidas);
    }

    public void performDamage(int damage) {
        currentLifes -= damage;

        assignLifes();
        updateLifes();

        if(currentLifes <= 0) {
            marryPlayer();
        }
    }

    public void updateLifes() {
        GetFreeObject();
    }

    private void GetFreeObject() {
        GameObject myItem;
        myItem = pilaVidas.Find(item => item.activeInHierarchy == false);
        Debug.Log("item " + myItem);
        myItem.SetActive(true);
        //return myItem;
    }

    private void marryPlayer() {
        Debug.Log("Te casaste, la cagaste");
    }
}
