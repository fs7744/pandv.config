using Pandv.Config.Test;
using System;

namespace Test
{
    class Program
    {
        public void Test()
        {
            var fixture = new ClientFixture();
            var configTest = new ConfigTest(fixture);
            configTest.LoadConfiguration();
            configTest.SetConfiguration();
            configTest.LoadReloadConfiguration();
            configTest.SetReloadConfiguration();
        }

        static void Main(string[] args)
        {
            new Program().Test();
            Console.WriteLine("Test Done");
        }
    }
}
