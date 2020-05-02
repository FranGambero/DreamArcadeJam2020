using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {

    public int floorNumber, roomNumber;
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

    private bool isPunished;

    public bool IsPunished { get => isPunished; set => isPunished = value; }

    private void Start() {
        floorNumber = 0; // Floor number
        roomNumber = 1; // Room number

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
        if (Input.GetAxisRaw("Vertical") > 0 && floorNumber < GameController.Instance.numFloors - 1 && roomNumber == 1) {
            //if(floorNumber > GrowBuilding.Instance.CurrentFloor + 1)
            if (!usingAxisUp) {
                usingAxisUp = true;
                movePosition(floorNumber + 1, roomNumber);
            }
        } else {
            usingAxisUp = false;
        }

        if (Input.GetAxisRaw("Vertical") < 0 && floorNumber > 0 && roomNumber == 1) {
            if (!usingAxisDown) {
                movePosition(floorNumber - 1, roomNumber);
                usingAxisDown = true;

            }
        } else {
            usingAxisDown = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0 && roomNumber < GameController.Instance.numRooms - 1) {
            if (!usingAxisRigth) {
                usingAxisRigth = true;
                sprite1.flipX = false;
                sprite2.flipX = false;
                movePosition(floorNumber, roomNumber + 1);
            }
        } else {
            usingAxisRigth = false;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && roomNumber > 0) {
            if (!usingAxisLeft) {
                usingAxisLeft = true;
                sprite1.flipX = true;
                sprite2.flipX = true;
                movePosition(floorNumber, roomNumber - 1);
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
        if (handCoroutine != null)
            StopCoroutine(handCoroutine);
        handCoroutine = StartCoroutine(ShowHand(GetSprite(b_Type)));

    }

    private void movePosition(int tmpFloorPos, int tmpRoomPos) {
        if (tmpFloorPos <= GrowBuilding.Instance.CurrentFloor + 1) {
            if (tmpRoomPos == 1 && this.floorNumber != tmpFloorPos) {
                Debug.Log("Hola");
                GameController.Instance.Elevators[tmpFloorPos].GetComponentInChildren<SpriteRenderer>().sprite = GameController.Instance.OpenElevator;
                GameController.Instance.Elevators[this.floorNumber].GetComponentInChildren<SpriteRenderer>().sprite = GameController.Instance.ClosedElevator;
            }
            this.floorNumber = tmpFloorPos;
            this.roomNumber = tmpRoomPos;

            this.transform.position = GameController.Instance.roomPosition[tmpFloorPos, tmpRoomPos].position;
            assignCurrentRoomController(tmpFloorPos, tmpRoomPos);
        }
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
