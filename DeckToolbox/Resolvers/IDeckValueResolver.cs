using System;
using System.Collections.Generic;
using System.Text;

namespace DeckToolbox.Resolvers
{
    public interface IDeckValueResolver
    {
        string GetDivisionName(int divisionId);
        string GetDeckPackName(int deckPackId);
        int[] GetPhaseIncomes(int divisionId);
    }
}
