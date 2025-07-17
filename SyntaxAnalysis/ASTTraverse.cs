using System.Text;

namespace MiniCompiler.SyntaxAnalysis;

public class AstTraverse
{
    private readonly StringBuilder _output = new StringBuilder();
    public string Traverse(Node node)
    {
        switch (node)
        {
            case ProgramNode programNode:
                _output.AppendLine("Program:");
                foreach (Node statement in programNode.Statements)
                {
                    Traverse(statement);
                }
                break;
            
            case ClassNode classNode:
                _output.AppendLine($"Class: {classNode.Name}");
                foreach (Node member in classNode.Body)
                {
                    Traverse(member);
                }
                break;
            
            case VariableDeclarationNode variableNode:
                _output.AppendLine($"Varibale Declaration: {variableNode.Name} ");
                if (variableNode.Initializer != null)
                {
                    Traverse(variableNode.Initializer);
                }
                break;
            
            case BinaryOperationNode binaryNode:
                _output.AppendLine($"Binary Operation {binaryNode.Operator}");
                Traverse(binaryNode.Left);
                Traverse(binaryNode.Right);
                break;
            
            case LiteralNode literalNode:
                _output.AppendLine($"Literal: {literalNode.Value}");
                break;
            
            case ExpressionNode expressionNode:
                _output.AppendLine($"Expression: {expressionNode.Value}");
                break;
            
            case AssignmentNode assignmentNode:
                _output.AppendLine($"Assignment: {assignmentNode.VariableName}");
                break;
            
            case FunctionCallNode functionCallNode:
                _output.AppendLine($"Function Call: {functionCallNode.FunctionName}");
                foreach (Node arguments in functionCallNode.Arguments)
                {
                    Traverse(arguments);
                }
                break;
            
            case IfNode ifNode:
                _output.AppendLine($"If statement");
                Traverse(ifNode.Condition);
                Traverse(ifNode.Body);
                break;
            
            case WhileNode whileNode:
                _output.AppendLine($"While loop");
                Traverse(whileNode.Condition);
                Traverse(whileNode.Body);
                break;
            case BlockNode blockNode:
                _output.AppendLine("Block:");
                foreach (Node statements in blockNode.Statements)
                {
                    Traverse(statements);
                }
                break;
            
                
            default:
                throw new Exception($"Unknown Node Type: {node.GetType().Name}");
        }

        return _output.ToString();
    }
}