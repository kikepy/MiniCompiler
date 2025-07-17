
namespace MiniCompiler.SyntaxAnalysis;

public abstract class Node { }

public class ProgramNode : Node
{
    public List<Node> Statements { get;  } 
    
    public ProgramNode(List<Node> statements)
    {
        Statements = statements;
    }
}

public class IfNode : Node
{
    public Node Condition { get; }
    public Node Body { get; }
    
    public IfNode(Node condition, Node body)
    {
        Condition = condition;
        Body = body;
    }
}

public class BlockNode : Node
{
    public List<Node> Statements { get; }
    
    public BlockNode(List<Node> statements)
    {
        Statements = statements;
    }
}

public class ExpressionNode : Node
{
    public string Value { get; }
    
    public ExpressionNode(string value)
    {
        Value = value;
    }
}

public class VariableDeclarationNode : Node
{
    public string Name { get; }
    public string Type { get; }
    public Node Initializer { get; }
    
    public VariableDeclarationNode(string name, string type, Node initializer = null)
    {
        Name = name;
        Type = type;
        Initializer = initializer;
    }
}

public class BinaryOperationNode : Node
{
    public Node Left { get; }
    public string Operator { get; }
    public Node Right { get; }
    
    public BinaryOperationNode(Node left, string operatorSymbol, Node right)
    {
        Left = left;
        Operator = operatorSymbol;
        Right = right;
    }
}

public class LiteralNode : Node
{
    public object Value { get; }
    public LiteralNode(object value)
    {
        Value = value;
    }
}

public class FunctionCallNode : Node
{
    public string FunctionName { get; }
    public List<Node> Arguments { get; }
    
    public FunctionCallNode(string functionName, List<Node> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }
}

public class AssignmentNode : Node
{
    public string VariableName { get; }
    public Node Value { get; }
    
    public AssignmentNode(string variableName, Node value)
    {
        VariableName = variableName;
        Value = value;
    }
}

public class WhileNode : Node
{
    public Node Condition { get; }
    public Node Body { get; }
    
    public WhileNode(Node condition, Node body)
    {
        Condition = condition;
        Body = body;
    }
}

public class ClassNode : Node
{
    public string Name { get; }
    public List<Node> Body { get; }

    public ClassNode(string name, List<Node> body)
    {
        Name = name;
        Body = body;
    }
}

public class ForNode : Node
{
    public Node Initialization { get; }
    public Node Condition { get; }
    public Node Increment { get; }
    public Node Body { get; }

    public ForNode(Node initialization, Node condition, Node increment, Node body)
    {
        Initialization = initialization;
        Condition = condition;
        Increment = increment;
        Body = body;
    }
}

public class ReturnNode : Node
{
    public Node Expression { get; }

    public ReturnNode(Node expression)
    {
        Expression = expression;
    }
}
