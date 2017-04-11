using System;
using System.Collections.Generic;
using System.Text;

namespace DeckToolbox.Resolvers
{
    public interface IDeckPackValueResolver
    {
        int GetAvailablePhase(int packId);
        int GetFactoryType(int packId);
        int GetExperienceLevel(int packId);
        string GetTransportName(int packId);
        string GetUnitName(int packId);
        int GetUnitCount(int packId);
        string GetFactoryName(int packId);
    }
}
