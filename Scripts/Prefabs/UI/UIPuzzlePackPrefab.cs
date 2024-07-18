
using UnityEngine;
using TMPro;
using System;

public class UIPuzzlePackPrefab : MonoBehaviour
{
    [SerializeField] Transform puzzleParent;
    [SerializeField] TextMeshProUGUI packNameText;
    [SerializeField] string key = "";

    private PuzzleBase puzzleBase;
    public UIPuzzlePrefab[] puzzlePrefabArray;

    public void Init(PuzzleBase _puzzleBase)
    {
        puzzleBase = _puzzleBase;
        packNameText.text = key;
        
        PreparePuzzles();
    }

    public UIPuzzlePrefab GetPuzzleByNum(int num) { return puzzlePrefabArray[num]; }

    public string GetKey() { return key; }

    public bool CheckAvaliblePacks()
    {
        bool avalible = false;

        for (int i = 0; i < puzzlePrefabArray.Length; i++)
        {
            if (Array.Exists(puzzlePrefabArray[i].partStatus, element => element == 0))
            {
                avalible = true;
                break;
            }
        }

        return avalible;
    }

    private void PreparePuzzles()
    {
        puzzlePrefabArray = new UIPuzzlePrefab[puzzleParent.childCount];
        Sprite[] sprites = puzzleBase.GetSpritesByKey(key);

        for (int i = 0; i < puzzlePrefabArray.Length; i++)
        {
            puzzlePrefabArray[i] = puzzleParent.GetChild(i).GetComponent<UIPuzzlePrefab>();
            puzzlePrefabArray[i].Init(2, sprites[i], i, key);
        }
    }
}
