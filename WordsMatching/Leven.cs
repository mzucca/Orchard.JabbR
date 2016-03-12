using System;
namespace WordsMatching
{
    internal class Leven : ISimilarity
    {
        private int Min3(int a, int b, int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }
        private int ComputeDistance(string s, string t)
        {
            int length = s.Length;
            int length2 = t.Length;
            int[,] array = new int[length + 1, length2 + 1];
            int result;
            if (length == 0)
            {
                result = length2;
            }
            else
            {
                if (length2 == 0)
                {
                    result = length;
                }
                else
                {
                    int i = 0;
                    while (i <= length)
                    {
                        array[i, 0] = i++;
                    }
                    int j = 0;
                    while (j <= length2)
                    {
                        array[0, j] = j++;
                    }
                    for (i = 1; i <= length; i++)
                    {
                        for (j = 1; j <= length2; j++)
                        {
                            int num = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1)) ? 0 : 1;
                            array[i, j] = this.Min3(array[i - 1, j] + 1, array[i, j - 1] + 1, array[i - 1, j - 1] + num);
                        }
                    }
                    result = array[length, length2];
                }
            }
            return result;
        }
        public float GetSimilarity(string string1, string string2)
        {
            float num = (float)this.ComputeDistance(string1, string2);
            float num2 = (float)string1.Length;
            if (num2 < (float)string2.Length)
            {
                num2 = (float)string2.Length;
            }
            float num3 = (float)string1.Length;
            if (num3 > (float)string2.Length)
            {
                num3 = (float)string2.Length;
            }
            float result;
            if (num2 == 0f)
            {
                result = 1f;
            }
            else
            {
                result = num2 - num;
            }
            return result;
        }
    }
}
