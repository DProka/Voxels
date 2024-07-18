
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Main Settings")]

    [SerializeField] GameSettings gameSettings;

    public static int lvlNum { get; private set; }
    public int movesCount { get; private set; }
    public static int playerCoins { get; private set; }

    private GeneralSave save;

    [Header("Links")]

    [SerializeField] CameraRotateAround mainCameraScript;
    [SerializeField] UIScript uiScript;
    [SerializeField] VoxModel model;
    [SerializeField] ModelsBase modelsBase;
    
    public static bool modelIsActive { get; private set; }
    public static int rotationStatus { get; private set; }

    private float timerBetweenCoins;
    private bool coinIsActive;

    private float timerBetweenBombs;
    private bool bombIsActive;

    [Header("Game")]

    public static Transform controllerTransform;
    private int maxCubesInModel;
    private int cubesInGame;
    private int lastChallengeModelNum;

    private int puzzleCubeNum;
    private bool puzzleInGame = false;

    private bool isBasicModel;
    private int modelNum;

    [Header("Tutorial")]

    [SerializeField] TutorialManager tutorial;

    private void Start()
    {
        lvlNum = 0;
        controllerTransform = transform;
        puzzleInGame = false;
        modelIsActive = false;
        lastChallengeModelNum = 0;

        Load();

        EventBus.updatePalyerCoins += UpdatePlayerCoins;

        EventBus.switchModelActive += SwitchModelActive;
        EventBus.loadBasicModel += LoadBasicModel;
        EventBus.loadChallendeModel += LoadChallengeModel;
        EventBus.checkIsModelLoaded += CheckModelIsLoaded;

        tutorial.Init();
        mainCameraScript.Init();

        model.Init(this);

        uiScript.Init(this);
        uiScript.UpdateLvlText(lvlNum);

        if(lvlNum < 8)
            LoadBasicModel();
        else
            LoadModelFromSave();
    }

    private void Update()
    {
        mainCameraScript.UpdateScript();

        CheckModelRotation();
        model.UpdateModel();

        uiScript.UpdateUI();

        tutorial.UpdateTutorial();

        UpdateCoinTimer();
        UpdateBombTimer();

        if (Input.GetKeyDown(KeyCode.Escape))
            uiScript.CloseScreens();
    }

    public void UpdatePlayerCoins(int coins) => playerCoins += coins;
    
    public void GoToNextLvl(int lvl)
    {
        EventBus.closeTutorialScreen?.Invoke();

        if (lvlNum > 0 && isBasicModel)
            lvlNum += lvl;

        SaveStats();
        LoadBasicModel();
    }

    public void RestartLevel()
    {
        if(cubesInGame < maxCubesInModel)
        {
            if (isBasicModel)
                LoadBasicModel();
            else
                LoadChallengeModel(0);

            mainCameraScript.ResetPosition();
        }
    }

    private void SwitchModelActive(bool isActive)
    {
        if (uiScript.CheckIsActive() != isActive)
        {
            model.SwitchModelActive(isActive);
            SwitchModelIsActive(isActive);
        }

        Debug.Log("Model is Active: " + modelIsActive);
    }

    private void SwitchModelIsActive(bool isActive) => modelIsActive = isActive;
    
    private void SetUISettings()
    {
        CheckTutorial();

        uiScript.UpdateLvlText(lvlNum);
        uiScript.CheckUIStatus(isBasicModel);

        EventBus.updateCubesCountText?.Invoke();
        //uiScript.SwitchLvlBar(isBasicModel);

        if (lvlNum > 5)
            uiScript.SwitchPuzzleAlbumButton(true);

        if (lvlNum > 7)
            uiScript.SwitchBombButton(true);

        CheckNewChallengeModel();
    }

    #region Camera

    private void SetCameraSettings()
    {
        float newMaxZoom = 10;

        if (lvlNum > 3)
            newMaxZoom = 15;

        if (maxCubesInModel > 100)
            newMaxZoom = 25;
        if (maxCubesInModel > 200)
            newMaxZoom = 35;
        if (maxCubesInModel > 350)
            newMaxZoom = 35;

        mainCameraScript.ResetPosition();
        mainCameraScript.SetCameraSettings(newMaxZoom);
    }
    #endregion

    #region Model

    public void SetCubesCount(int maxCubes, int cubesCount)
    {
        maxCubesInModel = maxCubes;
        cubesInGame = cubesCount;

        SetUpPuzzleCube();
        SetCameraSettings();
        SetUISettings();
    }

    public void RemoveCubeFromCount()
    {
        if(lvlNum != 2)
            EventBus.closeTutorialScreen?.Invoke();

        cubesInGame -= 1;
        
        if (cubesInGame <= 0)
        {
            uiScript.StartRewardScreenTimer();

            if (!isBasicModel)
                uiScript.UpdateCurrentChallengeModelStatus();
        }

        if (cubesInGame == maxCubesInModel - 3 && lvlNum == 7)
        {
            tutorial.SetTutorialStepByLvl(lvlNum);
            uiScript.SwitchBombButton(true);
        }

        CheckPuzzleCube();
        SubstractCoinTime();
        EventBus.substractCube?.Invoke();
        EventBus.updateCubesCountText?.Invoke();

        SaveModel();
    }

    private void LoadBasicModel()
    {
        SwitchModelIsActive(false);
        StartCoroutine(model.LoadModelFromVOXFile(true, lvlNum - 1));
        ResetCoinTimer();
        ResetBombTimer();
        isBasicModel = true;
        modelNum = lvlNum - 1;

        SaveModel();
    }
    
    private void LoadChallengeModel(int _modelNum)
    {
        SwitchModelIsActive(false);
        StartCoroutine(model.LoadModelFromVOXFile(false, _modelNum - 1));
        ResetCoinTimer();
        ResetBombTimer();
        isBasicModel = false;
        modelNum = _modelNum - 1;

        SaveModel();
    }

    private void LoadModelFromSave()
    {
        SwitchModelIsActive(false);
        StartCoroutine(model.LoadModelFromSave(isBasicModel, modelNum));
        ResetCoinTimer();
        ResetBombTimer();
    }

    private void CheckNewChallengeModel()
    {
        int newModelNum = 0;

        foreach(int num in modelsBase.challengeModelLvlArray)
        {
            if (num <= lvlNum)
                newModelNum = num;
        }

        if(newModelNum > lastChallengeModelNum)
        {
            lastChallengeModelNum = newModelNum;
            //uiScript.CallChallengeAlbumFirstTime();
        }
    }

    private void CheckModelRotation()
    {
        if(rotationStatus == 0)
        {
            if (Input.touchCount == 1)
                rotationStatus = 1;
        }
            
        else if (Input.touchCount == 0 || Input.touchCount == 2)
            rotationStatus = Input.touchCount;
    }

    private bool CheckModelIsLoaded() 
    {
        Debug.Log("is model loaded " + (maxCubesInModel == cubesInGame));
        return maxCubesInModel != cubesInGame; 
    }
    #endregion

    #region Coin Timer

    public void ResetCoinTimer()
    {
        timerBetweenCoins = gameSettings.timeBetweenCoins;
        coinIsActive = false;
    }

    private void UpdateCoinTimer()
    {
        if (lvlNum >= 6 && modelIsActive && !coinIsActive && !uiScript.CheckIsActive())
        {
            if (timerBetweenCoins > 0)
            {
                timerBetweenCoins -= Time.deltaTime;
                EventBus.updateCoinTimerText?.Invoke(timerBetweenCoins);
            }
            else
            {
                coinIsActive = true;
                //model.SetRandomCoinCube();
                model.SetRandomItemCube(1);
                EventBus.updateCoinTimerText?.Invoke(0);
            }
        }
    }
    
    private void SubstractCoinTime()
    {
        if (timerBetweenCoins > 5)
        {
            timerBetweenCoins -= 3;
        }
    }
    #endregion

    #region Puzzle Cube

    private void SetUpPuzzleCube()
    {
        if (isBasicModel)
        {
            if (lvlNum > 4)
            {
                puzzleCubeNum = maxCubesInModel / 3;
                puzzleInGame = true;
            }
        }
        else
            puzzleCubeNum = 50;
    }

    private void CheckPuzzleCube()
    {
        if(isBasicModel)
        {
            if (puzzleInGame && cubesInGame == puzzleCubeNum && lvlNum >= 5)
            {
                model.SetRandomItemCube(2);
                //model.SetRandomPuzzleCube();
                puzzleInGame = false;
            }
        }
        else
        {
            puzzleCubeNum--;

            if (puzzleCubeNum <= 0)
            {
                model.SetRandomItemCube(2);
                //model.SetRandomPuzzleCube();
                puzzleCubeNum = 50;
            }
        }
    }
    #endregion

    #region Bomb Cube

    public void ResetBombTimer()
    {
        timerBetweenBombs = gameSettings.timeBetweenBombs;
        bombIsActive = false;
    }

    private void UpdateBombTimer()
    {
        if (lvlNum >= 8 && modelIsActive && !bombIsActive && !uiScript.CheckIsActive())
        {
            if (timerBetweenBombs > 0)
            {
                timerBetweenBombs -= Time.deltaTime;
            }
            else
            {
                bombIsActive = true;
                //model.SetRandomBombCube();
                model.SetRandomItemCube(3);
            }
        }
    }
    #endregion

    private void CheckTutorial()
    {
        if(lvlNum <= 4)
            tutorial.SetTutorialStepByLvl(lvlNum);
        else if(lvlNum == 8 && isBasicModel && uiScript.CheckChallendeTutorial())
            tutorial.SetTutorialStepByLvl(lvlNum);
    }

    private void SaveStats() => save.SavePlayerStats(lvlNum, playerCoins);

    public void SaveModel() => save.SaveModel(isBasicModel, modelNum);
    
    private void Load()
    {
        save = new GeneralSave();
        save.Load();

        lvlNum = save.playerLvl;
        playerCoins = save.playerCoins;

        isBasicModel = save.isBasicModel;
        modelNum = save.modelNum;
    }

    private void OnDestroy()
    {
        EventBus.updatePalyerCoins -= UpdatePlayerCoins;

        EventBus.switchModelActive -= SwitchModelActive;
        EventBus.loadBasicModel -= LoadBasicModel;
        EventBus.loadChallendeModel -= LoadChallengeModel;
        EventBus.checkIsModelLoaded -= CheckModelIsLoaded;
    }
}
