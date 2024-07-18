
using System.Collections;
using UnityEngine;
using Spine;
using Spine.Unity;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [Header("Messages")]

    [SerializeField] Transform messagesParent;

    private GameObject[] messagesArray;

    [Header("Hand")]

    [SerializeField] GameObject handButton;
    [SerializeField] SkeletonGraphic handAnim;
    [SerializeField] float timeBetweenHandAnimations = 2f;
    [SerializeField] Vector3[] step2PositionsArray;
    [SerializeField] Transform bombButtonPos;

    private int stepCount;
    private int step2count;
    private bool animIsActive;
    private float animTimer;

    private bool isActive;

    public void Init()
    {
        EventBus.closeTutorialScreen += CloseAllSteps;

        PrepareMessages();
        PrepareHands();

        isActive = true;
        step2count = 0;

        if (SceneController.lvlNum > 8 && canvas.enabled)
        {
            canvas.enabled = false;
            isActive = false;
        }
    }

    public void UpdateTutorial()
    {
        UpdateHand();
    }

    public void SetTutorialStepByLvl(int num)
    {
        if (isActive)
        {
            CloseAllSteps();
            stepCount = num;

            switch (num)
            {
                case 1:
                    SwitchMessage(1);
                    SwitchHandAnimation(1);
                    break;

                case 2:
                    SwitchMessage(1);
                    SwitchHandAnimation(1);
                    MoveHandButton();
                    break;

                case 3:
                    SwitchMessage(2);
                    SwitchHandAnimation(2);
                    break;

                case 4:
                    SwitchMessage(3);
                    SwitchHandAnimation(3);
                    break;

                case 7:
                    SwitchMessage(4);
                    SwitchHandAnimation(4);
                    break;

                case 8:
                    SwitchMessage(5);
                    break;
            
                case 9:
                    canvas.enabled = false;
                    isActive = false;
                    break;
            }

            if (SceneController.lvlNum <= 8 && !canvas.enabled)
                canvas.enabled = true;
        }
    }

    public void CloseAllSteps()
    {
        if (SceneController.lvlNum >= 9)
        {
            canvas.enabled = false;
        }

        SwitchMessage(0);
        SwitchHandAnimation(0);

        animIsActive = false;
    }

    public void ButtonClick() 
    { 
        if(SceneController.lvlNum == 2)
        {
            MoveHandButton();
        } 
    }

    public void MoveHandButton()
    {
        if (step2count >= 4)
        {
            CloseAllSteps();
        }

        EventBus.removeCubeByID(step2count - 1);
        handButton.transform.DOLocalMove(step2PositionsArray[step2count], 0.3f);

        step2count += 1;

        Debug.Log("StepCount " + step2count);
    }

    #region Messages

    private void PrepareMessages()
    {
        messagesArray = new GameObject[messagesParent.childCount];

        for (int i = 0; i < messagesParent.childCount; i++)
        {
            messagesArray[i] = messagesParent.GetChild(i).gameObject;
        }
    }

    private void SwitchMessage(int num)
    {
        foreach(GameObject obj in messagesArray)
        {
            obj.SetActive(false);
        }

        if(num > 0)
            messagesArray[num - 1].SetActive(true);
    }
    #endregion

    #region Hand Animation

    private void PrepareHands()
    {
        handAnim.color = new Color(1, 1, 1, 0);
        animIsActive = false;
    }

    private void UpdateHand()
    {
        if (!gameObject.activeSelf)
            animIsActive = false;

        if (animIsActive)
        {
            animTimer += Time.deltaTime;

            if (animTimer > timeBetweenHandAnimations)
            {
                animTimer = 0;
                StartCoroutine(StartHandAnimation());
            }
        }
    }

    private IEnumerator StartHandAnimation()
    {
        handAnim.DOFade(1, 0.3f);
        yield return new WaitForSeconds(0.3f);
        handAnim.AnimationState.SetAnimation(0, "Pressing", false);
        handAnim.timeScale = 1f;
        //handAnim.startingLoop = false;
        yield return new WaitForSeconds(1f);
        handAnim.DOFade(0, 0.3f);
    }

    private void SwitchHandAnimation(int num)
    {
        switch (num)
        {
            case 0:
                handAnim.gameObject.SetActive(false);
                break;
                
            case 1:
                handAnim.AnimationState.SetAnimation(0, "Pressing", false);
                handAnim.timeScale = 1f;
                handAnim.transform.localPosition = new Vector3(0, -90, 0);
                handAnim.gameObject.SetActive(true);
                animIsActive = true;
                break;
                
            case 2:
                handAnim.DOFade(1, 0f);
                handAnim.gameObject.SetActive(true);
                handAnim.transform.localPosition = new Vector3(0, -480, 0);
                handAnim.AnimationState.SetAnimation(0, "Infinity", true);
                handAnim.timeScale = 1f;
                break;
    
            case 3:
                handAnim.gameObject.SetActive(true);
                handAnim.transform.localPosition = new Vector3(-90, -580, 0);
                handAnim.AnimationState.SetAnimation(0, "Zoom", true);
                handAnim.timeScale = 1f;
                handAnim.DOFade(1, 0f);
                break;

            case 4:
                handAnim.gameObject.SetActive(true);
                animIsActive = true;
                handAnim.transform.localPosition = new Vector3(55, -550, 0);
                //handAnim.transform.localPosition = bombButtonPos.localPosition;
                handAnim.transform.localRotation = new Quaternion(0, 0, -45, 0);
                handAnim.AnimationState.SetAnimation(0, "Pressing", false);
                handAnim.timeScale = 1f;
                handAnim.DOFade(1, 0f);
                break;
        }
    }
    #endregion

    private void OnDestroy()
    {
        EventBus.closeTutorialScreen -= CloseAllSteps;
    }
}
