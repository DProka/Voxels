using System.Collections;
using UnityEngine;
using TMPro;
using Spine.Unity;

public class UIBombRewardScreen : MonoBehaviour
{
    private Canvas screenCanvas;

    [Header("General")]

    [SerializeField] TextMeshProUGUI headText;
    [SerializeField] float timeBeforRewardScreen;
    [SerializeField] UIAnimationScreen animationScreen;
    [SerializeField] SkeletonGraphic glowSpine;

    public void Init()
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
        SwitchSpine(false);
    }

    private void SwitchSpine(bool isActive)
    {
        if (isActive)
        {
            glowSpine.enabled = true;
            glowSpine.timeScale = 1f;
        }
        else
        {
            glowSpine.enabled = false;
            glowSpine.timeScale = 0f;
        }
    }

    #region Main Screen

    public void OpenScreen(Vector3 bombPos)
    {
        StartCoroutine(CallRewardScreen(bombPos));
    }

    public void CloseScreen()
    {
        animationScreen.SwitchCanvasActive(false);
        SwitchSpine(false);
        screenCanvas.enabled = false;
        EventBus.switchUIActive?.Invoke(false);
    }

    private IEnumerator CallRewardScreen(Vector3 bombPos)
    {
        headText.text = "Puzzle Piece Found";
        animationScreen.SwitchImage(0);
        animationScreen.StartItemAnimation(bombPos, 3, timeBeforRewardScreen);

        yield return new WaitForSeconds(timeBeforRewardScreen);

        SwitchSpine(true);
        screenCanvas.enabled = true;
        EventBus.switchUIActive?.Invoke(true);
    }
    #endregion
}
