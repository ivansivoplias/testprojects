using System.Text;

namespace BullsAndCows
{
    public class Permutations
    {
        private int _systemBase; //n
        private int _combinationLength; //k
        private int _index;
        private bool _hasNext;
        private int[] _sequence;

        public bool HasNext
        {
            get
            {
                return _hasNext;
            }
        }

        public Permutations(int k, int n)
        {
            Initialize(k, n);
        }

        private void Initialize(int k, int n)
        {
            _systemBase = n;
            _combinationLength = k;
            _sequence = new int[k];
            _hasNext = true;

            _index = 0;
            _sequence[0] = -1;
            Add();
        }

        private void Add()
        {
            int j = _index;
            _sequence[j]++;

            for (j++; j < _combinationLength; j++)
            {
                _sequence[j] = 0;
            }
        }

        private bool Next()
        {
            int j;
            for (j = _combinationLength - 1; j >= 0; j--)
            {
                if (_systemBase != _sequence[j])
                {
                    break;
                }
            }

            _index = j;

            if (_index == -1)
            {
                return false;
            }
            Add();

            return true;
        }

        public string GetElement()
        {
            int j;
            var number = new StringBuilder();

            for (j = 0; j < _combinationLength; j++)
            {
                number.Append(_sequence[j]);
            }

            _hasNext = Next();

            return number.ToString();
        }
    }
}