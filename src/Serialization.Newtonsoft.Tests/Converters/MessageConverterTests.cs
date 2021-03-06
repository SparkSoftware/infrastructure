﻿using System;
using Spark.Cqrs.Commanding;
using Spark.Messaging;
using Xunit;

/* Copyright (c) 2015 Spark Software Ltd.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Test.Spark.Serialization.Converters
{
    namespace UsingMessageConverter
    {
        public class WhenWritingJson : UsingJsonConverter
        {
            [Fact]
            public void CanSerializeNullValue()
            {
                var json = WriteJson(default(Message<CommandEnvelope>));

                Validate(json, "null");
            }

            [Fact]
            public void CanSerializeToJson()
            {
                var id = Guid.Parse("cf72612c-8e46-4311-b40f-6c9279bce591");
                var aggregateId = Guid.Parse("092df347-8562-4b59-99a3-57e1e0d6fbc4");
                var message = new Message<CommandEnvelope>(id, HeaderCollection.Empty, new CommandEnvelope(aggregateId, new FakeCommand("My Command")));
                var json = WriteJson(message);

                Validate(json, @"
{
  ""id"": ""cf72612c-8e46-4311-b40f-6c9279bce591"",
  ""h"": {},
  ""p"": {
    ""a"": ""092df347-8562-4b59-99a3-57e1e0d6fbc4"",
    ""c"": {
      ""$type"": ""Test.Spark.Serialization.Converters.UsingMessageConverter.FakeCommand, Spark.Serialization.Newtonsoft.Tests"",
      ""Property"": ""My Command""
    }
  }
}");
            }
        }

        public class WhenReadingJson : UsingJsonConverter
        {
            [Fact]
            public void CanDeserializeNull()
            {
                Assert.Null(ReadJson<Message<CommandEnvelope>>("null"));
            }

            [Fact]
            public void CanDeserializeValidJson()
            {
                var message = ReadJson<Message<CommandEnvelope>>(@"
{
  ""id"": ""cf72612c-8e46-4311-b40f-6c9279bce591"",
  ""h"": {},
  ""p"": {
    ""a"": ""092df347-8562-4b59-99a3-57e1e0d6fbc4"",
    ""c"": {
      ""$type"": ""Test.Spark.Serialization.Converters.UsingMessageConverter.FakeCommand, Spark.Serialization.Newtonsoft.Tests"",
      ""Property"": ""My Command""
    }
  }
}");

                Assert.Equal(Guid.Parse("cf72612c-8e46-4311-b40f-6c9279bce591"), message.Id);
            }

            [Fact]
            public void PropertyOrderIrrelevant()
            {
                var message = ReadJson<Message<CommandEnvelope>>(@"
{
  ""h"": {},
  ""id"": ""cf72612c-8e46-4311-b40f-6c9279bce591"",
  ""p"": {
    ""a"": ""092df347-8562-4b59-99a3-57e1e0d6fbc4"",
    ""c"": {
      ""$type"": ""Test.Spark.Serialization.Converters.UsingMessageConverter.FakeCommand, Spark.Serialization.Newtonsoft.Tests"",
      ""Property"": ""My Command""
    }
  }
}");
                Assert.Equal(Guid.Parse("cf72612c-8e46-4311-b40f-6c9279bce591"), message.Id);
            }

            [Fact]
            public void CanTolerateMalformedJson()
            {
                var message = ReadJson<Message<CommandEnvelope>>(@"
{
  ""h"": {},
  ""id"": ""cf72612c-8e46-4311-b40f-6c9279bce591"",
  ""p"": {
    ""a"": ""092df347-8562-4b59-99a3-57e1e0d6fbc4"",
    ""c"": {
      ""$type"": ""Test.Spark.Serialization.Converters.UsingMessageConverter.FakeCommand, Spark.Serialization.Newtonsoft.Tests"",
      ""Property"": ""My Command""
    },
  },
}");
                Assert.Equal(Guid.Parse("cf72612c-8e46-4311-b40f-6c9279bce591"), message.Id);
            }
        }

        public class WhenWritingBson : UsingJsonConverter
        {
            [Fact]
            public void CanSerializeToBson()
            {
                var id = Guid.Parse("A96662A6-ABA8-4CF9-9882-86E7F52BF18C");
                var aggregateId = Guid.Parse("61296B2F-F040-472D-95DF-B1C3A32A7C7E");
                var message = new Message<CommandEnvelope>(id, HeaderCollection.Empty, new CommandEnvelope(aggregateId, new FakeCommand("My Command")));
                var bson = WriteBson(message);

                Validate(bson, "﻿3gAAAAVpZAAQAAAABKZiZqmoq/lMmIKG5/Ur8YwDaAAFAAAAAANwALUAAAAFYQAQAAAABC9rKWFA8C1Hld+xw6MqfH4DYwCVAAAAAiR0eXBlAGwAAABUZXN0LlNwYXJrLlNlcmlhbGl6YXRpb24uQ29udmVydGVycy5Vc2luZ01lc3NhZ2VDb252ZXJ0ZXIuRmFrZUNvbW1hbmQsIFNwYXJrLlNlcmlhbGl6YXRpb24uTmV3dG9uc29mdC5UZXN0cwACUHJvcGVydHkACwAAAE15IENvbW1hbmQAAAAA");
            }
        }

        public class WhenReadingBson : UsingJsonConverter
        {
            [Fact]
            public void CanDeserializeValidBson()
            {
                var bson = "3gAAAAVpZAAQAAAABKZiZqmoq/lMmIKG5/Ur8YwDaAAFAAAAAANwALUAAAAFYQAQAAAABC9rKWFA8C1Hld+xw6MqfH4DYwCVAAAAAiR0eXBlAGwAAABUZXN0LlNwYXJrLlNlcmlhbGl6YXRpb24uQ29udmVydGVycy5Vc2luZ01lc3NhZ2VDb252ZXJ0ZXIuRmFrZUNvbW1hbmQsIFNwYXJrLlNlcmlhbGl6YXRpb24uTmV3dG9uc29mdC5UZXN0cwACUHJvcGVydHkACwAAAE15IENvbW1hbmQAAAAA";
                var message = ReadBson<Message<CommandEnvelope>>(bson);

                Assert.Equal(Guid.Parse("A96662A6-ABA8-4CF9-9882-86E7F52BF18C"), message.Id);
            }
        }

        internal class FakeCommand : Command
        {
            public String Property { get; private set; }

            public FakeCommand(String property)
            {
                Property = property;
            }
        }
    }
}
