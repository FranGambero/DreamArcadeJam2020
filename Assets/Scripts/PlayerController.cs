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
    private bool touchUp = false;
    private bool touchDown = false;
    private bool touchLeft = false;
    private bool touchRigth = false;
    private bool canTouch;
    float pointer_x;
    float pointer_y;

    public bool IsPunished {
        get => isPunished; set {

            isPunished = value;
        }
    }

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
        if (PlayerStats.Instance.currentLifes > 0) {
            //pointer_x = Input.GetAxisRaw("Mouse X");
            //pointer_y = Input.GetAxisRaw("Mouse Y");
            //if (Input.touchCount > 0) {
            //    pointer_x = Input.touches[0].deltaPosition.x;
            //    pointer_y = Input.touches[0].deltaPosition.y;
            //} else {
            //    canTouch = true;
            //}
            Debug.Log("X " + pointer_x+" Y " + pointer_y);

            TryMoveUp();

            TryMoveDown();

            TryMoveLeft();

            TryMoveRight();





            checkInputs();
        }

    }

    public void TryMoveLeft(bool button = false) {
        if ((button || (pointer_x > pointer_y && pointer_x < 0 && canTouch) || Input.GetAxisRaw("Horizontal") < 0) && roomNumber > 0) {
            if (!usingAxisLeft || !touchRigth) {
                canTouch = false;
                touchRigth = true;
                usingAxisLeft = true;
                sprite1.flipX = true;
                sprite2.flipX = true;
                movePosition(floorNumber, roomNumber - 1);
            }
        } else {
            touchRigth = false;
            usingAxisLeft = false;
        }
    }

    public void TryMoveRight(bool button = false) {
        if ((button || (pointer_x > pointer_y && pointer_x > 0 && canTouch) || Input.GetAxisRaw("Horizontal") > 0) && roomNumber < GameController.Instance.numRooms - 1) {
            if (!usingAxisRigth || !touchLeft) {
                canTouch = false;
                touchLeft = true;
                usingAxisRigth = true;
                sprite1.flipX = false;
                sprite2.flipX = false;
                movePosition(floorNumber, roomNumber + 1);
            }
        } else {
            touchLeft = false;
            usingAxisRigth = false;
        }
    }

    public void TryMoveUp(bool button = false) {
        if ((button || (pointer_y > pointer_x && pointer_y > 0 && canTouch) || Input.GetAxisRaw("Vertical") > 0) && floorNumber < GameController.Instance.numFloors - 1 && roomNumber == 1) {
            //if(floorNumber > GrowBuilding.Instance.CurrentFloor + 1)
            if (!usingAxisUp || !touchUp) {
                canTouch = false;
                touchUp = true;
                usingAxisUp = true;
                movePosition(floorNumber + 1, roomNumber);
            }
        } else {
            usingAxisUp = false;
            touchUp = false;
        }
    }
    public void TryMoveDown(bool button = false) {
        if ((button || (pointer_y > pointer_x && pointer_y < 0 && canTouch) || Input.GetAxisRaw("Vertical") < 0) && floorNumber > 0 && roomNumber == 1) {
            if (!usingAxisDown || !touchDown) {
                canTouch = false;
                movePosition(floorNumber - 1, roomNumber);
                touchDown = true;
                usingAxisDown = true;

            }
        } else {
            usingAxisDown = false;
            touchDown = false;
        }
    }
    private void checkInputs() {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            KeyPressed(KeyCode.Alpha1, BreakdownType.Wrench);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            KeyPressed(KeyCode.Alpha2, BreakdownType.Hammer);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
            KeyPressed(KeyCode.Alpha3, BreakdownType.Extinguisher);
        }
    }
    public void KeyPressed(int i) {
        switch (i) {
            case 0:
                KeyPressed(KeyCode.Alpha1, BreakdownType.Wrench);
                break;
            case 1:
                KeyPressed(KeyCode.Alpha2, BreakdownType.Hammer);
                break;
            case 2:
                KeyPressed(KeyCode.Alpha3, BreakdownType.Extinguisher);
                break;
            default:
                break;
        }
    }
    private void KeyPressed(KeyCode key, BreakdownType b_Type) {
        if (!IsPunished) {

            if (currentRoom != null)
                currentRoom.ReduceBDCountdown(key);
            if (handCoroutine != null)
                StopCoroutine(handCoroutine);
            UIController.Instance.ClickToolButton(b_Type);
            if (handCoroutine != null)
                StopCoroutine(handCoroutine);
            handCoroutine = StartCoroutine(ShowHand(GetSprite(b_Type)));

        }
    }

    private void movePosition(int tmpFloorPos, int tmpRoomPos) {
        if (tmpFloorPos <= GrowBuilding.Instance.CurrentFloor + 1) {
            if (tmpRoomPos == 1 && this.floorNumber != tmpFloorPos) {
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

    private IEnumerator ShowHand(Sprite tool) {
        Hand.GetComponent<SpriteRenderer>().sprite = tool;
        Hand.SetActive(true);
        yield return new WaitForSeconds(.5f);
        Hand.SetActive(false);
        IsPunished = false;

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
