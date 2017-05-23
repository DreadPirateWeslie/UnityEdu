using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input_Interpreter
{
    public class Visualization
    {
        private String FunctionAsString { get; set; }

        public Visualization(Func<double, double, double> function)
        {
        }

        public Visualization()
        {
        }


        public double getValue(double x, double y)
        {
            return x + y * 2;
        }

    }
}
