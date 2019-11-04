using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class MineForm : Form
    {
        private MineBoard board = new MineBoard();

        private MouseButtons lastClickButton = MouseButtons.None;
        private long lastClickTicks = 0;
        
        private const int CellSize = 16;
        private const long ShortPeriod = 2000000L;
        private static readonly Rectangle CellRectangle = new Rectangle(0, 0, CellSize, CellSize);

        public MineForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init(9, 9, 10);
        }

        public void Init(int rows, int columns, int mines)
        {
            board.Init(rows, columns, mines);
            ClientSize = new Size(columns * CellSize + 1, rows * CellSize + mainMenu.Height + 1);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    g.ResetTransform();
                    g.TranslateTransform(j * 16, i * 16 + mainMenu.Height);
                    
                    DrawCell(g, board[i, j]);
                }
            }
        }

        private void DrawCell(Graphics g, MineCell cell)
        {
            Rectangle rt = CellRectangle;
            
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (cell.IsMine && board.Status == GameStatus.Lose)
            {
                g.FillRectangle(Brushes.DarkGray, rt);
                g.DrawString("@", Font, cell.Status == MineCellStatus.Exploded ? Brushes.Red : Brushes.DarkBlue, rt, sf);
            }
            else
            {
                if (cell.Status == MineCellStatus.Covered)
                {
                    g.FillRectangle(Brushes.DarkGray, rt);
                }
                else if (cell.Status == MineCellStatus.MarkMine)
                {
                    g.FillRectangle(Brushes.DarkGray, rt);
                    g.DrawString("#", Font, Brushes.DarkBlue, rt, sf);
                }
                else if (cell.Status == MineCellStatus.MarkDoubt)
                {
                    g.FillRectangle(Brushes.DarkGray, rt);
                    g.DrawString("?", Font, Brushes.DarkBlue, rt, sf);
                }
                else
                {
                    if (cell.NearbyMines > 0)
                    {
                        g.DrawString(cell.NearbyMines.ToString(), Font, Brushes.DarkBlue, rt, sf);
                    }
                }
            }

            g.DrawRectangle(Pens.Black, rt);

            sf.Dispose();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (board.Status != GameStatus.Playing && board.Status != GameStatus.NotStarted)
                return;
            
            int row, col;
            HitTest(e.Location, out row, out col);

            if (DateTime.Now.Ticks - lastClickTicks < ShortPeriod && ((lastClickButton | e.Button) == (MouseButtons.Left | MouseButtons.Right)))
            {
                board.UncoverNearBy(row, col);
            }
            
            if (e.Button == MouseButtons.Left)
            {
                board.Uncover(row, col);
            }
            else if (e.Button == MouseButtons.Right)
            {
                board.ChangeMark(row, col);
            }

            board.CheckGameResult();

            if (board.Status == GameStatus.Win)
            {
                MessageBox.Show("You Win!");
            }

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (DateTime.Now.Ticks - lastClickTicks < ShortPeriod)
            {
                lastClickButton |= e.Button;
            }
            else
            {
                lastClickButton = e.Button;
                lastClickTicks = DateTime.Now.Ticks;
            }
        }

        private void HitTest(Point pt, out int row, out int col)
        {
            col = pt.X / CellSize;
            row = (pt.Y - mainMenu.Height) / CellSize;
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItemNew_Click(object sender, EventArgs e)
        {
            this.Init(9, 9, 10);            
        }

        private void MineForm_Load(object sender, EventArgs e)
        {
            ;
        }
    }
}


