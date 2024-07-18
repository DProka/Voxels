namespace SaveData
{
    [System.Serializable]
    public class GeneralData
    {
        public int _playerLvl;
        public int _playerCoins;

        public bool _isBasicModel;
        public int _modelNum;

        public GeneralData()
        {
            _playerLvl = 1;
            _playerCoins = 0;

            _isBasicModel = true;
            _modelNum = 0;
        }
    }
}
