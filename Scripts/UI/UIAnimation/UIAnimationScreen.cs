
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimationScreen : MonoBehaviour
{
    [Header("Main")]

    [SerializeField] Canvas mainCanvas;

    private RectTransform canvasTransform;
    
    [Header("Reward Spine Anim")]

    [SerializeField] UIRewardSpineAnimations spineAnimations;

    [Header("Coin")]

    [SerializeField] Image coinImage;
    [SerializeField] Transform coinEndPos;
    
    [Header("Puzzle")]

    [SerializeField] Image puzzleImage;
    
    [Header("Bomb")]

    [SerializeField] Image bombImage;

    [Header("Items")]

    [SerializeField] Transform itemTransform;
    [SerializeField] Image[] itemImagesArray;
    private float animationSpeed;

    public void Init(float animSpeed)
    {
        canvasTransform = mainCanvas.GetComponent<RectTransform>();
        animationSpeed = animSpeed;
        SwitchImage(0);
    }

    #region RewardScreenAnimation

    public void StartRewardSpineAnimation()
    {
        SwitchCanvasActive(true);
        spineAnimations.StartSpineAnimation();
    }
    
    public void StartFlagsSpineAnimation()
    {
        SwitchCanvasActive(true);
        spineAnimations.StartFlagsAnimation();
    }

    #endregion

    #region Item Animation

    public void StartItemAnimation(Vector3 itemPos, int itemNum, float animSpeed)
    {
        SwitchImage(0);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, itemPos, null, out localPoint);
        itemTransform.localPosition = localPoint;
        itemTransform.localScale = new Vector3(0, 0, 0);
        spineAnimations.SwitchGameObject(false);
        SwitchImage(itemNum);

        itemTransform.DOMove(coinEndPos.position, animSpeed);
        itemTransform.DOScale(1, animSpeed);
        SwitchCanvasActive(true);
    }

    public void SwitchImage(int num) 
    {
        foreach(Image img in itemImagesArray)
        {
            img.enabled = false;
        }

        if (num > 0)
            itemImagesArray[num-1].enabled = true; 
    }
    #endregion

    #region Main

    public void SwitchCanvasActive(bool isActive) => mainCanvas.enabled = isActive;

    #endregion
}
