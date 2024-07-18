
public class GeneralSave
{
    public int playerLvl { get; private set; }
    public int playerCoins { get; private set; }
    
    public bool isBasicModel { get; private set; }
    public int modelNum { get; private set; }

    private const string saveKey = "GeneralSave";

    #region Player Credits

    public void SavePlayerStats(int lvl, int coins)
    {
        if(playerLvl != lvl)
            playerLvl = lvl;

        if (playerCoins != coins)
            playerCoins = coins;

        Save();
    }

    public void SaveModel(bool isBasic, int num)
    {
        isBasicModel = isBasic;
        modelNum = num;

        Save();
    }

    public int[] GetSave()
    {
        int[] save = new int[] { playerLvl, playerCoins };
        return save;
    }

    #endregion

    #region Save Load

    public void ResetSave()
    {
        SaveData.GeneralData general = new SaveData.GeneralData();

        playerLvl = general._playerLvl;
        playerCoins = general._playerCoins;

        isBasicModel = general._isBasicModel;
        modelNum = general._modelNum;

        Save();
    }

    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.GeneralData>(saveKey);

        playerLvl = data._playerLvl;
        playerCoins = data._playerCoins;

        isBasicModel = data._isBasicModel;
        modelNum = data._modelNum;
    }

    private SaveData.GeneralData GetSaveSnapshot()
    {
        SaveData.GeneralData data = new SaveData.GeneralData()
        {
            _playerLvl = playerLvl,
            _playerCoins = playerCoins,

            _isBasicModel = isBasicModel,
            _modelNum = modelNum,
        };

        return data;
    }
    #endregion
}
