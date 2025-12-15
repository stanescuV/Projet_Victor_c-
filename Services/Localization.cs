using System.Collections.Generic;

namespace Projet_Victor_c_
{
    public enum Language { EN, FR }

    public static class Localization
    {
        public static Language Current { get; set; } = Language.EN;

        private static readonly Dictionary<string, (string en, string fr)> _strings = new()
        {
            { "MainForm.Title", ("Network Manager - Simple", "Gestion réseau - Simple") },
            { "MainForm.AddHost", ("Add Host", "Ajouter hôte") },
            { "MainForm.AddInterface", ("Add Interface", "Ajouter interface") },
            { "MainForm.AddRule", ("Add Rule", "Ajouter règle") },
            { "MainForm.Save", ("Save", "Sauvegarder") },
            { "MainForm.DeleteHost", ("Delete Host", "Supprimer hôte") },
            { "MainForm.DeleteRule", ("Delete Rule", "Supprimer règle") },
            { "MainForm.LangFr", ("Français", "Français") },
            { "MainForm.LangEn", ("English", "Anglais") },

            { "HostForm.Title.Add", ("Add Host", "Ajouter hôte") },
            { "HostForm.Title.Edit", ("Edit Host", "Modifier hôte") },
            { "HostForm.HostName", ("Host name:", "Nom de l'hôte :") },
            { "HostForm.OS", ("OS:", "Système :") },
            { "HostForm.Description", ("Description (optional):", "Description (optionnelle) :") },
            { "HostForm.Tags", ("Tags (comma separated):", "Tags (séparés par des virgules) :") },

            { "InterfaceForm.Title.Add", ("Add Network Interface", "Ajouter interface réseau") },
            { "InterfaceForm.Title.Edit", ("Edit Network Interface", "Modifier interface réseau") },
            { "InterfaceForm.Host", ("Host:", "Hôte :") },
            { "InterfaceForm.Identifier", ("Identifier:", "Identifiant :") },
            { "InterfaceForm.Description", ("Description (optional):", "Description (optionnelle) :") },
            { "InterfaceForm.Status", ("Status:", "Statut :") },
            { "InterfaceForm.Mac", ("MAC Address:", "Adresse MAC :") },
            { "InterfaceForm.DHCP", ("DHCP:", "DHCP :") },
            { "InterfaceForm.IP", ("IP Address:", "Adresse IP :") },
            { "InterfaceForm.Mask", ("Subnet Mask/Prefix:", "Masque sous-réseau / Préfixe :") },
            { "InterfaceForm.Gateway", ("Gateway:", "Passerelle :") },
            { "InterfaceForm.Dns1", ("DNS Primary:", "DNS primaire :") },
            { "InterfaceForm.Dns2", ("DNS Secondary:", "DNS secondaire :") },

            { "RuleForm.Title.Add", ("Add Firewall Rule", "Ajouter règle firewall") },
            { "RuleForm.Title.Edit", ("Edit Firewall Rule", "Modifier règle firewall") },
            { "RuleForm.Identifier", ("Identifier:", "Identifiant :") },
            { "RuleForm.Description", ("Description (optional):", "Description (optionnelle) :") },
            { "RuleForm.Action", ("Action:", "Action :") },
            { "RuleForm.Allow", ("Allow", "Autoriser") },
            { "RuleForm.Block", ("Block", "Bloquer") },
            { "RuleForm.Program", ("Program (path/service):", "Programme (chemin/service) :") },
            { "RuleForm.PortType", ("Port type:", "Type de port :") },
            { "RuleForm.LocalPorts", ("Local ports:", "Ports locaux :") },
            { "RuleForm.RemotePorts", ("Remote ports:", "Ports distants :") },
            { "RuleForm.LocalAddress", ("Local address (or range):", "Adresse locale (ou plage) :") },
            { "RuleForm.RemoteAddress", ("Remote address (or range):", "Adresse distante (ou plage) :") },
            { "RuleForm.Tag", ("Tag:", "Tag :") },
            { "RuleForm.Enabled", ("Enabled:", "Activé :") },
            { "RuleForm.AttachIf", ("Attach to interfaces:", "Attacher aux interfaces :") },

            { "Common.OK", ("OK", "OK") },
            { "Common.Cancel", ("Cancel", "Annuler") }
        };

        public static string Get(string key)
        {
            if (_strings.TryGetValue(key, out var v))
            {
                return Current == Language.EN ? v.en : v.fr;
            }
            return key;
        }
    }
}
