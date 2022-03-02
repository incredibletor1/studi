using System;
using System.Collections.Generic;

namespace Studi.Proctoring.BackOffice_Api.Exceptions
{
    public class CannotBeArchivedAggregateException : AggregateException
    {
        public CannotBeArchivedAggregateException(IEnumerable<Exception> exceptions) : base(innerExceptions: exceptions)
        { 
        }
    }
    
    public class CannotBeArchivedException : Exception
    {
        public CannotBeArchivedException(string message) : base(message)
        { 
        }
    }
}
