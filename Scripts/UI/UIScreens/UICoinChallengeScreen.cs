
using UnityEngine;
using TMPro;
using System.Collections;

public class UICoinChallengeScreen : MonoBehaviour
{
    [Header("General")]

    [SerializeField] TextMeshProUGUI congratulationsText;
    [SerializeField] TextMeshProUGUI rewardText;

    private Canvas screenCanvas;
    private int reward;

    [Header("Animations")]

    [SerializeField] UIAnimationScreen animationScreen;

    public void Init()
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
    }

    //public void SetCongratulationsText(string text) => congratulationsText.text = text;

    public void ActivateContinueButton()
    {
        EventBus.activateInterstitialAD?.Invoke(reward);
        CloseScreen();
    }

    #region Main Screen

    public void OpenScreen()
    {
        congratulationsText.text = EventBus.getCongratulationsText?.Invoke();
        StartCoroutine(StartOpenAnimation());
    }

    public void CloseScreen()
    {
        StartCoroutine(StartCloseAnimation());
    }

    private IEnumerator StartOpenAnimation()
    {
        yield return new WaitForSeconds(1f);

        reward = Random.Range(10, 13);
        rewardText.text = "+" + reward;

        screenCanvas.enabled = true;
        animationScreen.StartFlagsSpineAnimation();
        EventBus.switchPlayerStats?.Invoke(true);
    }

    private IEnumerator StartCloseAnimation()
    {
        EventBus.updatePalyerCoins?.Invoke(reward);
        EventBus.closePlayerStatsWithCoin?.Invoke();

        yield return new WaitForSeconds(3f);

        screenCanvas.enabled = false;
        EventBus.switchUIActive?.Invoke(false);
        animationScreen.SwitchCanvasActive(false);
    }
    #endregion
}
