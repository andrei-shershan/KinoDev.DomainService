﻿using KinoDev.Shared.Enums;

namespace KinoDev.DomainService.Domain.DomainsModels
{
    public class Order : BaseGuidEntity
    {
        public decimal Cost { get; set; }

        public OrderState State { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
    }
}
