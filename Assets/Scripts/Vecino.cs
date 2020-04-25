using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour
{
    private Transform habitacion;


    private void Awake() {
        Debug.Log("Me despertaste wei");
        assignRoom(Random.Range(1, 3));
    }
    public void assignRoom(int nextHabitacion) {
        Debug.Log("La habias" + nextHabitacion);
        habitacion = PlayerController.Instance.positions[nextHabitacion, 0];
        transform.position = habitacion.position;
    }

    private void Update() {
        if (habitacion) {
            //transform.Translate(habitacion.position * Time.deltaTime * 2);
        }
    }
}
