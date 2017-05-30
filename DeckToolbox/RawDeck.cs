using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckToolbox.Models;
using DeckToolbox.Utils;

namespace DeckToolbox
{
    public class RawDeck
    {
        public string SourceDeckString { get; private set; }

        public int DivisionId { get; private set; }
        public RawDeckPack[] Packs { get; private set; }

        public RawDeck()
        {
            Packs = new RawDeckPack[0];
        }

        public void BuildFromDeckString(string base64DeckString)
        {
            SourceDeckString = base64DeckString;

            // It appears that in release additional @ markers are present when hero units are present in a deck.
            // Example with Pz Lehr's Wittman unit: GR53knqhekF7UXsieVJ6kXfxeWJ5oXdheMF4MXkRemF3UXlxe6F48Xnxd9F7MneheIF4YnhxeoF6EXhRvHE=@AAG8eA==@
            // It seems we can just happily ignore everything after the first @ mark.

            var heroUnitMarker = SourceDeckString.IndexOf("@", StringComparison.OrdinalIgnoreCase);
            if (heroUnitMarker != -1)
                SourceDeckString = SourceDeckString.Substring(0, heroUnitMarker);

            var dataBytes = Convert.FromBase64String(SourceDeckString);
            //var bits = string.Concat(dataBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0') + " "));

            List<RawDeckPack> packs = new List<RawDeckPack>();

            DivisionId = dataBytes[0];
            var packEntriesInDeck = dataBytes[1];

            int bitOffset = 16;
            for (var i = 0; i < packEntriesInDeck; i++)
            {
                var packId = (int)dataBytes.GetBitSection(bitOffset, 12);
                var numPacks = (int)dataBytes.GetBitSection(bitOffset + 12, 4);
                var pack = new RawDeckPack
                {
                    Id = packId,
                    NumPacks = numPacks
                };
                packs.Add(pack);
                bitOffset += 16;
            }

            Packs = packs.ToArray();
        }
        
    }
}
