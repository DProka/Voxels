using System.Collections;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField] ModelsBase modelBase;
    
    private SceneController sceneController;
    private UISave uiSave;
    private bool screenIsActive;

    [Header("Game Interface")]

    [SerializeField] UIGame gameUI;

    [Header("Player Stats")]

    [SerializeField] UIPlayerStatistic playerStat;

    [Header("EndRound Screen")]

    [SerializeField] UIEndRoundScreen endRoundScreen;
    [SerializeField] int rewardCoins;
    [SerializeField] float timeBeforeEndScreen;

    private float endRoundScreenTimer;
    private bool isEndRoundTimerActive;

    [Header("Puzzle Album")]

    [SerializeField] PuzzleBase puzzleBase;
    [SerializeField] UIPuzzleAlbum puzzleAlbum;

    private bool isADPuzzleAvailable;

    [Header("Challenge Album")]

    [SerializeField] UIChallengeAlbum challengeAlbum;

    [Header("Screens")]

    [SerializeField] UICoinRewardScreen rewardScreen;
    [SerializeField] UIBombRewardScreen bombScreen;
    [SerializeField] UICoinChallengeScreen challengeScreen;
    [SerializeField] UIFailScreen failScreen;
    [SerializeField] UISettings settingsScreen;

    public string[] congratulationsArray;

    [Header("Animation Screen")]

    [SerializeField] UIAnimationScreen animScreen;

    [Header("Daily Reward")]

    [SerializeField] UIDailyRewardScreen dailyRewardScreen;

    [Header("Rate Us")]

    [SerializeField] UIRateUsScreen rateUsScreen;
    
    [Header("Debug Menu")]

    [SerializeField] UIDebugMenu debugMenu;

    private int modelNum;

    public void Init(SceneController _gameController)
    {
        sceneController = _gameController;
        screenIsActive = false;

        uiSave = new UISave();

        EventBus.goToNextLvl += GoToNextLvl;
        EventBus.switchUIActive += SwitchUIActive;
        EventBus.switchPlayerStats += SwitchPlayerStats;
        EventBus.closePlayerStatsWithCoin += ClosePlayerStatsWithCoinAnim;

        EventBus.callCoinRewardScreen += OpenCoinRewardScreen;
        EventBus.callPuzzleRewardScreen += CallPuzzleRewardScreen;
        EventBus.callBombRewardScreen += CallBombRewardScreen;
        EventBus.switchChallengeAlbum += SwitchChallengeAlbum;
        EventBus.callChallengeReward += OpenChallengeRewardScreen;

        EventBus.activateInterstitialAD += ActivateInterstitialAD;
        EventBus.activateRewardedAD += ActivateRewardedAD;

        EventBus.getCongratulationsText += GetCongratulationsText;

        playerStat.Init();
        gameUI.Init(this, uiSave);
        endRoundScreen.Init();
        rewardScreen.Init();

        puzzleBase.Init();
        puzzleAlbum.Init(puzzleBase, uiSave);

        bombScreen.Init();
        settingsScreen.Init();
        animScreen.Init(1.5f);
        challengeScreen.Init();
        challengeAlbum.Init(uiSave);
        dailyRewardScreen.Init();
        rateUsScreen.Init();
        debugMenu.Init();

        congratulationsArray = new string[] { "Awesome", "Phenomenal", 
            "Brilliant", "Fantastic", "Incredible", "Beautiful", "Epic", 
            "Amazing", "Wonderful", "Fabulous", "Impressive", "Marvelous" };

        SwitchBombButton(false);
        SwitchPuzzleAlbumButton(false);
    }

    public void UpdateUI()
    {
        endRoundScreen.UpdateScreen();
        rewardScreen.UpdateScreen();
        rateUsScreen.UpdateScreen();

        UpdateEndRoundScreenTimer();
    }

    public void CheckUIStatus(bool isBasicModel)
    {
        SwitchLvlBar(isBasicModel);

        if (SceneController.lvlNum > 5)
            SwitchPuzzleAlbumButton(true);

        if (SceneController.lvlNum == 6)
            rateUsScreen.OpenScreen();

        if (SceneController.lvlNum > 7)
            SwitchBombButton(true);

        if(SceneController.lvlNum == modelBase.CheckNewModelNum() && uiSave.challengeStatus[0] == 0)
            gameUI.OpenNewChallengeModel();
        //else
        //    gameUI.SwitchChallengeButton(false);
    }

    public bool CheckIsActive() { return screenIsActive; }

    private string GetCongratulationsText()
    {
        int random = Random.Range(0, congratulationsArray.Length);
        return congratulationsArray[random];
    }

    public void CloseScreens()
    {
        SwitchUIActive(false);

        puzzleAlbum.CloseWindow();
        challengeAlbum.CloseScreen();
        settingsScreen.CloseScreen();
        dailyRewardScreen.CloseScreen();
        rateUsScreen.CloseScreen();
    }

    #region Buttons

    public void SwitchBombButton(bool isActive) => gameUI.SwitchBombButton(isActive);
    
    public void UpdateBombCount(int count) => gameUI.UpdateBombCount(count);

    public void SwitchPuzzleAlbumButton(bool isActive) => gameUI.SwitchPuzzleButton(isActive);

    #endregion

    #region Game UI

    public void UpdateLvlText(int lvlNum) { gameUI.UpdateLvlText(lvlNum); }

    public void SwitchLvlBar(bool isBasic) => gameUI.ActivatePlayerLevelBar(isBasic);

    public void RestartLevel() => sceneController.RestartLevel();

    private void SwitchUIActive(bool isActive)
    {
        screenIsActive = isActive;

        if (isActive)
        {
            gameUI.SwitchCanvasActive(false);
            EventBus.switchModelActive?.Invoke(false);
        }
        else
        {
            gameUI.SwitchCanvasActive(true);
            EventBus.switchModelActive?.Invoke(true);
        }
    }

    #endregion

    #region AD

    private void ActivateRewardedAD(int reward)
    {
        sceneController.UpdatePlayerCoins(reward);
        //GoToNextLvl();
    }
    
    private void ActivateInterstitialAD(int reward)
    {
        sceneController.UpdatePlayerCoins(reward);
        //GoToNextLvl();
    }
    #endregion
    
    #region Round Reward Screen

    private void GoToNextLvl()
    {
        sceneController.GoToNextLvl(1);
        screenIsActive = false;
    }

    public void StartRewardScreenTimer()
    {
        endRoundScreenTimer = timeBeforeEndScreen;
        isEndRoundTimerActive = true;
    }

    private void UpdateEndRoundScreenTimer()
    {
        if (isEndRoundTimerActive && !screenIsActive)
        {
            if (endRoundScreenTimer > 0)
                endRoundScreenTimer -= Time.deltaTime;

            else
            {
                endRoundScreen.CallRewardScreen(rewardCoins);
                isEndRoundTimerActive = false;
            }
        }
    }
    #endregion

    #region Coin Reward Screen

    public void GetCoinReward(bool withAD)
    {
        if (withAD)
        {
            int reward = 50 * rewardScreen.GetBonusFactor(true);
            ActivateRewardedAD(reward);
            EventBus.closePlayerStatsWithCoin?.Invoke();

        }
        else
        {
            ActivateInterstitialAD(0);
            EventBus.switchPlayerStats?.Invoke(false);
        }

        rewardScreen.CloseScreen(withAD);
        sceneController.ResetCoinTimer();
    }

    private void OpenCoinRewardScreen(float posX, float posY)
    {
        Vector3 coinPos = new Vector3(posX, posY, 0);
        rewardScreen.OpenScreen(50, coinPos);
    }
    #endregion

    #region Puzzle Reward Screen

    public void GetPuzzleReward(bool withAD)
    {
        if (withAD)
        {
            puzzleAlbum.OpenADPuzzle();

            //if (isADPuzzleAvailable)
            //{
            //    //puzzleManager.OpenNewPart();
            //    puzzleAlbum.OpenADPuzzle();
            //    isADPuzzleAvailable = false;
            //}
            //else
            //    ClosePuzzleRewardScreen();
        }
        else
            ClosePuzzleRewardScreen();
    }

    private void CallPuzzleRewardScreen(float posX, float posY)
    {
        Vector3 puzzlePos = new Vector3(posX, posY, 0);
        puzzleAlbum.CallRewardPuzzleScreen(puzzlePos);
        screenIsActive = true;
        isADPuzzleAvailable = true;
    }

    private void ClosePuzzleRewardScreen()
    {
        screenIsActive = false;
        SwitchPuzzleAlbumButton(true);
    }

    #endregion
    
    #region Bomb Reward Screen

    public void GetBombReward(bool withAD)
    {
        bombScreen.CloseScreen();
        UpdateBombCount(withAD ? 3 : 1);
        sceneController.ResetBombTimer();
    }

    private void CallBombRewardScreen(float posX, float posY)
    {
        Vector3 bombPos = new Vector3(posX, posY, 0);
        bombScreen.OpenScreen(bombPos);
    }
    #endregion

    #region Challenge Screen

    public void UpdateCurrentChallengeModelStatus() => challengeAlbum.UpdateFigureStatus();
    public bool CheckChallendeTutorial() { return uiSave.challengeStatus[0] == 0; }
    private void SwitchChallengeAlbum() => challengeAlbum.SwitchCanvas();
    private void OpenChallengeRewardScreen() => challengeScreen.OpenScreen();

    #endregion

    #region Player Stats

    private void SwitchPlayerStats(bool isActive)
    {
        SwitchUIActive(isActive);
        playerStat.SwitchHead(isActive);
    }

    private void ClosePlayerStatsWithCoinAnim() => playerStat.CloseWithCoinAnimation();

    #endregion

    #region Debug Menu

    public void OpenPuzzlePart() { }// => puzzleManager.OpenDebugPart();

    public void GetDebugCoins() { sceneController.UpdatePlayerCoins(500);  playerStat.UpdatePlayerCoinsText(); }

    public void CallDebugRateUs() => rateUsScreen.OpenScreen();
    #endregion

    private void OnDestroy()
    {
        EventBus.goToNextLvl -= GoToNextLvl;
        EventBus.switchUIActive -= SwitchUIActive;
        EventBus.switchPlayerStats -= SwitchPlayerStats;
        EventBus.closePlayerStatsWithCoin -= ClosePlayerStatsWithCoinAnim;

        EventBus.callCoinRewardScreen -= OpenCoinRewardScreen;
        EventBus.callPuzzleRewardScreen -= CallPuzzleRewardScreen;
        EventBus.callBombRewardScreen -= CallBombRewardScreen;
        EventBus.switchChallengeAlbum -= SwitchChallengeAlbum;
        EventBus.callChallengeReward -= OpenChallengeRewardScreen;

        EventBus.activateInterstitialAD -= ActivateInterstitialAD;
        EventBus.activateRewardedAD -= ActivateRewardedAD;

        EventBus.getCongratulationsText -= GetCongratulationsText;
    }
}
