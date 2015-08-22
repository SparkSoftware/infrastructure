﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Spark.Cqrs.Eventing;
using Spark.Cqrs.Eventing.Mappings;
using Spark.Cqrs.Eventing.Sagas;
using Spark.Cqrs.Eventing.Sagas.Sql;
using Spark.Cqrs.Eventing.Sagas.Sql.Dialects;
using Spark.Example.Benchmarks;
using Spark.Messaging;
using Module = Autofac.Module;

namespace Spark.Example.Modules
{
    public sealed class EventingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register underlying eventing infrastructure.
            builder.RegisterType<BlockingCollectionMessageBus<EventEnvelope>>().AsSelf().As<ISendMessages<EventEnvelope>>().As<IReceiveMessages<EventEnvelope>>().SingleInstance();
            builder.RegisterType<MessageReceiver<EventEnvelope>>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<EventHandlerRegistry>().As<IRetrieveEventHandlers>().SingleInstance();
            builder.RegisterType<TimeoutDispatcher>().As<PipelineHook>().SingleInstance();
            builder.RegisterType<EventPublisher>().Named<IPublishEvents>("EventPublisher").SingleInstance();
            builder.RegisterType<EventProcessor>().Named<IProcessMessages<EventEnvelope>>("EventProcessor").SingleInstance();

            // Register all event handlers.
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).Where(type => type.GetCustomAttribute<EventHandlerAttribute>() != null);

            // Register data store infrastructure.
            builder.RegisterType<SqlSagaStoreDialect>().AsSelf().As<ISagaStoreDialect>().SingleInstance();
            builder.RegisterType<SqlSagaStore>().AsSelf().Named<IStoreSagas>("SagaStore").SingleInstance();

            // Register decorators.
            builder.RegisterDecorator<IStoreSagas>((context, sagaStore) => new BenchmarkedSagaStore(sagaStore, context.Resolve<Statistics>()), "SagaStore").Named<IStoreSagas>("BenchmarkedSagaStore").SingleInstance();
            builder.RegisterDecorator<IStoreSagas>((context, sagaStore) => new CachedSagaStore(sagaStore), "BenchmarkedSagaStore").Named<IStoreSagas>("CachedSagaStore").SingleInstance();
            builder.RegisterDecorator<IStoreSagas>((context, sagaStore) => new HookableSagaStore(sagaStore, context.Resolve<IEnumerable<PipelineHook>>()), "CachedSagaStore").As<IStoreSagas>().SingleInstance();
            builder.RegisterDecorator<IPublishEvents>((context, eventPublisher) => new EventPublisherWrapper(eventPublisher, context.Resolve<Statistics>()), "EventPublisher").As<IPublishEvents>().SingleInstance();
            builder.RegisterDecorator<IProcessMessages<EventEnvelope>>((context, eventProcessor) => new EventProcessorWrapper(eventProcessor, context.Resolve<Statistics>()), "EventProcessor").As<IProcessMessages<EventEnvelope>>().SingleInstance();
        }
    }
}
