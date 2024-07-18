using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIPuzzleRewardScreen : MonoBehaviour
{
    [Header("General")]

    [SerializeField] TextMeshProUGUI headText;
    [SerializeField] float timeBeforRewardScreen;

    private Canvas screenCanvas;
    private PuzzleBase puzzleBase;

    [Header("Puzzle")]

    [SerializeField] Image mainImage;
    [SerializeField] Transform partParent;
    [SerializeField] float animSpeed;

    private Image[] partImagesArray;

    [Header("Animations")]

    [SerializeField] UIAnimationScreen animationScreen;

    public void Init(PuzzleBase _puzzleBase)
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
        puzzleBase = _puzzleBase;
        PrepareScreen();
    }

    #region Puzzles

    public void OpenPuzzlePartByNum(int partNum) => StartCoroutine(StartOpenAnimation(partNum));

    public void OpenCurrentPuzzle(UIPuzzlePrefab puzzle)
    {
        ResetParts();
        PreparePuzzle(puzzle);
        OpenScreen();
    }

    private IEnumerator StartOpenAnimation(int puzzleNum)
    {
        partImagesArray[puzzleNum].DOFade(0, animSpeed);

        yield return new WaitForSeconds(animSpeed);

        partImagesArray[puzzleNum].enabled = false;
    }
    
    private void PreparePuzzle(UIPuzzlePrefab puzzle)
    {
        headText.text = puzzle.key;
        mainImage.sprite = puzzle.GetMainSprite();

        for (int i = 0; i < partImagesArray.Length; i++)
        {
            partImagesArray[i].enabled = puzzle.partStatus[i] == 1 ? false : true;
        }
    }

    private void ResetParts()
    {
        foreach (Image part in partImagesArray)
        {
            part.DOFade(1, 0);
            part.enabled = true;
        }
    }
    #endregion

    #region Main Screen

    public void CallScreen(Vector3 puzzlePos, UIPuzzlePrefab puzzle, int partNum) => StartCoroutine(CallRewardScreen(puzzlePos, puzzle, partNum));

    public void CloseScreen()
    {
        animationScreen.SwitchCanvasActive(false);
        screenCanvas.enabled = false;
        EventBus.switchUIActive?.Invoke(false);
    }

    private IEnumerator CallRewardScreen(Vector3 puzzlePos, UIPuzzlePrefab puzzle, int partNum)
    {
        headText.text = "Puzzle Piece Found";
        animationScreen.StartItemAnimation(puzzlePos, 2, timeBeforRewardScreen);
        PreparePuzzle(puzzle);
        partImagesArray[partNum].enabled = true;

        yield return new WaitForSeconds(timeBeforRewardScreen);

        animationScreen.SwitchImage(0);
        OpenScreen();
        partImagesArray[partNum].DOFade(0, animSpeed);
    }

    private void OpenScreen()
    {
        screenCanvas.enabled = true;
        EventBus.switchUIActive?.Invoke(true);
    }

    private void PrepareScreen()
    {
        partImagesArray = new Image[partParent.childCount];

        for (int i = 0; i < partImagesArray.Length; i++)
        {
            partImagesArray[i] = partParent.GetChild(i).GetComponent<Image>();
            partImagesArray[i].enabled = true;
        }
    }
    #endregion
}
