using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GameForm form;
            if (args.Length >= 1 && args[0] == "h")
                form = new GameForm(host: true);
            else if (args.Length >= 1 && args[0] == "c")
                form = new GameForm(host: false);
            else
                form = new GameForm();

            Application.Run(form);
        }
    }
}
