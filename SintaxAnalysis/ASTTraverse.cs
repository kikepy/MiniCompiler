using System.Text;

namespace MiniCompiler.SintaxAnalysis;

public class ASTTraverse
{
    private readonly StringBuilder Output = new StringBuilder();
    public string Traverse(Node node)
    {
        switch (node)
        {
            case ProgramNode programNode:
                Output.AppendLine("Program:");
                foreach (Node statement in programNode.Statements)
                {
                    Traverse(statement);
                }
                break;
            
            case ClassNode classNode:
                Output.AppendLine($"Class: {classNode.Name}");
                foreach (Node member in classNode.Body)
                {
                    Traverse(member);
                }
                break;
            
            case VariableDeclarationNode variableNode:
                Output.AppendLine($"Varibale Declaration: {variableNode.Name} ");
                if (variableNode.Initializer != null)
                {
                    Traverse(variableNode.Initializer);
                }
                break;
            
            case BinaryOperationNode binaryNode:
                Output.AppendLine($"Binary Operation {binaryNode.Operator}");
                Traverse(binaryNode.Left);
                Traverse(binaryNode.Right);
                break;
            
            case LiteralNode literalNode:
                Output.AppendLine($"Literal: {literalNode.Value}");
                break;
            
            case ExpressionNode expressionNode:
                Output.AppendLine($"Expression: {expressionNode.Value}");
                break;
            
            case AssignmentNode assignmentNode:
                Output.AppendLine($"Assignment: {assignmentNode.VariableName}");
                break;
            
            case FunctionCallNode functionCallNode:
                Output.AppendLine($"Function Call: {functionCallNode.FunctionName}");
                foreach (Node arguments in functionCallNode.Arguments)
                {
                    Traverse(arguments);
                }
                break;
            
            case IfNode ifNode:
                Output.AppendLine($"If statement");
                Traverse(ifNode.Condition);
                Traverse(ifNode.Body);
                break;
            
            case WhileNode whileNode:
                Output.AppendLine($"While loop");
                Traverse(whileNode.Condition);
                Traverse(whileNode.Body);
                break;
            case BlockNode blockNode:
                Output.AppendLine("Block:");
                foreach (Node statements in blockNode.Statements)
                {
                    Traverse(statements);
                }
                break;
            
                
            default:
                throw new Exception($"Unknown Node Type: {node.GetType().Name}");
        }

        return Output.ToString();
    }
}