
using UnityEngine;
using TMPro;

public class DailyRewardPrefab : MonoBehaviour
{
    private UIDailyRewardScreen mainScreen;
    private int dayNum;
    private int status;

    [Header("Day Text")]

    [SerializeField] TextMeshProUGUI[] dayNumTextArray;
    
    [Header("Reward Text")]

    [SerializeField] TextMeshProUGUI[] rewardTextArray;

    private int reward;

    [Header("Parts")]

    [SerializeField] GameObject[] partsArray;

    public void Init(UIDailyRewardScreen screen, int num, int _reward, int _status)
    {
        mainScreen = screen;

        dayNum = num;
        SetTextInArray(dayNumTextArray, "Day " + dayNum);

        reward = _reward;
        SetTextInArray(rewardTextArray, "+" + reward);

        SwitchStatus(_status);
    }

    public void GetReward()
    {
        if(status == 1)
        {
            SwitchStatus(2);
            mainScreen.GetReward(dayNum - 1);
        }
    }

    public void SwitchStatus(int _status)
    {
        status = _status;

        foreach (GameObject obj in partsArray)
        {
            obj.SetActive(false);
        }

        partsArray[status].SetActive(true);
    }

    private void SetTextInArray(TextMeshProUGUI[] array, string text)
    {
        foreach(TextMeshProUGUI part in array)
        {
            part.text = text;
        }
    }
}
