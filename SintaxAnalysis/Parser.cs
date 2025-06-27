using MiniCompiler.LexicalAnalysis;

namespace MiniCompiler.SintaxAnalysis;
public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Node Parse()
    {
        return ParseProgram();
    }

    private Node ParseProgram()
    {
        List<Node> statements = new List<Node>();

        while (!IsAtEnd())
        {
            statements.Add(ParseStatement());
        }
        return new ProgramNode(statements);
    }

    private Node ParseStatement()
    {
        var current = Peek();
        return current.Type switch
        {
            TokenType.Class => ParseClass(),
            TokenType.Int or TokenType.String or TokenType.Bool or TokenType.Char or TokenType.Float
                => ParseVariableDeclaration(),
            TokenType.Identifier => ParseAssignment(),
            TokenType.If => ParseIfStatement(),
            TokenType.While => ParseWhileStatement(),
            TokenType.For => ParseForStatement(),
            _ => throw new Exception(
                $"Unexpected token: {current.Type} at line {current.Line}, column {current.Column}")
            
        };
    }

    private Node ParseVariableDeclaration()
    {
        var typeToken = Consume(Peek().Type);
        var name = Consume(TokenType.Identifier);
        Node? initializer = null;

        if (Check(TokenType.Assignment))
        {
            Consume(TokenType.Assignment);
            initializer = ParseExpression();
        }
        Consume(TokenType.SemiColon);
        return new VariableDeclarationNode(name.Value, typeToken.Value, initializer!);
    }
    
    private Node ParseAssignment()
    {
        Token identifier = Expect(TokenType.Identifier);

        Token assignmentOperator = Expect(TokenType.Assignment);

        Node value = ParseExpression();

        Expect(TokenType.SemiColon);

        return new AssignmentNode(identifier.Value, value);

    }

    private Node ParseExpression()
    {
        var left = ParsePrimary();
        
        while(!IsAtEnd() && Check(TokenType.Plus) || Check(TokenType.Minus) || Check(TokenType.Multiply) || Check(TokenType.Divide))
        {
            var operatorToken = Consume(Peek().Type);
            var right = ParsePrimary();
            left = new BinaryOperationNode(left, operatorToken.Value, right);
        }

        return left;
    }

    private Node ParseClass()
    {
        Consume(TokenType.Class);
        var className = Consume(TokenType.Identifier);

        var body = ParseBLock();

        return new ClassNode(className.Value, ((BlockNode)body).Statements);
    }

    private Node ParseIfStatement()
    {
        Consume(TokenType.If);
        var condition = ParseExpression();
        var body = ParseBLock();

        return new IfNode(condition, body);
    }

    private Node ParseWhileStatement()
    {
        Consume(TokenType.While);
        var condition = ParseExpression();

        var body = ParseBLock();

        return new WhileNode(condition, body);
    }
    
    private Node ParseForStatement()
    {
        Consume(TokenType.For);
        Consume(TokenType.ParenthesisStart);

        Node initialization = ParseStatement();
        Node condition = ParseExpression();
        Node increment = ParseStatement();

        Consume(TokenType.ParenthesisEnd);

        var body = ParseBLock();

        return new ForNode(initialization, condition, increment, body);
    }

    private Node ParseBLock()
    {
        Consume(TokenType.BracketStart);

        List<Node> statements = new List<Node>();
        while (!Check(TokenType.BracketEnd) && !IsAtEnd())
        {
            statements.Add(ParseStatement());
        }
        
        Consume(TokenType.BracketEnd);
        return new BlockNode(statements);
    }
    
    
    private Node ParsePrimary()
    {
        var token = Peek();
        
        Node node = token.Type switch
        {
            TokenType.IntegerLiteral => new LiteralNode(token.Value),
            TokenType.StringLiteral => new LiteralNode(token.Value),
            TokenType.BooleanLiteral => new LiteralNode(token.Value),
            TokenType.CharLiteral => new LiteralNode(token.Value),
            TokenType.FloatLiteral => new LiteralNode(token.Value),
            TokenType.Identifier => new ExpressionNode(token.Value),
            _ => throw new Exception($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}")
        };

        Advance();
        return node;
    }

    private Token Consume(TokenType type)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw new Exception($"Unexpected token {type} but found {Peek().Type} at line {Peek().Line}, column {Peek().Column}");
    }

    private Token Expect(TokenType type)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw new Exception($"Expected token {type} but found {Peek().Type} at line {Peek().Line} and column {Peek().Column}");
    }
    
    private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

    private Token Advance() => _tokens[_position++];
    private Token Peek() => _tokens[_position];
    
    private bool IsAtEnd() => _position >= _tokens.Count;
}