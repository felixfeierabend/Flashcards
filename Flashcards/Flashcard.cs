using System;
using System.Collections.Generic;
using System.Text;
using SerializableAttribute = System.SerializableAttribute;

namespace Flashcards
{
    [Serializable]
    public class Flashcard
    {
        public string Question { get; set; }
        public string Solution { get; set; }
        public int Difficulty { get; set; }
        public byte[] Image { get; set; }

        public Flashcard(string question, string solution, int difficulty = 5, byte[] image = null)
        {
            Question = question;
            Solution = solution;
            Difficulty = difficulty;
            Image = image;
        }
        
        private int CalculateLevenshteinDistance(string s1, string s2)
        {
            int n = s1.Length;
            int m = s2.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++) ;

            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost;

                    cost = s2[j - 1] == s1[i - 1] ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }



        public override bool Equals(object obj)
        {
            return obj is Flashcard dataset &&
                   Question == dataset.Question &&                   
                   Solution == dataset.Solution;
        }

        public override string ToString()
        {
            return Question;
        }
    }
}
