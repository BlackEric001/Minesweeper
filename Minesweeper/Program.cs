using System;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MineBoard area = new MineBoard();
            area.Init(10, 20, 9);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MineForm());
        }
    }
}
