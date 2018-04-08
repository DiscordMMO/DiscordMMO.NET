using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiscordMMO;

namespace DiscordMMOTesting
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void TestEquipCommand()
        {
            // Mark that we are running tests
            Modules.unitTesting = true;

            Program.Main();

        }
    }
}
