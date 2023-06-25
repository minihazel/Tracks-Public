using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutCustomization
{
    public partial class mainForm : Form
    {
        public string currentDir = Environment.CurrentDirectory;
        public string layoutConfig;

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            layoutConfig = Path.Combine(currentDir, "layout.json");
            if (File.Exists(layoutConfig))
            {
                drawLayout();
            }
            else
            {
                this.Text = "Layout.json doesn\'t exist";
            }

            b_placeholder.Select();
        }

        public void drawLayout()
        {
            string layout_content = File.ReadAllText(layoutConfig);
            JObject _layout = JObject.Parse(layout_content);
            JArray _tabs = (JArray)_layout["Tabs"];

            for (int i = 0; i < _tabs.Count; i++)
            {
                JObject property = (JObject)_tabs[i];
                Debug.WriteLine(property);

                string property_text = property["Text"].ToString();
                string property_name = property["Name"].ToString();

                Button btn = new Button();
                btn.Name = property_name;
                btn.Text = property_text;
                btn.Size = new Size(180, 35);
                btn.Location = new Point(15, 25 + (i * 40));
                btn.Visible = true;
                btn.MouseDown += new MouseEventHandler(btn_MouseDown);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                btn.FlatStyle= FlatStyle.Flat;
                btn.Cursor = Cursors.Hand;

                int spacing = 10;
                int panelWidth = this.ClientSize.Width - btn.Size.Width - spacing - 20;
                int panelHeight = this.ClientSize.Height - 30;

                Panel _region = new Panel();
                _region.Name = $"{property_name}_region";
                _region.Size = new Size(panelWidth, panelHeight);
                _region.BorderStyle = BorderStyle.FixedSingle;
                _region.Location = new Point(btn.Location.X + btn.Size.Width + spacing, 25);
                _region.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

                this.Controls.Add(btn);
                this.Controls.Add(_region);
            }   
        }

        private void btn_MouseDown(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;

            if (btn.Text != "")
            {
                foreach (Control otherButton in this.Controls)
                {
                    if (otherButton is Button otherBtn)
                    {
                        otherBtn.ForeColor = Color.LightGray;
                        otherBtn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    }
                }
                btn.ForeColor = Color.DodgerBlue;
                btn.FlatAppearance.BorderColor = Color.DodgerBlue;
            }

            b_placeholder.Select();
        }

    }
}
