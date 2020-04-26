using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public int numRooms, numFloors;
    public Transform[,] roomPosition;
    public Transform[] habitaciones;
    [SerializeField]
    public Sprite[,] spritevecinos;
    private int roomIndex;

    public List<Sprite> spritesElegidos;
    public List<Sprite> spritesLibres;


    public float timer, maxTime;
    public List<GameObject> listaVecinos;
    private int numVecinosActivos;

    private void Start() {
        maxTime = 5;
        timer = 0;

    }


    private void Awake() {
        numRooms = 3;
        numFloors = 4;
        roomIndex = 0;
        numVecinosActivos = 0;
        roomPosition = new Transform[numFloors, numRooms];

        fillMatrix();
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer >= maxTime && numVecinosActivos < 3) { // Los vecinos maximos que queramos mostrar de momento
            numVecinosActivos++;
            spawnVecino();
            timer = 0;
        }

    }

    private void fillMatrix() {
        for (int i = 0; i <= numFloors - 1; i++) {
            for (int j = 0; j <= numRooms - 1; j++) {
                roomPosition[i, j] = habitaciones[roomIndex];
                habitaciones[roomIndex].position = roomPosition[i, j].position;
                roomIndex++;
            }
        }
    }

    public Sprite[] assignSprite() {
        Sprite[] arrayResult = new Sprite[2];

        int spriteIndex = Random.Range(0, spritesLibres.Count);

        // Modificar si aumentaramos la cantidad de sprites por animacion
        if(spriteIndex % 2 != 0) {
            spriteIndex--;
        }

        for (int i = 0; i < arrayResult.Length; i++) {
            arrayResult[i] = spritesLibres[spriteIndex + i];
        }

        spritesElegidos.AddRange(arrayResult);
        spritesLibres.RemoveRange(spriteIndex, 2);

        return arrayResult;
    }

    private void spawnVecino() {
        listaVecinos.Find(item => item.activeInHierarchy == false).SetActive(true);
    }
}
