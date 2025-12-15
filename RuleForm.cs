using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace Projet_Victor_c_
{
    public class RuleForm : Form
    {
        private TextBox txtIdentifier;
        private TextBox txtDescription;
        private ComboBox cbAction;
        private TextBox txtProgram;
        private ComboBox cbPortType;
        private TextBox txtLocalPorts;
        private TextBox txtRemotePorts;
        private TextBox txtLocalAddress;
        private TextBox txtRemoteAddress;
        private TextBox txtTag;
        private CheckBox chkEnabled;
        private CheckedListBox clbInterfaces;
        private Button btnOk, btnCancel;

        private Func<string,bool> _isIdentifierUsed;
        private FirewallRule _editingRule;

        public FirewallRule Rule { get; private set; }
        public string[] SelectedInterfaceIds { get; private set; }

        //constructeur du firewall rule 
        public RuleForm(NetworkInterface[] interfaces, Func<string,bool> isIdentifierUsed, FirewallRule ruleToEdit = null)
        {
            _isIdentifierUsed = isIdentifierUsed;
            _editingRule = ruleToEdit;
            Text = ruleToEdit == null ? "Add Firewall Rule" : "Edit Firewall Rule";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            ClientSize = new Size(700, 560);
            StartPosition = FormStartPosition.CenterParent;

            // y = hauteur qu'on modifie au fur et à mesure
            int y = 10;
            var lblId = new Label { Text = "Identifier:", Location = new Point(10, y), AutoSize = true };
            txtIdentifier = new TextBox { Location = new Point(120, y), Width = 540 };

            y += 30;
            var lblDesc = new Label { Text = "Description (optional):", Location = new Point(10, y), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(120, y), Width = 540, Height = 50, Multiline = true };

            y += 60;
            var lblAction = new Label { Text = "Action:", Location = new Point(10, y), AutoSize = true };
            cbAction = new ComboBox { Location = new Point(120, y), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cbAction.Items.AddRange(new string[] { "Allow", "Block" }); cbAction.SelectedIndex = 0;

            var lblProgram = new Label { Text = "Program (path/service):", Location = new Point(260, y), AutoSize = true };
            txtProgram = new TextBox { Location = new Point(420, y), Width = 240 };

            y += 30;
            var lblPortType = new Label { Text = "Port type:", Location = new Point(10, y), AutoSize = true };
            cbPortType = new ComboBox { Location = new Point(120, y), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cbPortType.Items.AddRange(new string[] { "TCP", "UDP" }); cbPortType.SelectedIndex = 0;

            var lblLocalPorts = new Label { Text = "Local ports:", Location = new Point(260, y), AutoSize = true };
            txtLocalPorts = new TextBox { Location = new Point(340, y), Width = 320 };

            y += 30;
            var lblRemotePorts = new Label { Text = "Remote ports:", Location = new Point(10, y), AutoSize = true };
            txtRemotePorts = new TextBox { Location = new Point(120, y), Width = 540 };

            y += 30;
            var lblLocalAddr = new Label { Text = "Local address (or range):", Location = new Point(10, y), AutoSize = true };
            txtLocalAddress = new TextBox { Location = new Point(180, y), Width = 480 };

            y += 30;
            var lblRemoteAddr = new Label { Text = "Remote address (or range):", Location = new Point(10, y), AutoSize = true };
            txtRemoteAddress = new TextBox { Location = new Point(180, y), Width = 480 };

            y += 30;
            var lblTag = new Label { Text = "Tag:", Location = new Point(10, y), AutoSize = true };
            txtTag = new TextBox { Location = new Point(120, y), Width = 200 };

            var lblEnabled = new Label { Text = "Enabled:", Location = new Point(340, y), AutoSize = true };
            chkEnabled = new CheckBox { Location = new Point(410, y), Checked = true };

            y += 40;
            var lblIf = new Label { Text = "Attach to interfaces:", Location = new Point(10, y), AutoSize = true };
            clbInterfaces = new CheckedListBox { Location = new Point(10, y + 20), Width = 660 };

            
            int availableForList = this.ClientSize.Height - (clbInterfaces.Top + 90);
            clbInterfaces.Height = Math.Max(80, Math.Min(300, availableForList));

            foreach (var i in interfaces)
            {
                clbInterfaces.Items.Add(new CbItem(i.Identifier + " (host: " + (i.HostId?.Substring(0,8) ?? "") + ")", i.Id));
            }

            // si on edite, on préremplit les champs
            if (_editingRule != null)
            {
                txtIdentifier.Text = _editingRule.Identifier;
                txtDescription.Text = _editingRule.Description ?? "";
                cbAction.SelectedItem = _editingRule.Action == RuleAction.Block ? "Block" : "Allow";
                txtProgram.Text = _editingRule.Program ?? "";
                cbPortType.SelectedItem = _editingRule.PortType == PortType.UDP ? "UDP" : "TCP";
                txtLocalPorts.Text = _editingRule.LocalPorts ?? "";
                txtRemotePorts.Text = _editingRule.RemotePorts ?? "";
                txtLocalAddress.Text = _editingRule.LocalAddress ?? "";
                txtRemoteAddress.Text = _editingRule.RemoteAddress ?? "";
                txtTag.Text = _editingRule.Tag ?? "";
                chkEnabled.Checked = _editingRule.Enabled;
            }

            btnOk = new Button { Text = "OK", Size = new Size(80, 27) };
            btnCancel = new Button { Text = "Cancel", Size = new Size(80, 27) };

            int buttonsTop = clbInterfaces.Bottom + 10;
            if (buttonsTop + 40 > this.ClientSize.Height) buttonsTop = this.ClientSize.Height - 50;
            btnOk.Location = new Point(this.ClientSize.Width - 180, buttonsTop);
            btnCancel.Location = new Point(this.ClientSize.Width - 90, buttonsTop);

            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Controls.AddRange(new Control[] { lblId, txtIdentifier, lblDesc, txtDescription, lblAction, cbAction, lblProgram, txtProgram, lblPortType, cbPortType, lblLocalPorts, txtLocalPorts, lblRemotePorts, txtRemotePorts, lblLocalAddr, txtLocalAddress, lblRemoteAddr, txtRemoteAddress, lblTag, txtTag, lblEnabled, chkEnabled, lblIf, clbInterfaces, btnOk, btnCancel });

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        }

        public void PreselectInterfaces(IEnumerable<string> interfaceIds)
        {
            if (interfaceIds == null) return;
            var set = new HashSet<string>(interfaceIds);
            for (int i = 0; i < clbInterfaces.Items.Count; i++)
            {
                var item = (CbItem)clbInterfaces.Items[i];
                if (set.Contains(item.Value)) clbInterfaces.SetItemChecked(i, true);
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            var id = txtIdentifier.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(id)) { MessageBox.Show(this, "Identifier is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (id.Length > 200) { MessageBox.Show(this, "Identifier too long.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            //Si on edite on peut laisser le meme id
            if (_isIdentifierUsed(id) && _editingRule == null) { MessageBox.Show(this, "Identifier already used.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var desc = txtDescription.Text ?? "";
            if (desc.Length > 200) { MessageBox.Show(this, "Description too long (200 chars).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var action = cbAction.SelectedItem as string ?? "Allow";
            var prog = txtProgram.Text ?? "";
            var portType = cbPortType.SelectedItem as string ?? "TCP";
            var localPorts = txtLocalPorts.Text ?? "";
            var remotePorts = txtRemotePorts.Text ?? "";
            var localAddr = txtLocalAddress.Text ?? "";
            var remoteAddr = txtRemoteAddress.Text ?? "";
            var tag = txtTag.Text ?? "";
            var enabled = chkEnabled.Checked;

            // basic ports format validation
            if (!string.IsNullOrWhiteSpace(localPorts) && !IsValidPortSpecification(localPorts)) { MessageBox.Show(this, "Invalid local ports format.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!string.IsNullOrWhiteSpace(remotePorts) && !IsValidPortSpecification(remotePorts)) { MessageBox.Show(this, "Invalid remote ports format.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var selected = new List<string>();
            for (int i = 0; i < clbInterfaces.Items.Count; i++)
            {
                if (clbInterfaces.GetItemChecked(i))
                {
                    selected.Add(((CbItem)clbInterfaces.Items[i]).Value);
                }
            }

            if (_editingRule != null)
            {
                _editingRule.Identifier = id;
                _editingRule.Description = desc;
                _editingRule.Action = action == "Block" ? RuleAction.Block : RuleAction.Allow;
                _editingRule.Program = prog;
                _editingRule.PortType = portType == "UDP" ? PortType.UDP : PortType.TCP;
                _editingRule.LocalPorts = localPorts;
                _editingRule.RemotePorts = remotePorts;
                _editingRule.LocalAddress = localAddr;
                _editingRule.RemoteAddress = remoteAddr;
                _editingRule.Tag = tag;
                _editingRule.Enabled = enabled;

                Rule = _editingRule;
            }
            else
            {
                Rule = new FirewallRule
                {
                    Identifier = id,
                    Description = desc,
                    Action = action == "Block" ? RuleAction.Block : RuleAction.Allow,
                    Program = prog,
                    PortType = portType == "UDP" ? PortType.UDP : PortType.TCP,
                    LocalPorts = localPorts,
                    RemotePorts = remotePorts,
                    LocalAddress = localAddr,
                    RemoteAddress = remoteAddr,
                    Tag = tag,
                    Enabled = enabled
                };
            }

            SelectedInterfaceIds = selected.ToArray();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private const int MinPort = 0;
        private const int MaxPort = 65535;

        private bool IsValidPortSpecification(string spec)
        {
            if (string.IsNullOrWhiteSpace(spec))
            {
                return false;
            }

            foreach (string token in spec.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string value = token.Trim();

                if (value.Contains('-'))
                {
                    string[] range = value.Split('-');

                    if (range.Length != 2 ||
                        !int.TryParse(range[0], out int start) ||
                        !int.TryParse(range[1], out int end) ||
                        !IsValidPort(start) ||
                        !IsValidPort(end) ||
                        start > end)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!int.TryParse(value, out int port) || !IsValidPort(port))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsValidPort(int port)
        {
            return port >= MinPort && port <= MaxPort;
        }

        class CbItem { public string Text { get; } public string Value { get; } public CbItem(string t, string v) { Text = t; Value = v; } public override string ToString() => Text; }
    }
}
