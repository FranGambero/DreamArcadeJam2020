using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    public int maxLifes, currentLifes;
    public TextMeshProUGUI lifesText;

    private void Awake() {
        maxLifes = 5;
        currentLifes = 2;

        assignLifes();
    }

    private void assignLifes() {
        lifesText.text = currentLifes.ToString();
    }

    public void performDamage(int damage) {
        currentLifes -= damage;

        assignLifes();

        if(currentLifes <= 0) {
            marryPlayer();
        }
    }

    private void marryPlayer() {
        Debug.Log("Te casaste, la cagaste");
    }
}
