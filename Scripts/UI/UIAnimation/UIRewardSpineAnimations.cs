
using UnityEngine;
using Spine.Unity;
using Spine;
using System.Collections;

public class UIRewardSpineAnimations : MonoBehaviour
{
    [Header("Flags")]
    [SerializeField] GameObject flagObj;
    [SerializeField] SkeletonGraphic flagLSpine;
    [SerializeField] SkeletonGraphic flagRSpine;

    [Header("Tubes")]
    [SerializeField] GameObject tubeObj;
    [SerializeField] SkeletonGraphic tubeLSpine;
    [SerializeField] SkeletonGraphic tubeRSpine;

    [Header("Fireworks")]
    [SerializeField] GameObject fireworkObj;
    [SerializeField] SkeletonGraphic fireworksGSpine;
    [SerializeField] SkeletonGraphic fireworksRSpine;
    [SerializeField] SkeletonGraphic fireworksYSpine;

    public void SwitchGameObject(bool isActive) { gameObject.SetActive(isActive); }

    public void StartSpineAnimation()
    {
        SwitchGameObject(true);

        flagLSpine.gameObject.SetActive(true);
        flagRSpine.gameObject.SetActive(true);
        tubeObj.SetActive(true);
        fireworkObj.SetActive(true);

        flagLSpine.AnimationState.Complete += FlagAnimatoinComplete;
        flagRSpine.AnimationState.Complete += FlagAnimatoinComplete;
        tubeLSpine.AnimationState.TimeScale = 0.8f;
        tubeRSpine.AnimationState.TimeScale = 0.8f;
        flagLSpine.AnimationState.SetAnimation(0, "Create", false);
        flagRSpine.AnimationState.SetAnimation(0, "Create", false);

        tubeLSpine.AnimationState.SetAnimation(0, "Animation", false);
        tubeRSpine.AnimationState.SetAnimation(0, "Animation", false);
        //fireworksGSpine.AnimationState.SetAnimation(0, "Green", false);
        //fireworksRSpine.AnimationState.SetAnimation(0, "Red", false);
        //fireworksYSpine.AnimationState.SetAnimation(0, "Yellow", false);
        StartCoroutine(StartFireworksAnimation(0.1f, 1));
        StartCoroutine(StartFireworksAnimation(0.6f, 2));
        StartCoroutine(StartFireworksAnimation(0.3f, 3));
    }

    public void StartFlagsAnimation()
    {
        SwitchGameObject(true);

        flagLSpine.gameObject.SetActive(true);
        flagRSpine.gameObject.SetActive(true);
        tubeObj.SetActive(false);
        fireworkObj.SetActive(false);

        flagLSpine.AnimationState.Complete += FlagAnimatoinComplete;
        flagRSpine.AnimationState.Complete += FlagAnimatoinComplete; 
        flagLSpine.AnimationState.SetAnimation(0, "Create", false);
        flagRSpine.AnimationState.SetAnimation(0, "Create", false);
    }

    public void StopSpineAnimation()
    {
        flagLSpine.AnimationState.TimeScale = 0f;
        flagRSpine.AnimationState.TimeScale = 0f;
        tubeLSpine.AnimationState.TimeScale = 0f;
        tubeRSpine.AnimationState.TimeScale = 0f;
    }

    private void FlagAnimatoinComplete(TrackEntry trackEntry)
    {
        flagLSpine.AnimationState.Complete -= FlagAnimatoinComplete;
        flagRSpine.AnimationState.Complete -= FlagAnimatoinComplete;
        flagLSpine.AnimationState.SetAnimation(0, "IDLE", true);
        flagRSpine.AnimationState.SetAnimation(0, "Idle", true);
    }

    private IEnumerator StartFireworksAnimation(float delay, int animNum)
    {
        yield return new WaitForSeconds(delay);

        switch (animNum)
        {
            case 1:
                fireworksGSpine.AnimationState.SetAnimation(0, "Green", false);
                break;
        
            case 2:
                fireworksRSpine.AnimationState.SetAnimation(0, "Red", false);
                break;
        
            case 3:
                fireworksYSpine.AnimationState.SetAnimation(0, "Yellow", false);
                break;
        }
    }
}
