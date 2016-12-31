using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTGenerator
{
    public class Generator
    {
        private List<double[,]> results;

        public Generator(Method method, int count)
        {
            switch (method)
            {
                case Method.SimplexTable:
                    while (count == 0)
                    {
                        if (getTicket())
                            --count; 
                    }
                    break;
                case Method.Graphic:
                    break;
                default:
                    break;
            }
        }
        private bool getTicket()
        {
            
            return true;
        }
    }
}
