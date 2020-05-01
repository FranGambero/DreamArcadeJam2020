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

        spriteArray = GameController.Instance.assignSpritePlayer();

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
                movePosition(xPos + 1, yPos);
            }
        } else {
            usingAxisUp = false;
        }

        if (Input.GetAxisRaw("Vertical") < 0 && xPos > 0 && yPos == 1) {
            if (!usingAxisDown) {
                movePosition(xPos - 1, yPos);
                usingAxisDown = true;

            }
        } else {
            usingAxisDown = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0 && yPos < GameController.Instance.numRooms - 1) {
            if (!usingAxisRigth) {
                usingAxisRigth = true;
                sprite1.flipX = false;
                sprite2.flipX = false;
                movePosition(xPos, yPos + 1);
            }
        } else {
            usingAxisRigth = false;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && yPos > 0) {
            if (!usingAxisLeft) {
                usingAxisLeft = true;
                sprite1.flipX = true;
                sprite2.flipX = true;
                movePosition(xPos, yPos - 1);
            }
        } else {
            usingAxisLeft = false;
        }

        checkInputs();

    }

    private void checkInputs() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Pulsando wrench");

            KeyPressed(KeyCode.Alpha1, BreakdownType.Wrench);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("Pulsando hammer");

            KeyPressed(KeyCode.Alpha2, BreakdownType.Hammer);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("Pulsando extinguisher");
            KeyPressed(KeyCode.Alpha3, BreakdownType.Extinguisher);
        }
    }
    private void KeyPressed(KeyCode key, BreakdownType b_Type) {
        if (currentRoom != null)
            currentRoom.ReduceBDCountdown(key);
        if (handCoroutine != null)
            StopCoroutine(handCoroutine);
        UIController.Instance.ClickToolButton(b_Type);
        StartCoroutine(ShowHand(GetSprite(b_Type)));

    }

    private void movePosition(int xPos, int yPos) {
        Debug.Log(this.yPos + "y: " + yPos + ", " + this.xPos + "x: " + xPos);
        if (yPos == 1 && this.xPos != xPos) {
            Debug.Log("Hola");
            GameController.Instance.Elevators[xPos].GetComponentInChildren<SpriteRenderer>().sprite = GameController.Instance.OpenElevator;
            GameController.Instance.Elevators[this.xPos].GetComponentInChildren<SpriteRenderer>().sprite = GameController.Instance.ClosedElevator;
        }
        this.xPos = xPos;
        this.yPos = yPos;

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
