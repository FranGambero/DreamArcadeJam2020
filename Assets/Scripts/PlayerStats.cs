using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxLifes, currentLifes;

    private void Awake() {
        maxLifes = 5;
        currentLifes = 3;
    }

}
