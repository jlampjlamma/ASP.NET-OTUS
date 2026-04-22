using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PartnerPromoCodeLimit
    {
        public Guid Id { get; set; }

        public Guid PartnerId { get; set; }

        public virtual Partner Partner { get; set; }
        
        public DateTime CreateDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public DateTime EndDate { get; set; }

        [NotMapped]
        public bool IsActive => !CancelDate.HasValue && EndDate > DateTime.Now;

        public int Limit { get; set; }
    }
}