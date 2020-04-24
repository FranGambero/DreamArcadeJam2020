using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int indexPosition = 1;
    public Transform[] positions;

    private void Start() {
        startPosition();
    }

    private void startPosition() {
        this.transform.position = positions[indexPosition].position;
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.D) && indexPosition < positions.Length - 1) {
            indexPosition++;
            movePosition(indexPosition);
        } else if (Input.GetKeyDown(KeyCode.A) && indexPosition > 0) {
            indexPosition--;
            movePosition(indexPosition);
        }
    }

    private void movePosition(int nextPosition) {
        this.transform.position = positions[nextPosition].position;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
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
