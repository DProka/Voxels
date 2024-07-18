
using UnityEngine;
using TMPro;
using Spine.Unity;
using System.Collections;
using DG.Tweening;

public class UIGame : MonoBehaviour
{
    private UIScript uIScript;
    private UISave uiSave;

    [Header("Back")]

    [SerializeField] Canvas mainCanvas;
    [SerializeField] Canvas cameraCanvas;

    [Header("Upper part")]

    [SerializeField] TextMeshProUGUI lvlText;
    [SerializeField] TextMeshProUGUI movesText;
    [SerializeField] GameObject puzzleButton;

    [Header("Player Level")]

    [SerializeField] UILvlBar lvlBar;

    private bool levelIsActive;
    private int cubesToNextLvl;
    private int cubesCount;
    private float barStep;
    private int currentLvl;

    [Header("Bottom Part")]

    [SerializeField] UIBottomButton[] buttonsArray;

    [SerializeField] UIBottomButton challengeButton;

    private UIBombPart bombPart;

    public void Init(UIScript _uIScript, UISave _uiSave)
    {
        lvlBar.Init();
        bombPart = GetComponent<UIBombPart>();
        bombPart.Init();
        movesText.gameObject.SetActive(false);
        uIScript = _uIScript;
        uiSave = _uiSave;

        EventBus.substractCube += SubstractCube;

        InitButtons();

        SwitchBottomButton(1, true);
        SwitchBottomButton(2, true);
        SwitchBottomButton(3, SceneController.lvlNum >= 8);
        SwitchBottomButton(4, false);
        SwitchBottomButton(5, false);
    }

    public void UpdateLvlText(int lvlNum) { lvlText.text = $"Level: {lvlNum}"; }

    public void UpdateMovesText(int movesCount)
    {
        if (!movesText.gameObject.activeSelf)
            movesText.gameObject.SetActive(true);

        movesText.text = $"{movesCount} Moves";
    }

    #region Buttons

    public void RestartLevel() => uIScript.RestartLevel();
    public void SwitchBombButton(bool isActive) => bombPart.SwitchBombButton(isActive);
    public void SwitchPuzzleButton(bool isActive) => puzzleButton.SetActive(isActive);

    private void SwitchBottomButton(int buttonNum, bool isActive) => buttonsArray[buttonNum - 1].SwitchButton(isActive);
    
    private void InitButtons()
    {
        foreach(UIBottomButton button in buttonsArray)
        {
            button.Init();
        }
    }

    #endregion

    #region Challenge

    public void OpenNewChallengeModel()
    {
        SwitchBottomButton(3, true);
        challengeButton.SwitchAlert(true);
    }

    public void SwitchChallengeAlbum()
    {
        if (challengeButton.isActive)
        {
            EventBus.switchChallengeAlbum?.Invoke();
            challengeButton.SwitchAlert(false);
        }
    }
    #endregion

    #region Bomb Part

    public void ActivateBomb() => bombPart.ActivateBomb();
    public void UpdateBombCount(int count) => bombPart.UpdateBombCount(count);

    #endregion

    #region Player Level

    public void ActivatePlayerLevelBar(bool isBasic)
    {
        if (isBasic)
        {
            levelIsActive = false;
            lvlBar.SetActive(false);
            lvlText.gameObject.SetActive(true);
        }
        else
        {
            if (EventBus.checkIsModelLoaded.Invoke())
                LoadProgress();
            else
                SetNewLevel(0);
            
            levelIsActive = true;
            lvlBar.SetActive(true);
            lvlText.gameObject.SetActive(false);
        }
    }

    private void SetNewLevel(int num)
    {
        currentLvl = num; 
        
        int randomCount = Random.Range(30, 41);
        cubesToNextLvl = randomCount;
        cubesCount = 0;
        barStep = 100f / cubesToNextLvl;

        Debug.Log("Cubes on lvl: " + cubesToNextLvl);
        lvlBar.SetNewLvl(currentLvl);
    }

    private void SubstractCube()
    {
        if (levelIsActive)
        {
            cubesCount++;
            float barScale = (barStep * cubesCount) / 100;
            lvlBar.SetImageScale(barScale);

            if (cubesCount >= cubesToNextLvl)
            {
                SetNewLevel(currentLvl + 1);
                EventBus.callChallengeReward?.Invoke();
            }
            Debug.Log("Cubes to next lvl: " + cubesCount);
        }

        SaveProgress();
    }

    #endregion

    #region Save / Load

    private void SaveProgress()
    {
        uiSave.SaveChallengeLevel(currentLvl);
    }
    
    private void LoadProgress()
    {
        currentLvl = uiSave.currentChallengeLvl;
        SetNewLevel(currentLvl);
    }

    #endregion

    public void SwitchCanvasActive(bool isActive)
    {
        mainCanvas.enabled = isActive;
        cameraCanvas.enabled = isActive;
    }

    private void OnDestroy()
    {
        EventBus.substractCube -= SubstractCube;
    }
}
