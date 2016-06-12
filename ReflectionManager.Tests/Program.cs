using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NUnit.Gui;

namespace ReflectionManager.Tests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppEntry.Main(new[] { Application.ExecutablePath, "/run" });
        }
    }
}
