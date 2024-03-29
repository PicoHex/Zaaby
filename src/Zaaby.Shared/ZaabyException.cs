﻿namespace Zaaby.Shared;

public class ZaabyException : Exception
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public DateTimeOffset ThrowTime { get; set; } = DateTimeOffset.Now;

    public ZaabyException()
    {
    }

    public ZaabyException(string message) : base(message) =>
        Message = message;

    public ZaabyException(string message, string stackTrace = null) : base(message) =>
        StackTrace = stackTrace;

    public ZaabyException(string message, Exception inner, string stackTrace = null) : base(message, inner) =>
        StackTrace = stackTrace;

    protected ZaabyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <inheritdoc />
    /// <summary>Gets a message that describes the current exception.</summary>
    /// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
    public override string Message { get; }

    /// <inheritdoc />
    /// <summary>Gets or sets the name of the application or the object that causes the error.</summary>
    /// <returns>The name of the application or the object that causes the error.</returns>
    /// <exception cref="T:System.ArgumentException">The object must be a runtime <see cref="N:System.Reflection"></see> object</exception>
    public override string Source { get; set; }

    /// <inheritdoc />
    /// <summary>Gets a string representation of the immediate frames on the call stack.</summary>
    /// <returns>A string that describes the immediate frames of the call stack.</returns>
    public override string StackTrace { get; }
}