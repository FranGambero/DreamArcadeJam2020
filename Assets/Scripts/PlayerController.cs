using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {
    private int numRooms, numFloors;
    public Transform[,] positions;
    public Transform[] habitaciones;
    private int roomIndex;
    public int xPos, yPos;

    private void Start() {
        numRooms = 3;
        numFloors = 4;
        roomIndex = 0;
        xPos = yPos = 0;
        positions = new Transform[numFloors, numRooms];

        fillMatrix();
    }

    private void fillMatrix() {
        for (int i = 0; i <= numFloors - 1; i++) {
            for (int j = 0; j <= numRooms - 1; j++) {
                positions[i, j] = habitaciones[roomIndex];
                habitaciones[roomIndex].position = positions[i, j].position;
                roomIndex++;
            }
        }

        transform.position = positions[xPos, yPos].position;
    }

    private void Update() {
        // Esto se puede mejorar, molaria sacar la llamada a MovePosition y hacerla común
        if (Input.GetKeyDown(KeyCode.W) && xPos < numFloors - 1 && yPos == 1) {
            xPos++;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.S) && xPos > 0 && yPos == 1) {
            xPos--;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.D) && yPos < numRooms - 1) {
            yPos++;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.A) && yPos > 0) {
            yPos--;
            movePosition();
        }

    }

    private void movePosition() {
        this.transform.position = positions[xPos, yPos].position;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Cambiar por las zonas de las salas
        if (collision.CompareTag("Item")) {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Heart")){
            Debug.Log("Te quito vida wey");
            PlayerStats.Instance.performDamage(collision.gameObject.GetComponent<ItemDrop>().damage);
            Destroy(collision.gameObject);
        }
    }
}
