using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace Projet_Victor_c_
{
    public class InterfaceForm : Form
    {
        private TextBox txtIdentifier;
        private TextBox txtDescription;
        private ComboBox cbStatus;
        private TextBox txtMac;
        private TextBox txtIP;
        private TextBox txtMask;
        private TextBox txtGateway;
        private CheckBox chkDhcp;
        private TextBox txtDns1;
        private TextBox txtDns2;
        private ComboBox cbHost;
        private Button btnOk;
        private Button btnCancel;

        private string[] _existingIdsOnHost;
        private Func<string, bool> _isMacUsed;
        private Func<string, bool> _isIpUsed;

        public NetworkInterface Interface { get; private set; }

        public InterfaceForm(Host[] hosts, string[] existingIdsOnHost, Func<string,bool> isMacUsed, Func<string,bool> isIpUsed)
        {
            _existingIdsOnHost = existingIdsOnHost ?? Array.Empty<string>();
            _isMacUsed = isMacUsed;
            _isIpUsed = isIpUsed;

            Text = "Add Network Interface";
            ClientSize = new Size(480, 420);
            StartPosition = FormStartPosition.CenterParent;

            var y = 10;
            var lblHost = new Label { Text = "Host:", Location = new Point(10, y), AutoSize = true };
            cbHost = new ComboBox { Location = new Point(120, y), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var h in hosts) cbHost.Items.Add(new ComboBoxItem(h.HostName, h.Id));
            if (cbHost.Items.Count > 0) cbHost.SelectedIndex = 0;

            y += 30;
            var lblId = new Label { Text = "Identifier:", Location = new Point(10, y), AutoSize = true };
            txtIdentifier = new TextBox { Location = new Point(120, y), Width = 340 };

            y += 30;
            var lblDesc = new Label { Text = "Description (optional):", Location = new Point(10, y), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(120, y), Width = 340, Height = 60, Multiline = true }; 

            y += 70;
            var lblStatus = new Label { Text = "Status:", Location = new Point(10, y), AutoSize = true };
            cbStatus = new ComboBox { Location = new Point(120, y), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cbStatus.Items.AddRange(new string[] { "Up", "Down" }); cbStatus.SelectedIndex = 0;

            y += 30;
            var lblMac = new Label { Text = "MAC Address:", Location = new Point(10, y), AutoSize = true };
            txtMac = new TextBox { Location = new Point(120, y), Width = 200 };

            var lblDhcp = new Label { Text = "DHCP:", Location = new Point(340, y), AutoSize = true };
            chkDhcp = new CheckBox { Location = new Point(390, y) };

            y += 30;
            var lblIP = new Label { Text = "IP Address:", Location = new Point(10, y), AutoSize = true };
            txtIP = new TextBox { Location = new Point(120, y), Width = 200 };

            y += 30;
            var lblMask = new Label { Text = "Subnet Mask/Prefix:", Location = new Point(10, y), AutoSize = true };
            txtMask = new TextBox { Location = new Point(120, y), Width = 200 };

            y += 30;
            var lblGw = new Label { Text = "Gateway:", Location = new Point(10, y), AutoSize = true };
            txtGateway = new TextBox { Location = new Point(120, y), Width = 200 };

            y += 30;
            var lblDns1 = new Label { Text = "DNS Primary:", Location = new Point(10, y), AutoSize = true };
            txtDns1 = new TextBox { Location = new Point(120, y), Width = 200 };

            y += 30;
            var lblDns2 = new Label { Text = "DNS Secondary:", Location = new Point(10, y), AutoSize = true };
            txtDns2 = new TextBox { Location = new Point(120, y), Width = 200 };

            btnOk = new Button { Text = "OK", Location = new Point(300, 360), Size = new Size(80, 27) };
            btnCancel = new Button { Text = "Cancel", Location = new Point(390, 360), Size = new Size(80, 27) };

            Controls.AddRange(new Control[] { lblHost, cbHost, lblId, txtIdentifier, lblDesc, txtDescription, lblStatus, cbStatus, lblMac, txtMac, lblDhcp, chkDhcp, lblIP, txtIP, lblMask, txtMask, lblGw, txtGateway, lblDns1, txtDns1, lblDns2, txtDns2, btnOk, btnCancel });

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            var id = txtIdentifier.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(id)) { MessageBox.Show(this, "Identifier is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (id.Length > 200) { MessageBox.Show(this, "Identifier too long.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var desc = txtDescription.Text ?? "";
            if (desc.Length > 200) { MessageBox.Show(this, "Description too long (200 chars).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var hostId = ((ComboBoxItem)cbHost.SelectedItem)?.Value ?? null;
            if (hostId == null) { MessageBox.Show(this, "Select a host.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // ensure identifier unique on host
            if (_existingIdsOnHost != null && _existingIdsOnHost.Any(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(this, "Identifier already exists on selected host.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mac = txtMac.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(mac))
            {
                if (!IsValidMac(mac)) { MessageBox.Show(this, "Invalid MAC address format.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (_isMacUsed(mac)) { MessageBox.Show(this, "MAC address already used.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            }

            var ip = txtIP.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(ip))
            {
                if (!IPAddress.TryParse(ip, out _)) { MessageBox.Show(this, "Invalid IP address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (_isIpUsed(ip)) { MessageBox.Show(this, "IP address already used.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            }

            var mask = txtMask.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(mask) && !IsValidMask(mask)) { MessageBox.Show(this, "Invalid subnet mask/prefix.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var gw = txtGateway.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(gw) && !IPAddress.TryParse(gw, out _)) { MessageBox.Show(this, "Invalid gateway address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var dns1 = txtDns1.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(dns1) && !IPAddress.TryParse(dns1, out _)) { MessageBox.Show(this, "Invalid DNS primary.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var dns2 = txtDns2.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(dns2) && !IPAddress.TryParse(dns2, out _)) { MessageBox.Show(this, "Invalid DNS secondary.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var status = cbStatus.SelectedItem as string ?? "Down";
            var dhcp = chkDhcp.Checked;

            Interface = new NetworkInterface
            {
                Identifier = id,
                Description = desc,
                Status = status == "Up" ? IfStatus.Up : IfStatus.Down,
                MacAddress = mac,
                IP = ip,
                SubnetMask = mask,
                Gateway = gw,
                DHCP = dhcp,
                DnsPrimary = dns1,
                DnsSecondary = dns2,
                HostId = hostId
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool IsValidMac(string mac)
        {
            var rx = new Regex("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
            var rx2 = new Regex("^[0-9A-Fa-f]{12}$");
            return rx.IsMatch(mac) || rx2.IsMatch(mac);
        }

        private bool IsValidMask(string mask)
        {
            // accept either IP mask or prefix length like /24 or 24
            if (mask.StartsWith("/")) mask = mask.Substring(1);
            if (int.TryParse(mask, out var v)) { return v >= 0 && v <= 128; }
            return IPAddress.TryParse(mask, out _);
        }

        class ComboBoxItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboBoxItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
        }
    }
}
