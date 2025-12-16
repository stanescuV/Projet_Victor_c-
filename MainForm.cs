namespace Projet_Victor_c_
{
   
    public class MainForm : Form
    {
        private DataStore _ds;
        private ListBox listBoxHosts;
        private ListBox listBoxIf;
        private ListBox listBoxRules;
        private Button btnAddHost;
        private Button btnAddIf;
        private Button btnAddRule;
        private Button btnSave;
        private Button btnDeleteHost;
        private Button btnDeleteIf;
        private Button btnDeleteRule;

        public MainForm()
        {
            Text = "Manager Reseaux";
            ClientSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterScreen;

            listBoxHosts = new ListBox { Location = new Point(12, 12), Size = new Size(280, 360) };
            listBoxIf = new ListBox { Location = new Point(304, 12), Size = new Size(280, 360) };
            listBoxRules = new ListBox { Location = new Point(596, 12), Size = new Size(280, 360) };

            btnAddHost = new Button { Location = new Point(12, 380), Size = new Size(90, 27), Text = "Add Host" };
            btnAddIf = new Button { Location = new Point(108, 380), Size = new Size(110, 27), Text = "Add Interface" };
            btnAddRule = new Button { Location = new Point(224, 380), Size = new Size(110, 27), Text = "Add Rule" };
            btnSave = new Button { Location = new Point(596, 380), Size = new Size(90, 27), Text = "Save" };
            btnDeleteHost = new Button { Location = new Point(340, 380), Size = new Size(120, 27), Text = "Delete Host" };
            btnDeleteIf = new Button { Location = new Point(460, 380), Size = new Size(120, 27), Text = "Delete IntR" };
            btnDeleteRule = new Button { Location = new Point(736, 380), Size = new Size(120, 27), Text = "Delete Rule" };

            Controls.AddRange(new Control[] { listBoxHosts, listBoxIf, listBoxRules, btnAddHost, btnAddIf, btnAddRule, btnSave, btnDeleteHost, btnDeleteIf, btnDeleteRule });

            btnAddHost.Click += BtnAddHost_Click;
            btnAddIf.Click += BtnAddIf_Click;
            btnAddRule.Click += BtnAddRule_Click;
            btnSave.Click += BtnSave_Click;
            btnDeleteHost.Click += BtnDeleteHost_Click;
            btnDeleteIf.Click += BtnDeleteIf_Click;
            btnDeleteRule.Click += BtnDeleteRule_Click;

            listBoxHosts.DoubleClick += ListBoxHosts_DoubleClick;
            listBoxIf.DoubleClick += ListBoxIf_DoubleClick;
            listBoxRules.DoubleClick += ListBoxRules_DoubleClick;

            var file = Path.Combine(AppContext.BaseDirectory, "data.json");
            _ds = new DataStore(file);
            RefreshLists();
        }

        private void ListBoxRules_DoubleClick(object? sender, EventArgs e)
        {
            if (listBoxRules.SelectedIndex < 0) return;
            var idx = listBoxRules.SelectedIndex;
            if (idx >= _ds.Rules.Count) return;
            var rule = _ds.Rules[idx];

            var interfaces = _ds.Interfaces.ToArray();
            using var dlg = new RuleForm(interfaces, id => _ds.Rules.Any(r => string.Equals(r.Identifier, id, StringComparison.OrdinalIgnoreCase)), rule);

            // on prepare la liste des interfaces déjà attachées à cette règle
            var attachedInterfaceIds = _ds.Interfaces.Where(i => i.RuleIds.Contains(rule.Id)).Select(i => i.Id).ToArray();
            dlg.PreselectInterfaces(attachedInterfaceIds);

            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Rule != null)
            {
                foreach (var ni in _ds.Interfaces) ni.RuleIds.RemoveAll(rid => rid == dlg.Rule.Id);
                foreach (var iid in dlg.SelectedInterfaceIds ?? Array.Empty<string>())
                {
                    var ni = _ds.Interfaces.FirstOrDefault(i => i.Id == iid);
                    if (ni != null)
                    {
                        ni.RuleIds.Add(dlg.Rule.Id);
                    }
                }
                _ds.Save();
                RefreshLists();
            }
        }

        private void ListBoxIf_DoubleClick(object? sender, EventArgs e)
        {
            if (listBoxIf.SelectedIndex < 0) return;
            var idx = listBoxIf.SelectedIndex;
            if (idx >= _ds.Interfaces.Count) return;
            var ni = _ds.Interfaces[idx];

            var hosts = _ds.Hosts.ToArray();
            var existingIds = _ds.Interfaces.Where(i => i.HostId == ni.HostId && i.Id != ni.Id).Select(i => i.Identifier).ToArray();

            using var dlg = new InterfaceForm(hosts, existingIds, mac => IsMacUsed(mac), ip => IsIpUsed(ip), ni);
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Interface != null)
            {
                //on verifie l'association host - interface
                foreach (var h in _ds.Hosts)
                {
                    if (h.Id == dlg.Interface.HostId)
                    {
                        if (!h.InterfaceIds.Contains(dlg.Interface.Id)) h.InterfaceIds.Add(dlg.Interface.Id);
                    }
                    else
                    {
                        h.InterfaceIds.RemoveAll(id => id == dlg.Interface.Id);
                    }
                }

                _ds.Save();
                RefreshLists();
            }
        }

        private void ListBoxHosts_DoubleClick(object? sender, EventArgs e)
        {
            if (listBoxHosts.SelectedIndex < 0) return;
            var idx = listBoxHosts.SelectedIndex;
            if (idx >= _ds.Hosts.Count) return;
            var host = _ds.Hosts[idx];

            var existingNames = _ds.Hosts.Select(h => h.HostName).ToArray();
            using var dlg = new HostForm(existingNames, host);
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Host != null)
            {
                _ds.Save();
                RefreshLists();
            }
        }

        private void RefreshLists()
        {
            listBoxHosts.Items.Clear();
            foreach (var h in _ds.Hosts) listBoxHosts.Items.Add(h.HostName + " (" + (h.Id?.Length > 8 ? h.Id.Substring(0, 8) : h.Id) + ")");

            listBoxIf.Items.Clear();
            foreach (var i in _ds.Interfaces) listBoxIf.Items.Add(i.Identifier + " -> " + (i.HostId?.Length > 8 ? i.HostId.Substring(0, 8) : i.HostId));

            listBoxRules.Items.Clear();
            foreach (var r in _ds.Rules)
            {
                var count = _ds.Interfaces.Count(i => i.RuleIds.Contains(r.Id));
                listBoxRules.Items.Add(r.Identifier + " -> " + r.Action + (count > 0 ? " (attached: " + count + ")" : ""));
            }
        }

        private void BtnAddHost_Click(object? sender, EventArgs e)
        {
            var existingNames = _ds.Hosts.Select(h => h.HostName).ToArray();
            using var dlg = new HostForm(existingNames);
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Host != null)
            {
                // verifie si le nom n'existe pas déjà
                if (_ds.Hosts.Any(h => string.Equals(h.HostName, dlg.Host.HostName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show(this, "Host name already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _ds.Hosts.Add(dlg.Host);
                _ds.Save();
                RefreshLists();
            }
        }

        private void BtnDeleteHost_Click(object? sender, EventArgs e)
        {
            if (listBoxHosts.SelectedIndex < 0) { MessageBox.Show(this, "Select a host to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var idx = listBoxHosts.SelectedIndex;
            if (idx >= _ds.Hosts.Count) return;
            var host = _ds.Hosts[idx];

            var impacted = _ds.Interfaces.Where(i => i.HostId == host.Id).ToList();
            string msg = "Delete host '" + host.HostName + "'?\n";
            if (impacted.Any())
            {
                msg += "This will also remove the following interfaces:\n" + string.Join("\n", impacted.Select(i => i.Identifier + " (" + i.Id.Substring(0,8) + ")"));
            }

            var dr = MessageBox.Show(this, msg, "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr != DialogResult.OK) return;

            // on remove les interfaces pour ce host
            foreach (var it in impacted)
            {
                _ds.Interfaces.Remove(it);
            }

            // on remove le host
            _ds.Hosts.Remove(host);
            _ds.Save();
            RefreshLists();
        }

        private void BtnAddIf_Click(object? sender, EventArgs e)
        {
            if (!_ds.Hosts.Any()) { MessageBox.Show("Add a host first"); return; }

            var hosts = _ds.Hosts.ToArray();
            // on preselectionne le premier host
            var selectedHost = hosts.First();
            var existingIds = _ds.Interfaces.Where(i => i.HostId == selectedHost.Id).Select(i => i.Identifier).ToArray();

            using var dlg = new InterfaceForm(hosts, existingIds, mac => IsMacUsed(mac), ip => IsIpUsed(ip));
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Interface != null)
            {
                _ds.Interfaces.Add(dlg.Interface);
                var host = _ds.Hosts.FirstOrDefault(h => h.Id == dlg.Interface.HostId);
                if (host != null) host.InterfaceIds.Add(dlg.Interface.Id);
                _ds.Save();
                RefreshLists();
            }
        }

        private bool IsMacUsed(string mac)
        {
            if (string.IsNullOrEmpty(mac)) return false;
            return _ds.Interfaces.Any(i => string.Equals(i.MacAddress, mac, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsIpUsed(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            return _ds.Interfaces.Any(i => string.Equals(i.IP, ip, StringComparison.OrdinalIgnoreCase));
        }

        private void BtnAddRule_Click(object? sender, EventArgs e)
        {
            var hosts = _ds.Hosts.ToArray();
            var interfaces = _ds.Interfaces.ToArray();
            using var dlg = new RuleForm(interfaces, id => _ds.Rules.Any(r => string.Equals(r.Identifier, id, StringComparison.OrdinalIgnoreCase)));
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Rule != null)
            {
                _ds.Rules.Add(dlg.Rule);
                // on associe la regle aux interfaces sélectionnées
                foreach (var iid in dlg.SelectedInterfaceIds ?? Array.Empty<string>())
                {
                    var ni = _ds.Interfaces.FirstOrDefault(i => i.Id == iid);
                    if (ni != null)
                    {
                        ni.RuleIds.Add(dlg.Rule.Id);
                    }
                }
                _ds.Save();
                RefreshLists();
            }
        }

        private void BtnDeleteRule_Click(object? sender, EventArgs e)
        {
            if (listBoxRules.SelectedIndex < 0) { MessageBox.Show(this, "Select a rule to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var idx = listBoxRules.SelectedIndex;
            if (idx >= _ds.Rules.Count) return;
            var rule = _ds.Rules[idx];

            var impactedIf = _ds.Interfaces.Where(i => i.RuleIds.Contains(rule.Id)).ToList();
            string msg = "Delete rule '" + rule.Identifier + "'?\n";
            if (impactedIf.Any())
            {
                msg += "This rule is used by the following interfaces (host - interface):\n" + string.Join("\n", impactedIf.Select(i => {
                    var h = _ds.Hosts.FirstOrDefault(x => x.Id == i.HostId);
                    return (h != null ? h.HostName : i.HostId.Substring(0,8)) + " - " + i.Identifier + " (" + i.Id.Substring(0,8) + ")";
                }));
            }

            var dr = MessageBox.Show(this, msg, "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr != DialogResult.OK) return;

            // on remove la regle des interfaces qui l'utilisent
            foreach (var ni in impactedIf)
            {
                ni.RuleIds.RemoveAll(rid => rid == rule.Id);
            }

            _ds.Rules.Remove(rule);
            _ds.Save();
            RefreshLists();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _ds.Save();
            MessageBox.Show("Saved");
        }

        private void BtnDeleteIf_Click(object? sender, EventArgs e)
        {
            if (listBoxIf.SelectedIndex < 0)
            {
                MessageBox.Show(this, "Select an interface to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idx = listBoxIf.SelectedIndex;
            if (idx >= _ds.Interfaces.Count) return;
            var ni = _ds.Interfaces[idx];

            var host = _ds.Hosts.FirstOrDefault(h => h.Id == ni.HostId);

            string msg = "Delete interface '" + ni.Identifier + "'?";
            if (host != null) msg += "\nHost: " + host.HostName;
            if (ni.RuleIds != null && ni.RuleIds.Any())
            {
                msg += "\nThis interface is referenced by the following rules:\n" + string.Join("\n", ni.RuleIds.Select(rid => {
                    var r = _ds.Rules.FirstOrDefault(x => x.Id == rid);
                    return r != null ? r.Identifier : rid;
                }));
            }

            var dr = MessageBox.Show(this, msg, "Confirm delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr != DialogResult.OK) return;

            // remove the interface and cleanup host references
            _ds.Interfaces.Remove(ni);
            if (host != null) host.InterfaceIds.RemoveAll(id => id == ni.Id);

            _ds.Save();
            RefreshLists();
        }
    }
}
