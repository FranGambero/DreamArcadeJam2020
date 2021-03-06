﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public static RoomManager Instance;

    #region VARIABLES
    public List<Sprite> roomSpriteList;
    public List<Transform> roomPositionList;
    public GameObject roomPrefab;

    public int totalPoints;

    private List<RoomController> roomsList = new List<RoomController>();

    private RoomController _rc;

    public bool GODMODE = false;

    //TEST
    public bool generateRoom = false;

    #endregion

    private void Awake() {
        Instance = this;
        totalPoints = 0;
        //generateAllRooms();
    }

    private void Update() {
        //// TEST
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Debug.Log("Pulsando wrench");
        //    _rc.ReduceBDCountdown(KeyCode.Alpha1);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    Debug.Log("Pulsando hammer");
        //    _rc.ReduceBDCountdown(KeyCode.Alpha2);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    Debug.Log("Pulsando extinguisher");
        //    _rc.ReduceBDCountdown(KeyCode.Alpha3);
        //}

        //if(generateRoom == true)
        //{
        //    GenerateRoom();
        //    generateRoom = false;
        //}
    }

    #region Room Generation
    public void GenerateRoom() {
        if (roomPositionList.Count != 0) {
            RoomController newRoom = Instantiate(roomPrefab, GetNextPoint()).GetComponent<RoomController>();
            newRoom.SetRoomSprite(GetRandomSprite());

            roomsList.Add(newRoom);

            _rc = newRoom; // Nice name
        }
    }

    public void GenerateRoom(Transform transform, int floorNum, int roomNum) {
        if (roomPositionList.Count != 0) {
            RoomController newRoom = Instantiate(roomPrefab, GetNextPoint()).GetComponent<RoomController>();
            newRoom.xFloor = floorNum;
            newRoom.yRoom = roomNum;

            newRoom.SetRoomSprite(GetRandomSprite());

            roomsList.Add(newRoom);

            _rc = newRoom; // Nice name
        }
    }

    public Sprite GetRandomSprite() {
        Sprite randAux = roomSpriteList[(int)Random.Range(0, roomSpriteList.Count - 1)];
        roomSpriteList.Remove(randAux);

        return randAux;
    }

    private Transform GetNextPoint() {
        Transform aux = roomPositionList[0];
        roomPositionList.Remove(aux);

        return aux;
    }

    public RoomController getRoomController(int floorNum, int roomNum) {
        return roomsList.Find(o => o.xFloor == floorNum && o.yRoom == roomNum);
    }

    public List<RoomController> getRoomControllerList() {
        return roomsList;
    }

    private void generateAllRooms() {
        for (int i = 0; i < roomPositionList.Count; i++) {
            //GenerateRoom(roomPositionList[i]);
        }
    }

    #endregion
}
