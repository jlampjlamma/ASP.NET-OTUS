using System;

namespace PromoCodeFactory.Core.Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}