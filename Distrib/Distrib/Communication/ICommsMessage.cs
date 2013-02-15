using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    public interface ICommsMessage
    {
        CommsMessageType Type { get; }
    }

    [Serializable()]
    public abstract class CommsMessageBase : ICommsMessage
    {
        private readonly CommsMessageType _type;

        public CommsMessageBase(CommsMessageType type)
        {
            _type = type;
        }

        public CommsMessageType Type
        {
            get { return _type; }
        }
    }

    public interface IMethodInvokeCommsMessage : ICommsMessage
    {
        string MethodName { get; }
        object[] InvokeArgs { get; }
    }

    public interface IMethodInvokeResultCommsMessage : ICommsMessage
    {
        IMethodInvokeCommsMessage InvokeMessage { get; }
        object ReturnValue { get; }
    }

    [Serializable()]
    public sealed class MethodInvokeResultCommsMessage : CommsMessageBase, IMethodInvokeResultCommsMessage
    {
        private IMethodInvokeCommsMessage _invokeMessage;
        private object _result;

        public MethodInvokeResultCommsMessage(IMethodInvokeCommsMessage invokeMessage, object result)
            : base(CommsMessageType.MethodInvokeResult)
        {
            _invokeMessage = invokeMessage;
            _result = result;
        }

        public object ReturnValue
        {
            get { return _result; }
        }

        public IMethodInvokeCommsMessage InvokeMessage
        {
            get { return _invokeMessage; }
        }
    }


    public interface IExceptionCommsMessage : ICommsMessage
    {
        ICommsMessage CausingMessage { get; }
        string ExceptionMessage { get; }
    }

    [Serializable()]
    public sealed class ExceptionCommsMessage : CommsMessageBase, IExceptionCommsMessage
    {
        public ExceptionCommsMessage(ICommsMessage causingMessage, string exceptionMessage)
            : base(CommsMessageType.Exception)
        {
            this.CausingMessage = causingMessage;
            this.ExceptionMessage = exceptionMessage;
        }

        public ICommsMessage CausingMessage
        {
            get;
            set;
        }

        public string ExceptionMessage
        {
            get;
            set;
        }
    }


    [Serializable()]
    public sealed class MethodInvokeCommsMessage : CommsMessageBase, IMethodInvokeCommsMessage
    {
        private readonly string _methodName;
        private readonly object[] _invokeArgs;

        public MethodInvokeCommsMessage(string methodName, object[] invokeArgs)
            : base(CommsMessageType.MethodInvoke)
        {
            _methodName = methodName;
            _invokeArgs = invokeArgs;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public object[] InvokeArgs
        {
            get { return _invokeArgs; }
        }
    }

    public enum CommsMessageType
    {
        MethodInvoke,
        MethodInvokeResult,
        PropertyGet,
        PropertyGetResult,
        PropertySet,
        PropertySetResult,
        Exception,
    }
}
