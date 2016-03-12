using System;
using System.Collections;
using System.Text.RegularExpressions;
namespace WordsMatching
{
    internal class Tokeniser
    {
        private ArrayList Tokenize(string input)
        {
            ArrayList arrayList = new ArrayList(10);
            int num;
            for (int i = 0; i < input.Length; i = num)
            {
                char c = input[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                }
                num = input.Length;
                for (int j = 0; j < "\r\n\t \u00a0".Length; j++)
                {
                    int num2 = input.IndexOf("\r\n\t \u00a0"[j], i);
                    if (num2 < num && num2 != -1)
                    {
                        num = num2;
                    }
                }
                string value = input.Substring(i, num - i);
                arrayList.Add(value);
            }
            return arrayList;
        }
        private void Normalize_Casing(ref string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsSeparator(input[i]))
                {
                    input = input.Replace(input[i].ToString(), " ");
                }
            }
            int j = 1;
            while (j < input.Length - 2)
            {
                j++;
                if (char.IsUpper(input[j]) && char.IsLower(input[j + 1]) && !char.IsWhiteSpace(input[j - 1]) && !char.IsSeparator(input[j - 1]))
                {
                    input = input.Insert(j, " ");
                    j++;
                }
            }
        }
        public string[] Partition(string input)
        {
            Regex regex = new Regex("([ \\t{}():;])");
            this.Normalize_Casing(ref input);
            input = input.ToLower();
            string[] array = regex.Split(input);
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < array.Length; i++)
            {
                MatchCollection matchCollection = regex.Matches(array[i]);
                if (matchCollection.Count <= 0 && array[i].Trim().Length > 0)
                {
                    arrayList.Add(array[i]);
                }
            }
            array = new string[arrayList.Count];
            for (int i = 0; i < arrayList.Count; i++)
            {
                array[i] = (string)arrayList[i];
            }
            return array;
        }
    }
}
