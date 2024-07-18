
using System.Collections.Generic;
using UnityEngine;

public class UIPuzzleAlbum : MonoBehaviour
{
    public UIPuzzlePackPrefab[] packArray;

    [SerializeField] Transform packParent;
    [SerializeField] UIPuzzleRewardScreen puzzleScreen;

    private Canvas screenCanvas;
    private PuzzleBase puzzleBase;
    private UIPuzzlePrefab currentPuzzle;
    private UISave save;
    private int activePuzzlePack;
    private int activePuzzle;

    public void Init(PuzzleBase _puzzleBase, UISave _save)
    {
        screenCanvas = GetComponent<Canvas>();
        screenCanvas.enabled = false;
        puzzleBase = _puzzleBase;
        save = _save;

        puzzleScreen.Init(puzzleBase);

        EventBus.callCurrentPuzzleScreen += CallPuzzleScreen;
        EventBus.getRandomPuzzlePart += GetRandomPartNum;

        activePuzzlePack = save.currentPuzzle[0];
        activePuzzle = save.currentPuzzle[1];
        
        PreparePacks();

        currentPuzzle = packArray[activePuzzlePack].GetPuzzleByNum(activePuzzle);
    }

    #region Puzzles

    public void CallPuzzleScreen(int id, string key)
    {
        puzzleScreen.OpenCurrentPuzzle(GetPuzzle(id, key));
    }

    public void CallRewardPuzzleScreen(Vector3 puzzlePos)
    {
        CheckCurrentPuzzle(false);

        if (CheckAvaliblePuzzles())
            puzzleScreen.CallScreen(puzzlePos, currentPuzzle, GetRandomPartNum());
        else
            CallPuzzleScreen(currentPuzzle.id, currentPuzzle.key);
    }

    public void OpenADPuzzle()
    {
        CheckCurrentPuzzle(true);
        puzzleScreen.OpenPuzzlePartByNum(GetRandomPartNum());
    }

    private UIPuzzlePrefab GetPuzzle(int id, string key) 
    {
        UIPuzzlePrefab puzzle = null;

        for (int i = 0; i < packArray.Length; i++)
        {
            if (packArray[i].GetKey() == key)
                puzzle = packArray[i].GetPuzzleByNum(id);
        }

        return puzzle;
    }

    private int GetRandomPartNum()
    {
        if (CheckAvaliblePuzzles())
        {
            CheckCurrentPuzzle(false);

            List<int> closedParts = new List<int>();

            for (int i = 0; i < currentPuzzle.partStatus.Length; i++)
            {
                if (currentPuzzle.partStatus[i] == 0)
                    closedParts.Add(i);
            }

            int random = Random.Range(0, closedParts.Count);
            int part = closedParts[random];
            currentPuzzle.OpenPartByNum(part);

            Save();

            return part;
        }

        return 0;
    }

    private void CheckCurrentPuzzle(bool isAD)
    {
        if (System.Array.Exists(currentPuzzle.partStatus, element => element == 0))
        {
            return;
        }
        else
        {
            activePuzzlePack++;

            if(activePuzzlePack >= packArray.Length)
            {
                activePuzzlePack = 0;
                activePuzzle++;
            }

            if (activePuzzle >= packArray[0].puzzlePrefabArray.Length)
                return;

            currentPuzzle = packArray[activePuzzlePack].GetPuzzleByNum(activePuzzle);

            if(isAD)
                CallPuzzleScreen(currentPuzzle.id, currentPuzzle.key);
        }

        Debug.Log("Active pack " + activePuzzlePack + " : Active Puzzle " + activePuzzle);
    }

    private bool CheckAvaliblePuzzles()
    {
        bool hasPuzzles = false;

        for (int i = 0; i < packArray.Length; i++)
        {
            if (packArray[i].CheckAvaliblePacks())
                hasPuzzles = true;
        }

        return hasPuzzles;
    }
    #endregion

    #region Main Screen

    public void SwitchPuzzleAlbum()
    {
        if (!screenCanvas.enabled)
        {
            EventBus.switchUIActive?.Invoke(true);
            screenCanvas.enabled = true;
        }
        else
        {
            EventBus.switchUIActive?.Invoke(false);
            screenCanvas.enabled = false;
        }
    }

    public void CloseWindow()
    {
        screenCanvas.enabled = false;
    }

    private void PreparePacks()
    {
        packArray = new UIPuzzlePackPrefab[packParent.childCount];

        for (int i = 0; i < packArray.Length; i++)
        {
            packArray[i] = packParent.GetChild(i).GetComponent<UIPuzzlePackPrefab>();
            packArray[i].Init(puzzleBase);
        }
    }
    #endregion

    private void Save()
    {
        int[] status = new int[] { activePuzzlePack, activePuzzle };
        save.SaveActivePuzzle(status);
    }

    private void OnDestroy()
    {
        EventBus.callCurrentPuzzleScreen -= CallPuzzleScreen;
        EventBus.getRandomPuzzlePart -= GetRandomPartNum;
    }
}
