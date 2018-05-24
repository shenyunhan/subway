using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Newtonsoft.Json;
using WindowsFormsApp1;

namespace Subway
{
    using Data;

    public static class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());

        }
    }
}
