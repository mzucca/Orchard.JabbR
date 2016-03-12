using System;
using System.Diagnostics;
namespace WordsMatching
{
    public class BipartiteMatcher
    {
        private string[] _leftTokens;
        private string[] _rightTokens;
        private float[,] _cost;
        private float[] leftLabel;
        private float[] rightLabel;
        private int[] _previous;
        private int[] _incomming;
        private int[] _outgoing;
        private bool[] _leftVisited;
        private bool[] _rightVisited;
        private int leftLen;
        private int rightLen;
        private bool _errorOccured = false;
        private bool stop = false;
        public float Score
        {
            get
            {
                float result;
                if (this._errorOccured)
                {
                    result = 0f;
                }
                else
                {
                    result = this.GetScore();
                }
                return result;
            }
        }
        public BipartiteMatcher(string[] left, string[] right, float[,] cost)
        {
            if (left == null || right == null || cost == null)
            {
                this._errorOccured = true;
            }
            else
            {
                this._leftTokens = left;
                this._rightTokens = right;
                if (this._leftTokens.Length > this._rightTokens.Length)
                {
                    float[,] array = new float[this._rightTokens.Length, this._leftTokens.Length];
                    for (int i = 0; i < this._rightTokens.Length; i++)
                    {
                        for (int j = 0; j < this._leftTokens.Length; j++)
                        {
                            array[i, j] = cost[j, i];
                        }
                    }
                    this._cost = (float[,])array.Clone();
                    string[] leftTokens = this._leftTokens;
                    this._leftTokens = this._rightTokens;
                    this._rightTokens = leftTokens;
                }
                else
                {
                    this._cost = (float[,])cost.Clone();
                }
                this.MyInit();
                this.Make_Matching();
            }
        }
        private void MyInit()
        {
            this.Initialize();
            this._leftVisited = new bool[this.leftLen + 1];
            this._rightVisited = new bool[this.rightLen + 1];
            this._previous = new int[this.leftLen + this.rightLen + 2];
        }
        private void Initialize()
        {
            this.leftLen = this._leftTokens.Length - 1;
            this.rightLen = this._rightTokens.Length - 1;
            this.leftLabel = new float[this.leftLen + 1];
            this.rightLabel = new float[this.rightLen + 1];
            for (int i = 0; i < this.leftLabel.Length; i++)
            {
                this.leftLabel[i] = 0f;
            }
            for (int i = 0; i < this.rightLabel.Length; i++)
            {
                this.rightLabel[i] = 0f;
            }
            for (int i = 0; i <= this.leftLen; i++)
            {
                float num = -3.40282347E+38f;
                for (int j = 0; j <= this.rightLen; j++)
                {
                    if (this._cost[i, j] > num)
                    {
                        num = this._cost[i, j];
                    }
                }
                this.leftLabel[i] = num;
            }
        }
        private void Flush()
        {
            for (int i = 0; i < this._previous.Length; i++)
            {
                this._previous[i] = -1;
            }
            for (int i = 0; i < this._leftVisited.Length; i++)
            {
                this._leftVisited[i] = false;
            }
            for (int i = 0; i < this._rightVisited.Length; i++)
            {
                this._rightVisited[i] = false;
            }
        }
        private bool FindPath(int source)
        {
            this.Flush();
            this.stop = false;
            this.Walk(source);
            return this.stop;
        }
        private void Increase_Matchs(int li, int lj)
        {
            int[] array = (int[])this._outgoing.Clone();
            int num = li;
            this._outgoing[num] = lj;
            this._incomming[lj] = num;
            if (this._previous[num] != -1)
            {
                do
                {
                    int num2 = array[num];
                    int num3 = this._previous[num];
                    this._outgoing[num3] = num2;
                    this._incomming[num2] = num3;
                    num = num3;
                }
                while (this._previous[num] != -1);
            }
        }
        private void Walk(int i)
        {
            this._leftVisited[i] = true;
            for (int j = 0; j <= this.rightLen; j++)
            {
                if (this.stop)
                {
                    break;
                }
                if (!this._rightVisited[j] && this.leftLabel[i] + this.rightLabel[j] == this._cost[i, j])
                {
                    if (this._incomming[j] == -1)
                    {
                        this.stop = true;
                        this.Increase_Matchs(i, j);
                        break;
                    }
                    int num = this._incomming[j];
                    this._rightVisited[j] = true;
                    this._previous[num] = i;
                    this.Walk(num);
                }
            }
        }
        private float GetMinDeviation()
        {
            float num = 3.40282347E+38f;
            for (int i = 0; i <= this.leftLen; i++)
            {
                if (this._leftVisited[i])
                {
                    for (int j = 0; j <= this.rightLen; j++)
                    {
                        if (!this._rightVisited[j])
                        {
                            if (this.leftLabel[i] + this.rightLabel[j] - this._cost[i, j] < num)
                            {
                                num = this.leftLabel[i] + this.rightLabel[j] - this._cost[i, j];
                            }
                        }
                    }
                }
            }
            return num;
        }
        private void Relabels()
        {
            float minDeviation = this.GetMinDeviation();
            for (int i = 0; i <= this.leftLen; i++)
            {
                if (this._leftVisited[i])
                {
                    this.leftLabel[i] -= minDeviation;
                }
            }
            for (int i = 0; i <= this.rightLen; i++)
            {
                if (this._rightVisited[i])
                {
                    this.rightLabel[i] += minDeviation;
                }
            }
        }
        private void Make_Matching()
        {
            this._outgoing = new int[this.leftLen + 1];
            this._incomming = new int[this.rightLen + 1];
            for (int i = 0; i < this._outgoing.Length; i++)
            {
                this._outgoing[i] = -1;
            }
            for (int i = 0; i < this._incomming.Length; i++)
            {
                this._incomming[i] = -1;
            }
            for (int j = 0; j <= this.leftLen; j++)
            {
                if (this._outgoing[j] == -1)
                {
                    bool flag;
                    do
                    {
                        flag = this.FindPath(j);
                        if (!flag)
                        {
                            this.Relabels();
                        }
                    }
                    while (!flag);
                }
            }
        }
        private float GetTotal()
        {
            float num = 0f;
            float num2 = 0f;
            Trace.Flush();
            for (int i = 0; i <= this.leftLen; i++)
            {
                if (this._outgoing[i] != -1)
                {
                    num += this._cost[i, this._outgoing[i]];
                    Trace.WriteLine(string.Concat(new object[]
					{
						this._leftTokens[i],
						" <-> ",
						this._rightTokens[this._outgoing[i]],
						" : ",
						this._cost[i, this._outgoing[i]]
					}));
                    float num3 = (1f - (float)Math.Max(this._leftTokens[i].Length, this._rightTokens[this._outgoing[i]].Length) != 0f) ? (this._cost[i, this._outgoing[i]] / (float)Math.Max(this._leftTokens[i].Length, this._rightTokens[this._outgoing[i]].Length)) : 1f;
                    num2 += num3;
                }
            }
            return num;
        }
        public float GetScore()
        {
            float total = this.GetTotal();
            float num = (float)(this.rightLen + 1);
            int num2 = 0;
            int num3 = 0;
            string[] array = this._rightTokens;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                num2 += text.Length;
            }
            array = this._leftTokens;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                num3 += text.Length;
            }
            num = (float)Math.Max(num2, num3);
            float result;
            if (num > 0f)
            {
                result = total / num;
            }
            else
            {
                result = 1f;
            }
            return result;
        }
    }
}
