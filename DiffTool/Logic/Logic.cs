using System.IO;
using System.Linq;


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

        public static Pos[] GetLCS(string path1, string path2)
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
    }
}
