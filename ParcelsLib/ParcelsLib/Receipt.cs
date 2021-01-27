using System;
using System.Collections.Generic;
using System.Linq;

namespace ParcelsLib
{
    public class Receipt
    {
        public class ParcelSubtotalItem
        {
            public Parcel Item { get; set; }
            public string PricingCategory { get; set; }
            public decimal Price { get; set; }
        }

        public List<ParcelSubtotalItem> Parcels { get; set; } = new List<ParcelSubtotalItem>();
        public IEnumerable<ParcelSubtotalItem> ParcelsNonDiscounted => Parcels.Where(x => !IsUsedInDiscounts(x.Item.Id));
        public List<DiscountInfo> Discounts { get; set; } = new List<DiscountInfo>();
        public decimal Subtotal { get; set; } = 0;
        public decimal Shipping { get; set; }
        public decimal TotalDiscounts => Discounts.Select(x => x.DiscountAmount).Sum();
        public decimal Total => Subtotal + Shipping - TotalDiscounts;
        public Guid Id { get; set; } = Guid.NewGuid();

        public void AddParcel(Parcel p, string category, decimal price)
        {
            this.Parcels.Add(new ParcelSubtotalItem()
            {
                Item = p,
                PricingCategory = category,
                Price = price
            });

            this.Subtotal += price;
        }

        public bool IsUsedInDiscounts(Guid parcelId)
        {
            return Discounts.Any(x => x.ParcelIds.Contains(parcelId));
        }
    }
}
