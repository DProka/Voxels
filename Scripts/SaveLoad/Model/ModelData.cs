namespace SaveData
{
    [System.Serializable]
    public class ModelData
    {
        public int[] _savedCubesID;
        public int[] _savedCubesStatus;

        public ModelData()
        {
            _savedCubesID = new int[0];
            _savedCubesStatus = new int[0];
        }
    }
}
