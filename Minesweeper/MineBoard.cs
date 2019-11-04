using System;
using System.Collections;

namespace Minesweeper
{
    public enum GameStatus
    {
        NotStarted,
        Playing,
        Win,
        Lose
    }

    public enum MineCellStatus : short
    {
        Covered,
        MarkDoubt,
        MarkMine,
        Uncovered,
        Exploded
    }

    public struct MineCell
    {
        public MineCellStatus Status;
        public bool IsMine;        
        public byte NearbyMines;
    }

    public class MineBoard
    {
        private MineCell[,] cells = new MineCell[0, 0];

        private int totalMines;
        private int makedMines;
        private GameStatus status;

        public int Columns { get { return cells.GetLength(1); } }

        public int Rows { get { return cells.GetLength(0); } }

        public int TotalMines { get { return totalMines; } }
        public int MarkedMines { get { return makedMines; } }

        public GameStatus Status { get { return status; } }

        public MineCell this[int row, int col] { get { return cells[row, col]; } }

        public void Init(int rows, int columns, int mines)
        {
            if (rows <= 0 || columns <= 0 || mines >= columns * rows)
            {
                throw new ArgumentException();
            }

            cells = new MineCell[rows, columns];
            totalMines = mines;
            makedMines = 0;
            status = GameStatus.NotStarted;

            PlaceRandomMines();
            UpdateNearbyMinesCount();
        }

        public void Uncover(int row, int col)
        {
            if (!IsValidCell(row, col))
                return;

            if (status == GameStatus.NotStarted)
            {
                status = GameStatus.Playing;
            }

            MineCell cell = cells[row, col];

            if (cell.Status != MineCellStatus.Covered)
                return;
            
            if (cell.IsMine)
            {
                cells[row, col].Status = MineCellStatus.Exploded;
                this.status = GameStatus.Lose;
                return;
            }
            else
            {
                cells[row, col].Status = MineCellStatus.Uncovered;

            }

            // If the surrounding mines number is 0, then turn around the 8 positions.
            if (!cell.IsMine && cell.NearbyMines == 0)
            {
                for (int i = 0; i < moves.GetLength(0); i++)
                    Uncover(row + moves[i, 0], col + moves[i, 1]);
            }
        }

        /// <summary>
        /// Set or cancel the flag, which is equivalent to the right mouse click behavior
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void ChangeMark(int row, int col)
        {
            if (!IsValidCell(row, col))
                return;
            
            if (status == GameStatus.NotStarted)
            {
                status = GameStatus.Playing;
            }
            
            MineCell cell = cells[row, col];

            // Set or cancel the flag, which is equivalent to the right mouse click behavior
            switch (cell.Status)
            {
                case MineCellStatus.Covered:
                    cell.Status = MineCellStatus.MarkMine;
                    makedMines++;
                    break;
                case MineCellStatus.MarkMine:
                    cell.Status = MineCellStatus.MarkDoubt;
                    break;
                case MineCellStatus.MarkDoubt:
                    makedMines--;
                    cell.Status = MineCellStatus.Covered;
                    break;
                default:
                    break;
            }

            cells[row, col] = cell;
        }

        /// <summary>
        /// Left and right mouse button simultaneously press behavior
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void UncoverNearBy(int row, int col)
        {
            MineCell cell = cells[row, col];

            if (cell.Status == MineCellStatus.Uncovered && cell.NearbyMines > 0)
            {
                byte minesFound = 0;

                for (int i = 0; i < moves.GetLength(0); i++)
                {
                    int r = row + moves[i, 0];
                    int c = col + moves[i, 1];

                    if (IsValidCell(r, c) && cells[r, c].Status == MineCellStatus.MarkMine)
                        minesFound++;
                }

                if (minesFound == cell.NearbyMines)
                {
                    for (int i = 0; i < moves.GetLength(0); i++)
                    {
                        int r = row + moves[i, 0];
                        int c = col + moves[i, 1];

                        Uncover(r, c);
                    }
                }
            }
        }

        /// <summary>
        /// Verify game results
        /// </summary>
        public void CheckGameResult()
        {
            if (makedMines == totalMines)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        if (cells[i, j].Status == MineCellStatus.Covered)
                        {
                            return;
                        }
                    }
                }

                status = GameStatus.Win;
            }
        }

        private void PlaceRandomMines()
        {
            int count = 0;
            Random r = new Random((int)DateTime.Now.Ticks);

            while (count < totalMines)
            {
                int row = r.Next(0, Rows);
                int col = r.Next(0, Columns);

                if (!cells[row, col].IsMine)
                {
                    cells[row, col].IsMine = true;
                    count++;
                }
            }
        }

        private bool IsValidCell(int row, int col)
        {
            return (row >= 0 && col >= 0 && row < Rows && col < Columns);
        }

        /**/
        /// <summary>
        /// Possible displacement
        /// </summary>
        private int[,] moves = new int[8, 2] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

        private byte GetNearbyMines(int row, int col)
        {
            byte minesFound = 0;

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int r = row + moves[i, 0];
                int c = col + moves[i, 1];

                if (IsValidCell(r, c) && cells[r, c].IsMine)
                    minesFound++;
            }
            return minesFound;
        }

        private void UpdateNearbyMinesCount()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    cells[i, j].NearbyMines = GetNearbyMines(i, j);
                }
            }
        }
    }
}
