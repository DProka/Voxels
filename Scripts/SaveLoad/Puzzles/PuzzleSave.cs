
using UnityEngine;

public class PuzzleSave
{
    public int[] partStatusArray { get; private set; }

    private string saveKey = "";

    public PuzzleSave(string key)
    {
        saveKey = key;

        Load();
    }

    public void SavePuzzleProgress(int[] parts)
    {
        partStatusArray = parts;

        Save();
    }

    #region Save Load

    public void ResetSave()
    {
        SaveData.PuzzleData general = new SaveData.PuzzleData();

        partStatusArray = general._partStatusArray;

        Save();
    }

    public void Save() => SaveManager.Save(saveKey, GetSaveSnapshot());
    
    public void Load()
    {
        var data = SaveManager.Load<SaveData.PuzzleData>(saveKey);

        partStatusArray = data._partStatusArray;
    }

    private SaveData.PuzzleData GetSaveSnapshot()
    {
        SaveData.PuzzleData data = new SaveData.PuzzleData()
        {
            _partStatusArray = partStatusArray,
        };

        return data;
    }
    #endregion
}
