using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float timer, maxTime;
    public List<GameObject> listaVecinos;

    private void Awake() {
        maxTime = 3;
        timer = 0;
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer >= maxTime) {
            spawnVecino();
            timer = 0;
        }

    }

    private void spawnVecino() {
        listaVecinos.Find(item => item.activeInHierarchy == false).SetActive(true);
    }
}
