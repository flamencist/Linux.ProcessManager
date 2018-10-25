using System.IO;
using System.Linq;
using System.Text;
using Linux;
using Xunit;

namespace ProcessManager.Tests
{
    public class SpecificDelimiterTextReaderTests
    {
        [Fact]
        public void SpecificDelimiterTextReader_ReadLines_Returns_List_Of_String()
        {
            var data = "line1\0line2\0line3";
            var buffer = Encoding.UTF8.GetBytes(data);
            using (var memoryStream = new MemoryStream(buffer))
            {
                var actual = new SpecificDelimiterTextReader().ReadLines(memoryStream).ToList();
                Assert.Equal(3,actual.Count);
                Assert.Equal("line1",actual[0]);
                Assert.Equal("line2",actual[1]);
                Assert.Equal("line3",actual[2]);
            }
        }
        
        [Fact]
        public void SpecificDelimiterTextReader_ReadLines_Split_By_Specified_Character()
        {
            var data = "line1\nline2\nline3";
            var buffer = Encoding.UTF8.GetBytes(data);
            using (var memoryStream = new MemoryStream(buffer))
            {
                var actual = new SpecificDelimiterTextReader(1024,'\n').ReadLines(memoryStream).ToList();
                Assert.Equal(3,actual.Count);
                Assert.Equal("line1",actual[0]);
                Assert.Equal("line2",actual[1]);
                Assert.Equal("line3",actual[2]);
            }
        }
        
        [Fact]
        public void SpecificDelimiterTextReader_ReadLines_Empty_If_Lines_Not_Found()
        {
            var data = string.Empty;
            var buffer = Encoding.UTF8.GetBytes(data);
            using (var memoryStream = new MemoryStream(buffer))
            {
                var actual = new SpecificDelimiterTextReader().ReadLines(memoryStream).ToList();
                Assert.Empty(actual);
            }
        }
    }
}