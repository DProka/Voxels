
using UnityEngine;
using TMPro;

public class UIChallengeAlbum : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerCoinText;
    [SerializeField] TextMeshProUGUI playerCoinShadowText;
    [SerializeField] Transform prefabParent;
    [SerializeField] ModelsBase modelsBase;

    private UISave uiSave;
    private Canvas mainCanvas;
    private UIChallengePrefab[] prefabArray;
    public int[] challengeStatusArray;
    public int currentModelNum = 0;

    public void Init(UISave _uiSave)
    {
        mainCanvas = GetComponent<Canvas>();
        mainCanvas.enabled = false;
        uiSave = _uiSave;

        PreparePrefabList();
    }

    public void UpdateFigureStatus()
    {
        challengeStatusArray[currentModelNum - 1] = 2;
        prefabArray[currentModelNum - 1].SwitchStatus(2);
        uiSave.SaveChallengeAlbum(challengeStatusArray);
    }

    public void OpenModelByNum(int num)
    {
        if (num > 6)
            return;

        EventBus.loadChallendeModel?.Invoke(num);
        currentModelNum = num;
        SwitchCanvas();
    }

    public void GoBackToBasic()
    {
        EventBus.loadBasicModel?.Invoke();
        SwitchCanvas();
    }

    private void PreparePrefabList()
    {
        challengeStatusArray = new int[prefabParent.childCount];
        prefabArray = new UIChallengePrefab[challengeStatusArray.Length];

        if (uiSave.challengeStatus.Length > 0)
            challengeStatusArray = uiSave.challengeStatus;
        else
            uiSave.SaveChallengeAlbum(challengeStatusArray);

        for (int i = 0; i < prefabArray.Length; i++)
        {
            prefabArray[i] = prefabParent.GetChild(i).GetComponent<UIChallengePrefab>();
            
            if(i < 6)
                prefabArray[i].Init(this, i + 1, modelsBase.challengeModelRewardArray[i], modelsBase.challengeModelLvlArray[i]);
            else
                prefabArray[i].Init(this, i + 1, 999, 999);
        }

        CheckOpenedChallenges();
    }

    private void CheckOpenedChallenges()
    {
        int lvlNum = SceneController.lvlNum;

        for (int i = 0; i < modelsBase.challengeModelLvlArray.Length; i++)
        {
            if(modelsBase.challengeModelLvlArray[i] <= lvlNum)
            {
                if (challengeStatusArray[i] != 2)
                    challengeStatusArray[i] = 1;
            }
            else
            {
                challengeStatusArray[i] = 0;
            }

            prefabArray[i].SwitchStatus(challengeStatusArray[i]);
        }
    }

    #region Main Window

    public void SwitchCanvas()
    {
        if (mainCanvas.enabled)
        {
            EventBus.switchUIActive?.Invoke(false);
            mainCanvas.enabled = false;
        }
        else
        {
            EventBus.switchUIActive?.Invoke(true);
            mainCanvas.enabled = true;
            playerCoinText.text = "" + SceneController.playerCoins;
            playerCoinShadowText.text = "" + SceneController.playerCoins;
            CheckOpenedChallenges();
        }

        EventBus.closeTutorialScreen?.Invoke();
    }

    public void CloseScreen()
    {
        mainCanvas.enabled = false;
    }
    #endregion
}
