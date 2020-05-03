using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Singleton<PlayerStats>
{
    public int maxLifes, currentLifes;
    public List<GameObject> pilaVidas;
    public GameObject Corazao;

    private void Awake() {
        pilaVidas = new List<GameObject>();
        assignLifes();
    }

    private void assignLifes() {
        // Cambiar por asignacion de imagenes segun nivel dificultad

    }
    [ContextMenu("Damage")]
    public void performDamage() {
        performDamage(1);
    }

    public void performDamage(int damage) {
        UIController.Instance.HideLife(currentLifes-1);
        currentLifes -= damage;

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
        UIController.Instance.showGameOver();
    }
}
