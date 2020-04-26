using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public int numRooms, numFloors;
    public Transform[,] roomPosition;
    public Transform[] habitaciones;
    private int roomIndex;

    public float timer, maxTime;
    public List<GameObject> listaVecinos;
    private int numVecinosActivos;
    private bool vecinito;

    private void Start() {
        numRooms = 3;
        numFloors = 4;
        roomIndex = 0;
        numVecinosActivos = 0;
        roomPosition = new Transform[numFloors, numRooms];

        fillMatrix();
    }


    private void Awake() {
        maxTime = 5;
        timer = 0;
        vecinito = false;
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer >= maxTime && numVecinosActivos < 3) { //Los vecinos maximos que queramos mostrar de momento
            numVecinosActivos++;
            spawnVecino();
            timer = 0;
        }

    }

    private void fillMatrix() {
        for (int i = 0; i <= numFloors - 1; i++) {
            for (int j = 0; j <= numRooms - 1; j++) {
                roomPosition[i, j] = habitaciones[roomIndex];
                habitaciones[roomIndex].position = roomPosition[i, j].position;
                roomIndex++;
            }
        }

        //PlayerController.Instance.summonPlayer();

    }

    private void spawnVecino() {
        listaVecinos.Find(item => item.activeInHierarchy == false).SetActive(true);
    }
}
