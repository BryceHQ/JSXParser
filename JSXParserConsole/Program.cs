using JSXParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var data = Input();
                var parser = new Parser(data);
                var program = parser.Parse();
                Output(program.Format(true));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        static string Input()
        {
            using (var sr = new StreamReader("input.js"))
            {
                return sr.ReadToEnd();
            }
        }

        static void Output(string data)
        {
            using (var sw = new StreamWriter("output.js"))
            {
                sw.Write(data);
            }
        }
    }
}
