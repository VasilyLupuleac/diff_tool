using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logic;


namespace Model
{
    public class Model
    {
        private static void Main()
        {
        }
        public List<ChangeBlock> ChangeBlocks { get; }

        public Model(string path1, string path2)
        {
            var lcs = FileOperations.GetLCS(path1, path2);
            ChangeBlocks = LcsToChangeBlocks(path1, path2, lcs);
        }

        public static List<ChangeBlock> LcsToChangeBlocks(string path1, string path2, IEnumerable<FileOperations.Pos> lcs)
        {
            var len1 = File.ReadLines(path1).Count();
            var len2 = File.ReadLines(path2).Count();
            var changeBlocks = new List<ChangeBlock>();
            var lcsWithEof = new List<FileOperations.Pos>(lcs) { new FileOperations.Pos(len1, len2) }; // Consider ends of files as fictive equal lines
            var end1 = -1;
            var end2 = -1;

            using (var lines1 = new OneWayReader(path1))
                using (var lines2 = new OneWayReader(path2))
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
                                lines1.LineRange(start1 + 1, end1),
                                lines2.LineRange(start2 + 1, end2)
                            ));
                    }

            return changeBlocks;
        }
    }
}

