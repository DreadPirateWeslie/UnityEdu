using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Interpreter
{
	public class ArrayUtils
	{
		/**
		 * Gets the command match regex.
		 *
		 * @param cmds
		 *            the cmds
		 * @return the command match regex
		 */
		public static string getCommandMatchRegex(Dictionary<string, string> cmds)
		{
			StringBuilder strb = new StringBuilder();
			bool visited = false;
			foreach (string key in cmds.Keys)
			{
				strb.Append("(" + key + cmds[key] + ")|");
				visited = true;
			}
			if (!visited)
			{
				return "-(.*)"; // the "nothing" regex, or "not anything"
			}
			strb.Length--;
			// replacement for c#
			//strb.deleteCharAt(strb.length() - 1);
			return strb.ToString();
		}

		/**
		 * Gets the delimiter inclusive split regex.
		 *
		 * @param delimiters
		 *            the delimiters
		 * @return the delimiter inclusive split regex as String
		 */
		public static string getDelimiterInclusiveSplitRegex(List<string> delimiters)
		{
			StringBuilder strb = new StringBuilder();
			bool visited = false;
			foreach (string key in delimiters)
			{
				strb.Append("(?=" + key + ")|(?<=" + key + ")|");
				visited = true;
			}
			if (!visited)
			{
				return "-(.*)"; // the "nothing" regex, or "not anything"
			}
			strb.Length--;
			// replacement for c#
			//strb.deleteCharAt(strb.length() - 1);
			return strb.ToString();
		}

		/**
		 * Gets the character isolated split regex.
		 *
		 * @param delimiters
		 *            the delimiters
		 * @return the character isolated split regex
		 */
		public static string getCharacterIsolatedSplitRegex(List<string> delimiters)
		{
			StringBuilder strb = new StringBuilder();
			bool visited = false;
			foreach (string key in delimiters)
			{
				strb.Append("(?=(?<![a-zA-Z])" + key + ")|(?<=" + key + "(?![a-zA-Z]))|");
				visited = true;
			}
			if (!visited)
			{
				return "-(.*)"; // the "nothing" regex, or "not anything"
			}
			strb.Length--;
			// replacement for c#
			//strb.deleteCharAt(strb.length() - 1);
			return strb.ToString();
		}
		/**
		 * Convert a Token array to a stack.
		 *
		 * @param arr
		 *            the Token array
		 * @return the stack result
		 */
		public static Stack<Token> arrayToStack(Token[] arr)
		{
			Stack<Token> temp = new Stack<Token>();
			for (int i = 0; i < arr.Count(); i++)
			{
				temp.Push(arr[i]);
			}
			return temp;
		}
		/**
		 * Replaces a range of positions with a specified array of Strings.
		 *
		 * @param left
		 *            the left index
		 * @param right
		 *            the right index
		 * @param dst
		 *            the desired array of replacement Strings
		 * @param src
		 *            the source string array
		 * @return the desired result copied as string[]
		 */
		public static string[] replace(int left, int right, string[] dst, string[] src)
		{
			if (left < 0 || right > src.Count() - 1 || right < left)
			{
				return src;
			}
			int delta = right - left + 1;
			string[] temp = new string[src.Count() + (dst.Count() - delta)];
			for (int i = 0; i < left; i++)
			{
				temp[i] = src[i];
			}
			for (int i = 0; i < dst.Count(); i++)
			{
				temp[left + i] = dst[i];
			}
			for (int i = right + 1; i < src.Count(); i++)
			{
				temp[left + dst.Count() + (i - (right + 1))] = src[i];
			}
			List<string> temp_list = temp.ToList();
			temp_list.RemoveAll(s => String.IsNullOrEmpty(s));
			return temp_list.ToArray();
		}

		/**
		 * Replaces a range of positions with a specified array of Tokens.
		 *
		 * @param left
		 *            the left index exclusive
		 * @param right
		 *            the right index exclusive
		 * @param dst
		 *            the desired array of replacement Tokens
		 * @param src
		 *            the source string array
		 * @return the token[] result
		 */
		public static Token[] replace(int left, int right, Token[] dst, Token[] src)
		{
			if (left < 0 || right > src.Count() - 1 || right < left)
			{
				return src;
			}
			int delta = right - left + 1;
			Token[] temp = new Token[src.Count() + (dst.Count() - delta)];
			for (int i = 0; i < left; i++)
			{
				temp[i] = src[i];
			}
			for (int i = 0; i < dst.Count(); i++)
			{
				temp[left + i] = dst[i];
			}
			for (int i = right + 1; i < src.Count(); i++)
			{
				temp[left + dst.Count() + (i - (right + 1))] = src[i];
			}
			return temp;
		}

		/**
		 * Replace a range of positions with a specific element.
		 *
		 * @param <T>
		 *            the generic type
		 * @param left
		 *            the left index
		 * @param right
		 *            the right index
		 * @param dst
		 *            the desired replacement
		 * @param src
		 *            the source string
		 * @return the desired result copied as string[]
		 */
		public static T[] replace<T>(int left, int right, T dst, T[] src)
		{
			if (left < 0 || right > src.Count() - 1)
			{
				return src;
			}
			int delta = right - left;
			int length = (dst == null) ? src.Count() - delta - 1 : src.Count() - delta;
			T[] temp = new T[length];//(T[])Array.newInstance(src.getClass().getComponentType(), length);
			for (int i = 0; i < left; i++)
			{
				temp[i] = src[i];
			}
			if (dst != null)
			{
				temp[left] = dst;
			}
			for (int i = right + 1; i < src.Count(); i++)
			{
				temp[i - delta] = src[i];
			}
			return temp;
		}

		public static void printArray(string[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				Console.Write(arr[i] + ",");
			}
			Console.WriteLine();
		}

		public static string[] tokenArrayToStringArray(Token[] tokens)
		{
			string[] str = new string[tokens.Count()];
			for (int i = 0; i < tokens.Count(); i++)
			{
				str[i] = tokens[i].sval;
			}
			return str;
		}
		/**
		 * Insert between specified indices, exclusive of the indices.
		 *
		 * @param left
		 *            the left index exclusive
		 * @param right
		 *            the right index exclusive
		 * @param dst
		 *            the desired insertion
		 * @param src
		 *            the source String array
		 * @return the result as string[]
		 */
		public static string[] insertBetween(int left, int right, string dst, string[] src)
		{
			if (left < 0 || right > src.Count() - 1 || right < left)
			{
				return src;
			}
			string[] temp = new string[src.Count() + 1];
			for (int i = 0; i <= left; i++)
			{
				temp[i] = src[i];
			}
			temp[left + 1] = dst;
			for (int i = right; i < src.Count(); i++)
			{
				temp[i + 1] = src[i];
			}
			return temp;
		}
		/**
		 * Returns a String array from a given Stack of type String
		 * 
		 * @param stack
		 *            of type String
		 * @return the String[] result
		 */
		public static string[] stringStackToArray(Stack<string> stack)
		{
			string[] temp = new string[stack.Count()];
			for (int i = 0; i < temp.Count(); i++)
			{
				temp[temp.Count() - i - 1] = stack.Pop();
			}
			return temp;
		}
		/**
		 * Appends the elements of a String array into a String.
		 *
		 * @param arr
		 *            the String array
		 * @return the resulting string
		 */
		public static string stringArray(string[] arr)
		{
			StringBuilder str = new StringBuilder();
			foreach (string s in arr)
			{
				str.Append(s);
			}
			return str.ToString();
		}

	}
}
