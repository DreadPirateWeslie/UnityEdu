using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Input_Interpreter.Token;

namespace Input_Interpreter
{
	public class NotationUtils
	{
		public static string[] infixToPostfix(Calc c, string[] infix)
		{
			Stack<string> postfix = new Stack<string>();
			Stack<string> temp = new Stack<string>();
			for (int i = 0; i < infix.Count(); i++)
			{
				string curr = infix[i];
				if (c.isOperand(curr))
				{
					postfix.Push(curr);
				}
				else if (curr.Equals("("))
				{
					temp.Push(curr);
				}
				else if (curr.Equals(")"))
				{
					while (!temp.Peek().Equals("("))
					{
						postfix.Push(temp.Pop());
					}
					temp.Pop();
				}
				else if (c.isOperator(curr) || c.isFunction(curr))
				{
					while (temp.Count() != 0 && !temp.Peek().Equals("(") && (getPrecedence(c, curr) <= getPrecedence(c, temp.Peek())))
					{
						postfix.Push(temp.Pop());
					}
					temp.Push(curr);
				}
			}
			while (temp.Count() != 0)
			{
				postfix.Push(temp.Pop());
			}
			return ArrayUtils.stringStackToArray(postfix);
		}
		/**
		* This method generates an array of infix from postfix (my reversed prefix)
		* 
		* @param c
		*            the Calculator
		* @param postfixStack
		*            the Stack of type Token
		* @return the result as String[]
		* @throws Exception
		*/
		public static string[] postfixToInfix(Calc c, Stack<Token> postfixStack)
		{
			// as used in the calculator, this point has valid reversed prefix
			Stack<string> infixStack = new Stack<string>();
			while (postfixStack.Count() != 0)
			{
				switch (postfixStack.Peek().type)
				{
					case TokenType.CONSTANT:
					case TokenType.NUMBER:
						return new string[] { postfixStack.Pop().dval.ToString() };
					case TokenType.LOCKED:
					case TokenType.VARIABLE:
					case TokenType.OPERATOR:
						string string_operator = postfixStack.Pop().sval;
						string[] operand1 = postfixToInfix(c, postfixStack);
						string[] operand2 = postfixToInfix(c, postfixStack);
						infixStack.Push("(");
						foreach (string s in operand1)
						{
							infixStack.Push(s);
						}
						infixStack.Push(string_operator);
						foreach (string s in operand2)
						{
							infixStack.Push(s);
						}
						infixStack.Push(")");
						return ArrayUtils.stringStackToArray(infixStack);
					case TokenType.FUNCTION:
						string function = postfixStack.Pop().sval;
						string[] args = postfixToInfix(c, postfixStack);
						infixStack.Push("(");
						infixStack.Push(function);
						if (!args[0].Equals("("))
						{ // for redundancies
							infixStack.Push("(");
						}
						foreach (string s in args)
						{
							infixStack.Push(s);
						}
						if (!args[args.Count() - 1].Equals(")"))
						{ // for redundancies
							infixStack.Push(")");
						}
						infixStack.Push(")");
						return ArrayUtils.stringStackToArray(infixStack);
				}
			}
			return ArrayUtils.stringStackToArray(infixStack);
		}
		/**
		* Gets the precedence of an operator.
		*
		* @param c
		*            the instance of Calc
		* @param operator
		*            the operator
		* @return the precedence
		*/
		private static int getPrecedence(Calc c, string string_operator)
		{
			if (c.isFunction(string_operator))
			{
				return 5;
			}
			switch (string_operator)
			{
				case "^":
					return 4;
				case "/":
				case "*":
				case "%":
					return 3;
				case "+":
				case "-":
					return 2;
				default:
					return 1; // shouldn't get here after validation
			}
		}
	}
}
