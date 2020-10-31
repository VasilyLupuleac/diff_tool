using NUnit.Framework;

namespace Logic.UnitTests
{
    [TestFixture]
    public class TestFileOperations
    {
        private static readonly string[] _125 = {"1", "2", "5"};
        private static readonly string[] _123 = {"1", "2", "3"};
        private static readonly string[] _doubleEmpty = {"", ""};
        private static readonly string[] _empty1235 = {"", "1", "2", "3", "5"};
        private static readonly string[] _noLines = { };

        private static object[] _testCasesSameFile =
        {
            _125,
            _doubleEmpty,
            _noLines
        };

        [Test]
        [TestCaseSource(nameof(_testCasesSameFile))]
        public void TestGetLcsSameFile(string[] lines)
        {
            var lcs = FileOperations.GetLcs(lines, lines);
            Assert.AreEqual(lcs.Length, lines.Length);
            for (var i = 0; i < lcs.Length; i++)
            {
                Assert.AreEqual(lcs[i].Pos1, i);
                Assert.AreEqual(lcs[i].Pos2, i);
            }
        }

        [Test]
        public void TestGetLcsGeneral()
        {
            var lcs12 = FileOperations.GetLcs(_123, _125);
            Assert.AreEqual(lcs12.Length, 2);
            Assert.AreEqual(lcs12[0].Pos1, 0);
            Assert.AreEqual(lcs12[1].Pos2, 1);

            var lcsEmpty = FileOperations.GetLcs(_doubleEmpty, _empty1235);
            Assert.AreEqual(lcsEmpty.Length, 1);
            Assert.AreEqual(lcsEmpty[0].Pos1, 0);
            Assert.AreEqual(lcsEmpty[0].Pos2, 0);

            var lcs125 = FileOperations.GetLcs(_empty1235, _125);
            Assert.AreEqual(lcs125.Length, 3);
            Assert.AreEqual(lcs125[0].Pos1, 1);
            Assert.AreEqual(lcs125[2].Pos2, 2);

            var lcsNo = FileOperations.GetLcs(_123, _noLines);
            Assert.Zero(lcsNo.Length);
        }

        [Test]
        public void TestGetChangeBlocksGeneral()
        {
            var lcs12 = new[]
            {
                new FileOperations.Pos(0, 0),
                new FileOperations.Pos(1, 1)
            };

            var cb1 = FileOperations.GetChangeBlocks(_123, _125, lcs12);
            Assert.AreEqual(cb1.Count, 1);
            Assert.AreEqual(cb1[0].Delete, new[] {"3"});
            Assert.AreEqual(cb1[0].Insert, new[] {"5"});
            Assert.AreEqual(cb1[0].StartPos, 2);
            Assert.AreEqual(cb1[0].EndPos, 2);


            var lines1 = new[]
            {
                "Line №1",
                "Line №2",
                "Line №3",
                "Line №4",
                "Line №5",
                "Line №6",
                "Line №7",
                "Line №8",
                "Line №9",
                "Line №10"
            };

            var lines2 = new[]
            {
                "Line №0",
                "Line №1",
                "Line №3",
                "Line №4",
                "Line 5",
                "Line №6",
                "Line №7",
                "Line №8",
                "Extra line 1",
                "Extra line 2",
                "Final line"
            };

            var lcs = new[]
            {
                new FileOperations.Pos(0, 1),
                new FileOperations.Pos(2, 2),
                new FileOperations.Pos(3, 3),
                new FileOperations.Pos(5, 5),
                new FileOperations.Pos(6, 6),
                new FileOperations.Pos(7, 7)
            };

            var changeBlocks = FileOperations.GetChangeBlocks(lines1, lines2, lcs);
            Assert.AreEqual(changeBlocks.Count, 4);
            Assert.Zero(changeBlocks[0].Delete.Length);
            Assert.Zero(changeBlocks[1].Insert.Length);
            Assert.AreEqual(changeBlocks[3].Delete, new[] {"Line №9", "Line №10"});
            Assert.AreEqual(changeBlocks[2].Delete.Length, 1);
            Assert.AreEqual(changeBlocks[2].Insert.Length, 1);
        }
    }
}