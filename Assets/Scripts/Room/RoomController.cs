using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    #region Variables

    public int yRoom, xFloor;

    private Transform roomTrans;
    private Sprite roomSprite;
    private Vecino neighbor;
    public bool centipedesInMyVagina = false;
    public SpriteRenderer room_renderer;


    [Header("Breakdown")]

    public List<BreakdownType> availableBDTypes = new List<BreakdownType> { BreakdownType.Wrench, BreakdownType.Hammer, BreakdownType.Extinguisher };
    public List<GameObject> BD_SpawnList;
    public GameObject BD_Prefab;

    public float minTime, maxTime;

    private List<Breakdown> breakdownList = new List<Breakdown>();
    private Coroutine generateBreakdown_Routine;
    private int maxBreakdowns;

    [Header("Wrong Breakdown Punish")]
    public float punishTime = 0.5f;

    [Header("Anger")]
    private int timeForAnger = 4;


    [Header("Income")]
    public int maxPointsGain;
    public int punishPerBD;
    public float timeBetweeenIncome = 1f;

    private IEnumerator income_Routine;


    [Header("Progression")]
    public List<int> pointsForProgression;
    public int percentageToReduce = 30;


    #endregion

    private void Awake()
    {
        room_renderer.sprite = roomSprite;
        maxBreakdowns = BD_SpawnList.Count;
    }

    private void Start()
    {
        generateBreakdown_Routine = StartCoroutine(GenerateBreakdownsByTimer());
        StartCoroutine(FullBDReduceAngerTimer());
    }

    #region Setters

    public void SetRoomTransform(Transform newTransform)
    {
        roomTrans = newTransform;
    }

    public void SetRoomSprite(Sprite sprite)
    {
        this.roomSprite = sprite;
        room_renderer.sprite = roomSprite;

        if (yRoom == 2)
        {
            room_renderer.flipX = true;
        }
        else
        {
            room_renderer.flipX = false;
        }
    }

    public void SetRoom(Transform newTransform, int xPos, int yPos, Sprite sprite)
    {
        SetRoomTransform(newTransform);
        //SetRoomCoords(xPos, yPos);
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

    #region Getters

    public bool HasNeighbor()
    {
        return neighbor != null ? true : false;
    }

    public bool isAvailable()
    {
        return xFloor <= GrowBuilding.Instance.CurrentFloor + 1;
    }

    #endregion

    #region Room Management

    public void ResetRoom()
    {
        Debug.Log("Reset Room");
        centipedesInMyVagina = false;
        StopGenerateBreakdownsByTimer();
        StopGeneratingIncome();

        foreach (Breakdown bd in breakdownList)
        {
            StopAngerTimer(bd);
        }

        breakdownList.ForEach(o => ResetBD(o));

        breakdownList = new List<Breakdown>();
        neighbor = null;
        Debug.Log("Room Reseted ++");

        generateBreakdown_Routine = StartCoroutine(GenerateBreakdownsByTimer());
    }

    private void ResetBD(Breakdown bd)
    {
        freeSpawnPoint(bd);
        Destroy(bd.gameObject);
        Debug.Log("Reseting BD");
    }

    #endregion

    #region Breakdown Creation
    private void CreateBreakdown()
    {
        if (BD_SpawnList.Count > 0)
        {
            Breakdown newBD = Instantiate(BD_Prefab, RandomSpawnPoint()).GetComponent<Breakdown>();
            newBD.AssignRandomBDType();
            //newBD.Anger_Routine = StartCoroutine(ReduceAngerTimer(newBD));
            breakdownList.Add(newBD);
        }
    }

    private IEnumerator GenerateBreakdownsByTimer()
    {
        while (true)
        {
            if (!RoomManager.Instance.GODMODE)
            {
                if (HasNeighbor() && centipedesInMyVagina)
                {
                    CheckProgression();
                    yield return new WaitForSeconds(Random.Range(minTime, maxTime));
                    CreateBreakdown();
                }
                else
                    yield return null;
            } else {
                yield return null;
            }
        }
    }


    private void StopGenerateBreakdownsByTimer()
    {
        if (generateBreakdown_Routine != null)
            StopCoroutine(generateBreakdown_Routine);
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

    #region Breakdown Anger Management


    private void StopAngerTimer(Breakdown bd)
    {
        if (bd.Anger_Routine != null)
            StopCoroutine(bd.Anger_Routine);
    }

    private IEnumerator FullBDReduceAngerTimer()
    {

        float auxCountdown = this.timeForAnger;

        while (true)
        {
            if (breakdownList.Count < maxBreakdowns)
            {
                auxCountdown = this.timeForAnger;
                yield return null;
            }
            else if (breakdownList.Count >= maxBreakdowns)
            {
                yield return new WaitForSeconds(1f);
                auxCountdown -= 1;
                Debug.Log("++ TIEMPO PARA ENFADO "+auxCountdown);
                if(auxCountdown <= 0)
                {
                    auxCountdown = this.timeForAnger;
                    neighbor.Rage();
                    neighbor.numEnfados -= 1;

                    if (neighbor.numEnfados <= 0) {
                        neighbor.leaveRoom(true);
                    }
                }
            }
        }
    }
    #endregion

    #region Breakdown Removal

    public void ReduceBDCountdown(KeyCode key)
    {
        Debug.Log("Se ha pulsado tecla");
        Breakdown auxBD;
        if (!PlayerController.Instance.IsPunished)
        {
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
    }

    private void removeBD(Breakdown bd)
    {
        Debug.Log("Test removeBD: " + bd);
        if (bd != null)
        {
            // quitar de lista
            freeSpawnPoint(bd);
            StopAngerTimer(bd);
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

        bool rightBD = false;

        foreach (Breakdown bd in breakdownList)
        {
            if ((int)bd.GetBDType() == (int)type)
            {

                rightBD = true;

                if (bd.ReduceCountdown() <= 0)
                {
                    Debug.Log("-- El CD es 0");
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.Play("Fix", true);
                    //availableBDTypes.Add(bd.GetBDType());
                    return bd;
                }
                break;
            }
        }

        if (!rightBD)
            StartPunishPlayer();
       

        return null;
    }

    private void StartPunishPlayer()
    {
        StartCoroutine(PunishPlayer());
    }

    private IEnumerator PunishPlayer()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play("Fail", true);
        PlayerController.Instance.IsPunished = true;
        yield return new WaitForSeconds(punishTime);
        // PlayerController.Instance.IsPunished = false;
    }

    #endregion

    #region Income

    public void StartGeneratingIncome()
    {
        income_Routine = generateIncome();
        StartCoroutine(income_Routine);
    }


    public void StopGeneratingIncome()
    {
        if (income_Routine != null)
            StopCoroutine(income_Routine);
    }
    private IEnumerator generateIncome()
    {
        while (true)
        {
            RoomManager.Instance.totalPoints += CalculateIncome();
            yield return new WaitForSeconds(timeBetweeenIncome);
        }
    }

    private int CalculateIncome()
    {
        return Mathf.Clamp((maxPointsGain - (punishPerBD * breakdownList.Count)), 0, maxPointsGain);
    }

    private void CheckProgression()
    {
        if (pointsForProgression.Count != 0)
        {
            if (RoomManager.Instance.totalPoints >= pointsForProgression[0])
            {
                DoProgress();
                pointsForProgression.RemoveAt(0);
            }
        }
    }

    private void DoProgress()
    {
        maxTime = ((100 - percentageToReduce) * maxTime) / 100;
        minTime = ((100 - percentageToReduce) * minTime) / 100;
    }

    #endregion

}
