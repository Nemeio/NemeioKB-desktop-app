using System;

namespace Nemeio.Core.Errors
{
    /// <summary>
    /// This class intendes to record when an error code has been used and what class "triggered" or recorded it
    /// The intent is to be able to track wher ethe error has been generated for debugging/support prupose
    /// </summary>
    public class ErrorTrace
    {
        public ErrorCode Error { get; }
        public DateTime TimeStamp { get; }
        public object Sender { get; }

        public ErrorTrace(ErrorCode errorCode, object sender)
        {
            TimeStamp = DateTime.Now;
            Error = errorCode;
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }
    }
}
