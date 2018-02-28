using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace EasyNetQ
{
    public class RequestMessage<TMessage>
        : IRequest where TMessage : class
    {
        public RequestMessage(TMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            this.Message = message;
        }

        public TMessage Message { get; }
    }
}
