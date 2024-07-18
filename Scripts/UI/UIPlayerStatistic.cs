
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class UIPlayerStatistic : MonoBehaviour
{
    [Header("Head Part")]

    [SerializeField] Transform headPart;

    [Header("Player Coins")]

    [SerializeField] TextMeshProUGUI playerCoinsText;
    [SerializeField] TextMeshProUGUI сoinsShadowText;
    [SerializeField] float hidePosY;

    private float startPosY;

    [Header("Pile Of Coins")]

    [SerializeField] UIPileReward pileScript;
    [SerializeField] RectTransform finishPosition;

    public void Init()
    {
        headPart.gameObject.SetActive(false);
        startPosY = headPart.position.y;
        pileScript.Init();
    }

    #region Head Part

    public void SwitchHead(bool isActive)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StartCoroutine(SetHeadPosition(isActive));
    }

    private IEnumerator SetHeadPosition(bool isVisible)
    {
        if (isVisible)
        {
            UpdatePlayerCoinsText();
            headPart.gameObject.SetActive(true);
            headPart.DOMoveY(startPosY, 0.5f);
        }
        else
        {
            startPosY = headPart.position.y;
            headPart.DOMoveY(startPosY + 200, 0.5f);

            yield return new WaitForSeconds(0.5f);

            headPart.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Player Coins

    public void UpdatePlayerCoinsText() 
    { 
        playerCoinsText.text = $"{SceneController.playerCoins}";
        сoinsShadowText.text = playerCoinsText.text; 
    }

    #endregion

    #region Pile of coins

    public void CloseWithCoinAnimation() => StartCoroutine(StartCoinAnimation());
    
    private IEnumerator StartCoinAnimation()
    {
        UpdatePlayerCoinsText();
        pileScript.SetFinishPosition(finishPosition.position);
        pileScript.StartRewardPileAnimation();
        
        yield return new WaitForSeconds(3f);

        StartCoroutine(SetHeadPosition(false));
    }
    #endregion
}
