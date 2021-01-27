using System;
using System.Collections.Generic;

namespace ParcelsLib
{
    public interface IDiscountRule
    {
        string DiscountName { get; set; }
        DiscountInfo GetDiscount(Receipt receipt);
    }

    public class DiscountInfo
    {
        public string DiscountName { get; set; }
        public List<Guid> ParcelIds { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}