using System;
using System.IO;
using System.Collections.Generic;

namespace Model
{
    public class OneWayReader : IDisposable
    {
        private readonly StreamReader _stream;
        private int _position;

        public OneWayReader(string path)
        {
            _stream = new StreamReader(path);
            _position = 0;
        }

        public string NextLine()
        {
            _position++;
            return _stream.ReadLine();
        }

        public string Line(int number)
        {
            while (_position < number)
                NextLine();

            return NextLine();
        }

        public string[] LineRange(int start, int end)
        {
            while (_position < start)
                NextLine();

            var range = new string[end - start];
            for (var i = 0; i < end - start; i++)
                range[i] = NextLine();

            return range;
        }

        public string[] ReadToEnd(int start)
        {
            while (_position < start)
                NextLine();

            var range = new List<string>();
            for (var line = NextLine(); !(line is null); line = NextLine())
                range.Add(line);

            return range.ToArray();
        }

        public void Dispose()
        {
            _stream.Close();
        }
    }
}