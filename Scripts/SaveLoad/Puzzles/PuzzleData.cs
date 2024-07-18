namespace SaveData
{
    [System.Serializable]
    public class PuzzleData
    {
        public int[] _partStatusArray;

        public PuzzleData()
        {
            _partStatusArray = new int[9];
        }
    }
}
