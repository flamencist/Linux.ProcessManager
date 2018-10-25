using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Linux
{
    internal sealed class SpecificDelimiterTextReader
    {
        private readonly char _delimiter;
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly byte[] _bytes;

        public SpecificDelimiterTextReader(int bufferSize = 1024, char delimiter = '\0')
        {
            _delimiter = delimiter;
            _bytes = new byte[bufferSize];
        }

        public IEnumerable<string> ReadLines(Stream source)
        {
            int byteCount;

            while ((byteCount = source.Read(_bytes, 0, _bytes.Length)) != 0)
            {
                for (var i = 0; i < byteCount; i++)
                {
                    var ch = Convert.ToChar(_bytes[i]);
                    if (ch != _delimiter)
                    {
                        _builder.Append(ch);
                    }
                    else
                    {
                        yield return _builder.ToString();
                        _builder.Clear();
                    }
                }
            }

            if (_builder.Length > 0)
            {
                yield return _builder.ToString();
                _builder.Clear();
            }
        }
    }
}