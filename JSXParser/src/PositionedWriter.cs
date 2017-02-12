using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    public class PositionedWriter : IDisposable
    {
        private int _breakPoint;
        private int _indentSpace;

        private StringWriter _writer = new StringWriter();
        private int _indent;
        private int _line;
        private int _column;
        public int Line
        {
            get;
        }

        public int Column
        {
            get;
        }

        public PositionedWriter() : this(70, 2)
        {
        }

        public PositionedWriter(int breakPoint, int indentSpace)
        {
            _breakPoint = breakPoint;
            _indentSpace = indentSpace;
        }

        public void Write(string value)
        {
            _column += value.Length;
            _writer.Write(value);
        }

        public void WriteLine(string value)
        {
            this.Write(value);
            this.NewLine();
        }

        public void NewLine()
        {
            _line++;
            _column = 0;
            _writer.WriteLine();
        }

        public bool ReachLineEnd()
        {
            return _column > _breakPoint;
        }
        
        public void Indent()
        {
            SpacePad(this, _indent);
        }

        public void IndentForward()
        {
            _indent += _indentSpace;
        }

        public void IndentBack()
        {
            _indent -= _indentSpace;
        }

        public void Dispose()
        {
            _writer.Dispose();
            this.Indent();
        }

        public override string ToString()
        {
            return _writer.ToString();
        }


        private static readonly string[] SPACES = {
            " ", "  ", "    ", "        ",			// 1,2,4,8 spaces
		    "                ",						// 16 spaces
			"                                "      // 32 spaces
        };	

        protected static void SpacePad(PositionedWriter writer, int length)
        {
            while (length >= 32)
            {
                writer.Write(SPACES[5]);
                length -= 32;
            }

            for (int i = 4; i >= 0; i--)
            {
                if ((length & (1 << i)) != 0)
                {
                    writer.Write(SPACES[i]);
                }
            }
        }
    }
}
