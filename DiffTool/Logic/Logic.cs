using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Logic
{

    public static class FileOperations
    {
        private static void Main()
        {
        }

        public readonly struct Pos
        {
            public readonly int Pos1;
            public readonly int Pos2;

            public Pos(int x1, int x2)
            {
                Pos1 = x1;
                Pos2 = x2;
            }
        }

        /// <summary>
        /// Calculate the longest common subsequence of the contents of two files
        /// </summary>
        /// <param name="path1"> Path to the first file </param>
        /// <param name="path2"> Path to the first file </param>
        /// <returns>  Longest common subsequence as a collection of pairs of positions </returns>
        public static Pos[] GetLcs(string path1, string path2, bool useHirschberg=true)
        {
            if (useHirschberg)
            {
                var lines1 = File.ReadAllLines(path1);
                var lines2 = File.ReadAllLines(path2);
                var lcs = CalculateLcsHirschberg(
                    new Subarray(lines1, 0, lines1.Length),
                    new Subarray(lines1, 0, lines1.Length)
                ).ToArray();
                Array.Sort(lcs, (a, b) => a.Pos1 - b.Pos1);
                return lcs;
            }

            return CalculateLcsWithoutLoading(path1, path2);
        }

        public static Pos[] CalculateLcsWithoutLoading(string path1, string path2)
        {
            var len1 = File.ReadLines(path1).Count(); // ReadLines() is lazy so it doesn't load the contents into memory
            var len2 = File.ReadLines(path1).Count();  //

            var lcs = new int[len1 + 1, len2 + 1]; // lcs[i, j] is the length of the longest common subsequence of lines1[0..i] and lines2[0..j]
            var prev = new Pos[len1 + 1, len2 + 1]; // Used for restoring the subsequence

            using (var lines1 = new StreamReader(path1))
                for (var i = 0; i < len1; i++)
                {
                    var line1 = lines1.ReadLine();

                    using (var lines2 = new StreamReader(path2))
                        for (var j = 0; j < len2; j++)
                        {
                            var line2 = lines2.ReadLine();
                            if (line1 == line2) // This pair of lines can be included into the LCS
                            {
                                lcs[i + 1, j + 1] = lcs[i, j] + 1;
                                prev[i + 1, j + 1] = new Pos(i, j);
                            }

                            else if (lcs[i + 1, j] > lcs[i, j + 1]) // The current line in the second file is not used in the LCS of this prefix
                            {
                                lcs[i + 1, j + 1] = lcs[i + 1, j];
                                prev[i + 1, j + 1] = new Pos(i + 1, j);
                            }
                            else
                            {
                                lcs[i + 1, j + 1] = lcs[i, j + 1]; // The current line in the first file is not used in the LCS of this prefix
                                prev[i + 1, j + 1] = new Pos(i, j + 1);
                            }
                        }
                }



            var lcsLength = lcs[len1, len2];
            var lcsSample = new Pos[lcsLength];

            var curPos = lcsLength - 1;       // Restoring the subsequence from the end
            int cur1 = len1, cur2 = len2; //
            while (curPos >= 0)
            {
                var pr = prev[cur1, cur2];

                if (pr.Pos1 == cur1 - 1 && pr.Pos2 == cur2 - 1)
                    lcsSample[curPos--] = new Pos(cur1, cur2);  // Lines are equal so they can be included into the LCS

                cur1 = pr.Pos1;
                cur2 = pr.Pos2;
            }

            return lcsSample;
        }


        private readonly struct Subarray
        {
            public readonly string[] Array;
            public readonly int Start;
            public readonly int End;
            public readonly bool IsReversed;
            public int Length
            {
                get
                {
                    if (Start >= End)
                        return Start - End;
                    return End - Start;
                }
            }

            public string this[int i]
            {
                get 
                { 
                    if (IsReversed)
                        return Array[End - 1 - i];
                    return Array[Start + i];
                }
            }

            public int Index(int i)
            {
                if (IsReversed)
                    return End - i;
                else 
                    return Start + i;
            }

            public Subarray(string[] array, int start, int end, bool rev=false)
            {
                Array = array;
                Start = start;
                End = end;
                IsReversed = rev;
            }
            public Subarray(Subarray sa, int start, int end, bool rev=false)
            {
                Array = sa.Array;
                Start = sa.Index(start);
                End = sa.Index(end);
                IsReversed = rev ^ sa.IsReversed;
            }
        }


        static int[] LcsLength(Subarray a, Subarray b)
        {
            var len1 = a.Length;
            var len2 = b.Length;
            var lcs = new int[len2 + 1];  // LCS length of current prefix of a and all prefixes of b
            var prev = new int[len2 + 1]; // LCS  length of previous prefix of a and all prefixes of b

            for (var i = 1; i <= len1; i++)
            {
                for (var j = 1; j <= len2; j++)
                {
                    if (a[i - 1] == b[j - 1])
                        lcs[j] = prev[j - 1] + 1;
                    else
                        lcs[j] = Math.Max(prev[j], lcs[j - 1]);
                }
                prev = lcs;
            }
            return lcs;
        }

        private static List<Pos> CalculateLcsHirschberg(Subarray lines1, Subarray lines2)
        {
            var lcs = new List<Pos>();
            var len1 = lines1.Length;
            var len2 = lines2.Length;
            if (len2 <= 0)
                return lcs;

            if (lines1.Length == 1)
                for (int i = 0; i < len2; i++)
                    if (lines2[i] == lines1[0])
                    {
                        lcs.Add(new Pos(lines1.Index(0), lines2.Index(i)));
                        return lcs;
                    }

            var mid = len1 / 2;
            var f = LcsLength(new Subarray(lines1, 0, mid), lines2);
            var s = LcsLength(
                        new Subarray(lines1, mid + 1,len1, true),
                        new Subarray(lines2, 0, len2, true)
                    );

            var max = s[1];
            var maxInd = 0;

            for (int i = 0; i < len2; i++)
                if (f[i] + s[i + 1] > max)
                {
                    max = f[i] + s[i + 1];
                    maxInd = i;
                }

            if (f[lines2.Length - 1] > max)
                maxInd = len2 - 1;

            lcs = CalculateLcsHirschberg(
                new Subarray(lines1, 0, mid),
                new Subarray(lines2, 0, maxInd)
            );
            lcs.AddRange(
                CalculateLcsHirschberg(
                new Subarray(lines1, mid + 1, len1),
                new Subarray(lines2, maxInd + 1, len2)
                )
            );

            return lcs;
        }
    }

}
