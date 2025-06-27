namespace MiniCompiler.LexicalAnalysis;

public enum TokenType
{
    //BlockStructure
    BracketStart,
    BracketEnd,
    SemiColon,
    
    //KeyWords
    If,
    Else,
    While,
    For,
    Return,
    Class,
    Function,
    Private,
    Public,
    
    
    //Types
    Int, 
    String,
    Bool,
    Float,
    Double,
    Char,
    
    //Identifiers and Literals
    Identifier,
    IntegerLiteral,
    StringLiteral,
    BooleanLiteral,
    FloatLiteral,
    CharLiteral,
    DoubleLiteral,
    
    //Operators
    Assignment,
    Plus,
    Minus,
    Multiply,
    Divide,
    
    //Comparison
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    //Logical
    And,
    Or,
    Not,
    //Miscellaneous
    Comma,
    Dot,
    Whitespace,
    Comment,
    Error,
    EOF,

    ParenthesisStart,
    TernaryColon,
    TernaryQuestionMark,
    BitwiseOr,
    BitwiseAnd,
    ParenthesisEnd
}


public class Token
{
    public TokenType Type { get; }
    public string Value { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"{Type} ('{Value}') at line: {Line}, column: {Column}";
    }
}