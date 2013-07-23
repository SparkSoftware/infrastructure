﻿using System;
using Moq;
using Spark.Infrastructure.Commanding;
using Spark.Infrastructure.Configuration;
using Spark.Infrastructure.Domain;
using Spark.Infrastructure.EventStore;
using Spark.Infrastructure.Messaging;
using Xunit;

/* Copyright (c) 2012 Spark Software Ltd.
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

namespace Spark.Infrastructure.Tests.Commanding
{
    public static class UsingCommandProcessor
    {
        public abstract class UsingCommandProcessorBase
        {
            protected readonly Mock<IProcessCommandSettings>  Settings = new Mock<IProcessCommandSettings>();
            protected readonly Mock<IRetrieveCommandHandlers> HandlerRegistry = new Mock<IRetrieveCommandHandlers>();
            protected readonly Mock<IStoreAggregates> AggregateStore = new Mock<IStoreAggregates>();
            protected readonly CommandProcessor Processor;

            protected UsingCommandProcessorBase()
            {
                Settings.Setup(mock => mock.BoundedCapacity).Returns(100);
                Settings.Setup(mock => mock.MaximumConcurrencyLevel).Returns(10);
                Settings.Setup(mock => mock.RetryTimeout).Returns(TimeSpan.FromSeconds(10));

                Processor = new CommandProcessor(HandlerRegistry.Object, AggregateStore.Object, Settings.Object);
            }
        }

        public class WhenCreatingNewProcessor
        {
            [Fact]
            public void CommandHandlerRegistryCannotBeNull()
            {
                var aggregateStore = new Mock<IStoreAggregates>();
                var ex = Assert.Throws<ArgumentNullException>(() => new CommandProcessor(null, aggregateStore.Object));

                Assert.Equal("commandHandlerRegistry", ex.ParamName);
            }

            [Fact]
            public void AggregateStoreFactoryCannotBeNull()
            {
                var handlerRegistry = new Mock<IRetrieveCommandHandlers>();
                var ex = Assert.Throws<ArgumentNullException>(() => new CommandProcessor(handlerRegistry.Object, null));

                Assert.Equal("aggregateStore", ex.ParamName);
            }
        }

        public class WhenProcessingWithoutExceptions : UsingCommandProcessorBase
        {
            [Fact]
            public void SaveAggregateAfterCommandHandled()
            {
                var command = new FakeCommand();
                var aggregate = new FakeAggregate();
                var envelope = new CommandEnvelope(GuidStrategy.NewGuid(), command);
                var message = Message.Create(GuidStrategy.NewGuid(), HeaderCollection.Empty, envelope);

                HandlerRegistry.Setup(mock => mock.GetHandlerFor(command)).Returns(new CommandHandler(typeof(FakeAggregate), (a, c) => { }));
                AggregateStore.Setup(mock => mock.Get(typeof(FakeAggregate), envelope.AggregateId)).Returns(aggregate);

                Processor.Process(message);

                AggregateStore.Verify(mock => mock.Save(aggregate, It.IsAny<CommandContext>()), Times.Once());
            }
        }

        public class WhenProcessingWithConcurrencyException : UsingCommandProcessorBase
        {
            [Fact]
            public void ReloadAggregateWithLatestState()
            {
                var save = 0;
                var command = new FakeCommand();
                var aggregate = new FakeAggregate();
                var envelope = new CommandEnvelope(GuidStrategy.NewGuid(), command);
                var message = Message.Create(GuidStrategy.NewGuid(), HeaderCollection.Empty, envelope);
                var ex = new ConcurrencyException();

                HandlerRegistry.Setup(mock => mock.GetHandlerFor(command)).Returns(new CommandHandler(typeof(FakeAggregate), (a, c) => { }));
                AggregateStore.Setup(mock => mock.Get(typeof(FakeAggregate), envelope.AggregateId)).Returns(aggregate);
                AggregateStore.Setup(mock => mock.Save(aggregate, It.IsAny<CommandContext>())).Callback(() =>
                    {
                        if (++save == 1)
                            throw ex;
                    });

                Processor.Process(message);

                AggregateStore.Verify(mock => mock.Get(typeof(FakeAggregate), envelope.AggregateId), Times.Exactly(2));
            }

            [Fact]
            public void WillTimeoutEventuallyIfCannotSave()
            {
                Settings.Setup(mock => mock.RetryTimeout).Returns(TimeSpan.FromMilliseconds(20));

                var command = new FakeCommand();
                var aggregate = new FakeAggregate();
                var envelope = new CommandEnvelope(GuidStrategy.NewGuid(), command);
                var message = Message.Create(GuidStrategy.NewGuid(), HeaderCollection.Empty, envelope);
                var processor = new CommandProcessor(HandlerRegistry.Object, AggregateStore.Object, Settings.Object);

                SystemTime.ClearOverride();

                HandlerRegistry.Setup(mock => mock.GetHandlerFor(command)).Returns(new CommandHandler(typeof(FakeAggregate), (a, c) => { }));
                AggregateStore.Setup(mock => mock.Get(typeof(FakeAggregate), envelope.AggregateId)).Returns(aggregate);
                AggregateStore.Setup(mock => mock.Save(aggregate, It.IsAny<CommandContext>())).Callback(() => { throw new ConcurrencyException(); });

                Assert.Throws<TimeoutException>(() => processor.Process(message));
            }
        }

        private class FakeAggregate : Aggregate
        {
            public void Handle(FakeCommand command)
            { }
        }

        private class FakeCommand : Command
        { }
    }
}
