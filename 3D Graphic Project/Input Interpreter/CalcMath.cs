using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Input_Interpreter.Token;

namespace Input_Interpreter
{
	public class CalcMath
	{
		/**
		 * Operate two tokens with an operator token.
		 *
		 * @param operand1
		 *            the first operand
		 * @param operand2
		 *            the second operand
		 * @param operator
		 *            the operator
		 * @return the token[] result
		 * @throws Exception
		 *             the exception
		 */
		public static Token[] operate(Token operand1, Token operand2, Token token_operator)
		{
			Double num1 = 0.0, num2 = 0.0;
			try
			{
				num1 = operand1.dval;
				num2 = operand2.dval;
			}
			catch (Exception e)
			{
				return new Token[] { operand2, operand1, token_operator }; // postfix
			}
			double ans;
			switch (token_operator.sval)
			{
				case "+":
					ans = num1 + num2;
					return new Token[] { new Token(TokenType.NUMBER, ans) };
				case "-":
					ans = num1 - num2;
					return new Token[] { new Token(TokenType.NUMBER, ans) };
				case "*":
					ans = num1 * num2;
					return new Token[] { new Token(TokenType.NUMBER, ans) };
				case "/":
					if (num2 == 0.0)
					{
						throw new Exception("cannot divide by zero");
					}
					else
					{
						ans = num1 / num2;
						return new Token[] { new Token(TokenType.NUMBER, ans) };
					}
				case "^":
					ans = pow(num1, num2);
					return new Token[] { new Token(TokenType.NUMBER, ans) };
				case "%":
					ans = num1 % num2;
					return new Token[] { new Token(TokenType.NUMBER, ans) };
				default:
					throw new Exception(token_operator + " invalid operator");
			}
		}

		// cowlinator's adapted Math.pow fix on stack overflow
		public static double pow(double expBase, double power)
		{
			bool negative = (expBase < 0);
			if (negative && HasEvenDenominator(power) && !isWhole(power))
			{
				return Double.NaN; // sqrt(-1) = i
			}
			else
			{
				if (negative && HasOddDenominator(power) && !isWhole(power))
				{
					return -1 * Math.Pow(Math.Abs(expBase), power);
				}
				else
				{
					return Math.Pow(expBase, power);
				}
			}
		}

		private static bool isWhole(double power)
		{
			return power % 1 == 0;
		}

		// cowlinator's adapted Math.pow fix on stack overflow
		private static bool HasEvenDenominator(double power)
		{
			if (power == 0)
			{
				return false;
			}
			else if (power % 1 == 0)
			{
				return false;
			}
			double inverse = Math.Round(1 / power);
			if (inverse % 2 == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		// cowlinator's adapted Math.pow fix on stack overflow
		private static bool HasOddDenominator(double power)
		{
			if (power == 0)
			{
				return false;
			}
			else if (power % 1 == 0)
			{
				return false;
			}
			double inverse = Math.Round(1 / power);
			if (Math.Round((inverse + 1)) % 2 == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/**
		 * Pass the arguments to a function and calculate the result.
		 *
		 * @param c
		 *            the calculator instance
		 * @param func
		 *            the function token
		 * @param args
		 *            the arguments as Token[]
		 * @return the answer as Token[]
		 * @throws Exception
		 *             for invalid function names or invalid arguments
		 */
		public static Token[] function(Calc c, Token func, Token[] args)
		{
			if (func.type != TokenType.FUNCTION)
			{
				throw new Exception(func.ToString() + " is an invalid function");
			}
			String identifier = func.sval;
			if (c.hasLockedToken(args))
			{
				Token[] ret = new Token[args.Count() + 1];
				List<Token> temp = new List<Token>();
				temp.AddRange(args.ToList());
				temp.Add(func); // post fix
				return temp.ToArray();
			}
			switch (func.sval)
			{
				case "sin":
					return new Token[] { new Token(TokenType.NUMBER, Math.Sin(args[0].dval)) };
				case "cos":
					return new Token[] { new Token(TokenType.NUMBER, Math.Cos(args[0].dval)) };
				case "tan":
					return new Token[] { new Token(TokenType.NUMBER, Math.Tan(args[0].dval)) };
				case "asin":
					return new Token[] { new Token(TokenType.NUMBER, Math.Asin(args[0].dval)) };
				case "acos":
					return new Token[] { new Token(TokenType.NUMBER, Math.Acos(args[0].dval)) };
				case "atan":
					return new Token[] { new Token(TokenType.NUMBER, Math.Atan(args[0].dval)) };
				case "ln":
					if (args[0].dval <= 0)
					{
						throw new Exception(args[0].dval + " is <= 0");
					}
					return new Token[] { new Token(TokenType.NUMBER, Math.Log(args[0].dval)) };
				case "log":
					if (args[0].dval <= 0)
					{
						throw new Exception(args[0].dval + " is <= 0");
					}
					else
					{
						return new Token[] { new Token(TokenType.NUMBER, Math.Log10(args[0].dval)) };
					}
				case "abs":
					return new Token[] { new Token(TokenType.NUMBER, Math.Abs(args[0].dval)) };
				case "rng":
					Random rand = new Random();
					return new Token[] { new Token(TokenType.NUMBER, rand.NextDouble() * args[0].dval) };
				case "round":
					return new Token[] { new Token(TokenType.NUMBER, (double)Math.Round(args[0].dval)) };
				default:
					Token[] functokens = c.getFunction(identifier).definition;
					for (int i = 0; i < functokens.Count(); i++)
					{
						Token t = functokens[i];
						if (t.type == TokenType.VARIABLE && t.sval.Equals(c.getFunction(identifier).variable))
						{
							functokens = ArrayUtils.replace(i, i, args[0], functokens);
							i = 0;
						}
					}
					if (c.hasLockedToken(functokens))
					{
						return functokens;
					}
					else
					{
						Stack<Token> stack = ArrayUtils.arrayToStack(functokens);
						//ArrayUtils.printArray(stack.ToArray()); //??
						return new Token[] { new Token(TokenType.NUMBER, c.computePostfixStack(stack)[0].dval) };
					}
			}
		}
	}
}
