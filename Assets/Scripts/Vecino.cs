using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour {
    public Transform calle;
    private Transform habitacion;
    public bool moveHorizontal;
    private Transform nextRoom;
    private Queue<Transform> tour;

    public SpriteRenderer spriteRenderer1, spriteRenderer2;
    public Sprite[] spriteArray;
    public Animator myAnimator;

    private int targetFloor, targetRoom, currentFloor;

    [ContextMenu("VIVA ZAPATERO")]
    public void initVecino() {
        currentFloor = 0;
        myAnimator = GetComponent<Animator>();
        startSpriteAnim();

        tour = new Queue<Transform>();
        tour.Enqueue(calle);
        moveHorizontal = false;

        Invoke(nameof(assignRoom), 1);
    }

    private void startSpriteAnim() {
        spriteArray = GameController.Instance.assignSprite();
        spriteRenderer1.sprite = spriteArray[0];
        spriteRenderer2.sprite = spriteArray[1];

        spriteRenderer1.sortingOrder = spriteRenderer2.sortingOrder = 420;
    }
    public void assignRoom() {
        // Primero buscamos planta, hay que checkear si no esta ocupado con el script de Nacho
        targetFloor = UnityEngine.Random.Range(0, GameController.Instance.numFloors);
        targetRoom = UnityEngine.Random.Range(0, GameController.Instance.numRooms); // Quitar que vaya al rellano (room == 1)

        if (targetRoom == 1) {
            targetRoom = (gameObject.GetInstanceID() % 2 == 0) ? 0 : 2;
        }

        //Ahora vamos a la planta
        Transform planta;
        while (currentFloor <= targetFloor) {
            planta = GameController.Instance.roomPosition[currentFloor, 1];
            tour.Enqueue(planta);
            currentFloor++;
        }

        //Y finalmente a la propia habitacion
        habitacion = GameController.Instance.roomPosition[targetFloor, targetRoom];
        tour.Enqueue(habitacion);
        Debug.Log("Tu habitacion es: " + habitacion);

        moveHorizontal = true;

        nextRoom = tour.Dequeue();
        flipDirection();

    }

    private void flipDirection() {
        if (transform.position.x > nextRoom.position.x) {
            spriteRenderer1.flipX = spriteRenderer2.flipX = true;
        } else {
            spriteRenderer1.flipX = spriteRenderer2.flipX = false;
        }

        toggleAnimation();
    }

    private void Update() {
        if (moveHorizontal) {
            transform.position = Vector3.MoveTowards(transform.position, nextRoom.position, Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, nextRoom.position) < .25f) {
                spriteRenderer1.sortingOrder = spriteRenderer2.sortingOrder = 1;
                moveHorizontal = false;
                toggleAnimation();
                Invoke(nameof(selectNextRoom), .5f);
            }
        }
    }

    private void selectNextRoom() {
        if (tour.Count > 1) {
            nextRoom = tour.Dequeue();
            transform.position = nextRoom.position;
            Invoke(nameof(selectNextRoom), 1);
        } else if(tour.Count == 1){
            nextRoom = tour.Dequeue();
            moveHorizontal = true;
            flipDirection();
        } else {
            Debug.Log("He terminao :3");
            // Llamar movimientos por la salica, Room Ignacio
        }
    }

    public void toggleAnimation() {
        if (moveHorizontal || tour.Count <= 0) {
            myAnimator.Play("WalkAnim");
        } else {
            myAnimator.Play("IdleAnim");
        }
    }
}
