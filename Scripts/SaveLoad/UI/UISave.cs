
public class UISave
{
    public int[] currentPuzzle { get; private set; }

    public int[] challengeStatus { get; private set; }
    public int currentChallengeLvl { get; private set; }

    private string saveKey = "UISave";

    public UISave()
    {
        Load();
    }

    public void SaveActivePuzzle(int[] status)
    {
        currentPuzzle = status;
        Save();
    }
    
    public void SaveChallengeAlbum(int[] status)
    {
        challengeStatus = status;
        Save();
    }

    public void SaveChallengeLevel(int lvl)
    {
        currentChallengeLvl = lvl;
        Save();
    }

    #region Save Load

    public void ResetSave()
    {
        SaveData.UIData general = new SaveData.UIData();

        currentPuzzle = general._currentPuzzle;

        challengeStatus = general._challengeStatus;
        currentChallengeLvl = general._currentChallengeLvl;

        Save();
    }

    private void Save() => SaveManager.Save(saveKey, GetSaveSnapshot());

    private void Load()
    {
        var data = SaveManager.Load<SaveData.UIData>(saveKey);

        currentPuzzle = data._currentPuzzle;

        challengeStatus = data._challengeStatus;
        currentChallengeLvl = data._currentChallengeLvl;
    }

    private SaveData.UIData GetSaveSnapshot()
    {
        SaveData.UIData data = new SaveData.UIData()
        {
            _currentPuzzle = currentPuzzle,

            _challengeStatus = challengeStatus,
            _currentChallengeLvl = currentChallengeLvl,
        };

        return data;
    }
    #endregion
}
