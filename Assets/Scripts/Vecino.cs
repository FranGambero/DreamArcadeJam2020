using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour
{
    private Transform hall;
    private Transform habitacion;
    public bool hasHabitacion;


    private void Awake() {
        hall = PlayerController.Instance.positions[0, 1];
        Debug.Log("Me despertaste wei");
        hasHabitacion = false;
        assignRoom(Random.Range(1, 3));
    }
    public void assignRoom(int nextHabitacion) {
        habitacion = PlayerController.Instance.positions[nextHabitacion, 0];
        Debug.Log("La habias" + habitacion.position);
        transform.position = habitacion.position;
        hasHabitacion = true;
    }

    private void Update() {
        if (hasHabitacion) {
                                     // bruh
        }
    }
}
