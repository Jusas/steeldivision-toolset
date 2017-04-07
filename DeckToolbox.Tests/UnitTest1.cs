using System;
using Xunit;

namespace DeckToolbox.Tests
{
    public class DeckTest
    {
        [Fact]
        public void Test1()
        {
            RawDeck deck = new RawDeck();
            deck.BuildFromDeckString("Jx+2AbRxtiG0kbPSs+G0obTitzG3AbcRtlK3IbNRs4GzkbQBtTG1QbWxt3G1EbTBtEG2wbbRtYG2QbXRtjG2Yg==");
        }
    }
}
