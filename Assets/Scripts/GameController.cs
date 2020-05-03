using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController> {
    public int numRooms, numFloors;
    public Transform[,] roomPosition;
    public Transform[] habitaciones;
    public Transform[] Elevators;
    [SerializeField]
    public Sprite[,] spritevecinos;
    public Sprite ClosedElevator, OpenElevator;
    private int roomIndex;

    public List<Sprite> spritesElegidos;
    public List<Sprite> spritesLibres, spritesCaserxLibres;
    public string PLAYER_SELECTED_KEY = "PLAYER_SELECTED_KEY";

    public float timer, maxTime, minTime;
    public List<GameObject> listaVecinos;
    private int numVecinosActivos;
    public bool isMainMenu = false;

    private void Awake() {
        numRooms = 3;
        numFloors = 5;
        roomIndex = 0;
        numVecinosActivos = 0;
        roomPosition = new Transform[numFloors, numRooms];

        maxTime = 8; // Mismo que lo de abajo pero maximo
        minTime = 5; // Tiempo minimo que tardará en aparecer el vecino
        timer = 0;

    }

    private void Start() {
        fillMatrix();
    }

    private void Update() {
        timer += Time.deltaTime;
        numVecinosActivos = RoomManager.Instance.getRoomControllerList().FindAll(o => o.HasNeighbor()).Count;
        bool habitacionesLibres = RoomManager.Instance.getRoomControllerList().FindAll(o => !o.HasNeighbor() && o.isAvailable()).Count > 0;
        if (timer >= maxTime && habitacionesLibres){ 
            spawnVecino();
            newTimer();
        }
    }

    private void newTimer() {
        timer = 0;
        maxTime = Random.Range(minTime, minTime + numVecinosActivos*2); // Curva de evolucion del tiempo que tardan en aparecer los vecinos, habria que adaptarla
    }

    private void fillMatrix() {
        for (int i = 0; i <= numFloors - 1; i++) {
            for (int j = 0; j <= numRooms - 1; j++) {
                roomPosition[i, j] = habitaciones[roomIndex].transform;
                habitaciones[roomIndex].transform.position = roomPosition[i, j].position;
                if (j != 1) {
                    RoomManager.Instance.GenerateRoom(habitaciones[roomIndex].transform, i, j);
                }
                roomIndex++;
            }
        }

       // GrowBuilding.Instance.SetInPostion(0);
    }

    public Sprite[] assignSpriteVecino() {
        Sprite[] arrayResult = new Sprite[2];

        int spriteIndex = UnityEngine.Random.Range(0, spritesLibres.Count);

        // Modificar si aumentaramos la cantidad de sprites por animacion
        if (spriteIndex % 2 != 0) {
            spriteIndex--;
        }

        for (int i = 0; i < arrayResult.Length; i++) {
            arrayResult[i] = spritesLibres[spriteIndex + i];
        }

        spritesElegidos.AddRange(arrayResult);
        spritesLibres.RemoveRange(spriteIndex, 2);

        return arrayResult;
    }

    public Sprite[] assignSpritePlayer() {
        Sprite[] arrayResult = new Sprite[2];

        int spriteIndex = PlayerPrefs.GetInt(PLAYER_SELECTED_KEY, -1);

        if (spriteIndex == -1) {
            arrayResult = assignSpriteVecino();
        } else {

            for (int i = 0; i < arrayResult.Length; i++) {
                arrayResult[i] = spritesCaserxLibres[spriteIndex + i];
            }
        }

        return arrayResult;
    }

    private void spawnVecino() {
        // Esto tampoco funciona :(
        if (listaVecinos.Count >= numVecinosActivos) {
            Debug.LogWarning("Total vecinos " + listaVecinos.Count + " y hay: " + numVecinosActivos);
            GameObject nextVecino = listaVecinos.Find(item => item.activeInHierarchy == false);
            nextVecino.SetActive(true);
            nextVecino.GetComponent<Vecino>().initVecino();
        }
    }
}
