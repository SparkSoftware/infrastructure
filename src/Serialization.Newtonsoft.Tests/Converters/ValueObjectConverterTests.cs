﻿using System;
using System.Text.RegularExpressions;
using Spark;
using Spark.Serialization.Converters;
using Xunit;

/* Copyright (c) 2013 Spark Software Ltd.
 * 
 * This source is subject to the GNU Lesser General Public License.
 * See: http://www.gnu.org/copyleft/lesser.html
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE. 
 */

namespace Test.Spark.Serialization.Converters
{
    public static class UsingValueObjectConverter
    {
        public class WhenWritingJson : UsingJsonConverter
        {
            [Fact]
            public void CanSerializeNullValue()
            {
                var json = WriteJson(new ValueObjectConverter(), default(ValueObject));

                Validate("null", json);
            }

            [Fact]
            public void CanSerializeStringValueObjectToJson()
            {
                var value = new EmailAddress("CBaxter@sparksoftware.net");
                var json = WriteJson(new ValueObjectConverter(), value);

                Validate("\"cbaxter@sparksoftware.net\"", json);
            }

            [Fact]
            public void CanSerializePrimitiveValueObjectToJson()
            {
                var value = new TestId(123);
                var json = WriteJson(new ValueObjectConverter(), value);

                Validate("123", json);
            }
        }

        public class WhenReadingJson : UsingJsonConverter
        {
            [Fact]
            public void CanDeserializeNull()
            {
                Assert.Null(ReadJson<EmailAddress>(new ValueObjectConverter(), "null"));
            }

            [Fact]
            public void CanDeserializeValidStringValueObjectJson()
            {
                var json = "\"CBaxter@sparksoftware.net\"";
                var value = ReadJson<EmailAddress>(new ValueObjectConverter(), json);

                Assert.Equal(new EmailAddress("CBaxter@sparksoftware.net"), value);
            }

            [Fact]
            public void CanDeserializeValidPrimitiveValueObjectJson()
            {
                var json = "123";
                var value = ReadJson<TestId>(new ValueObjectConverter(), json);

                Assert.Equal(new TestId(123), value);
            }

            [Fact]
            public void CanThrowFormatExceptionIfValueInvalid()
            {
                Assert.Throws<FormatException>(() => ReadJson<EmailAddress>(new ValueObjectConverter { Strict = true }, "\"cbaxter\""));
            }

            [Fact]
            public void CanReturnNullIfValueInvalid()
            {
                Assert.Null(ReadJson<EmailAddress>(new ValueObjectConverter { Strict = false }, "\"cbaxter\""));
            }
        }

        public sealed class EmailAddress : ValueObject<String>
        {
            private static readonly Regex EmailPattern = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            public EmailAddress(String email)
                : base(email)
            { }

            protected override Boolean TryGetValue(String value, out String result)
            {
                var match = EmailPattern.Match((value ?? String.Empty).Trim().ToLowerInvariant());

                result = match.Success ? match.Value : null;

                return result != null;
            }
        }

        public sealed class TestId : ValueObject<Int32>
        {
            public TestId(Int32 id)
                : base(id)
            { }

            protected override Boolean TryParse(String value, out Int32 result)
            {
                return Int32.TryParse(value, out result);
            }
        }
    }
}
