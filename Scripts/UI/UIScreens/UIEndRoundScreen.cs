
using UnityEngine;
using TMPro;
using System.Collections;

public class UIEndRoundScreen : MonoBehaviour
{
    [Header("General")]

    [SerializeField] TextMeshProUGUI congratulationsText;

    private Canvas screenCanvas;
    private int bonusCoins;
    private int lastBonus;

    [Header("Buttons")]

    [SerializeField] GameObject startButton;
    [SerializeField] TextMeshProUGUI startAdButtonText;
    [SerializeField] GameObject mainButton;
    [SerializeField] TextMeshProUGUI adButtonText;

    [Header("Animations")]

    [SerializeField] UIAnimationScreen animationScreen;
    [SerializeField] UIBonusLine bonusLine;
    [SerializeField] float lineAnimationSpeed;

    public void Init()
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
        bonusLine.Init(lineAnimationSpeed);
    }

    public void UpdateScreen()
    {
        if (screenCanvas.isActiveAndEnabled)
        {
            bonusLine.UpdateLine();
            UpdateADButtonText();
        }
    }

    #region Bonus

    private int GetBonusFactor(bool isAD) 
    {
        if (isAD)
            return SetBonusFactor();
        else
            return 1;
    }

    private int SetBonusFactor()
    {
        int bonus = 0;
        float scale = bonusLine.GetLineScale();

        if (scale >= 0 && scale <= 0.2)
        {
            bonus = 1;
        }
        else if (scale > 0.2 && scale <= 0.4)
        {
            bonus = 3;
        }
        else if (scale > 0.4 && scale <= 0.6)
        {
            bonus = 5;
        }
        else if (scale > 0.6 && scale <= 0.8)
        {
            bonus = 4;
        }
        else if (scale > 0.8 && scale <= 1)
        {
            bonus = 2;
        }

        return bonus;
    }

    private void UpdateADButtonText()
    {
        if (lastBonus != SetBonusFactor())
        {
            if(mainButton.activeSelf)
                adButtonText.text = $"+ {bonusCoins * SetBonusFactor()}";
            else
                startAdButtonText.text = $"+ {bonusCoins * SetBonusFactor()}";
        }
    }
    #endregion

    #region Main Screen

    public void CallRewardScreen(int _bonusCoins)
    {
        bonusCoins = _bonusCoins;

        if (SceneController.lvlNum > 3)
        {
            startButton.SetActive(false);
            mainButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(true);
            mainButton.SetActive(false);
        }

        animationScreen.StartRewardSpineAnimation();
        OpenScreen();
    }

    public void CloseWithRewarded()
    {
        int reward = 50 * GetBonusFactor(true);
        EventBus.activateRewardedAD?.Invoke(reward);
        EventBus.closePlayerStatsWithCoin?.Invoke();
        StartCoroutine(CloseScreen(true));
    }

    public void CloseWithInterstitial()
    {
        EventBus.activateInterstitialAD?.Invoke(0);
        EventBus.switchPlayerStats?.Invoke(false);
        StartCoroutine(CloseScreen(false));
    }

    private void OpenScreen()
    {
        animationScreen.SwitchImage(0);
        congratulationsText.text = EventBus.getCongratulationsText?.Invoke();
        EventBus.switchPlayerStats?.Invoke(true);
        EventBus.switchUIActive?.Invoke(true);
        screenCanvas.enabled = true;
        bonusLine.SwitchActive(true);
    }

    private IEnumerator CloseScreen(bool withAD)
    {
        bonusLine.SwitchActive(false);

        yield return new WaitForSeconds(withAD ? 3f : 0.2f);

        screenCanvas.enabled = false;
        animationScreen.SwitchCanvasActive(false);
        EventBus.switchUIActive?.Invoke(false);
        EventBus.goToNextLvl?.Invoke();
    }
    #endregion
}
