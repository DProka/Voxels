
public class ModelSave
{
    public int[] savedCubesID { get; private set; }
    public int[] savedCubesStatus { get; private set; }

    private const string saveKey = "ModelSave";

    #region Player Credits

    public void SaveModel(int[] cubes, int[] status)
    {
        savedCubesID = cubes;
        savedCubesStatus = status;

        Save();
    }

    #endregion

    #region Save Load

    public void ResetSave()
    {
        SaveData.ModelData general = new SaveData.ModelData();

        savedCubesID = general._savedCubesID;
        savedCubesStatus = general._savedCubesStatus;

        Save();
    }

    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.ModelData>(saveKey);

        savedCubesID = data._savedCubesID;
        savedCubesStatus = data._savedCubesStatus;
    }

    private SaveData.ModelData GetSaveSnapshot()
    {
        SaveData.ModelData data = new SaveData.ModelData()
        {
            _savedCubesID = savedCubesID,
            _savedCubesStatus = savedCubesStatus,
        };

        return data;
    }
    #endregion
}
