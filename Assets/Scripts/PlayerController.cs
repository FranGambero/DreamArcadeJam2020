using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {

    public int xPos, yPos;
    public SpriteRenderer sprite1, sprite2;
    public Sprite[] spriteArray;
    public Animator myAnimator;

    private void Start() {
        xPos = 0; // Floor number
        yPos = 1; // Room number

        spriteArray = GameController.Instance.assignSprite();

        summonPlayer();
    }

    public void summonPlayer() {
        sprite1.sprite = spriteArray[0];
        sprite2.sprite = spriteArray[1];

        transform.position = GameController.Instance.roomPosition[xPos, yPos].position;
    }

    private void Update() {
        // Esto se puede mejorar, molaria sacar la llamada a MovePosition y hacerla común
        if (Input.GetKeyDown(KeyCode.W) && xPos < GameController.Instance.numFloors - 1 && yPos == 1) {
            xPos++;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.S) && xPos > 0 && yPos == 1) {
            xPos--;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.D) && yPos < GameController.Instance.numRooms - 1) {
            yPos++;
            sprite1.flipX = false;
            sprite2.flipX = false;
            movePosition();
        } else if (Input.GetKeyDown(KeyCode.A) && yPos > 0) {
            yPos--;
            sprite1.flipX = true;
            sprite2.flipX = true;
            movePosition();
        }

    }

    private void movePosition() {
        this.transform.position = GameController.Instance.roomPosition[xPos, yPos].position;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Cambiar por las zonas de las salas
        if (collision.CompareTag("Item")) {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Heart")){
            Debug.Log("Te quito vida wey");
            PlayerStats.Instance.performDamage(collision.gameObject.GetComponent<ItemDrop>().damage);
            Destroy(collision.gameObject);
        }
    }
}
