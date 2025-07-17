using MiniCompiler.LexicalAnalysis;

namespace MiniCompiler.SyntaxAnalysis;
public class Parser
{
    private readonly TokenCursor _tokens;

    public Parser(List<Token> tokens)
    {
        _tokens = new TokenCursor(tokens);
    }

    public Node Parse()
    {
        return new StatementParser(_tokens).Parse();
    }
}
