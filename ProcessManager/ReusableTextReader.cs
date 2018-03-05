using System.IO;
using System.Text;

namespace Linux
{
    internal sealed class ReusableTextReader
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly byte[] _bytes;
        private readonly char[] _chars;
        private readonly Decoder _decoder;

        public ReusableTextReader(Encoding encoding = null, int bufferSize = 1024)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            _decoder = encoding.GetDecoder();
            _bytes = new byte[bufferSize];
            _chars = new char[encoding.GetMaxCharCount(_bytes.Length)];
        }

        public string ReadAllText(Stream source)
        {
            int byteCount;
            while ((byteCount = source.Read(_bytes, 0, _bytes.Length)) != 0)
                _builder.Append(_chars, 0, _decoder.GetChars(_bytes, 0, byteCount, _chars, 0));
            var str = _builder.ToString();
            _builder.Clear();
            _decoder.Reset();
            return str;
        }
    }
}