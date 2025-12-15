using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Projet_Victor_c_
{
    // Simple self-contained form (no designer) to avoid editing existing Form1
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

        public MainForm()
        {
            Text = "Network Manager - Simple";
            ClientSize = new Size(800, 450);
            StartPosition = FormStartPosition.CenterScreen;

            listBoxHosts = new ListBox { Location = new Point(12, 12), Size = new Size(240, 300) };
            listBoxIf = new ListBox { Location = new Point(262, 12), Size = new Size(240, 300) };
            listBoxRules = new ListBox { Location = new Point(512, 12), Size = new Size(260, 300) };

            btnAddHost = new Button { Location = new Point(12, 320), Size = new Size(90, 27), Text = "Add Host" };
            btnAddIf = new Button { Location = new Point(108, 320), Size = new Size(110, 27), Text = "Add Interface" };
            btnAddRule = new Button { Location = new Point(224, 320), Size = new Size(90, 27), Text = "Add Rule" };
            btnSave = new Button { Location = new Point(512, 320), Size = new Size(90, 27), Text = "Save" };
            btnDeleteHost = new Button { Location = new Point(320, 320), Size = new Size(110, 27), Text = "Delete Host" };

            Controls.AddRange(new Control[] { listBoxHosts, listBoxIf, listBoxRules, btnAddHost, btnAddIf, btnAddRule, btnSave, btnDeleteHost });

            btnAddHost.Click += BtnAddHost_Click;
            btnAddIf.Click += BtnAddIf_Click;
            btnAddRule.Click += BtnAddRule_Click;
            btnSave.Click += BtnSave_Click;
            btnDeleteHost.Click += BtnDeleteHost_Click;

            var file = System.IO.Path.Combine(AppContext.BaseDirectory, "data.json");
            _ds = new DataStore(file);
            RefreshLists();
        }

        private void RefreshLists()
        {
            listBoxHosts.Items.Clear();
            foreach (var h in _ds.Hosts) listBoxHosts.Items.Add(h.HostName + " (" + (h.Id?.Length > 8 ? h.Id.Substring(0, 8) : h.Id) + ")");

            listBoxIf.Items.Clear();
            foreach (var i in _ds.Interfaces) listBoxIf.Items.Add(i.Identifier + " -> " + (i.HostId?.Length > 8 ? i.HostId.Substring(0, 8) : i.HostId));

            listBoxRules.Items.Clear();
            foreach (var r in _ds.Rules) listBoxRules.Items.Add(r.Identifier + " -> " + r.Action);
        }

        private void BtnAddHost_Click(object? sender, EventArgs e)
        {
            var existingNames = _ds.Hosts.Select(h => h.HostName).ToArray();
            using var dlg = new HostForm(existingNames);
            var res = dlg.ShowDialog(this);
            if (res == DialogResult.OK && dlg.Host != null)
            {
                // ensure unique HostName again
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

            // remove interfaces
            foreach (var it in impacted)
            {
                _ds.Interfaces.Remove(it);
            }

            // remove host
            _ds.Hosts.Remove(host);
            _ds.Save();
            RefreshLists();
        }

        private void BtnAddIf_Click(object? sender, EventArgs e)
        {
            if (!_ds.Hosts.Any()) { MessageBox.Show("Add a host first"); return; }
            var h = _ds.Hosts.First();
            var nif = new NetworkInterface { Identifier = "eth" + (_ds.Interfaces.Count + 1), HostId = h.Id };
            _ds.Interfaces.Add(nif);
            h.InterfaceIds.Add(nif.Id);
            _ds.Save();
            RefreshLists();
        }

        private void BtnAddRule_Click(object? sender, EventArgs e)
        {
            var r = new FirewallRule { Identifier = "rule-" + (_ds.Rules.Count + 1) };
            _ds.Rules.Add(r);
            _ds.Save();
            RefreshLists();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _ds.Save();
            MessageBox.Show("Saved");
        }
    }
}
