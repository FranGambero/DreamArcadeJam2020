using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public int damage = 1;
    private void Update() {
        this.transform.Translate(Vector3.down * Time.deltaTime * 2);
    }
}
