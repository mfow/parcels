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
            public double MaxWeight { get; set; }
            public decimal PricePerAdditionalKG { get; set; }

            public bool Match(Parcel parcel)
            {
                double maxDim = Math.Max(parcel.Width, Math.Max(parcel.Height, parcel.Depth));

                if (maxDim > this.DimensionMax)
                {
                    return false;
                }

                return true;
            }

            public decimal GetPrice(Parcel parcel)
            {
                var extraWeight = (decimal)Math.Max(0, parcel.Weight - this.MaxWeight);
                return this.Price + extraWeight * this.PricePerAdditionalKG;
            }
        }

        public class ShippingSpeedRule
        {
            public decimal SubtotalMultiplier { get; set; }
        }

        private readonly List<DeliveryCategory> categories;
        private readonly Dictionary<ShippingSpeed, ShippingSpeedRule> shippingRules;

        private readonly List<IDiscountRule> discountRules;
        public DeliveryCostEngine()
        {
            this.categories = new List<DeliveryCategory>()
            {
                new DeliveryCategory() { DimensionMax = 10, Price = 3, Name = "small", PricePerAdditionalKG = 2, MaxWeight = 1 },
                new DeliveryCategory() { DimensionMax = 50, Price = 8, Name = "medium", PricePerAdditionalKG = 2, MaxWeight = 3 },
                new DeliveryCategory() { DimensionMax = 100, Price = 15, Name = "large", PricePerAdditionalKG = 2, MaxWeight = 6 },
                new DeliveryCategory() { DimensionMax = double.MaxValue, Price = 25, Name = "xl", PricePerAdditionalKG = 2, MaxWeight = 10 },
                new DeliveryCategory() { DimensionMax = double.MaxValue, Price = 50, Name = "heavy", PricePerAdditionalKG = 1, MaxWeight = 50 },
            };

            this.shippingRules = new Dictionary<ShippingSpeed, ShippingSpeedRule>();
            this.shippingRules.Add(ShippingSpeed.Regular, new ShippingSpeedRule() { SubtotalMultiplier = 0 });
            this.shippingRules.Add(ShippingSpeed.Speedy, new ShippingSpeedRule() { SubtotalMultiplier = 1 });

            this.discountRules = new List<IDiscountRule>()
            {
                new DiscountCheapestItemRule()
                {
                    CategoryNames = new List<string>() { "small" },
                    DiscountName = "4th_small_item_free",
                    RequiredItems = 4
                },
                new DiscountCheapestItemRule()
                {
                    CategoryNames = new List<string>() { "medium" },
                    DiscountName = "3rd_medium_item_free",
                    RequiredItems = 3
                },
                new DiscountCheapestItemRule()
                {
                    CategoryNames = new List<string>() { "small", "medium", "large", "xl", "heavy" },
                    DiscountName = "5th_item_free",
                    RequiredItems = 5
                }
            };
        }

        public Receipt ComputePrices(IEnumerable<Parcel> parcels, ShippingSpeed speed = ShippingSpeed.Regular)
        {
            var receipt = new Receipt();

            foreach (var parcel in parcels)
            {
                var cheapestCategory = categories.Where(x => x.Match(parcel))
                    .Select(x => new Tuple<string, decimal>(x.Name, x.GetPrice(parcel)))
                    .OrderBy(x => x.Item2)
                    .First();

                receipt.AddParcel(parcel, cheapestCategory.Item1, cheapestCategory.Item2);
            }

            ApplyDiscounts(receipt);

            var shippingRule = shippingRules[speed];
            receipt.Shipping = receipt.Subtotal * shippingRule.SubtotalMultiplier;
            
            return receipt;
        }

        private void ApplyDiscounts(Receipt receipt)
        {
            while (true)
            {
                var bestDiscount = discountRules.Select(x => x.GetDiscount(receipt))
                    .Where(x => x != null)
                    .OrderByDescending(x => x.DiscountAmount)
                    .FirstOrDefault();

                if (bestDiscount == null)
                {
                    return;
                }
                else
                {
                    receipt.Discounts.Add(bestDiscount);
                }
            }
        }
    }
}
