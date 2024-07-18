namespace SaveData
{
    [System.Serializable]
    public class UIData
    {
        public int[] _currentPuzzle;

        public int[] _challengeStatus;
        public int _currentChallengeLvl;

        public UIData()
        {
            _currentPuzzle = new int[] { 0, 0 };
            _challengeStatus = new int[0];

            _currentChallengeLvl = 0;
        }
    }
}
