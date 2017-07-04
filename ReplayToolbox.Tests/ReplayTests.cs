using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace ReplayToolbox.Tests
{
    public class ReplayTests
    {
        [Fact]
        public async Task TestReadingReplay()
        {
            var asm = GetType().GetTypeInfo().Assembly;
            var embeddedFiles = asm.GetManifestResourceNames();
            var replayFile =
                embeddedFiles.FirstOrDefault(
                    x => x.Contains("beta-8-4-2017-rpl.rpl3"));

            var replayStream = asm.GetManifestResourceStream(replayFile);
            var replay = await Replay.FromStream(replayStream);
            Assert.Equal("300077108", replay.ReplayHeader.Game.Version);
        }

        [Fact]
        public async Task TestReadingBuggyReplay()
        {
            var asm = GetType().GetTypeInfo().Assembly;
            var embeddedFiles = asm.GetManifestResourceNames();
            var replayFile =
                embeddedFiles.FirstOrDefault(
                    x => x.Contains("no-map-bug-replay-4-7-2017-rpl.rpl3"));

            var replayStream = asm.GetManifestResourceStream(replayFile);

            bool didThrow = false;
            try
            {
                var replay = await Replay.FromStream(replayStream);
            }
            catch (Exception e)
            {
                didThrow = true;
            }

            Assert.Equal(true, didThrow);
        }
    }
}
