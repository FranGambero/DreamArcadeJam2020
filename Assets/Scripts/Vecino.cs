using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour {
    public Transform calle;
    private RoomController habitacion;
    public bool moveHorizontal;
    private Vector3 nextRoom;
    private Queue<Vector3> tour;

    public SpriteRenderer spriteRenderer1, spriteRenderer2;
    public Sprite[] spriteArray;
    public Animator myAnimator;

    private float patrullaOffset = 2.222517f;
    private float yOffset = -.6755119f; // Josefa la cerda

    private Coroutine patrolCoroutine;

    private int targetFloor, targetRoom, currentFloor;

    [ContextMenu("VIVA ZAPATERO")]
    public void initVecino() {
        currentFloor = 0;
        myAnimator = GetComponent<Animator>();
        startSpriteAnim();

        tour = new Queue<Vector3>();
        tour.Enqueue(calle.position);
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
        List<RoomController> habitasioneLibre = RoomManager.Instance.getRoomControllerList().FindAll(o => !o.HasNeighbor());
        habitacion = habitasioneLibre[UnityEngine.Random.Range(0, habitasioneLibre.Count)];
        habitacion.SetNeighbor(this);

        //targetFloor = UnityEngine.Random.Range(0, GameController.Instance.numFloors);
        //targetRoom = UnityEngine.Random.Range(0, GameController.Instance.numRooms); // Quitar que vaya al rellano (room == 1)

        targetFloor = habitacion.xFloor;
        targetRoom = habitacion.yRoom;

        if (targetRoom == 1) {
            targetRoom = (gameObject.GetInstanceID() % 2 == 0) ? 0 : 2;
        }

        //Ahora vamos a la planta
        Transform planta;
        while (currentFloor <= targetFloor) {
            planta = GameController.Instance.roomPosition[currentFloor, 1];
            tour.Enqueue(planta.position);
            currentFloor++;
        }

        //Y finalmente a la propia habitacion
        tour.Enqueue(habitacion.transform.position + new Vector3(0, yOffset));

        moveHorizontal = true;

        nextRoom = tour.Dequeue();
        flipDirection();

    }

    private void flipDirection() {
        if (transform.position.x > nextRoom.x) {
            spriteRenderer1.flipX = spriteRenderer2.flipX = true;
        } else {
            spriteRenderer1.flipX = spriteRenderer2.flipX = false;
        }

        toggleAnimation();
    }

    private void Update() {
        if (moveHorizontal) {
            transform.position = Vector3.MoveTowards(transform.position, nextRoom, Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, nextRoom) < .25f) {
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
            transform.position = nextRoom;
            Invoke(nameof(selectNextRoom), 1);
        } else if(tour.Count == 1){
            nextRoom = tour.Dequeue();
            moveHorizontal = true;
            flipDirection();
        } else {
            Debug.Log("He terminao :3");
            // Llamar movimientos por la salica, Room Ignacio
            startPatrulla();
            habitacion.centipedesInMyVagina = true;
        }
    }

    private void startPatrulla() {
        getOffsetPosition();
        //StopCoroutine(patrolCoroutine);
        patrolCoroutine = StartCoroutine(patrulla());
    }

    private IEnumerator patrulla() {
        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, nextRoom, Time.deltaTime);
            if (Vector3.Distance(transform.position, nextRoom) <= .2f) {
                if (nextRoom == habitacion.transform.position + Vector3.up * yOffset) {
                    getOffsetPosition();
                } else {
                    nextRoom = habitacion.transform.position + Vector3.up * yOffset;
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 4f));
                flipDirection();
            }
            yield return new WaitForEndOfFrame();

        }
    }

    private void getOffsetPosition() {
        if (habitacion.yRoom == 0) {
            nextRoom = habitacion.transform.position - Vector3.right * patrullaOffset;
        } else {
            nextRoom = habitacion.transform.position - Vector3.left * patrullaOffset;
        }
        nextRoom += Vector3.up * yOffset;
    }

    public void toggleAnimation() {
        if (moveHorizontal || tour.Count <= 0) {
            myAnimator.Play("WalkAnim");
        } else {
            myAnimator.Play("IdleAnim");
        }
    }
}
