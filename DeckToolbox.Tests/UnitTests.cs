using System;
using System.Collections.Generic;
using System.Linq;
using DeckToolbox.Resolvers;
using DeckToolbox.Utils;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DeckToolbox.Tests
{
    public class DeckTest
    {
        [Fact]
        public void TestBuilding()
        {
            RawDeck deck = new RawDeck();

            // Sample deck:
            // 3rd Armored "Spearhead"
            // 2 cards of Scouts + Jeep (A, x4, 25pts)
            // 1 card of ENG.LEADER + FORD GPA (B, x3, 20pts)
            // 1 card of M5A1 (A, x5, 90pts)
            deck.BuildFromDeckString("JwO2ErdBtBE=");
            
            Assert.Equal(2, deck.Packs.FirstOrDefault(x => x.Id == 2913).NumPacks);
            Assert.Equal(1, deck.Packs.FirstOrDefault(x => x.Id == 2932).NumPacks);
            Assert.Equal(1, deck.Packs.FirstOrDefault(x => x.Id == 2881).NumPacks);
            Assert.Equal(39, deck.DivisionId);
        }

        [Fact]
        public void TestResolvers()
        {
            
            var units = new
            {
                MetaData = new
                {
                    DataIdentifier = "Units"
                },
                Data = new object[]
                {
                    new
                    {
                        Id = 1,
                        DescriptorId = "u-d-id",
                        TypeUnitHint = "TEST UNIT",
                        NameInMenu = "TEST UNIT",
                        Category = 2,
                        MotherCountry = "USA",
                        Factory = 10,
                        ProductionPrice = 25,
                        ResolvedIsRecon = false,
                        ResolvedIsCommand = true
                    },
                    new
                    {
                        Id = 2,
                        DescriptorId = "tp-d-id",
                        TypeUnitHint = "TEST TRANSPORT",
                        NameInMenu = "TEST TRANSPORT",
                        Category = 2,
                        MotherCountry = "USA",
                        Factory = 10,
                        ProductionPrice = 10,
                        ResolvedIsRecon = false,
                        ResolvedIsCommand = false
                    }
                }
            };

            var packs = new
            {
                MetaData = new
                {
                    DataIdentifier = "Packs"
                },
                Data = new object[]
                {
                    new
                    {
                        Id = 1,
                        AvailableFromPhase = 0,
                        FactoryType = 10,
                        ExperienceLevel = 1,
                        TransportDescriptorId = "tp-d-id",
                        UnitDescriptorId = "u-d-id",
                        TransportAndUnitCount = 4
                    }
                }
            };

            var divisions = new
            {
                MetaData = new
                {
                    DataIdentifier = "Divisions"
                },
                Data = new object[]
                {
                    new
                    {
                        Id = 1,
                        DivisionName = "TEST DIVISION",
                        DivisionNickName = "",
                        PhaseIncome = new int[]
                        {
                            10,
                            20,
                            30
                        }
                    }
                }
            };

            var sourceFiles = new List<string>() {"Units.json", "Divisions.json", "Packs.json"};
            var jsonReader = new Mock<IJsonFileReader>();
            jsonReader.Setup(x => x.ReadFile("Units.json")).Returns(JsonConvert.SerializeObject(units));
            jsonReader.Setup(x => x.ReadFile("Divisions.json")).Returns(JsonConvert.SerializeObject(divisions));
            jsonReader.Setup(x => x.ReadFile("Packs.json")).Returns(JsonConvert.SerializeObject(packs));

            var deckResolver = new JsonDeckValueResolver(jsonReader.Object, sourceFiles);

            var divisionName = deckResolver.GetDivisionName(1);
            var packName = deckResolver.GetDeckPackName(1);
            var incomes = deckResolver.GetPhaseIncomes(1);

            var packResolver = new JsonDeckPackValueResolver(jsonReader.Object, sourceFiles);

            var phase = packResolver.GetAvailablePhase(1);
            var xp = packResolver.GetExperienceLevel(1);
            var fac = packResolver.GetFactoryType(1);
            var tpName = packResolver.GetTransportName(1);
            var uName = packResolver.GetUnitName(1);
            var count = packResolver.GetUnitCount(1);

            Assert.Equal("TEST DIVISION", divisionName);
            Assert.Equal("TEST UNIT + TEST TRANSPORT", packName);
            Assert.Collection(incomes, (x) => Assert.Equal(10, x), (x) => Assert.Equal(20, x), (x) => Assert.Equal(30, x));

            Assert.Equal(0, phase);
            Assert.Equal(1, xp);
            Assert.Equal(10, fac);
            Assert.Equal("TEST TRANSPORT", tpName);
            Assert.Equal("TEST UNIT", uName);
            Assert.Equal(4, count);
        }
    }
}
