﻿using System;

namespace Zaaby.DDD.Abstractions.Domain
{
    public interface IDomainEventHandler
    {
    }

    public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }

    public class DomainEventHandlerAttribute : Attribute
    {
        public string HandleName { get; }

        public DomainEventHandlerAttribute(string handleName)
        {
            if (string.IsNullOrWhiteSpace(handleName))
                throw new ArgumentNullException(nameof(handleName));
            HandleName = handleName.Trim();
        }
    }
}