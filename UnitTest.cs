

namespace Projet_Victor_c_
{
    // Quelques unit tests simples 
    internal static class UnitTest
    {
        public static void RunAll()
        {
            TestHostDefaults();
            TestNetworkInterfaceDefaults();
            TestFirewallRuleDefaults();
        }

        private static void TestHostDefaults()
        {
            var host = new Host();
            AssertNotNullOrEmpty(host.Id, "Host.Id should be generated");
            AssertEqual(host.HostName, string.Empty, "Host.HostName default should be empty string");
            AssertEqual(host.OS, OSKind.Windows, "Host.OS default should be Windows");
            AssertNotNull(host.Tags, "Host.Tags should be initialized");
            AssertTrue(host.Tags.Count == 0, "Host.Tags should start empty");
        }

        private static void TestNetworkInterfaceDefaults()
        {
            var networkInterface = new NetworkInterface();
            AssertNotNullOrEmpty(networkInterface.Id, "NetworkInterface.Id should be generated");
            AssertEqual(networkInterface.Identifier, string.Empty, "NetworkInterface.Identifier default should be empty string");
            AssertEqual(networkInterface.Status, IfStatus.Down, "NetworkInterface.Status default should be Down");
            AssertNotNull(networkInterface.RuleIds, "NetworkInterface.RuleIds should be initialized");
            AssertTrue(networkInterface.RuleIds.Count == 0, "NetworkInterface.RuleIds should start empty");
        }

        private static void TestFirewallRuleDefaults()
        {
            var rule = new FirewallRule();
            AssertNotNullOrEmpty(rule.Id, "FirewallRule.Id should be generated");
            AssertEqual(rule.Identifier, string.Empty, "FirewallRule.Identifier default should be empty string");
            AssertEqual(rule.Action, RuleAction.Allow, "FirewallRule.Action default should be Allow");
            AssertEqual(rule.PortType, PortType.TCP, "FirewallRule.PortType default should be TCP");
            AssertTrue(rule.Enabled, "FirewallRule.Enabled default should be true");
        }

        // --- small assertion helpers ---
        private static void AssertTrue(bool condition, string message)
        {
            if (!condition) throw new UnitTestException(message);
        }

        private static void AssertEqual<T>(T actual, T expected, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(actual, expected))
                throw new UnitTestException($"{message}. Expected: {expected}, Actual: {actual}");
        }

        private static void AssertNotNull(object? obj, string message)
        {
            if (obj is null) throw new UnitTestException(message);
        }

        private static void AssertNotNullOrEmpty(string? s, string message)
        {
            if (string.IsNullOrEmpty(s)) throw new UnitTestException(message);
        }

        private class UnitTestException : Exception
        {
            public UnitTestException(string message) : base("UnitTest failed: " + message) { }
        }
    }
}
