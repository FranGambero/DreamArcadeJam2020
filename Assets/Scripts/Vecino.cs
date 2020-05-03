using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vecino : MonoBehaviour {
    public Transform calle;
    private RoomController habitacion;
    public bool moveHorizontal, leaving = false;
    private Vector3 nextRoom;
    private Queue<Vector3> tour;

    public SpriteRenderer spriteRenderer1, spriteRenderer2;
    public Sprite[] spriteArray;
    public Animator myAnimator;

    private float patrullaOffset = 2.222517f;
    private float yOffset = -.6755119f; // Josefa la cerda

    private Coroutine patrolCoroutine, blinkCoroutine;

    private int targetFloor, targetRoom, currentFloor;
    public int numEnfados;

    [ContextMenu("VIVA ZAPATERO")]
    public void initVecino() {
        numEnfados = 3;
        currentFloor = 0;
        myAnimator = GetComponent<Animator>();
        startSpriteAnim();

        tour = new Queue<Vector3>();
        tour.Enqueue(calle.position);
        moveHorizontal = leaving = false;

        Invoke(nameof(assignRoom), 1);
    }

    private void startSpriteAnim() {
        spriteArray = GameController.Instance.assignSpriteVecino();
        spriteRenderer1.sprite = spriteArray[0];
        spriteRenderer2.sprite = spriteArray[1];

        changeSpriteOrderInLayer();
    }

    private void changeSpriteOrderInLayer() {
        spriteRenderer1.sortingOrder = spriteRenderer2.sortingOrder = 420;
    }

    public void assignRoom() {
        // Primero buscamos planta, hay que checkear si no esta ocupado con el script de Nacho
        List<RoomController> habitasioneLibre = RoomManager.Instance.getRoomControllerList().FindAll(o => !o.HasNeighbor() && o.isAvailable());
        habitacion = habitasioneLibre[UnityEngine.Random.Range(0, habitasioneLibre.Count)];
        habitacion.SetNeighbor(this);

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
        } else if (tour.Count == 1) {
            nextRoom = tour.Dequeue();
            moveHorizontal = true;
            flipDirection();
        } else if (!leaving) {
            startPatrulla();
            habitacion.centipedesInMyVagina = true;
            habitacion.GetComponent<RoomController>().StartGeneratingIncome();
        }

        if (leaving && tour.Count <= 0) {
            StartCoroutine(setInactive());
        }
    }

    private void startPatrulla() {
        getOffsetPosition();
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
                myAnimator.Play("IdleAnim");

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

    internal void Rage() {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        Debug.Log("Enfadaaoooooooo");
        blinkCoroutine = StartCoroutine(Blink());
    }

    IEnumerator Blink() {
        Color auxColor = numEnfados == 1 ? Color.red : Color.white;
        for (int i = 0; i < 3; i++) {

            spriteRenderer1.color = auxColor;
            spriteRenderer2.color = auxColor;

            yield return new WaitForSeconds(.25f);
            Color color = new Color();
            color.a = 0;
            spriteRenderer1.color = color;
            spriteRenderer2.color = color;
            yield return new WaitForSeconds(.25f);
        }
        spriteRenderer1.color = Color.white;
        spriteRenderer2.color = Color.white;

    }

    public void toggleAnimation() {
        if (moveHorizontal || tour.Count <= 0) {
            myAnimator.Play("WalkAnim");
        } else {
            myAnimator.Play("IdleAnim");
        }
    }

    [ContextMenu("Mata vecino")]
    public void leaveRoom(bool enfadao = false) {
        if (enfadao) {
            PlayerStats.Instance.performDamage();
        }

        //habitacion.GetComponent<RoomController>().StopGeneratingIncome();
        habitacion.GetComponent<RoomController>().ResetRoom();
        currentFloor = targetFloor;

        leaving = true;
        StopCoroutine(patrolCoroutine);

        Transform planta;
        while (currentFloor >= 0) {
            planta = GameController.Instance.roomPosition[currentFloor, 1];
            tour.Enqueue(planta.position);
            currentFloor--;
        }

        tour.Enqueue(calle.position);

        // Selecciona hacia que lado va irse según donde estuviese mirando
        if (spriteRenderer1.flipX) {
            tour.Enqueue(calle.position + new Vector3(-14f, 0, 0));
        } else {
            tour.Enqueue(calle.position + new Vector3(14f, 0, 0));
        }

        nextRoom = tour.Dequeue();
        moveHorizontal = true;
        flipDirection();
    }

    [ContextMenu("Mata vecino test")]
    public void testLeave() {
        leaveRoom();
    }

    private IEnumerator setInactive() {
        flipDirection();
        yield return new WaitForSeconds(11);   // Lo que tarda en desaperecer una vez empieza a moverse por la calle

        this.gameObject.SetActive(false);
    }
}
