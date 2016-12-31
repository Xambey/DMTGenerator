using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTGenerator
{
    public class Generator
    {
        public Generator(Method method, int count)
        {
            switch (method)
            {
                case Method.SimplexTable:
                    break;
                case Method.Graphic:
                    break;
                default:
                    break;
            }
        }
    }
}
