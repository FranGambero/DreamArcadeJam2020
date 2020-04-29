using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    #region Variables

    private int xRoom, yFloor;
    private Transform roomTrans;
    private Sprite roomSprite;

    private Vecino neighbor;

    public List<BreakdownType> availableBDTypes = new List<BreakdownType>{BreakdownType.Wrench, BreakdownType.Hammer, BreakdownType.Extinguisher};
    private List<Breakdown> breakdownList = new List<Breakdown>();
    public SpriteRenderer room_renderer;

    public List<GameObject> BD_SpawnList;

    public GameObject BD_Prefab;

    #endregion

    //TEST
    public bool generateBD = false;

    private void Awake()
    {
        room_renderer.sprite = roomSprite;
    }

    private void Update()
    {
        //TEST
        if(generateBD == true)
        {
            CreateBreakdown();
            generateBD = false;
        }
    }

    #region Setters

    public void SetRoomTransform(Transform newTransform)
    {
        roomTrans = newTransform;
    }

    public void SetRoomCoords(int xPos, int yPos)
    {
        this.xRoom = xPos;
        this.yFloor = yPos;
    }

    public void SetRoomSprite(Sprite sprite)
    {
        this.roomSprite = sprite;
        room_renderer.sprite = roomSprite;
    }

    public void SetRoom(Transform newTransform, int xPos, int yPos, Sprite sprite)
    {
        SetRoomTransform(newTransform);
        SetRoomCoords(xPos, yPos);
        SetRoomSprite(sprite);
    }

    public void SetNeighbor(Vecino newNeighbor)
    {
        if (!HasNeighbor())
        {
            neighbor = newNeighbor;
        }
    }

    #endregion

    public bool HasNeighbor()
    {
        return neighbor != null ? true : false;
    }

    #region Breakdown Creation
    private void CreateBreakdown()
    {
        if (BD_SpawnList.Count > 0)
        {
            Breakdown newBD = Instantiate(BD_Prefab, RandomSpawnPoint()).GetComponent<Breakdown>();
            newBD.AssignRandomBDType();
            breakdownList.Add(newBD);
        }
    }

    private Transform RandomSpawnPoint()
    {
        GameObject spawnPoint = BD_SpawnList[(int)Random.Range(0, BD_SpawnList.Count)];
        //GameObject spawnPoint = BD_SpawnList[0];
        BD_SpawnList.Remove(spawnPoint);
        return spawnPoint.transform;
    }

    private BreakdownType GetRandomBDType()
    {
        int randAux = (int)Random.Range(0, availableBDTypes.Count);
        availableBDTypes.Remove((BreakdownType)randAux);
        return (BreakdownType)randAux;
    }

    #endregion

    #region Breakdown Removal

    public void ReduceBDCountdown(KeyCode key)
    {
        Debug.Log("Se ha pulsado tecla");
        Breakdown auxBD;
        switch (key)
        {
            case (KeyCode.Alpha1):
                Debug.Log(" - usando wrench");
                //Reduce Wrench
                auxBD = ReduceBDCountdownByType(BreakdownType.Wrench);
                removeBD(auxBD);

                break;

            case (KeyCode.Alpha2):
                Debug.Log(" + usando hammer");
                //Reduce Hammer
                auxBD = ReduceBDCountdownByType(BreakdownType.Hammer);
                removeBD(auxBD);

                break;

            case (KeyCode.Alpha3):
                Debug.Log(" * usando extinguisher");
                //Reduce Extinguisher
                auxBD = ReduceBDCountdownByType(BreakdownType.Extinguisher);
                removeBD(auxBD);

                break;

            default:
                break;
        }
    }

    private void removeBD(Breakdown bd)
    {
        Debug.Log("Test removeBD: "+bd);
        if (bd != null)
        {
            // quitar de lista
            freeSpawnPoint(bd);
            breakdownList.Remove(bd);
            Destroy(bd.gameObject);
        }
    }

    private void freeSpawnPoint(Breakdown bd)
    {
        BD_SpawnList.Add(bd.transform.parent.gameObject);
    }

    private Breakdown ReduceBDCountdownByType(BreakdownType type)
    {
        foreach(Breakdown bd in breakdownList)
        {
            if((int)bd.GetBDType() == (int)type)
            {
                if (bd.ReduceCountdown() <= 0)
                {
                    Debug.Log("-- El CD es 0");
                    //availableBDTypes.Add(bd.GetBDType());
                    return bd;
                }
                break;
            }
        }
        return null;
    }

    #endregion

}
