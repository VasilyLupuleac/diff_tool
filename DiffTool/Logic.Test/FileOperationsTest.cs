using Logic;
using NUnit.Framework;
using System.IO;

namespace Logic.Test
{
    [TestFixture]
    public class TestFileOperations
    {
        private static readonly string[] _125 = { "1", "2", "5" };
        private static readonly string[] _123 = { "1", "2", "3" };
        private static readonly string[] DoubleEmpty = { "", "" };
        private static readonly string[] Empty1235 = { "", "1", "2", "3", "5" };
        private static readonly string[] NoLines = { };

        private static object[] _testCasesSameFile =
        {
            _125,
            DoubleEmpty,
            NoLines
        };

        [Test]
        [TestCaseSource(nameof(_testCasesSameFile))]
        public void TestGetLCSSameFile(string[] lines)
        {
            var dir = "Temp";
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "file.txt");
            using (File.Create(path));
            File.WriteAllLines(path, lines);
            var lcs = FileOperations.GetLcs(path, path);
            Assert.AreEqual(lcs.Length, lines.Length);
            for (var i = 0; i < lcs.Length; i++)
            {
                Assert.AreEqual(lcs[i].Pos1, i);
                Assert.AreEqual(lcs[i].Pos2, i);
            }

            File.Delete(path);
            Directory.Delete(dir);
        }

        [Test]
        public void TestGetLCSGeneral()
        {
            var dir = "Temp";
            Directory.CreateDirectory(dir);
            var path1 = Path.Combine(dir, "file1.txt");
            var path2 = Path.Combine(dir, "file2.txt");
            using (File.Create(path1));
            using (File.Create(path2));

            File.WriteAllLines(path1, _123);
            File.WriteAllLines(path2, _125);
            var lcs12 = FileOperations.GetLcs(path1, path2);
            Assert.AreEqual(lcs12.Length, 2);
            Assert.AreEqual(lcs12[0].Pos1, 0);
            Assert.AreEqual(lcs12[1].Pos2, 1);

            File.WriteAllLines(path1, DoubleEmpty);
            File.WriteAllLines(path2, Empty1235);
            var lcsEmpty = FileOperations.GetLcs(path1, path2);
            Assert.AreEqual(lcsEmpty.Length, 1);
            Assert.AreEqual(lcsEmpty[0].Pos1, 0);
            Assert.AreEqual(lcsEmpty[0].Pos2, 0);

            File.WriteAllLines(path1, Empty1235);
            File.WriteAllLines(path2, _125);
            var lcs125 = FileOperations.GetLcs(path1, path2);
            Assert.AreEqual(lcs125.Length, 3);
            Assert.AreEqual(lcs125[0].Pos1, 1);
            Assert.AreEqual(lcs125[2].Pos2, 2);

            File.WriteAllLines(path1, _123);
            File.WriteAllLines(path2, NoLines);
            var lcsNo = FileOperations.GetLcs(path1, path2);
            Assert.Zero(lcsNo.Length);

            File.Delete(path1);
            File.Delete(path2);
            Directory.Delete(dir);
        }
    }
}