using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Interpreter
{   
    class MultiVariableFunction
    {
        List<Func<double>> function = new List<Func<double>>();
        Dictionary<string, Func<double>> basicFunctions = new Dictionary<string, Func<double>>();

        private String expression;
        private static Func<double, double, double> function1 = (x,y) => x+y;

        public MultiVariableFunction(String functionAsString)
        {
            expression = functionAsString;
        }

        public double calculate(double x, double y){
            String toCalculate = expression;
            while(toCalculate.Contains("x")){
                toCalculate.Replace("x", "(" + x + ")");
            }
            while(toCalculate.Contains("y")){
                toCalculate.Replace("y", "(" + y+")");
            }
            return Double.Parse(new Calc().calculate(toCalculate));
        }



    }
}
