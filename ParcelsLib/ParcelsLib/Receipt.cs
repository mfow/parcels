﻿using System;
using System.Collections.Generic;

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
        public decimal Subtotal { get; set; } = 0;
        public decimal Shipping { get; set; }
        public decimal Total => Subtotal + Shipping;
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
    }
}
