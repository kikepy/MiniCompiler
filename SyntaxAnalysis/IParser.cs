using Microsoft.VisualBasic.Logging;
using MiniCompiler.LexicalAnalysis;
using System;
using Serilog;
using MiniCompiler.Exceptions;

namespace MiniCompiler.SyntaxAnalysis;

public interface IParser
{
    Node Parse();
}

public class ExpressionParser : IParser
{
    private readonly TokenCursor _cursor;
    
    private readonly Dictionary<TokenType, Func<Token, Node>> _primaryParsers = new()
    {
        { TokenType.IntegerLiteral, token => new LiteralNode(token.Value) },
        { TokenType.BooleanLiteral ,token =>  new LiteralNode(token.Value)},
        { TokenType.CharLiteral ,token =>  new LiteralNode(token.Value)},
        { TokenType.StringLiteral ,token =>  new LiteralNode(token.Value)},
        { TokenType.FloatLiteral ,token =>  new LiteralNode(token.Value)},
        { TokenType.Identifier, token => new ExpressionNode(token.Value) }
    };
    
    public ExpressionParser(TokenCursor cursor)
    {
        _cursor = cursor;
    }

    public Node Parse()
    {
        var left = ParsePrimary();

        while (!_cursor.IsAtEnd() && IsOperator(_cursor.Peek().Type))
        {
            var operatorToken = _cursor.Advance(); // Consume the operator
            var right = ParsePrimary(); // Parse the right operand
            left = new BinaryOperationNode(left, operatorToken.Value, right);
        }

        return left;
    }

    private Node ParsePrimary()
    {
        var token = _cursor.Peek();
        if (token.Type == TokenType.ParenthesisStart)
        {
            _cursor.Advance(); // Consume '('
            var expression = Parse(); // Parse the inner expression
            if (_cursor.Peek().Type == TokenType.ParenthesisEnd)
            {
                _cursor.Advance(); // Consume ')'
            }
            else
            {
                throw new Exception($"Expected ')' at line {token.Line}, column {token.Column}");
            }
            return expression;
        }

        if (_primaryParsers.TryGetValue(token.Type, out var parser))
        {
            var node = parser(token);
            _cursor.Advance(); // Ensure the cursor advances after consuming the token
            return node;
        }

        throw new Exception($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}");
    }
    
    private bool IsOperator(TokenType type) =>
        type is TokenType.LessThan or TokenType.GreaterThan or TokenType.Equals or TokenType.NotEquals or
            TokenType.Plus or TokenType.Minus or TokenType.Multiply or TokenType.Divide;
}

public class StatementParser : IParser
{
    private readonly TokenCursor _cursor;
    private readonly Dictionary<TokenType, Func<Node>> _parsers;

    public StatementParser(TokenCursor cursor)
    {
        _cursor = cursor;
        _parsers = new Dictionary<TokenType, Func<Node>>()
        {
            { TokenType.If, ParseIfStatement },
            { TokenType.While, ParseWhileStatement },
            { TokenType.Class, ParseClass },
            { TokenType.Int, ParseVariableDeclaration },
            { TokenType.Float, ParseVariableDeclaration },
            { TokenType.String, ParseVariableDeclaration },
            { TokenType.Bool, ParseVariableDeclaration },
            { TokenType.Char, ParseVariableDeclaration },
            { TokenType.Return, ParseReturnStatement }
        };
    }

    public Node Parse()
    {
        var statements = new List<Node>();

        while (!_cursor.IsAtEnd())
        {
            var token = _cursor.Peek();
            if (_parsers.TryGetValue(token.Type, out var parser))
            {
                statements.Add(parser());
            }
            else
            {
                throw new Exception($"Unexpected token: {token.Type} at line {token.Line}, column {token.Column}");
            }
        }

        return new ProgramNode(statements);
    }

    private Node ParseVariableDeclaration()
    {
        Console.WriteLine($"Parsing variable declaration: {_cursor.Peek()}");
        var tokenType = _cursor.Advance();
        var nameToken = _cursor.Advance();

        Node initializer = null;
        if (_cursor.Peek().Type == TokenType.Assignment)
        {
            _cursor.Advance();
            initializer = new ExpressionParser(_cursor).Parse();
        }

        if (_cursor.Peek().Type == TokenType.SemiColon)
        {
            _cursor.Advance();
        }
        else
        {
            throw new Exception($"Expected ';' after variable declaration at line {nameToken.Line}, column {nameToken.Column}");
        }

        return new VariableDeclarationNode(nameToken.Value, tokenType.Value, initializer);
    }

    private Node ParseIfStatement()
    {
        Console.WriteLine($"Parsing 'if' statement: {_cursor.Peek()}");
        _cursor.Advance(); // Consume 'if'

        // Ensure '(' is present
        var openParenthesis = _cursor.Peek();
        if (openParenthesis.Type != TokenType.ParenthesisStart)
        {
            throw new Exception($"Expected '(' after 'if' at line {openParenthesis.Line}, column {openParenthesis.Column}");
        }
        Console.WriteLine($"Found '(': {openParenthesis}");
        _cursor.Advance(); // Consume '('

        // Parse the condition
        var condition = new ExpressionParser(_cursor).Parse();
        Console.WriteLine($"Parsed condition: {condition}");

        // Ensure ')' is present
        var closeParenthesis = _cursor.Peek();
        if (closeParenthesis.Type != TokenType.ParenthesisEnd)
        {
            throw new Exception($"Expected ')' after condition at line {closeParenthesis.Line}, column {closeParenthesis.Column}");
        }
        Console.WriteLine($"Found ')': {closeParenthesis}");
        _cursor.Advance(); // Consume ')'

        // Parse the block
        var body = ParseBlock();
        Console.WriteLine($"Parsed block: {body}");

        return new IfNode(condition, body);
    }

    private Node ParseWhileStatement()
    {
        _cursor.Advance(); // Consume 'while'
        var condition = new ExpressionParser(_cursor).Parse();
        var body = ParseBlock();
        return new WhileNode(condition, body);
    }

    private Node ParseClass()
    {
        _cursor.Advance(); // Consume 'class'
        var name = _cursor.Advance(); // Consume identifier
        var body = ParseBlock();
        
        return new ClassNode(name.Value, ((BlockNode)body).Statements);
    }

    private Node ParseBlock()
    {
        var openBracket = _cursor.Peek();
        if (openBracket.Type != TokenType.BracketStart)
        {
            throw new Exception($"Expected '{{' at line {openBracket.Line}, column {openBracket.Column}");
        }
        
        _cursor.Advance(); // Consume '{'
        var statements = new List<Node>();

        while (!_cursor.IsAtEnd())
        {
            var currentToken = _cursor.Peek();
            
            if(currentToken.Type == TokenType.BracketEnd)
                break;

            if (_parsers.TryGetValue(currentToken.Type, out var parser))
            {
                statements.Add(parser());
            }
            else
            {
                throw new Exception($"Unexpected token: {currentToken.Type} at line {currentToken.Line}, column {currentToken.Column}");
            }
        }
        
        var closeBracket = _cursor.Peek();
        Console.WriteLine($"Found '}}' at {closeBracket.Line}, {closeBracket.Column} "); 
        if (closeBracket.Type != TokenType.BracketEnd)
        {
            throw new Exception($"Expected '}}' at line {closeBracket.Line}, column {closeBracket.Column}");
        }
        _cursor.Advance(); // Consume '}'
        
        
        return new BlockNode(statements);
    }

    private Node ParseReturnStatement()
    {
        _cursor.Advance(); // Consume 'return'
        var expression = new ExpressionParser(_cursor).Parse();

        if (_cursor.Peek().Type == TokenType.SemiColon)
        {
            _cursor.Advance(); // Consume ';'
        }
        else
        {
            throw new Exception($"Expected ';' after return statement at line {_cursor.Peek().Line}, column {_cursor.Peek().Column}");
        }

        return new ReturnNode(expression);
    }
}

public class TokenCursor
{
    private readonly List<Token> _tokens;
    private int _position;
    
    public TokenCursor(List<Token> tokens)
    {
        _tokens = tokens;
        _position = 0;
    }

    public Token Peek()
    {
        if (_position >= _tokens.Count)
        {
            throw new SyntaxErrorException("Unexpected end of input while peeking.");
        }
        
        return _tokens[_position];
    }

    public Token Advance()
    {
        if (_position >= _tokens.Count)
        {
            
            return new Token(TokenType.EOF, string.Empty, -1, -1);
        }
        return _tokens[_position++];
    }

    public bool IsAtEnd() => _position >= _tokens.Count;
}