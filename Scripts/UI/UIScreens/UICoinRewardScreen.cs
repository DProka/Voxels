using System.Collections;
using UnityEngine;
using TMPro;

public class UICoinRewardScreen : MonoBehaviour
{
    [Header("General")]

    [SerializeField] TextMeshProUGUI congratulationsText;
    [SerializeField] float timeBeforRewardScreen;

    private Canvas screenCanvas;
    private int bonusCoins;
    private int lastBonus;

    [Header("Buttons")]

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
        if (screenCanvas.enabled)
        {
            bonusLine.UpdateLine();
            UpdateADButtonText();
        }
    }

    #region Bonus

    public int GetBonusFactor(bool isAD)
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
            adButtonText.text = $"+ {bonusCoins * SetBonusFactor()}";
        }
    }
    #endregion

    #region Main Screen

    public void OpenScreen(int _bonusCoins, Vector3 coinPos)
    {
        congratulationsText.text = EventBus.getCongratulationsText?.Invoke();
        StartCoroutine(CallRewardScreen(_bonusCoins, coinPos));
    }

    public void CloseScreen(bool isAD)
    {
        StartCoroutine(CloseScreenAnimation(isAD));
    }

    private IEnumerator CallRewardScreen(int _bonusCoins, Vector3 coinPos)
    {
        animationScreen.SwitchImage(0);
        bonusCoins = _bonusCoins;
        animationScreen.StartItemAnimation(coinPos, 1, timeBeforRewardScreen);

        yield return new WaitForSeconds(timeBeforRewardScreen);

        screenCanvas.enabled = true;
        bonusLine.SwitchActive(true);
        animationScreen.StartFlagsSpineAnimation();
        EventBus.switchPlayerStats?.Invoke(true);
    }

    private IEnumerator CloseScreenAnimation(bool isAD)
    {
        bonusLine.SwitchActive(false);

        yield return new WaitForSeconds(isAD ? 3f : 0.2f);

        screenCanvas.enabled = false;
        animationScreen.SwitchCanvasActive(false);
        EventBus.switchUIActive?.Invoke(false);
    }

    #endregion
}
