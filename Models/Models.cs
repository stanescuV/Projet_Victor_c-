using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Projet_Victor_c_
{
    public enum OSKind { Windows, Linux, Mac, Other }
    public enum IfStatus { Up, Down }
    public enum RuleAction { Block, Allow }
    public enum PortType { TCP, UDP }

    public class Host
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string HostName { get; set; } = "";
        public OSKind OS { get; set; } = OSKind.Windows;
        public string Description { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> InterfaceIds { get; set; } = new List<string>();
    }

    public class NetworkInterface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Identifier { get; set; } = ""; // unique per host
        public string HostId { get; set; } = "";
        public string Description { get; set; } = "";
        public IfStatus Status { get; set; } = IfStatus.Down;
        public string MacAddress { get; set; } = "";
        public string IP { get; set; } = ""; // v4 or v6
        public string SubnetMask { get; set; } = "";
        public string Gateway { get; set; } = "";
        public bool DHCP { get; set; } = false;
        public string DnsPrimary { get; set; } = "";
        public string DnsSecondary { get; set; } = "";
        public List<string> RuleIds { get; set; } = new List<string>();
    }

    public class FirewallRule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Identifier { get; set; } = ""; // unique
        public string Description { get; set; } = "";
        public RuleAction Action { get; set; } = RuleAction.Allow;
        public string Program { get; set; } = "";
        public PortType PortType { get; set; } = PortType.TCP;
        public string LocalPorts { get; set; } = "";
        public string RemotePorts { get; set; } = "";
        public string LocalAddress { get; set; } = "";
        public string RemoteAddress { get; set; } = "";
        public string Tag { get; set; } = "";
        public bool Enabled { get; set; } = true;
    }
}
