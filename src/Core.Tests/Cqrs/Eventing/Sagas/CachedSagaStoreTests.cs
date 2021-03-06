﻿using System;
using System.Runtime.Caching;
using Moq;
using Spark;
using Spark.Cqrs.Eventing;
using Spark.Cqrs.Eventing.Sagas;
using Spark.Data;
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

namespace Test.Spark.Cqrs.Eventing.Sagas
{
    namespace UsingCachedSagaStore
    {
        public class WhenCreatingSaga
        {
            [Fact]
            public void DelegateToDecoratedSagaStore()
            {
                var sagaStore = new Mock<IStoreSagas>();
                var sagaId = GuidStrategy.NewGuid();

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    cachedSagaStore.CreateSaga(typeof(Saga), sagaId);

                    sagaStore.Verify(mock => mock.CreateSaga(typeof(Saga), sagaId), Times.Once());
                }
            }
        }

        public class WhenTryingToRetrieveCachedSaga
        {
            [Fact]
            public void CacheAndReuseExistingSaga()
            {
                var sagaStore = new Mock<IStoreSagas>();
                var sagaId = GuidStrategy.NewGuid();

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    Saga cachedSaga = new FakeSaga();

                    sagaStore.Setup(mock => mock.TryGetSaga(typeof(Saga), sagaId, out cachedSaga)).Returns(true);

                    Assert.True(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out cachedSaga));
                    Assert.True(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out cachedSaga));

                    sagaStore.Verify(mock => mock.TryGetSaga(typeof(Saga), sagaId, out cachedSaga), Times.Once());
                }
            }

            [Fact]
            public void NullSagaNotCached()
            {
                var sagaStore = new Mock<IStoreSagas>();
                var sagaId = GuidStrategy.NewGuid();

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    Saga cachedSaga = default(Saga);

                    sagaStore.Setup(mock => mock.TryGetSaga(typeof(Saga), sagaId, out cachedSaga)).Returns(false);

                    Assert.False(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out cachedSaga));
                    Assert.False(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out cachedSaga));

                    sagaStore.Verify(mock => mock.TryGetSaga(typeof(Saga), sagaId, out cachedSaga), Times.Exactly(2));
                }
            }

            [Fact]
            public void AlwaysReturnCopyOfCachedSaga()
            {
                var sagaStore = new Mock<IStoreSagas>();
                var sagaId = GuidStrategy.NewGuid();

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    Saga cachedSaga = new FakeSaga(), result1, result2;
                    
                    sagaStore.Setup(mock => mock.TryGetSaga(typeof(Saga), sagaId, out cachedSaga)).Returns(true);

                    Assert.True(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out result1));
                    Assert.True(cachedSagaStore.TryGetSaga(typeof(Saga), sagaId, out result2));
                    Assert.NotSame(result1, result2);
                }
            }

            private class FakeSaga : Saga
            {
                protected override void Configure(SagaConfiguration saga)
                { }
            }
        }

        public class WhenGettingScheduledTimeouts
        {
            [Fact]
            public void DelegateToDecoratedSagaStore()
            {
                var sagaStore = new Mock<IStoreSagas>();
                var upperBound = DateTime.Now;

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    cachedSagaStore.GetScheduledTimeouts(upperBound);

                    sagaStore.Verify(mock => mock.GetScheduledTimeouts(upperBound), Times.Once());
                }
            }
        }

        // ReSharper disable AccessToDisposedClosure
        public class WhenSavingSaga
        {
            [Fact]
            public void SagaUpdatedAfterSaveIfNotCompleted()
            {
                var saga = new FakeSaga { CorrelationId = GuidStrategy.NewGuid() };
                var memoryCache = new MemoryCache(Guid.NewGuid().ToString());
                var sagaStore = new Mock<IStoreSagas>();

                using (var sagaContext = new SagaContext(typeof(FakeSaga), saga.CorrelationId, new FakeEvent()))
                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object, TimeSpan.FromMinutes(1), memoryCache))
                {
                    cachedSagaStore.Save(saga, sagaContext);

                    Assert.Same(saga, memoryCache.Get(typeof(FakeSaga).GetFullNameWithAssembly() + '-' + saga.CorrelationId));
                }
            }

            [Fact]
            public void SagaRemovedFromCacheIfCompleted()
            {
                var saga = new FakeSaga { CorrelationId = GuidStrategy.NewGuid() };
                var sagaStore = new Mock<IStoreSagas>();
                var cachedSaga = default(Saga);

                using (var sagaContext = new SagaContext(typeof(FakeSaga), saga.CorrelationId, new FakeEvent()))
                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    cachedSagaStore.Save(saga, sagaContext);

                    saga.Completed = true;

                    cachedSagaStore.Save(saga, sagaContext);

                    Assert.False(cachedSagaStore.TryGetSaga(typeof(FakeSaga), saga.CorrelationId, out cachedSaga));
                }
            }

            [Fact]
            public void SagaRemovedFromCacheIfConcurrencyExceptionThrown()
            {
                var saga = new FakeSaga { CorrelationId = GuidStrategy.NewGuid() };
                var sagaStore = new Mock<IStoreSagas>();
                var cachedSaga = default(Saga);
                var save = 0;

                using (var sagaContext = new SagaContext(typeof(FakeSaga), saga.CorrelationId, new FakeEvent()))
                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    sagaStore.Setup(mock => mock.Save(It.IsAny<Saga>(), sagaContext)).Callback(() => { if (save++ == 1) { throw new ConcurrencyException(); } });
                    cachedSagaStore.Save(saga, sagaContext);

                    Assert.True(cachedSagaStore.TryGetSaga(typeof(FakeSaga), saga.CorrelationId, out cachedSaga));
                    Assert.Throws<ConcurrencyException>(() => cachedSagaStore.Save(saga, sagaContext));
                    Assert.False(cachedSagaStore.TryGetSaga(typeof(FakeSaga), saga.CorrelationId, out cachedSaga));
                }
            }

            private class FakeSaga : Saga
            {
                protected override void Configure(SagaConfiguration saga)
                { }
            }

            private class FakeEvent : Event
            { }
        }
        // ReSharper restore AccessToDisposedClosure

        public class WhenPurgingSagas
        {
            [Fact]
            public void DelegateToDecoratedSagaStore()
            {
                var sagaStore = new Mock<IStoreSagas>();

                using (var cachedSagaStore = new CachedSagaStore(sagaStore.Object))
                {
                    cachedSagaStore.Purge();

                    sagaStore.Verify(mock => mock.Purge(), Times.Once());
                }
            }
        }
    }
}
