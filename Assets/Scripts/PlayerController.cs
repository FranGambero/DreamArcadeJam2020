using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {

    public int xPos, yPos;
    public Animator myAnimator;
    bool usingAxisUp = false;
    bool usingAxisLeft = false;
    bool usingAxisRigth = false;
    bool usingAxisDown = false;
    Coroutine handCoroutine;

    [Header("Sprites")]
    public SpriteRenderer sprite1;
    public SpriteRenderer sprite2;
    public GameObject Hand;
    public Sprite[] spriteArray;
    public Sprite wrenchSprite;
    public Sprite hammerSprite;
    public Sprite extinguisherSprite;

    public RoomController currentRoom;

    private void Start() {
        xPos = 0; // Floor number
        yPos = 1; // Room number

        spriteArray = GameController.Instance.assignSprite();

        summonPlayer();
    }

    public void summonPlayer() {
        sprite1.sprite = spriteArray[0];
        sprite2.sprite = spriteArray[1];

        //transform.position = GameController.Instance.roomPosition[xPos, yPos].position;
    }

    private void Update() {
        // Esto se puede mejorar, molaria sacar la llamada a MovePosition y hacerla común
        if (Input.GetAxisRaw("Vertical") > 0 && xPos < GameController.Instance.numFloors - 1 && yPos == 1) {
            if (!usingAxisUp) {
                usingAxisUp = true;
                xPos++;
                movePosition();
            }
        } else {
            usingAxisUp = false;
        }

        if (Input.GetAxisRaw("Vertical") < 0 && xPos > 0 && yPos == 1) {
            if (!usingAxisDown) {
                xPos--;
                movePosition();
                usingAxisDown = true;

            }
        } else {
            usingAxisDown = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0 && yPos < GameController.Instance.numRooms - 1) {
            if (!usingAxisRigth) {
                usingAxisRigth = true;
                yPos++;
                sprite1.flipX = false;
                sprite2.flipX = false;
                movePosition();
            }
        } else {
            usingAxisRigth = false;
            Debug.Log("False");
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && yPos > 0) {
            if (!usingAxisLeft) {
                usingAxisLeft = true;
                yPos--;
                sprite1.flipX = true;
                sprite2.flipX = true;
                movePosition();
            }
        } else {
            usingAxisLeft = false;
        }

        checkInputs();

    }

    private void checkInputs() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Pulsando wrench");
            if (currentRoom != null)
                currentRoom.ReduceBDCountdown(KeyCode.Alpha1);
            if (handCoroutine != null)
                StopCoroutine(handCoroutine);
            StartCoroutine(ShowHand(GetSprite(BreakdownType.Wrench)));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("Pulsando hammer");
            if (currentRoom != null)
                currentRoom.ReduceBDCountdown(KeyCode.Alpha2);
            if (handCoroutine != null)
                StopCoroutine(handCoroutine);
            StartCoroutine(ShowHand(GetSprite(BreakdownType.Hammer)));

        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("Pulsando extinguisher");
            if (currentRoom != null)
                currentRoom.ReduceBDCountdown(KeyCode.Alpha3);
            if (handCoroutine != null)
                StopCoroutine(handCoroutine);
            StartCoroutine(ShowHand(GetSprite(BreakdownType.Extinguisher)));

        }
    }

    private void movePosition() {
        this.transform.position = GameController.Instance.roomPosition[xPos, yPos].position;
        assignCurrentRoomController(xPos, yPos);
    }

    private void assignCurrentRoomController(int floorPos, int roomPos) {
        //currentRoom = GameController.Instance.roomPosition[floorPos, roomPos].GetComponentInChildren<RoomController>();
        currentRoom = RoomManager.Instance.getRoomController(floorPos, roomPos);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Cambiar por las zonas de las salas
        if (collision.CompareTag("Item")) {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Heart")) {
            Debug.Log("Te quito vida wey");
            PlayerStats.Instance.performDamage(collision.gameObject.GetComponent<ItemDrop>().damage);
            Destroy(collision.gameObject);
        }
    }
    private IEnumerator ShowHand(Sprite tool) {
        Hand.GetComponent<SpriteRenderer>().sprite = tool;
        Hand.SetActive(true);
        yield return new WaitForSeconds(.5f);
        Hand.SetActive(false);

    }
    public Sprite GetSprite(BreakdownType type) {
        Sprite sprite;
        switch (type) {
            case (BreakdownType.Wrench):
                sprite = wrenchSprite;
                break;

            case (BreakdownType.Hammer):
                sprite = hammerSprite;
                break;

            case (BreakdownType.Extinguisher):
                sprite = extinguisherSprite;
                break;

            default:
                sprite = null;
                break;
        }
        return sprite;
    }
}
