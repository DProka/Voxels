using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyRewardScreen : MonoBehaviour
{
    [SerializeField] Transform prefabParent;

    private Canvas mainCanvas;
    private DailyRewardPrefab[] dayArray;

    [Header("Settings")]

    [SerializeField] int[] rewardArray;

    private int[] dayStatusArray;
    private int dayNum;

    public void Init()
    {
        mainCanvas = GetComponent<Canvas>();
        mainCanvas.enabled = false;

        GetDayList();
    }

    public void GetReward(int dayNum)
    {
        EventBus.updatePalyerCoins?.Invoke(rewardArray[dayNum]);
        dayStatusArray[dayNum] = 2;
    }

    private void GetDayList()
    {
        LoadDayStatusArray();

        dayArray = new DailyRewardPrefab[prefabParent.childCount];

        Debug.Log("Day array " + dayArray.Length);

        for (int i = 0; i < dayArray.Length; i++)
        {
            dayArray[i] = prefabParent.GetChild(i).GetComponent<DailyRewardPrefab>();
            dayArray[i].Init(this, i + 1, rewardArray[i], dayStatusArray[i]);
        }
    }

    private void LoadDayStatusArray()
    {
        dayStatusArray = new int[6];
        dayStatusArray[0] = 2;
        dayStatusArray[1] = 1;
        dayStatusArray[2] = 1;

        //CheckCurrentDayNum();
    }

    private void CheckCurrentDayNum()
    {
        if (dayNum < rewardArray.Length - 1)
            dayNum += 1;
        else
            dayNum = 0;

        for (int i = 0; i < dayNum; i++)
        {
            if (dayStatusArray[i] == 0)
                dayStatusArray[i] = 1;
        }
    }

    #region Main Window

    public void SwitchCanvas()
    {
        if (mainCanvas.enabled)
        {
            EventBus.switchPlayerStats?.Invoke(false);
            mainCanvas.enabled = false;
        }
        else
        {
            EventBus.switchPlayerStats?.Invoke(true);
            mainCanvas.enabled = true;
        }
    }

    public void CloseScreen()
    {
        mainCanvas.enabled = false;
    }
    #endregion
}
