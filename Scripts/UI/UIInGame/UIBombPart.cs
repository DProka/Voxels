using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIBombPart : MonoBehaviour
{
    [Header("Bomb Button")]

    [SerializeField] GameObject bombButtonObj;
    [SerializeField] Transform bombImage;

    private bool bombIsActive;

    [Header("Bomb Count")]

    [SerializeField] GameObject bombCountObj;
    [SerializeField] TextMeshProUGUI bombCountText;

    private int bombCount;

    [Header("Buy Button")]

    [SerializeField] GameObject buyButton;

    public void Init()
    {
        bombIsActive = false;
        UpdateBombCount(3);
        buyButton.SetActive(false);
        buyButton.transform.localScale = new Vector3(0, 0, 0);
    }

    #region Bomb Button

    public void SwitchBombButton(bool isActive)
    {
        bombButtonObj.SetActive(isActive);
        bombCountObj.SetActive(isActive);
    }

    public void ActivateBomb()
    {
        if (!bombIsActive)
        {
            if (bombCount > 0)
            {

                EventBus.activateBomb?.Invoke();
                StartCoroutine(StartBombAnimation());
                UpdateBombCount(-1);
                bombIsActive = true;

                if (SceneController.lvlNum == 7)
                    EventBus.closeTutorialScreen?.Invoke();
            }
            else
            {
                BuyBombs(3);
            }
        }

    }

    private IEnumerator StartBombAnimation()
    {
        bombImage.DOLocalRotate(new Vector3(0f, 0f, -360f), 1f, RotateMode.FastBeyond360);
        bombImage.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 1f, 3);

        yield return new WaitForSeconds(1f);

        bombIsActive = false;
    }
    #endregion

    #region BombCount

    public void UpdateBombCount(int count)
    {
        bombCount += count;
        bombCountText.text = "" + bombCount;
        SwitchBuyButton(bombCount <= 0);
    }

    public void SwitchBuyButton(bool isActive) => StartCoroutine(AnimateBuyButton(isActive));

    private void BuyBombs(int count)
    {
        if(SceneController.playerCoins >= 300)
        {
            EventBus.updatePalyerCoins?.Invoke(-300);
            UpdateBombCount(count);
        }
    }

    private IEnumerator AnimateBuyButton(bool isActive)
    {
        if (isActive)
        {
            buyButton.SetActive(true);
            buyButton.transform.DOScale(1f, 0.3f);
            bombCountObj.transform.DOScale(0.2f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            bombCountObj.SetActive(false);
        }
        else
        {
            bombCountObj.SetActive(true);
            bombCountObj.transform.DOScale(1, 0.3f);
            buyButton.transform.DOScale(0.2f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            buyButton.SetActive(false);
        }
    }
    #endregion
}
