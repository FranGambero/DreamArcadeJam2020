using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour {
    private Transform hall;
    private Transform habitacion;
    public bool hasHabitacion; //Cambiar por algo mejor
    private Transform nextRoom;
    private Queue<Transform> tour;
    private int listIndex;


    private void Awake() {
        tour = new Queue<Transform>();
        listIndex = 0;
        hall = GameController.Instance.roomPosition[0, 1];
        tour.Enqueue(hall);
        hasHabitacion = false;

        Invoke(nameof(assignRoom), 1);
    }
    public void assignRoom() {
        // Primero buscamos planta, hay que checkear si no esta ocupado con el script de Nacho
        int randomFloor = UnityEngine.Random.Range(0, GameController.Instance.numFloors);
        int randomRoom = UnityEngine.Random.Range(0, GameController.Instance.numRooms); // Quitar que vaya al rellano (room == 1)

        if(randomRoom == 1) {
            randomRoom = (gameObject.GetInstanceID() % 2 == 0) ? 0 : 2;
        }

        //Ahora vamos a la planta
        Transform planta = GameController.Instance.roomPosition[randomFloor, 1];
        tour.Enqueue(planta);

        //Y finalmente a la propia habitacion
        habitacion = GameController.Instance.roomPosition[randomFloor, randomRoom];
        tour.Enqueue(habitacion);
        Debug.Log("Tu habitacion es: " + habitacion);
        hasHabitacion = true;
        selectNextRoom();

    }

    private void Update() {
        if (hasHabitacion) {
            transform.position =  Vector3.MoveTowards(transform.position, nextRoom.position, Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, nextRoom.position) < .25f) {
                selectNextRoom();
            }
        }
    }

    private void selectNextRoom() {
        if (tour.Count > 0) {
            nextRoom = tour.Dequeue();

        } else {
            //transform.position = hall.position;
            Debug.Log("He terminao :3");
            hasHabitacion = false;
        }
    }
}
