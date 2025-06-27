using System;
using System.Windows.Forms;

namespace MiniCompiler;

static class Program
{
    
    [STAThread]
    static void Main()
    {
        // Configuración inicial para aplicaciones de Windows Forms
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Ejecutar el formulario principal
        Application.Run(new MainForm());
    }
}