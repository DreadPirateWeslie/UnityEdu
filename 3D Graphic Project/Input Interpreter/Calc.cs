using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Input_Interpreter.Token;
using System.Text.RegularExpressions;

namespace Input_Interpreter
{
	public class Calc
	{
		public string MASTER_REGEX, CMD_REGEX, DECLR_REGEX = "^[^=]*=[^=]*$";
		public string[] ORDER_OPS = { "^", "%", "/", "*", "+", "-" };
		public string[] DEFAULT_FUNCS = { "sin", "cos", "tan", "asin", "acos", "atan", "ln", "log", "abs", "rng", "round" };
		public Dictionary<string, Token> constants;
		public Dictionary<string, Token[]> constant_defs;
		public Dictionary<string, string> commands;
		public List<string> variables;
		public List<Function> functions;


		static void Main(string[] args)
		{
			Calc c = new Calc();
			Console.WriteLine("--------------------------");
			while (true)
			{
				string input = Console.ReadLine();
				if (input.Trim().Equals("q"))
					break;
				try
				{
					Console.WriteLine(c.dispatch(input));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		public Calc()
		{
			constants = new Dictionary<string, Token>();
			functions = new List<Function>();
			constant_defs = new Dictionary<string, Token[]>();
			variables = new List<string>();
			commands = new Dictionary<string, string>();
			constants["pi"] = new Token(TokenType.NUMBER, Math.PI);
			constants["e"] = new Token(TokenType.NUMBER, Math.E);
			declare("cosh(x)=(e^x+e^-x)/2");
			declare("sinh(x)=(e^x-e^-x)/2");
			declare("tanh(x)=sinh(x)/cosh(x)");
			// define commands
			commands["var"] = "[a-zA-Z_]+";
			commands["print"] = "[a-zA-Z_]+";
			CMD_REGEX = ArrayUtils.getCommandMatchRegex(commands);
			// define default functions
			foreach (string func in DEFAULT_FUNCS)
			{
				Dictionary<string, Token[]> defmap = new Dictionary<string, Token[]>();
				defmap["x"] = new Token[] { new Token(TokenType.FUNCTION, "x") };
				functions.Add(new Function(func, "x", new Token[] { new Token(TokenType.FUNCTION, func) }));
			}
			update();
		}

		/*
		 * Decide what to do with the input: Evaluate an expression or
		 * declare/define a function/constant or command.
		 *
		 * @param input
		 *            the user's raw input
		 * @return the system message or answer as String
		 * @throws Exception
		 *             the exception (caught exceptions)
		 */
		public string dispatch(string input)
		{
			if (Regex.IsMatch(input, DECLR_REGEX))
			{ // exactly one = sign
				return declare(input);
			}
			else if (Regex.IsMatch(input, CMD_REGEX))
			{
				return command(input);
			}
			else
			{
				return calculate(input);
			}
		}
		/**
		 * Declare and define a function or constant.
		 * 
		 * @param input
		 *            the user's raw input
		 * @return the system message as String
		 * @throws Exception
		 *             the exception (invalid declaration/definition)
		 */
		public string declare(string input)
		{
			if (input.IndexOf("=") == 0)
			{
				throw new Exception("empty declaration");
			}
			if (input.IndexOf("=") == input.Count() - 1)
			{
				throw new Exception("empty definition");
			}
			string declaration = Regex.Split(input, "=")[0].Trim();
			string definition = Regex.Split(input, "=")[1];
			if (Regex.IsMatch(declaration, "[a-zA-Z_]+[(][a-zA-Z_]+[)]"))
			{
				// it's a function
				string name = Regex.Split(declaration, "\\(|\\)")[0];
				foreach (Function func in functions)
				{
					if (func.identifier == name) throw new Exception("\"" + name + "\" is already declared!");
				}
				string var = Regex.Split(declaration, "\\(|\\)")[1];
				if (String.IsNullOrEmpty(var)) throw new Exception("\"" + var + "\" is an invalid variable");
				command("var " + var);
				Token[] deftokens = tokenize(definition);
				Stack<Token> stack = ArrayUtils.arrayToStack(deftokens);
				Dictionary<string, Token[]> map = new Dictionary<string, Token[]>();
				functions.Remove(getFunction(name));
				functions.Add(new Function(name, var, deftokens));
				update();
				return name + "(" + var + ")=" + ArrayUtils.stringArray(NotationUtils.postfixToInfix(this, stack));
			}
			else if (Regex.IsMatch(declaration, "[a-zA-Z_]+"))
			{
				// it's a constant
				Token[] deftokens = tokenize(definition);
				if (hasLockedToken(deftokens))
				{
					throw new Exception("\"" + declaration + "\" contains locked token(s)");
				}
				constant_defs[declaration] = deftokens;
				update();
				return "\"" + declaration + "\" defined as \"" + ArrayUtils.stringArray(ArrayUtils.tokenArrayToStringArray(deftokens)) + "\"";
			}
			else
			{
				throw new Exception("\"" + declaration + "\" is an invalid declaration");
			}
		}

		/**
	 * Accepts line determined to be in command format.
	 *
	 * @param input
	 *            the input
	 * @return the result/report as String
	 * @throws Exception
	 *             the exception
	 */
		public string command(string input)
		{
			string cmd = Regex.Split(input, " ")[0], arg = Regex.Split(input, " ")[1];
			switch (cmd)
			{
				case "var":
					variables.Add(arg);
					if (isConstant(arg))
					{
						constants.Remove(arg);
						constant_defs.Remove(arg);
					}
					update();
					return "\"" + arg + "\" declared as a variable";
				case "print":
					if (isFunction(arg))
					{
						Function f = getFunction(arg);
						return f.identifier + "(" + f.variable + ") = " + ArrayUtils.stringArray(NotationUtils.postfixToInfix(this, ArrayUtils.arrayToStack(f.definition)));
					}
					else if (isConstant(arg))
					{
						return arg + " = " + constants[arg];
					}
					throw new Exception(arg + " is not a constant or function");
				default:
					return "invalid command";
			}
		}

		/**
		 * Checks if provided tokens array contains a variable toke n.
		 *
		 * @param tokens
		 *            the tokens array
		 * @return true, if successful
		 */
		public bool hasVariable(Token[] tokens)
		{
			foreach (Token token in tokens)
			{
				if (token.type == TokenType.VARIABLE)
				{
					return true;
				}
			}
			return false;
		}

		/**
		 * Update regexes, updates constants and orders functions.
		 *
		 * @throws Exception
		 *             the exception
		 */
		public void update()
		{
			foreach (string key in constant_defs.Keys)
			{
				constants[key] = new Token(TokenType.NUMBER, computePostfixStack(ArrayUtils.arrayToStack(constant_defs[key]))[0].dval);
			}
			int[] funclengths = new int[DEFAULT_FUNCS.Count()];
			for (int i = 0; i < DEFAULT_FUNCS.Count(); i++)
			{
				funclengths[i] = DEFAULT_FUNCS[i].Count();
			}
			string PAREN_REGEX = ArrayUtils.getDelimiterInclusiveSplitRegex(new List<string> { "\\)", "\\(" });
			string OPR_REGEX = "(?=-)|" + ArrayUtils.getDelimiterInclusiveSplitRegex(new List<string> { "\\^", "%", "/", "\\*", "\\+" });
			List<string> consts = new List<string>();
			foreach (string key in constants.Keys)
			{
				consts.Add(key);
			}
			string CONST_REGEX = ArrayUtils.getCharacterIsolatedSplitRegex(consts);
			List<string> identifiers = new List<string>();
			foreach (Function f in functions)
			{
				identifiers.Add(f.identifier);
			}
			string FUNC_REGEX = ArrayUtils.getCharacterIsolatedSplitRegex(identifiers);
			string VAR_REGEX = ArrayUtils.getCharacterIsolatedSplitRegex(variables);
			MASTER_REGEX = PAREN_REGEX + "|" + OPR_REGEX + "|" + CONST_REGEX + "|" + VAR_REGEX + "|" + FUNC_REGEX;
		}
		/**
		 * Calculate raw user's input.
		 *
		 * @param input
		 *            the user's input
		 * @return the result as String
		 * @throws Exception
		 *             the exception
		 */
		public string calculate(string input)
		{
			if (String.IsNullOrEmpty(input))
			{
				throw new Exception("empty input");
			}
			Token[] tokens = tokenize(input);
			ArrayUtils.printArray(ArrayUtils.tokenArrayToStringArray(tokens));
			Stack<Token> stack = ArrayUtils.arrayToStack(tokens);
			Token[] ans = computePostfixStack(stack);
			return ArrayUtils.stringArray(NotationUtils.postfixToInfix(this, ArrayUtils.arrayToStack(ans)));
		}

		/**
		 * Returns an array of type Token from given user's input. The user's input
 		 * is split with my old regexes, converted to reversed prefix, and Populated
		 * into a Token array.Exceptions will be caught in subsequent methods for
		 * bad token input
		 *
		 * @param input
		 * the user's raw input
		 * @return the token[] result
		 * @throws Exception
		 *             the exception
		 */
		public Token[] tokenize(string input)
		{
			string[] infix = format(input);
			Console.WriteLine("infix = ");
			ArrayUtils.printArray(infix);
			string[] postfix = NotationUtils.infixToPostfix(this, infix);
			Console.WriteLine("postfix = ");
			ArrayUtils.printArray(postfix);
			Token[] tokens = new Token[postfix.Count()];
			for (int i = 0; i < postfix.Count(); i++)
			{
				string s = postfix[i];
				if (isNumber(s))
				{
					tokens[i] = new Token(TokenType.NUMBER, s, Double.Parse(s));
				}
				else if (isVariable(s))
				{
					tokens[i] = new Token(TokenType.VARIABLE, s);
				}
				else if (isConstant(s))
				{
					tokens[i] = new Token(TokenType.CONSTANT, s, constants[s].dval);
				}
				else if (isFunction(s))
				{
					tokens[i] = new Token(TokenType.FUNCTION, s);
				}
				else if (isOperator(s))
				{
					tokens[i] = new Token(TokenType.OPERATOR, s);
				}
				else
				{
					throw new Exception(postfix[i] + " is a bad token (tokenizer)");
				}
			}

			return tokens;
		}

		/**
		 * Compute postfix stack.
		 *
		 * @param postfix
		 *            the tokens stack
		 * @return the token[] result
		 * @throws Exception
		 *             the exception
		 */
		public Token[] computePostfixStack(Stack<Token> postfix)
		{
			// change isEmpty to count != 0
			while (postfix.Count() != 0)
			{
				switch (postfix.Peek().type)
				{
					case TokenType.FUNCTION:
						Token func = postfix.Pop();
						Token[] args = computePostfixStack(postfix);
						if (hasLockedToken(args))
						{
							return CalcMath.function(this, func, args);
						}
						else
						{
							Token r = CalcMath.function(this, func, args)[0];
							if (r.dval == -0)
							{ // -0 fix
								r.dval *= -1;
							}
							return new Token[] { r };
						}
					case TokenType.OPERATOR:
						Token op = postfix.Pop();
						Token[] num2 = computePostfixStack(postfix);
						Token[] num1 = computePostfixStack(postfix);
						if (hasLockedToken(num1) || hasLockedToken(num2))
						{
							List<Token> temp = new List<Token>();
							temp.AddRange(num2.ToList());
							temp.AddRange(num1.ToList());
							temp.Add(op); // post fix
							Token[] ret = new Token[temp.Count()];
							ret = temp.ToArray();
							return ret;
						}
						else
						{
							Token r = CalcMath.operate(num1[0], num2[0], op)[0];
							if (r.dval == -0)
							{ // -0 fix
								r.dval *= -1;
							}
							return new Token[] { r };
						}
					case TokenType.CONSTANT:
						return new Token[] { new Token(TokenType.NUMBER, postfix.Pop().dval) };
					case TokenType.VARIABLE:
					case TokenType.NUMBER:
					case TokenType.LOCKED:
						if (postfix.Peek().dval == -0)
						{ // -0 fix
							postfix.Peek().dval *= -1;
						}
						return new Token[] { postfix.Pop() };
				}
			}
			throw new Exception("empty tokens");
		}

		/**
		 * Validate input and format into to array of strings.
		 *
		 * @param input
		 *            the input
		 * @return the string[] result
		 * @throws Exception
		 *             the exception (invalid input due to formatting or math)
		 */
		public string[] format(string input)
		{
			input = input.Replace(" ", "");
			string[] tokens = Regex.Split(input, MASTER_REGEX);
			int open = 0, close = 0;
			for (int i = 0; i < tokens.Count(); i++)
			{
				if (tokens[i].Equals("("))
				{
					open++;
				}
				if (tokens[i].Equals(")"))
				{
					close++;
				}
			}
			// check validity of the input
			if (open != close)
			{
				throw new Exception(open + " open and " + (close) + " close parentheses");
			}

			for (int i = 0; i < tokens.Count() - 1; i++)
			{
				if (isFunction(tokens[i]) && !tokens[i + 1].Equals("("))
				{
					throw new Exception("expected ( at after index " + i);
				}
			}

			// handle implied multiplication and subtraction problems with tokens
			// it's the worst part of the program
			tokens = handleAdjacentTokens(tokens);

			if (isOperator(tokens[0]))
			{
				throw new Exception("operator " + tokens[0] + " at start of input");
			}
			if (isOperator(tokens[tokens.Count() - 1]))
			{
				throw new Exception("operator " + tokens[tokens.Count() - 1] + " at end of input");
			}
			for (int i = 0; i < tokens.Count() - 1; i++)
			{
				if (isOperator(tokens[i]) && isOperator(tokens[i + 1]))
				{
					throw new Exception("adjacent operators " + tokens[i] + tokens[i + 1]);
				}
			}

			// re-split tokens
			tokens = Regex.Split("(" + ArrayUtils.stringArray(tokens) + ")", MASTER_REGEX);
			List<string> temp_tokens = tokens.ToList();
			temp_tokens.RemoveAll(s => String.IsNullOrEmpty(s));
			tokens = temp_tokens.ToArray();

			// test parenthesis tokens
			try
			{
				matchParentheses(tokens);
			}
			catch (Exception e)
			{
				throw e;
			}
			tokens = handleAdjacentTokens(tokens);
			Console.WriteLine("Handled tokens = ");
			ArrayUtils.printArray(tokens);
			foreach (string token in tokens)
			{
				if (!isOperand(token) && !isOperator(token) && !isFunction(token) && !token.Equals("(") && !token.Equals(")"))
				{
					throw new Exception(token + " is a bad token (format)");
				}
			}
			for (int i = tokens.Count() - 1; i >= 2; i--)
			{
				string third = tokens[i];
				string second = tokens[i - 1];
				string first = tokens[i - 2];
				if (isOperand(first) && second.Equals("^") && isOperand(third))
				{
					tokens = ArrayUtils.replace(i - 2, i, new string[] { "(", first, second, third, ")" }, tokens);
				}
			}
			return tokens;
		}

		/**
		 * Handles adjacent tokens, adds elements for implied multiplication, and
		 * fixes the regex splitting problem with subtraction/negation.
		 *
		 * @param tokens
		 *            the tokens
		 * @return the resulting tokens as string[]
		 */
		private string[] handleAdjacentTokens(string[] tokens)
		{
			Console.WriteLine("Tokens to handle: ");
			ArrayUtils.printArray(tokens);
			for (int i = 0; i < tokens.Count() - 1; i++)
			{
				// NEW SOLUTION 5-9-16, updated 10th to include functions
				string token = tokens[i];
				string next = tokens[i + 1];
				if ((isOperand(token) || token.Equals(")")) && next.Contains("-"))
				{
					// changed 4-11-17
					Console.WriteLine("replacing \"" + tokens[i + 1] + "\", split =");
					ArrayUtils.printArray(Regex.Split(tokens[i + 1], "(?<=-)"));
					tokens = ArrayUtils.replace(i + 1, i + 1, Regex.Split(tokens[i + 1], "(?<=-)"), tokens);// "(?<=-)|(?=-)"));
				}
			}

			if (tokens.Count() >= 2)
			{
				if (tokens[0].Equals("-") && (isVariable(tokens[1]) || isConstant(tokens[1]) || isFunction(tokens[1])))
				{
					tokens = ArrayUtils.replace(0, 1, new string[] { "(", "-1", "*", tokens[1], ")" }, tokens);
				}
			}
			for (int i = 0; i < tokens.Count() - 2; i++)
			{
				string first = tokens[i];
				string second = tokens[i + 1];
				string third = tokens[i + 2];
				if (isOperator(first) && second.Equals("-") && (isVariable(third) || isConstant(third)))
				{
					tokens = ArrayUtils.replace(i + 1, i + 2, new string[] { "(", "-1", "*", tokens[i + 2], ")" }, tokens);
				}
				if (first.Equals("(") && second.Equals("-") && (isVariable(third) || isConstant(third)))
				{
					tokens = ArrayUtils.replace(i + 1, i + 2, new string[] { "(", "-1", "*", tokens[i + 2], ")" }, tokens);
				}
				if (isOperator(first) && second.Equals("-") && isFunction(third))
				{
					tokens = ArrayUtils.replace(i + 1, i + 2, new string[] { "-1", "*", tokens[i + 2] }, tokens);
				}
				if (first.Equals("(") && second.Equals("-") && isFunction(third))
				{
					tokens = ArrayUtils.replace(i + 1, i + 2, new string[] { "-1", "*", tokens[i + 2] }, tokens);
				}
			}
			for (int i = 0; i < tokens.Count() - 1; i++)
			{
				string token = tokens[i];
				string next = tokens[i + 1];
				// many cases of implied multiplication
				if (isConstant(token) && isConstant(next))
				{
					if (!isConstant(token + next))
					{
						tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
					}
					else
					{
						tokens = ArrayUtils.replace(i, i + 1, token + next, tokens);
					}
				}
				if (isNumber(token) && isConstant(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (token.Equals(")") && next.Equals("("))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isNumber(token) && next.Equals("("))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isConstant(token) && next.Equals("("))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (token.Equals(")") && isNumber(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (token.Equals(")") && isConstant(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (token.Equals(")") && isVariable(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isVariable(token) && next.Equals("("))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isNumber(token) && isVariable(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isVariable(token) && isVariable(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isConstant(token) && isVariable(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isVariable(token) && isConstant(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isVariable(token) && isNumber(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (token.Equals("-") && next.Equals("("))
				{
					tokens = ArrayUtils.replace(i, i + 1, new String[] { "-1", "*", "(" }, tokens);
				}
				if (isNumber(token) && isFunction(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isConstant(token) && isFunction(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
				if (isVariable(token) && isFunction(next))
				{
					tokens = ArrayUtils.insertBetween(i, i + 1, "*", tokens);
				}
			}
			return tokens;
		}

		/**
		 * Gets the function from the functions ArrayList.
		 *
		 * @param identifier
		 *            the function identifier/name
		 * @return the function
		 */
		public Function getFunction(string identifier)
		{
			foreach (Function f in functions)
			{
				if (identifier.Equals(f.identifier))
				{
					return f;
				}
			}
			return null;
		}

		/**
		 * Matches parentheses, throws an exception if a problem occurs.
		 *
		 * @param tokens
		 *            the tokens
		 * @throws Exception
		 *             the exception
		 */
		public void matchParentheses(string[] tokens)
		{
			string[] temp = new string[tokens.Count()];
			for (int i = 0; i < temp.Count(); i++)
			{
				temp[i] = tokens[i];
			}

			for (int i = temp.Count() - 1; i >= 0; i--)
			{
				if (temp[i].Equals("(") && i != 0)
				{
					for (int j = i; j < temp.Count(); j++)
					{
						if (temp[j].Equals(")") && j != temp.Count() - 1)
						{
							temp[j] = "checked";
							goto left;
						}
					}
					throw new Exception("unmatched parenthesis at index " + i);
				}
				left: { }
			}
		}

		public bool isConstant(string s)
		{
			foreach (string key in constants.Keys)
			{
				if (s.Equals(key))
				{
					return true;
				}
			}
			return false;
		}

		public bool isNumber(string s)
		{
			try
			{
				Double.Parse(s);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}

		public bool isOperator(string s)
		{
			foreach (string op in ORDER_OPS)
			{
				if (s.Equals(op))
				{
					return true;
				}
			}
			return false;
		}

		public bool isFunction(string s)
		{
			foreach (Function func in functions)
			{
				if (s.Equals(func.identifier))
				{
					return true;
				}
			}
			return false;
		}

		public bool isOperand(string s)
		{
			return (isConstant(s) || isNumber(s) || isVariable(s));
		}

		/**
		 * Checks if provided tokens array contains a locked token.
		 *
		 * @param tokens
		 *            the tokens array
		 * @return true, if successful
		 */
		public bool hasLockedToken(Token[] tokens)
		{
			// variables locked for now
			foreach (Token t in tokens)
			{
				if (t.type == TokenType.LOCKED || t.type == TokenType.VARIABLE)
				{
					return true;
				}
			}
			return false;
		}
		/**
		 * Checks if input is variable.
		 *
		 * @param s
		 *            the potential variable
		 * @return true, if s is variable
		 */
		public bool isVariable(string s)
		{
			foreach (string var in variables)
			{
				if (s.Equals(var))
				{
					return true;
				}
			}
			return false;
		}
	}
}
