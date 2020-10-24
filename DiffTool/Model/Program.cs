using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Model
{
    class Logic
    {
        static void Main(string[] args)
        {
            int[] a = new int[10];
        }

        public class ChangeBlock
        {
            public int startPos;
            public int endPos;
            public string[] delete;
            public string[] insert;

            public ChangeBlock(int start, int end, string[] del, string[] ins)
            {
                startPos = start;
                endPos = end;
                delete = del;
                insert = ins;
            }
        }

        private class Pos
        { 
            public int pos1, pos2;

            public Pos(int x1, int x2)
            {
                pos1 = x1;
                pos2 = x2;
            }
        }

        private Pos[] GetLCS(string[] lines1, string[] lines2)
        {
            var len1 = lines1.Length;
            var len2 = lines2.Length;
            var lcs = new int[len1 + 1, len1 + 1];
            var prev = new Pos[len1 + 1, len2 + 1];

            for (var i = 1; i <= len1; i++)
            {
                for (var j = 1; j <= len2; j++)
                {
                    if (lines1[i] == lines2[j])
                        lcs[i, j] = lcs[i - 1, j - 1] + 1;
                    else if (lcs[i, j - 1] < lcs[i - 1, j])
                    {
                        lcs[i, j] = lcs[i, j - 1];
                        prev[i, j] = new Pos(i, j - 1);
                    }
                    else
                    {
                        lcs[i, j] = lcs[i - 1, j];
                        prev[i, j] = new Pos(i - 1, j);
                    }
                }
            }


            var lcsLength = lcs[len1, len2];
            var lcsSample = new Pos[lcsLength];
            var curPos = lcsLength - 1;

            int i = len1, j = len2;
            while (curPos >= 0)
            {
                if (prev[i, j] == new Pos(i - 1, j - 1))
                    lcsSample[curPos--] = new Pos(i-- , j--);
                else
                {
                    i = prev[i, j].pos1;
                    j = prev[i, j].pos2;
                }
            }

            return lcsSample;
        }

        private List<ChangeBlock> GetChangeBlocks(Pos[] lcs, string[] lines1, string[] lines2)
        {
            var changeBlocks = new List<ChangeBlock>();
            for (int i = 1; i < lcs.Length; i++)
            {
                int start1 = lcs[i - 1].pos1;
                int start2 = lcs[i - 1].pos2;
                int end1 = lcs[i].pos1;
                int end2 = lcs[i].pos2;
                if (end1 - start1 > 1)
                    changeBlocks.Add(new ChangeBlock(
                        start1 + 1,
                        end1,
                        (new ArraySegment<string>(lines1, start1 + 1, end1 - start1)).Array,
                        (new ArraySegment<string>(lines2, start2 + 1, end2 - start2)).Array
                    ));
            }

            return changeBlocks;
        }
        
        
    }
}
