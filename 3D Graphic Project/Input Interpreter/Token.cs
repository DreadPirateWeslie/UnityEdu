using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Interpreter
{
	public class Token
	{
		public TokenType type;
		public string sval;
		public double dval;
		public enum TokenType { OPERATOR, FUNCTION, NUMBER, VARIABLE, CONSTANT, LOCKED }

		public Token(TokenType type, String sval)
		{
			this.type = type;
			this.sval = sval;
		}

		public Token(TokenType type, double dval)
		{
			this.type = type;
			this.dval = dval;
		}

		public Token(TokenType type, String sval, double dval)
		{
			this.type = type;
			this.sval = sval;
			this.dval = dval;
		}
		public override string ToString()
		{
			switch (type)
			{
				case TokenType.NUMBER:
				case TokenType.CONSTANT:
					return dval.ToString();
				default:
					return sval;
			}
		}
	}
}
