using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Model;
using Logic;
using NUnit.Framework;

namespace Model.Test
{
    [TestFixture]
    public class FileDifferenceModelTest
    {
        [Test]
        public void TestGetChangeBlocksGeneral()
        {
            var dir = "Temp";
            Directory.CreateDirectory(dir);
            var path1 = Path.Combine(dir, "file1.txt");
            var path2 = Path.Combine(dir, "file2.txt");
            using (File.Create(path1));
            using (File.Create(path2));

            var lcs12 = new[]
            {
                new FileOperations.Pos(0, 0),
                new FileOperations.Pos(1, 1)
            };

            string[] lines125 = { "1", "2", "5" };
            string[] lines123 = { "1", "2", "3" };

            File.WriteAllLines(path1, lines123);
            File.WriteAllLines(path2, lines125);
            var model12 = new FileDifferenceModel(path1, path2);
            var changeBlocks12 = model12.ChangeBlocks;
            Assert.AreEqual(changeBlocks12.Count, 1);
            Assert.AreEqual(changeBlocks12[0].Delete, new[] { "3" });
            Assert.AreEqual(changeBlocks12[0].Insert, new[] { "5" });
            Assert.AreEqual(changeBlocks12[0].StartPos2, 2);
            Assert.AreEqual(changeBlocks12[0].EndPos2, 3);


            var lines1 = new[]
            {
                "Line ¹1",
                "Line ¹2",
                "Line ¹3",
                "Line ¹4",
                "Line ¹5",
                "Line ¹6",
                "Line ¹7",
                "Line ¹8",
                "Line ¹9",
                "Line ¹10"
            };

            var lines2 = new[]
            {
                "Line ¹0",
                "Line ¹1",
                "Line ¹3",
                "Line ¹4",
                "Line 5",
                "Line ¹6",
                "Line ¹7",
                "Line ¹8",
                "Extra line 1",
                "Extra line 2",
                "Final line",
                "Line ¹9",
                "Line ¹10"
            };

            var lcs = new[]
            {
                new FileOperations.Pos(0, 1),
                new FileOperations.Pos(2, 2),
                new FileOperations.Pos(3, 3),
                new FileOperations.Pos(5, 5),
                new FileOperations.Pos(6, 6),
                new FileOperations.Pos(7, 7),
                new FileOperations.Pos(9, 12)

            };

            File.WriteAllLines(path1, lines1);
            File.WriteAllLines(path2, lines2);
            var model = new FileDifferenceModel(path1, path2);
            var changeBlocks = model.ChangeBlocks;
            Assert.AreEqual(changeBlocks.Count, 4);
            Assert.Zero(changeBlocks[0].Delete.Length);
            Assert.Zero(changeBlocks[1].Insert.Length);
            Assert.AreEqual(changeBlocks[3].Delete, new[] { "Line ¹9"});
            Assert.AreEqual(changeBlocks[2].Delete.Length, 1);
            Assert.AreEqual(changeBlocks[2].Insert.Length, 1);

            File.Delete(path1);
            File.Delete(path2);
            Directory.Delete(dir);
        }
    }
}