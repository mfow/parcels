using System;
using System.Collections.Generic;
using System.Linq;

namespace ParcelsLib
{
    public class DiscountCheapestItemRule : IDiscountRule
    {
        public List<string> CategoryNames { get; set; } = new List<string>();
        public int RequiredItems { get; set; }
        public string DiscountName { get; set; }

        public DiscountInfo GetDiscount(Receipt receipt)
        {
            var parcelsInCategory = receipt.ParcelsNonDiscounted.Where(x => CategoryNames.Contains(x.PricingCategory))
                .OrderBy(x => x.Price)
                .ToList();

            if (parcelsInCategory.Count < RequiredItems)
            {
                return null;
            }

            return new DiscountInfo()
            {
                DiscountAmount = parcelsInCategory.First().Price, // discount is on the cheapest price
                DiscountName = DiscountName,
                ParcelIds = parcelsInCategory
                    // use the cheapest items possible for the discount so that for a 2nd discount of the same type,
                    // we discount a more expensive item.
                    .Take(RequiredItems)
                    .Select(x => x.Item.Id).ToList()
            };
        }
    }
}
