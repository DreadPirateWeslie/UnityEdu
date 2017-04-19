using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Interpreter
{
	public class Function
	{
		public string identifier { get; set; }
		public string variable { get; set; }
		public Token[] definition { get; set; }

		public Function(string identifier, string variable, Token[] definition)
		{
			this.identifier = identifier;
			this.variable = variable;
			this.definition = definition;
		}

	}
}