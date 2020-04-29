using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakdown : MonoBehaviour
{
    #region VARIABLES
    public int maxCountdown = 1;
    public int minCountdown = 1;

    public SpriteRenderer _sr;

    public Sprite wrenchSprite;
    public Sprite hammerSprite;
    public Sprite extinguisherSprite;

    private BreakdownType bd_type;
    private int bd_countdown;

    #endregion

    public void Awake()
    {
        bd_type = BreakdownType.Undefined;
        AssignCountdown();
    }

    #region Getters

    public BreakdownType GetBDType()
    {
        return bd_type;
    }

    public int GetBDCountdown()
    {
        return bd_countdown;
    }

    #endregion

    #region TYPE
    public void AssignRandomBDType()
    {
        int randAux = (int)Random.Range(0, System.Enum.GetValues(typeof(BreakdownType)).Length-1);
        bd_type = (BreakdownType)randAux;

        AssignSpriteByType();
    }

    public void AssignUnusedBDType(BreakdownType unusedBDType)
    {
        bd_type = unusedBDType;
    }

    private void AssignSpriteByType()
    {
        switch (bd_type)
        {
            case (BreakdownType.Wrench):
                _sr.sprite = wrenchSprite;
                break;

            case (BreakdownType.Hammer):
                _sr.sprite = hammerSprite;
                break;

            case (BreakdownType.Extinguisher):
                _sr.sprite = extinguisherSprite;
                break;

            default:
                break;

        }
    }

    #endregion

    #region COUNTDOWN

    private void AssignCountdown()
    {
        bd_countdown = Random.Range(minCountdown, maxCountdown);
        Debug.Log("Countdown: "+bd_countdown);
    }

    public int ReduceCountdown()
    {
        bd_countdown -= 1;
        Debug.Log("Se ha reducido el countdown a "+bd_countdown);
        return bd_countdown;
    }

    #endregion

}
