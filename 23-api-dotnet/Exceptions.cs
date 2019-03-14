using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual.Exceptions
{
    public class InvalidCredentials : ApplicationException
    {
        public InvalidCredentials() : base() { }
        public InvalidCredentials(string message) : base(message) { }
        public InvalidCredentials(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidCredentials(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class PermissionDenied : ApplicationException
    {
        public PermissionDenied() : base() { }
        public PermissionDenied(string message) : base(message) { }
        public PermissionDenied(string message, System.Exception inner) : base(message, inner) { }
        protected PermissionDenied(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class MalformedRequest : ApplicationException
    {
        public MalformedRequest() : base() { }
        public MalformedRequest(string message) : base(message) { }
        public MalformedRequest(string message, System.Exception inner) : base(message, inner) { }
        protected MalformedRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class FailedRequest : ApplicationException
    {
        public FailedRequest() : base() { }
        public FailedRequest(string message) : base(message) { }
        public FailedRequest(string message, System.Exception inner) : base(message, inner) { }
        protected FailedRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
