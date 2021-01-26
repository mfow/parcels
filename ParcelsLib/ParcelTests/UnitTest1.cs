using ParcelsLib;
using System;
using System.Collections.Generic;
using Xunit;

namespace ParcelTests
{
    public class UnitTest1
    {
        private readonly DeliveryCostEngine engine;

        public UnitTest1()
        {
            engine = new DeliveryCostEngine();
        }

        [Fact]
        public void TestNoPackagesAreFree()
        {
            var receipt = engine.ComputePrices(new List<Parcel>());

            Assert.Empty(receipt.Parcels);
            Assert.Equal(0, receipt.Total);
        }

        [Theory]
        [InlineData(9, 8, 7, 3, "small")]
        [InlineData(10, 9, 7, 3, "small")]
        [InlineData(9, 10, 7, 3, "small")]
        [InlineData(9, 8, 10, 3, "small")]
        [InlineData(11, 9, 7, 8, "medium")]
        [InlineData(11, 11, 11, 8, "medium")]
        [InlineData(11, 99, 7, 15, "large")]
        [InlineData(11, 9, 100, 15, "large")]
        [InlineData(100, 9, 4, 15, "large")]
        [InlineData(11, 9, 101, 25, "xl")]
        [InlineData(101, 100, 100, 25, "xl")]
        [InlineData(11, 101, 101, 25, "xl")]
        public void TestSinglePackagePrice(double width, double height, double depth, decimal price, string category)
        {
            var receipt = engine.ComputePrices(new List<Parcel>()
            {
                new Parcel()
                {
                    Id = Guid.NewGuid(),
                    Width = width,
                    Height = height,
                    Depth = depth
                }
            });

            var p = Assert.Single(receipt.Parcels);
            Assert.Equal(category, p.PricingCategory);
            Assert.Equal(price, receipt.Total);
        }
    }
}
