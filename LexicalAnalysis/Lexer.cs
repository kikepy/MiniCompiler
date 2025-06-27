using System.Security;
using System.Text;

namespace MiniCompiler.LexicalAnalysis;

public class Lexer(string sourceCode)
{
    private int _position;
    private int _line = 1;
    private int _column = 1; 
    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
    {
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "return", TokenType.Return },
        { "function", TokenType.Function },
        { "int", TokenType.Int },
        { "string", TokenType.String },
        { "double", TokenType.Double },
        { "true", TokenType.BooleanLiteral},
        { "false", TokenType.BooleanLiteral},
        { "char", TokenType.Char },
        { "class", TokenType.Class },
        { "bool", TokenType.Bool },
        { "private", TokenType.Private },
        { "float", TokenType.Float},
        { "public", TokenType.Public }
    };
    
    private static readonly Dictionary<string, TokenType> Operators = new Dictionary<string, TokenType>()
    {
        { "==", TokenType.Equals },
        { "!=", TokenType.NotEquals },
        { ">=", TokenType.GreaterThanOrEqual },
        { "<=", TokenType.LessThanOrEqual },
        { "&&", TokenType.And },
        { "||", TokenType.Or },
        { ">", TokenType.GreaterThan },
        { "<", TokenType.LessThan },
        { "=", TokenType.Assignment },
        { "+", TokenType.Plus },
        { "-", TokenType.Minus },
        { "*", TokenType.Multiply },
        { "/", TokenType.Divide },
        { "{", TokenType.BracketStart },
        { "}", TokenType.BracketEnd },
        { ";", TokenType.SemiColon },
        { ",", TokenType.Comma },
        { ".", TokenType.Dot },
        { "(", TokenType.ParenthesisStart},
        { ")", TokenType.ParenthesisEnd },
        { "&", TokenType.BitwiseAnd },
        { "|", TokenType.BitwiseOr },
        { "!", TokenType.Not },
        { "?", TokenType.TernaryQuestionMark },
        { ":", TokenType.TernaryColon }
    };
    
    

    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();

        while (_position < sourceCode.Length)
        {
            char current = sourceCode[_position];

            if (char.IsWhiteSpace(current))
            {
                SkipWhiteSpace();
            }
            else if (current == '\'')
            {
                tokens.Add(ReadCharLiteral());
            }
            else if (current == '"')
            {
                tokens.Add(ReadStringLiteral());
            }
            else if (char.IsLetter(current))
            {
                string remaining = sourceCode[_position..];
                if (remaining.StartsWith("true") || remaining.StartsWith("false"))
                {
                    tokens.Add(ReadBooleanLiteral());
                }
                else
                {
                    tokens.Add(ReadIdentifierOrKeyword());
                }
            }
            else if (char.IsDigit(current))
            {
                tokens.Add(ReadNumber());
            }
            else if (current == '/' && Peek() == '/')
            {
                tokens.Add(ReadComment());
            }
            else if (current == '/' && Peek() == '*')
            {
                tokens.Add(ReadComment());
            }
            else if("{};=+-*/&|!<>,.".Contains(current))
            {
                tokens.Add(ReadSymbol());
            }
            else
            {
                tokens.Add(HandleError(current));
            }
        }

        return tokens;
    }
    
    
    public Token NextToken()
    {
        if (IsAtEnd())
        {
            return new Token(TokenType.EOF, string.Empty, _line, _column);
        }

        char current = sourceCode[_position];

        if (char.IsWhiteSpace(current))
        {
            SkipWhiteSpace();
            return NextToken(); // Skip whitespace and get the next token
        }

        if (char.IsLetter(current))
        {
            return ReadIdentifierOrKeyword();
        }

        if (char.IsDigit(current))
        {
            return ReadNumber();
        }

        return ReadSymbol();
    }
    private void SkipWhiteSpace()
    {
        while(!IsAtEnd() && char.IsWhiteSpace(sourceCode[_position]))
        {
            if (sourceCode[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }

            _position++;
        }
    }
    
    private Token ReadIdentifierOrKeyword()
    {
        int start = _position;
        while (!IsAtEnd() && char.IsLetterOrDigit(sourceCode[_position]))
        {
            _position++;
            _column++;
        }

        string value = sourceCode[start.._position];
        TokenType type = Keywords.TryGetValue(value, out var keywordType) ? keywordType : TokenType.Identifier;

        return new Token(type, value, _line, _column - value.Length);
    }

    private Token ReadStringLiteral()
    {

        int startColumn = _position;
        _position++;
        _column++;

        var builder = new StringBuilder();

        while (!IsAtEnd() && sourceCode[_position] != '"')
        {
            if (sourceCode[_position] == '\\' && Peek() != '\0')
            {
                _position++;
                _column++;
                char escape = sourceCode[_position];
                builder.Append(escape switch
                {
                    'n' => '\n',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    _ => escape

                });
            }
            else
            {
                builder.Append(sourceCode[_position]);
            }
            _position++;
            _column++;
        }
        
        if(IsAtEnd() || sourceCode[_position] != '"')
        {
            throw new Exception($"Unterminated string literal at line {_line}, column {_column}");
        }

        _position++;
        _column++;

        return new Token(TokenType.StringLiteral, builder.ToString(), _line, startColumn);
    }

    private Token ReadNumber()
    {
        int start = _position;
        bool hasDecimalPoint = false;
        
        while (!IsAtEnd() && char.IsDigit(sourceCode[_position]) || sourceCode[_position] == '.')
        {
            if (sourceCode[_position] == '.')
            {
                if (hasDecimalPoint)
                {
                    throw new Exception($"Invalid number format at line {_line}, column{_column}");
                }

                hasDecimalPoint = true;
            }
            
            _position++;
            _column++;
        }

        string value = sourceCode[start.._position];
        TokenType type = hasDecimalPoint ? TokenType.FloatLiteral : TokenType.IntegerLiteral;
        return new Token(type, value, _line, _column - value.Length);
    }

    private Token ReadBooleanLiteral()
    {
        int start = _position;
        string value = string.Empty;

        while (!IsAtEnd() && char.IsLetter(sourceCode[_position]))
        {
            value += sourceCode[_position];
            _position++;
            _column++;
        }

        if (value == "true" || value == "false")
        {
            return new Token(TokenType.BooleanLiteral, value, _line, _column);
        }

        return HandleError(sourceCode[_position]);
    }

    private Token ReadCharLiteral()
    {
        int startColumn = _position;
        _position++;
        _column++;

        if (IsAtEnd() || sourceCode[_position] == '\'')
        {
            return new Token(TokenType.Error, $"Empty character at line {_line}, column {startColumn}", _line, startColumn);
        }

        char value;
        if (sourceCode[_position] == '\\' && Peek() != '\0')
        {
            _position++;
            _column++;
            if (IsAtEnd())
            {
                return new Token(TokenType.Error, $"Untermined escape sequence at line {_line} column {startColumn}", _line,
                    startColumn);
            }
            value = sourceCode[_position] switch
            {
                'n' => '\n',
                't' => '\t',
                '\'' => '\'',
                '\\' => '\\',
                _ => sourceCode[_position]
            };
        }
        else
        {
            value = sourceCode[_position];
        }

        _position++;
        _column++;

        if (IsAtEnd() || sourceCode[_position] != '\'')
        {
            return new Token(TokenType.Error, $"Unterminated character literal at line {_line}, column {startColumn}", _line, startColumn);
        }

        _position++;
        _column++;

        return new Token(TokenType.CharLiteral, value.ToString(), _line, startColumn);
    }

    private Token ReadComment()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        if (sourceCode[_position] == '/' && Peek() == '/')
        {
            _position += 2;
            _column += 2;

            while (!IsAtEnd() && sourceCode[_position] != '\n')
            {
                _position++;
                _column++;
            }
        }
        else if (sourceCode[_position] == '/' && Peek() == '*')
        {
            _position += 2;
            _column += 2;
            while (!IsAtEnd() && !(sourceCode[_position] == '*' && Peek() == '/'))
            {
                if (sourceCode[_position] == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }

                _position++;
            }

            if (IsAtEnd())
            {
                return new Token(TokenType.Error, $"Unterminated comment at line {startLine}, column {startColumn}", startLine, startColumn);
            }

            _position += 2;
            _column += 2;
        }
        else
        {
            return new Token(TokenType.Error, $"Unexpected comment syntax at line {startLine}, column {startColumn}", startLine, startColumn);
        }

        string value = sourceCode[start.._position];
        return new Token(TokenType.Comment, value, startLine, startColumn);
    }

    private Token ReadSymbol()
    {
        string value = _position + 1 < sourceCode.Length
            ? $"{sourceCode[_position]}{sourceCode[_position + 1]}"
            : sourceCode[_position].ToString();
        
        if(Operators.TryGetValue(value, out var type))
        {
            _position += value.Length;
            _column += value.Length;
            return new Token(type, value, _line, _column - value.Length);
        }

        value = sourceCode[_position].ToString();
        if (Operators.TryGetValue(value, out type))
        {
            _position++;
            _column++;
            return new Token(type, value, _line, _column);
        }

        return HandleError(sourceCode[_position]);
    }
    
    private char Peek() => _position + 1 < sourceCode.Length ? sourceCode[_position + 1] : '\0';
    
    private Token HandleError(char current)
    {
        string value = current.ToString();
        _position++;
        _column++;
        return new Token(TokenType.Error, $"Unexpected character { value }", _line, _column);
    }
    
    private bool IsAtEnd() => _position >= sourceCode.Length;
} 