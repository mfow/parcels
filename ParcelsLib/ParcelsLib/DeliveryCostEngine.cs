﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ParcelsLib
{
    public class DeliveryCostEngine
    {
        private class DeliveryCategory
        {
            public double DimensionMax { get; set; }
            public decimal Price { get; set; }
            public string Name { get; set; }

            public bool Match(Parcel parcel)
            {
                double maxDim = Math.Max(parcel.Width, Math.Max(parcel.Height, parcel.Depth));

                if (maxDim > this.DimensionMax)
                {
                    return false;
                }

                return true;
            }
        }

        private readonly List<DeliveryCategory> categories;

        public DeliveryCostEngine()
        {
            this.categories = new List<DeliveryCategory>()
            {
                new DeliveryCategory() { DimensionMax = 10, Price = 3, Name = "small" },
                new DeliveryCategory() { DimensionMax = 50, Price = 8, Name = "medium" },
                new DeliveryCategory() { DimensionMax = 100, Price = 15, Name = "large" },
                new DeliveryCategory() { DimensionMax = double.MaxValue, Price = 25, Name = "xl" },
            };
        }

        public Receipt ComputePrices(IEnumerable<Parcel> parcels)
        {
            var receipt = new Receipt();

            foreach (var parcel in parcels)
            {
                var cheapestCategory = categories.Where(x => x.Match(parcel)).OrderBy(x => x.Price).First();

                receipt.AddParcel(parcel, cheapestCategory.Name, cheapestCategory.Price);
            }

            return receipt;
        }
    }
}