using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gvss_project
{
    public static class Computations
    {
        public static string getCapLetAndIndex(char capLetter, int index)
        {
            if (Char.IsLetter(capLetter))
            {
                capLetter = Char.ToUpper(capLetter);
                string str = capLetter.ToString();
                foreach (char c in index.ToString())
                {
                    str += ((char)(0x2080 + Int32.Parse(c.ToString()))).ToString();
                }
                return str;
            }
            else
            {
                throw new InvalidArgument();
            }

        }

        public static string getUniqueCode(char capLetter, uint index, SensorArrowSign arrowSign)
        {
            int u = 0x02b9b + (int)arrowSign;
            string str = capLetter.ToString();
            foreach (char c in index.ToString())
            {
                str += ((char)(0x2080 + Int32.Parse(c.ToString()))).ToString();
            }
            str += ((char)u).ToString();
            return str;
        }
    }
}
