﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController> {
    public int numRooms, numFloors;
    public Transform[,] roomPosition;
    public Transform[] habitaciones;
    public Transform[] Elevators;
    [SerializeField]
    public Sprite[,] spritevecinos;
    public Sprite ClosedElevator, OpenElevator;
    private int roomIndex;

    public List<Sprite> spritesElegidos;
    public List<Sprite> spritesLibres, spritesCaserxLibres;
    public string PLAYER_SELECTED_KEY = "PLAYER_SELECTED_KEY";

    public float timer, maxTime, minTime;
    public List<GameObject> listaVecinos;
    private int numVecinosActivos;

    private void Awake() {
        numRooms = 3;
        numFloors = 5;
        roomIndex = 0;
        numVecinosActivos = 0;
        roomPosition = new Transform[numFloors, numRooms];

        maxTime = 5;
        minTime = 3;
        timer = 0;

    }

    private void Start() {
        fillMatrix();
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= maxTime && numVecinosActivos < 6) { // Los vecinos maximos que queramos mostrar de momento
            numVecinosActivos++;
            spawnVecino();
            timer = 0;
        }
    }

    private void fillMatrix() {
        for (int i = 0; i <= numFloors - 1; i++) {
            for (int j = 0; j <= numRooms - 1; j++) {
                roomPosition[i, j] = habitaciones[roomIndex].transform;
                habitaciones[roomIndex].transform.position = roomPosition[i, j].position;
                if (j != 1) {
                    RoomManager.Instance.GenerateRoom(habitaciones[roomIndex].transform, i, j);
                }
                roomIndex++;
            }
        }
    }

    public Sprite[] assignSpriteVecino() {
        Sprite[] arrayResult = new Sprite[2];

        int spriteIndex = Random.Range(0, spritesLibres.Count);

        // Modificar si aumentaramos la cantidad de sprites por animacion
        if (spriteIndex % 2 != 0) {
            spriteIndex--;
        }

        for (int i = 0; i < arrayResult.Length; i++) {
            arrayResult[i] = spritesLibres[spriteIndex + i];
        }

        spritesElegidos.AddRange(arrayResult);
        spritesLibres.RemoveRange(spriteIndex, 2);

        return arrayResult;
    }

    public Sprite[] assignSpritePlayer() {
        Sprite[] arrayResult = new Sprite[2];

        int spriteIndex = PlayerPrefs.GetInt(PLAYER_SELECTED_KEY, -1);

        if (spriteIndex == -1) {
            arrayResult = assignSpriteVecino();
        } else {

            for (int i = 0; i < arrayResult.Length; i++) {
                arrayResult[i] = spritesCaserxLibres[spriteIndex + i];
            }
        }

        return arrayResult;
    }

    private void spawnVecino() {
        GameObject nextVecino = listaVecinos.Find(item => item.activeInHierarchy == false);
        nextVecino.SetActive(true);
        nextVecino.GetComponent<Vecino>().initVecino();


    }
}
