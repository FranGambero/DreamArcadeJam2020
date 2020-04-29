using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region VARIABLES
    public List<Sprite> roomSpriteList;
    public List<Transform> roomPositionList;
    public GameObject roomPrefab;

    private List<RoomController> roomsList = new List<RoomController>();

    private RoomController _rc;

    //TEST
    public bool generateRoom = false;

    #endregion

    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Pulsando wrench");
            _rc.ReduceBDCountdown(KeyCode.Alpha1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Pulsando hammer");
            _rc.ReduceBDCountdown(KeyCode.Alpha2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Pulsando extinguisher");
            _rc.ReduceBDCountdown(KeyCode.Alpha3);
        }

        if(generateRoom == true)
        {
            GenerateRoom();
            generateRoom = false;
        }
    }

#region Room Generation
    public void GenerateRoom()
    {
        if (roomPositionList.Count != 0)
        {
            RoomController newRoom = Instantiate(roomPrefab, GetNextPoint()).GetComponent<RoomController>();
            newRoom.SetRoomSprite(GetRandomSprite());

            roomsList.Add(newRoom);

            _rc = newRoom;
        }
    }

    private Sprite GetRandomSprite()
    {
        int randAux = (int)Random.Range(0, roomSpriteList.Count - 1);
        Debug.Log("Getting Sprite " + roomSpriteList[randAux].name);
        return roomSpriteList[randAux];
    }

    private Transform GetNextPoint()
    {
        Transform aux = roomPositionList[0];
        Debug.Log("Next Point "+aux);
        roomPositionList.Remove(aux);

        return aux;
    }

    #endregion
}
