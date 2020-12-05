using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using Logic;


namespace Model
{
    public class FileDifferenceModel
    {
        private static void Main()
        {
        }


        public string Path1 { get; set; }
        public string Path2 { get; set; }

        public List<ChangeBlock> ChangeBlocks { get; private set; }

        public readonly bool IsEmpty;

        public void CalculateDifference()
        {
            var lcs = FileOperations.GetLCS(Path1, Path2);
            ChangeBlocks = LcsToChangeBlocks(Path1, Path2, lcs);
        }

        public FileDifferenceModel(string path1, string path2)
        {
            Path1 = path1;
            Path2 = path2;
            CalculateDifference();

            IsEmpty = ChangeBlocks.Count() == 0;
        }

        /// <summary>
        /// Construct Change blocks using pre-calculated longest common subsequence of lines
        /// </summary>
        /// <param name="path1"> Path to the first file </param>
        /// <param name="path2"> Path to the first file </param>
        /// <param name="lcs"> Longest common subsequence as a collection of pairs of line numbers </param>
        /// <returns> List of blocks of changes </returns>
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
                                start1 + 1,
                                end1,
                                start2 + 1,
                                end2,
                                lines1.LineRange(start1 + 1, end1),
                                lines2.LineRange(start2 + 1, end2)
                            ));
                    }

            return changeBlocks;
        }

        public void Save(string filename)
        {
            using (var saveFile = new StreamWriter(filename))
            {
                saveFile.WriteLine(Path1);
                saveFile.WriteLine(Path2);
                using (var md5 = MD5.Create())
                {
                    using (var stream1 = File.OpenRead(Path1))
                        saveFile.WriteLine(md5.ComputeHash(stream1));

                    using (var stream2 = File.OpenRead(Path2))
                        saveFile.WriteLine(md5.ComputeHash(stream2));
                }
                foreach (var changeBlock in ChangeBlocks)
                    changeBlock.Save(saveFile);
            }
            
        }

        public void Load(string filename)
        {
            using (var saveFile = new StreamWriter(filename))
            {
                saveFile.WriteLine(Path1);
                saveFile.WriteLine(Path2);
                using (var md5 = MD5.Create())
                {
                    using (var stream1 = File.OpenRead(Path1))
                        saveFile.WriteLine(md5.ComputeHash(stream1));

                    using (var stream2 = File.OpenRead(Path2))
                        saveFile.WriteLine(md5.ComputeHash(stream2));
                }
                foreach (var changeBlock in ChangeBlocks)
                    changeBlock.Save(saveFile);

            }

        }

    }
}

