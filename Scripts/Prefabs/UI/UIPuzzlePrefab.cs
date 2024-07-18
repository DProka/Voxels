using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIPuzzlePrefab : MonoBehaviour
{
    public int id { get; private set; }
    public string key { get; private set; }
    public int[] partStatus;// { get; private set; }

    [SerializeField] Image mainImage;
    [SerializeField] Transform partParent;

    private PuzzleSave saveScript;
    private Image[] partImagesArray;
    private float animSpeed;

    public void Init(float animationSpeed, Sprite newSprite, int _id, string _key)
    {
        id = _id;
        animSpeed = animationSpeed;
        mainImage.sprite = newSprite;
        key = _key;

        saveScript = new PuzzleSave(key + id);
        partStatus = saveScript.partStatusArray;

        PreparePuzzle();
    }

    public Sprite GetMainSprite() { return mainImage.sprite; }

    public void OpenCurrentPuzzleButton()
    {
        if (CheckOpenedParts())
            EventBus.callCurrentPuzzleScreen?.Invoke(id, key);
    }

    #region Parts

    public void OpenPartByNum(int num)
    {
        partStatus[num] = 1;
        partImagesArray[num].enabled = false;

        Save();
    }

    private bool CheckOpenedParts()
    {
        bool hasParts = false;

        if(Array.Exists(partStatus, elem => elem == 1))
            hasParts = true;

        //for (int i = 0; i < partStatus.Length; i++)
        //{
        //    if (partStatus[i] == 1)
        //        hasParts = true;
        //}

        return hasParts;
    }
    #endregion

    #region Save/Load

    public void ResetSave()
    {
        foreach (Image part in partImagesArray)
        {
            part.DOFade(1, 0);
            part.enabled = true;
        }

        partStatus = new int[9];
        Save();
    }

    private void PreparePuzzle()
    {
        partImagesArray = new Image[partParent.childCount];

        for (int i = 0; i < partImagesArray.Length; i++)
        {
            partImagesArray[i] = partParent.GetChild(i).GetComponent<Image>();
            partImagesArray[i].enabled = partStatus[i] == 0 ? true : false;
        }
    }

    private void Save() => saveScript.SavePuzzleProgress(partStatus);
    #endregion
}
