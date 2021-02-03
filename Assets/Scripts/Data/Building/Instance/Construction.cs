using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Data;
using CT.UI;
using CT.Manager;
using CT.Net;

public class Construction : MonoBehaviour
{
    public ConstructionData Data { get; private set; }
    public GameTime Countdown => countdown;

    public float FillRatio => 1f - Countdown.TotalHours / startTime.TotalHours;

    GameObject fx;

    public GameObject timeUI;
    public GameTimeTextManager timeTextManager;

    GameTime startTime;

    public int RushGemCost => countdown.GemCost;

    GameTime countdown;
    IEnumerator countdownRoutine;

    public bool IsBusy => Data != null;

    public void Init(ConstructionData data)
    {
        Data = data;
        countdown = data.time;
        startTime = data.startTime;
        fx = Instantiate(data.fx, transform.position, transform.rotation, transform);
        transform.position = GridManager.ToPosition(data.x, data.y);
        transform.rotation = data.rotation;
        StartCoroutine((countdownRoutine = CountdownRoutine()));
    }

    IEnumerator CountdownRoutine()
    {
        timeTextManager.SetTime(countdown);
        timeUI.SetActive(true);
        while (countdown.TotalSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            countdown.seconds--;
            if (countdown.seconds == -1)
            {
                countdown.seconds = 59;
                countdown.minutes--;
                if (countdown.minutes == -1)
                {
                    countdown.minutes = 59;
                    countdown.hours--;
                }
            }
            timeTextManager.SetTime(countdown);
        }
        CompleteConstruction();
    }

    void CompleteConstruction()
    {
        timeUI.SetActive(false);
        Destroy(fx);
        GameManager.instance.FinishConstruction(Data);
        ConstructionManager.instance.OnConstructionCompleted(this);
        Data = null;
        Destroy(gameObject);
    }

    public void Cancel()
    {
        StopCoroutine(countdownRoutine);
        ConstructionManager.instance.OnConstructionCancelled(this);
        Destroy(gameObject);
    }

    public void Rush()
    {
        StopCoroutine(countdownRoutine);
        if(countdown > GameTime.FreeRushTime)
        {
            int gems = RushGemCost;
            var player = GameManager.Player;
            player.gems -= gems;
            ClientSend.SubtractGems(player.username, gems);
        }
        CompleteConstruction();
    }
}
