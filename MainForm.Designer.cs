using System.Windows.Forms;

namespace MiniCompiler;

public partial class MainForm : Form
{
    private TextBox txtSourceCode;
    private Button btnParse;
    private TextBox txtAST;

    private void InitializeComponent()
    {
        this.txtSourceCode = new TextBox();
        this.btnParse = new Button();
        this.txtAST = new TextBox();

        // txtSourceCode
        this.txtSourceCode.Multiline = true;
        this.txtSourceCode.Location = new System.Drawing.Point(12, 12);
        this.txtSourceCode.Size = new System.Drawing.Size(776, 200);
        this.txtSourceCode.ScrollBars = ScrollBars.Vertical;

        // btnParse
        this.btnParse.Location = new System.Drawing.Point(12, 220);
        this.btnParse.Size = new System.Drawing.Size(100, 30);
        this.btnParse.Text = "Parse Code";
        this.btnParse.Click += new System.EventHandler(this.btnParse_Click);

        // txtAST
        this.txtAST.Multiline = true;
        this.txtAST.Location = new System.Drawing.Point(12, 260);
        this.txtAST.Size = new System.Drawing.Size(776, 180);
        this.txtAST.ScrollBars = ScrollBars.Vertical;
        this.txtAST.ReadOnly = true;

        // MainForm
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.txtSourceCode);
        this.Controls.Add(this.btnParse);
        this.Controls.Add(this.txtAST);
        this.Text = "MiniCompiler";
    }
}