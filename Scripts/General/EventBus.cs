
using System;
using UnityEngine;

public static class EventBus
{
    #region Player

    public static Action<int> updatePalyerCoins;
    #endregion

    #region Game

    public static Action goToNextLvl;
    public static Action<int> updateMovesCount;
    public static Action activateBomb;
    #endregion

    #region Model

    public static Action<bool> switchModelActive;
    public static Action loadBasicModel;
    public static Action<int> loadChallendeModel;
    public static Action updatePraparedCubes;
    public static Func<bool> checkIsModelLoaded;
    #endregion

    #region Cubes

    public static Action substractCube;
    public static Action<int> removeCubeByID;
    public static Action<int> onCubeDestroyed;
    #endregion

    #region UI

    public static Func<string> getCongratulationsText;

    public static Action<bool> switchUIActive;
    public static Action<bool> switchPlayerStats;
    public static Action closePlayerStatsWithCoin;

    public static Action<float, float> callCoinRewardScreen;
    public static Action<float, float> callPuzzleRewardScreen;
    public static Action<float, float> callBombRewardScreen;
    public static Action<int, string> callCurrentPuzzleScreen;
    public static Action switchChallengeAlbum;
    public static Action callChallengeReward;


    public static Action<int> activateRewardedAD;
    public static Action<int> activateInterstitialAD;
    #endregion

    #region Tutorial

    public static Action closeTutorialScreen;
    #endregion

    #region Debug Menu

    public static Action<int> updateLoadingDebugText;
    public static Action updateCubesCountText;
    public static Action<float> updateCoinTimerText;
    public static Func<int> getRandomPuzzlePart;
    #endregion
}
