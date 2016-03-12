using System;
using System.Text;
namespace WordsMatching
{
    public class MatchsMaker
    {
        private string _lString;
        private string _rString;
        private string[] _leftTokens;
        private string[] _rightTokens;
        private int leftLen;
        private int rightLen;
        private float[,] cost;
        private Similarity getSimilarity;
        private bool _accentInsensitive;
        public float Score
        {
            get
            {
                return this.GetScore();
            }
        }
        public MatchsMaker(string left, string right)
            : this(left, right, false)
        {
        }
        public MatchsMaker(string left, string right, bool accentInsensitive)
        {
            this._accentInsensitive = accentInsensitive;
            this._lString = left;
            this._rString = right;
            if (this._accentInsensitive)
            {
                this._lString = this.StripAccents(this._lString);
                this._rString = this.StripAccents(this._rString);
            }
            this.MyInit();
        }
        private string StripAccents(string input)
        {
            string text = "àÀâÂäÄáÁéÉèÈêÊëËìÌîÎïÏòÒôÔöÖùÙûÛüÜçÇ’ñ";
            string text2 = "aAaAaAaAeEeEeEeEiIiIiIoOoOoOuUuUuUcC'n";
            StringBuilder stringBuilder = new StringBuilder(input);
            for (int i = 0; i < text.Length; i++)
            {
                char oldChar = text[i];
                char newChar = text2[i];
                stringBuilder.Replace(oldChar, newChar);
            }
            stringBuilder.Replace("œ", "oe");
            stringBuilder.Replace("Æ", "ae");
            return stringBuilder.ToString();
        }
        private void MyInit()
        {
            ISimilarity @object = new Leven();
            this.getSimilarity = new Similarity(@object.GetSimilarity);
            Tokeniser tokeniser = new Tokeniser();
            this._leftTokens = tokeniser.Partition(this._lString);
            this._rightTokens = tokeniser.Partition(this._rString);
            if (this._leftTokens.Length > this._rightTokens.Length)
            {
                string[] leftTokens = this._leftTokens;
                this._leftTokens = this._rightTokens;
                this._rightTokens = leftTokens;
                string lString = this._lString;
                this._lString = this._rString;
                this._rString = lString;
            }
            this.leftLen = this._leftTokens.Length - 1;
            this.rightLen = this._rightTokens.Length - 1;
            this.Initialize();
        }
        private void Initialize()
        {
            this.cost = new float[this.leftLen + 1, this.rightLen + 1];
            for (int i = 0; i <= this.leftLen; i++)
            {
                for (int j = 0; j <= this.rightLen; j++)
                {
                    this.cost[i, j] = this.getSimilarity(this._leftTokens[i], this._rightTokens[j]);
                }
            }
        }
        public float GetScore()
        {
            BipartiteMatcher bipartiteMatcher = new BipartiteMatcher(this._leftTokens, this._rightTokens, this.cost);
            return bipartiteMatcher.Score;
        }
    }
}
