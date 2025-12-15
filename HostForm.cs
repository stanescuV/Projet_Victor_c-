using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Projet_Victor_c_
{
    public class HostForm : Form
    {
        private TextBox txtName;
        private ComboBox cbOS;
        private TextBox txtDesc;
        private TextBox txtTags;
        private Button btnOk;
        private Button btnCancel;
        private string[] _existingNames;

        private Host _editingHost;

        public Host Host { get; private set; }

        // Constructor for creating new host
        public HostForm(string[] existingNames) : this(existingNames, null) { }

        // Constructor for editing existing host
        public HostForm(string[] existingNames, Host hostToEdit)
        {
            _existingNames = existingNames ?? Array.Empty<string>();
            _editingHost = hostToEdit;

            Text = hostToEdit == null ? "Add Host" : "Edit Host";
            ClientSize = new Size(400, 320);
            StartPosition = FormStartPosition.CenterParent;

            var lblName = new Label { Text = "Host name:", Location = new Point(10, 10), AutoSize = true };
            txtName = new TextBox { Location = new Point(10, 30), Width = 360 };

            var lblOS = new Label { Text = "OS:", Location = new Point(10, 60), AutoSize = true };
            cbOS = new ComboBox { Location = new Point(10, 80), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cbOS.Items.AddRange(new string[] { "Windows", "Linux", "Mac", "Other" });
            cbOS.SelectedIndex = 0;

            var lblDesc = new Label { Text = "Description (optional):", Location = new Point(10, 110), AutoSize = true };
            txtDesc = new TextBox { Location = new Point(10, 130), Width = 360, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };

            var lblTags = new Label { Text = "Tags (comma separated):", Location = new Point(10, 220), AutoSize = true };
            txtTags = new TextBox { Location = new Point(10, 240), Width = 360 };

            btnOk = new Button { Text = "OK", Location = new Point(200, 270), Size = new Size(80, 27) };
            btnCancel = new Button { Text = "Cancel", Location = new Point(290, 270), Size = new Size(80, 27) };

            Controls.AddRange(new Control[] { lblName, txtName, lblOS, cbOS, lblDesc, txtDesc, lblTags, txtTags, btnOk, btnCancel });

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // If editing, pre-fill values
            if (_editingHost != null)
            {
                txtName.Text = _editingHost.HostName;
                cbOS.SelectedItem = _editingHost.OS.ToString();
                txtDesc.Text = _editingHost.Description ?? "";
                txtTags.Text = string.Join(",", (_editingHost.Tags != null) ? _editingHost.Tags : new System.Collections.Generic.List<string>());

                // when editing, exclude current name from uniqueness checks
                _existingNames = _existingNames.Where(n => !string.Equals(n, _editingHost.HostName, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            var name = txtName.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(this, "Host name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (name.Length > 200) // arbitrary limit
            {
                MessageBox.Show(this, "Host name too long.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_existingNames.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(this, "Host name already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var desc = txtDesc.Text ?? "";
            if (desc.Length > 2000)
            {
                MessageBox.Show(this, "Description too long (2000 chars).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tags = (txtTags.Text ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim()).Where(t => t.Length > 0).ToList();

            var os = OSKind.Windows;
            var sel = cbOS.SelectedItem as string;
            if (sel == "Windows") os = OSKind.Windows;
            else if (sel == "Linux") os = OSKind.Linux;
            else if (sel == "Mac") os = OSKind.Mac;
            else os = OSKind.Other;

            if (_editingHost != null)
            {
                // update existing host
                _editingHost.HostName = name;
                _editingHost.OS = os;
                _editingHost.Description = desc;
                _editingHost.Tags = tags;
                Host = _editingHost;
            }
            else
            {
                Host = new Host
                {
                    HostName = name,
                    OS = os,
                    Description = desc,
                    Tags = tags
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeComponent()
        {

        }
    }
}
