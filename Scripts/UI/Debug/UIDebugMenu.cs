
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIDebugMenu : MonoBehaviour
{
    [Header("Links")]

    [SerializeField] SceneController sceneController;
    [SerializeField] UIScript uIScript;
    [SerializeField] VoxModel model;
    [SerializeField] ModelsBase modelBase;

    [Header("Statistics")]

    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] TextMeshProUGUI cubesCountText;
    [SerializeField] TextMeshProUGUI resolutionText;
    [SerializeField] TextMeshProUGUI coinTimerText;

    [Header("Menu")]

    [SerializeField] GameObject buttons;
    private bool isActive;

    public void Init()
    {
        EventBus.updateLoadingDebugText += UpdateLoadingText;
        EventBus.updateCubesCountText += UpdateCubesCountText;
        EventBus.updateCoinTimerText += UpdateCoinTimerText;
    }

    #region Buttons

    public void SwitchButtons()
    {
        if (!isActive)
        {
            buttons.SetActive(true);
            isActive = true;
        }
        else
        {
            buttons.SetActive(false);
            isActive = false;
        }
    }
    #endregion

    public void GoToLevelDebug(int num) => sceneController.GoToNextLvl(num);
    
    public void GetItemCube(int cubeNum) => model.SetRandomItemCube(cubeNum);

    public void PlayCheck() => model.PlayCheck();

    public void ReloadLastModel() => sceneController.RestartLevel();

    public void ResetSaves()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void UpdateLoadingText(int num)
    {
        if (num > 0)
        {
            loadingText.text = $"Prepared voxels: {num}";
            loadingText.color = Color.red;
        }
        else
        {
            loadingText.text = "Completed";
            loadingText.color = Color.green;
        }
    }

    public void UpdateCoinTimerText(float time) => coinTimerText.text = $"Time to next coin: {Mathf.Round(time)}";
    
    public void UpdateCubesCountText() => cubesCountText.text = $"Cubes count: {model.GetVoxCount()}";

    public void GetCoins() => uIScript.GetDebugCoins();

    public void CallRateUs() => uIScript.CallDebugRateUs();

    public void GetPuzzlePart() => EventBus.getRandomPuzzlePart?.Invoke();

    private void OnDestroy()
    {
        EventBus.updateLoadingDebugText -= UpdateLoadingText;
        EventBus.updateCubesCountText -= UpdateCubesCountText;
        EventBus.updateCoinTimerText -= UpdateCoinTimerText;
    }
}
