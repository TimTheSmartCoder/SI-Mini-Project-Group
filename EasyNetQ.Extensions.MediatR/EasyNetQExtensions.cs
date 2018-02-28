using System;
using System.Threading.Tasks;
using EasyNetQ.Consumer;
using EasyNetQ.FluentConfiguration;
using MediatR;

namespace EasyNetQ
{
    public static class EasyNetQExtensions
    {
        public static IDisposable Receive<TMessage>(
            this IBus bus,
            string queue,
            IMediator mediator)
            where TMessage : class
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));
            if (string.IsNullOrWhiteSpace(queue))
                throw new ArgumentNullException(nameof(queue));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            // Construct message handler which internally uses the
            // given mediator to route incoming messages to.
            var messageHandler = ConstructMessageHandler<TMessage>(mediator);

            // Attach messagehandler to receive.
            var disposable = bus.Receive(queue, messageHandler);

            return disposable;
        }

        public static ISubscriptionResult SubscribeAsync<TMessage>(
            this IBus bus,
            string subscriptionId,
            IMediator mediator)
            where TMessage : class
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new ArgumentNullException(nameof(subscriptionId));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            var handler = ConstructMessageHandler<TMessage>(mediator);

            var result = bus.SubscribeAsync(subscriptionId, handler);

            return result;
        }

        public static ISubscriptionResult SubscribeAsync<TMessage>(
            this IBus bus,
            string subscriptionId,
            IMediator mediator,
            Action<ISubscriptionConfiguration> configuration)
            where TMessage : class
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new ArgumentNullException(nameof(subscriptionId));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            var handler = ConstructMessageHandler<TMessage>(mediator);

            var result = bus.SubscribeAsync(
                subscriptionId,
                handler,
                configuration);

            return result;
        }

        public static IReceiveRegistration Add<TMessage>(
            this IReceiveRegistration receiveRegistration,
            IMediator mediator)
            where TMessage : class
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            var handler = ConstructMessageHandler<TMessage>(mediator);

            return receiveRegistration.Add<TMessage>(handler);
        }

        private static Func<TMessage, Task> ConstructMessageHandler<TMessage>(
            IMediator mediator)
            where TMessage : class
        {
            Task OnMessage<TMessage>(TMessage message)
                where TMessage : class
            {
                // Create message command for handling the given message
                // through the mediator, it will automaticly search for
                // an handler which takes the given MessageCommand with
                // the given message..
                var command = new RequestMessage<TMessage>(message);

                // Get the task to execute from the handler.
                var handler = mediator.Send(command);

                // Continue the task to supply error handling.
                var result = handler.ContinueWith((task) =>
                {
                    if (!task.IsCompleted || task.IsFaulted)
                        throw new Exception("Message failed to be proccessed.");
                });

                return result;
            }

            return OnMessage;
        }
    }
}
