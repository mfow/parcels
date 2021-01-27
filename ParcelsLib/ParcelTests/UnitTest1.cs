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
            Assert.Equal(0, receipt.Subtotal);
        }

        [Fact]
        public void TestPackageCollection1()
        {
            var receipt = engine.ComputePrices(new List<Parcel>()
            {
                new Parcel()
                {
                    // small 

                    Id = Guid.NewGuid(),
                    Width = 9,
                    Height = 8,
                    Depth = 7
                },
                new Parcel()
                {
                    // medium

                    Id = Guid.NewGuid(),
                    Width = 11,
                    Height = 11,
                    Depth = 11
                },
                new Parcel()
                {
                    // large

                    Id = Guid.NewGuid(),
                    Width = 100,
                    Height = 9,
                    Depth = 4
                },
                new Parcel()
                {
                    // xl

                    Id = Guid.NewGuid(),
                    Width = 101,
                    Height = 11,
                    Depth = 11
                }
            });

            Assert.Equal(4, receipt.Parcels.Count);
            Assert.Equal(3, receipt.Parcels[0].Price);
            Assert.Equal(8, receipt.Parcels[1].Price);
            Assert.Equal(15, receipt.Parcels[2].Price);
            Assert.Equal(25, receipt.Parcels[3].Price);
            Assert.Equal(3 + 8 + 15 + 25, receipt.Total);
        }

        [Fact]
        public void TestPackageCollectionDiscounted1()
        {
            var receipt = engine.ComputePrices(new List<Parcel>()
            {
                new Parcel()
                {
                    // small 

                    Id = Guid.NewGuid(),
                    Width = 9,
                    Height = 8,
                    Depth = 7
                },
                new Parcel()
                {
                    // medium

                    Id = Guid.NewGuid(),
                    Width = 11,
                    Height = 11,
                    Depth = 11
                },
                new Parcel()
                {
                    // large

                    Id = Guid.NewGuid(),
                    Width = 100,
                    Height = 9,
                    Depth = 4
                },
                new Parcel()
                {
                    // xl

                    Id = Guid.NewGuid(),
                    Width = 101,
                    Height = 11,
                    Depth = 11
                },
                new Parcel()
                {
                    // xl

                    Id = Guid.NewGuid(),
                    Width = 102,
                    Height = 11,
                    Depth = 11
                }
            });

            Assert.Equal(5, receipt.Parcels.Count);
            Assert.Equal(3, receipt.Parcels[0].Price);
            Assert.Equal(8, receipt.Parcels[1].Price);
            Assert.Equal(15, receipt.Parcels[2].Price);
            Assert.Equal(25, receipt.Parcels[3].Price);
            Assert.Equal(3, receipt.TotalDiscounts);
            Assert.Equal(8 + 15 + 25 + 25, receipt.Total);
        }


        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 2)] // test multiple discounts

        public void TestPackageCollectionSmallDiscounts(int numSmallParcels, int numDiscounts)
        {
            var parcels = new List<Parcel>();

            for (int i = 0; i < numSmallParcels; i++)
            {
                parcels.Add(new Parcel()
                {
                    // small 

                    Id = Guid.NewGuid(),
                    Width = 9,
                    Height = 8,
                    Depth = 7
                });
            }

            var receipt = engine.ComputePrices(parcels, ShippingSpeed.Regular);

            Assert.Equal(numSmallParcels, receipt.Parcels.Count);
            Assert.Equal(3 * numDiscounts, receipt.TotalDiscounts);
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

        [Theory]
        [InlineData(9, 8, 7, 6)]
        [InlineData(11, 9, 7, 16)]
        [InlineData(11, 99, 7, 30)]
        [InlineData(11, 101, 101, 50)]
        public void TestSpeedyShippingPrice(double width, double height, double depth, decimal price)
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
            }, ShippingSpeed.Speedy);

            var p = Assert.Single(receipt.Parcels);
            Assert.NotEqual(0, receipt.Shipping);
            Assert.Equal(receipt.Total, receipt.Subtotal + receipt.Shipping);
            Assert.Equal(price, receipt.Total);
        }

        [Theory]
        [InlineData(9, 8, 7, 1, 3)]
        [InlineData(9, 8, 7, 2, 5)]
        [InlineData(11, 9, 7, 2, 8)]
        [InlineData(11, 9, 7, 4, 10)]
        [InlineData(11, 11, 11, 2, 8)]
        [InlineData(11, 99, 7, 6, 15)]
        [InlineData(11, 99, 7, 10, 23)]
        [InlineData(11, 9, 101, 10, 25)]
        [InlineData(11, 9, 101, 50, 50)]
        [InlineData(11, 9, 101, 60, 60)]
        public void TestParcelsWithExtraWeight(double width, double height, double depth, double weight, decimal price)
        {
            var receipt = engine.ComputePrices(new List<Parcel>()
            {
                new Parcel()
                {
                    Id = Guid.NewGuid(),
                    Width = width,
                    Height = height,
                    Depth = depth,
                    Weight = weight
                }
            });

            var p = Assert.Single(receipt.Parcels);
            Assert.Equal(price, receipt.Total);
        }
    }
}
