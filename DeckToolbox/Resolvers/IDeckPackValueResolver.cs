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
        int GetTransportPrice(int packId);
        string GetUnitName(int packId);
        int GetUnitPrice(int packId);
        int GetUnitCount(int packId);
        string GetFactoryName(int packId);
        bool GetIsRecon(int packId);
        bool GetIsCommand(int packId);
        bool GetIsObserver(int packId);
        bool GetMainUnitHasWeapons(int packId);
        int GetMainUnitFrontalArmor(int packId);
        int GetMainUnitTurretArcAngle(int packId);
        int GetMainUnitAp(int packId);
        int GetMainUnitHe(int packId);

        string[] GetValidPackFactoryNames();
    }
}
