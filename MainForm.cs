using System;
using System.Windows.Forms;
using MiniCompiler.LexicalAnalysis;
using MiniCompiler.SyntaxAnalysis;

namespace MiniCompiler;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void btnParse_Click(object sender, EventArgs e)
    {
        string sourceCode = txtSourceCode.Text;

        // Run the lexer
        var lexer = new Lexer(sourceCode);
        var tokens = lexer.Tokenize();
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        // Run the parser
        var parser = new Parser(tokens);
        Node ast;
        try
        {
            ast = parser.Parse();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error parsing: {ex.Message}", "Parser Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Display the AST
        var traverser = new AstTraverse();
        string travesalResult = traverser.Traverse(ast);
        txtAST.Text = travesalResult;
    }
}