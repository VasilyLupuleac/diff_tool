using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Threading;


namespace Logic
{
    public class ChangeBlock
    {
        public int StartPos;  // Position in the second (changed) file
        public int EndPos;    //
        public string[] Delete;
        public string[] Insert;

        public ChangeBlock(int start, int end, string[] del, string[] ins)
        {
            StartPos = start;
            EndPos = end;
            Delete = del;
            Insert = ins;
        }
    }
    public static class FileOperations
    {
        private static void Main(string[] args) { }

        public class Pos
        {
            public readonly int Pos1;
            public readonly int Pos2;

            public Pos(int x1, int x2)
            {
                Pos1 = x1;
                Pos2 = x2;
            }
        }

        public static List<ChangeBlock> GetDiff(string path1, string path2)
        {
            var lines1 = File.ReadAllLines(path1);
            var lines2 = File.ReadAllLines(path2);
            var lcs = GetLcs(lines1, lines2);
            return GetChangeBlocks(lines1, lines2, lcs);
        }

        public static Pos[] GetLcs(IReadOnlyList<string> lines1, IReadOnlyList<string> lines2)
        {
            var len1 = lines1.Count;
            var len2 = lines2.Count;
            var lcs = new int[len1 + 1, len2 + 1]; // lcs[i, j] is the length of the longest common subsequence of lines1[0..i] and lines2[0..j]
            var prev = new Pos[len1 + 1, len2 + 1]; // Used for restoring the subsequence

            for (var i = 0; i < len1; i++)
            {
                for (var j = 0; j < len2; j++)
                {
                    if (lines1[i] == lines2[j]) // This pair of lines can be included into the LCS
                    {
                        lcs[i + 1, j + 1] = lcs[i, j] + 1;
                        prev[i, j] = new Pos(i - 1, j - 1);
                    }

                    else if (lcs[i + 1, j] > lcs[i, j + 1]) // The current line in the second file is not used in the LCS of this prefix
                    {
                        lcs[i + 1, j + 1] = lcs[i + 1, j];
                        prev[i, j] = new Pos(i, j - 1);
                    }
                    else
                    {
                        lcs[i + 1, j + 1] = lcs[i, j + 1]; // The current line in the first file is not used in the LCS of this prefix
                        prev[i, j] = new Pos(i - 1, j);
                    }
                }
            }



            var lcsLength = lcs[len1, len2];
            var lcsSample = new Pos[lcsLength];

            var curPos = lcsLength - 1;       // Restoring the subsequence from the end
            int cur1 = len1 - 1, cur2 = len2 - 1; //
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

        public static List<ChangeBlock> GetChangeBlocks(string[] lines1, string[] lines2, IEnumerable<Pos> lcs)
        {
            var changeBlocks = new List<ChangeBlock>();
            var lcsWithEof = new List<Pos>(lcs) { new Pos(lines1.Length, lines2.Length) }; // Consider ends of files as fictive equal lines
            var end1 = -1;
            var end2 = -1;

            foreach (var pos in lcsWithEof)
            {
                var start1 = end1;
                var start2 = end2;
                end1 = pos.Pos1;
                end2 = pos.Pos2;
                if (end1 - start1 > 1 || end2 - start2 > 1)  // Equal lines are not consecutive so there are changes between them
                    changeBlocks.Add(new ChangeBlock(
                        start2 + 1,
                        end2 - 1,
                        lines1[(start1 + 1)..end1],
                        lines2[(start2 + 1)..end2]
                    ));
            }

            return changeBlocks;
        }
    }



}
