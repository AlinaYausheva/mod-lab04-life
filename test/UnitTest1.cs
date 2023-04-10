using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace TestProject3
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckJSON()
        {
            Board board = Program.ChangeSetting("board.json");
            Board expected = new Board(25, 25, 1, 0.5);
            Assert.AreEqual(expected.Height, board.Height);
            Assert.AreEqual(expected.CellSize, board.CellSize);
            Assert.AreEqual(expected.Columns, board.Columns);
            Assert.AreEqual(expected.Rows, board.Rows);
            Assert.AreEqual(expected.Width, board.Width);
        }

        [TestMethod]
        public void CheckRead()
        {
            Board board = Program.ReadFile("box.txt");
            Assert.IsTrue(board.Cells[1,2].IsAlive);
            Assert.IsTrue(board.Cells[2,1].IsAlive);
            Assert.IsTrue(board.Cells[2,3].IsAlive);
            Assert.IsTrue(board.Cells[3,2].IsAlive);
        }

        [TestMethod]
        public void CheckSave()
        {
            Board board = new Board(3, 3, 1, 0.5);
            board.Cells[0, 0].IsAlive = true;
            board.Cells[0, 1].IsAlive = false;
            board.Cells[0, 2].IsAlive = true;
            board.Cells[1, 0].IsAlive = false;
            board.Cells[1, 1].IsAlive = true;
            board.Cells[1, 2].IsAlive = false;
            board.Cells[2, 0].IsAlive = true;
            board.Cells[2, 1].IsAlive = false;
            board.Cells[2, 2].IsAlive = true;
            Program.SaveFile(board);

            Board expected = Program.ReadFile("save.txt");
            Assert.AreEqual(expected.Cells[0, 0].IsAlive, board.Cells[0, 0].IsAlive);
            Assert.AreEqual(expected.Cells[0, 1].IsAlive, board.Cells[0, 1].IsAlive);
            Assert.AreEqual(expected.Cells[0, 2].IsAlive, board.Cells[0, 2].IsAlive);
            Assert.AreEqual(expected.Cells[1, 0].IsAlive, board.Cells[1, 0].IsAlive);
            Assert.AreEqual(expected.Cells[1, 1].IsAlive, board.Cells[1, 1].IsAlive);
            Assert.AreEqual(expected.Cells[1, 2].IsAlive, board.Cells[1, 2].IsAlive);
            Assert.AreEqual(expected.Cells[2, 0].IsAlive, board.Cells[2, 0].IsAlive);
            Assert.AreEqual(expected.Cells[2, 1].IsAlive, board.Cells[2, 1].IsAlive);
            Assert.AreEqual(expected.Cells[2, 2].IsAlive, board.Cells[2, 2].IsAlive);

        }

        [TestMethod]
        public void CheckCountIsAsive1()
        {
            Board board = Program.ReadFile("file1.txt");
            int count = board.CountIsAlive();
            int expected = 155;
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void CheckCountIsAsive2()
        {
            Board board = Program.ReadFile("loaf.txt");
            int count = board.CountIsAlive();
            int expected = 7;
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void CheckFigure1()
        {
            Board board;
            
            Figure hive = new Figure();
            
            board = Program.ReadFile("file3.txt");

           
            hive = Program.MackMask("hive.txt", hive);
            bool expected = Program.PartToCheck(hive, 0, 3, board);
            Assert.AreEqual(expected, true);
        }

        [TestMethod]
        public void CheckFigure2()
        {
            Board board;
            Figure[] figures = new Figure[11];

            board = Program.ReadFile("file3.txt");

            figures = Program.MackAllMasks();
            figures = Program.AllCheck(board, figures);
            
            int[] expected = {0,3,0,0,2,0,0,0,0,0,0 };
            for (int i = 0;i<11;i++)
                Assert.AreEqual(expected[i], figures[i].count);

        }

        [TestMethod]
        public void CheckFigure3()
        {
            Board board;
            Figure[] figures = new Figure[11];

            board = Program.ReadFile("file1.txt");

            figures = Program.MackAllMasks();
            figures = Program.AllCheck(board, figures);

            int[] expected = { 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 11; i++)
                Assert.AreEqual(expected[i], figures[i].count);
        }

        [TestMethod]
        public void CheckFigure4()
        {
            Board board;
            Figure[] figures = new Figure[11];

            board = Program.ReadFile("file2.txt");

            figures = Program.MackAllMasks();
            figures = Program.AllCheck(board, figures);

            int[] expected = { 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 11; i++)
                Assert.AreEqual(expected[i], figures[i].count);
        }

        [TestMethod]
        public void CheckSymmetry1()
        {
            Board board;
            board = Program.ReadFile("file1.txt");

            Assert.AreEqual(board.CheckSimmetriHorizontal(),false);
            Assert.AreEqual(board.CheckSimmetriVertical(), false);
        }

        [TestMethod]
        public void CheckSymmetry2()
        {
            Board board;
            board = Program.ReadFile("box.txt");

            Assert.AreEqual(board.CheckSimmetriHorizontal(), true);
            Assert.AreEqual(board.CheckSimmetriVertical(), true);
        }

        [TestMethod]
        public void CheckSymmetry3()
        {
            Board board;
            board = Program.ReadFile("hive.txt");

            Assert.AreEqual(board.CheckSimmetriHorizontal(), true);
            Assert.AreEqual(board.CheckSimmetriVertical(), true);
        }
    }
}
