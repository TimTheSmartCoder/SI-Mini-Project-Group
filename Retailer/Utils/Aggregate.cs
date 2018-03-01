using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retailer.Client.Utils
{
    public class Aggregate<TRequest, TResponse>
    {
        private readonly IList<TResponse> responses;
        private readonly int numberOfResponsesForComplete;

        public Aggregate(string correlationId, int numberOfResponsesForComplete, TRequest request)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
                throw new ArgumentNullException(nameof(correlationId));

            this.CorrelationId = correlationId;
            this.numberOfResponsesForComplete = numberOfResponsesForComplete;

            this.responses = new List<TResponse>();
        }

        public string CorrelationId { get;  }

        public TRequest Request { get; }

        public void Add(TResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            this.responses.Add(response);
        }

        public bool IsComplete()
        {
            return this.responses.Count >= numberOfResponsesForComplete;
        }

        public TResponse GetResult(Func<TResponse, bool> condition)
        {
            return this.responses.FirstOrDefault(condition);
        }
    }
}
