using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutCustomization
{
    public partial class mainForm : Form
    {
        public Color listBackcolor = Color.FromArgb(255, 28, 28, 28);
        public Color listSelectedcolor = Color.FromArgb(255, 40, 40, 40);
        public Color listHovercolor = Color.FromArgb(255, 35, 35, 35);

        public Color borderColor = Color.FromArgb(255, 100, 100, 100);
        public Color borderColorActive = Color.DodgerBlue;
        public bool isLoaded = false;

        List<Label> regLabels = new List<Label>();
        List<Label> v4Labels = new List<Label>();
        List<Label> v2Labels = new List<Label>();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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
                this.Size = new Size(796, 615);
                drawLayout();
            }
            else
            {
                this.Text = "Layout.json doesn\'t exist";
            }

            b_placeholder.Select();
        }

        public async void drawLayout()
        {
            List<Label> items = new List<Label>();

            int fullWidth = 0;
            int fullHeight = 0;
            int fullLocX = 0;
            int fullLocY = 0;
            int lastInt = 0;

            string layout_content = File.ReadAllText(layoutConfig);
            JObject _layout = JObject.Parse(layout_content);
            JArray _tabs = (JArray)_layout["Tabs"];

            for (int i = 0; i < _tabs.Count; i++)
            {
                JObject property = (JObject)_tabs[i];

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
                _region.AutoScroll = false;
                _region.HorizontalScroll.Enabled = false;
                _region.HorizontalScroll.Visible = false;
                _region.HorizontalScroll.Maximum = 0;
                _region.AutoScroll = true;

                this.Controls.Add(btn);
                this.Controls.Add(_region);

                fullWidth = panelWidth;
                fullHeight = panelHeight;

                fullLocX = btn.Location.X + btn.Size.Width + spacing;
                fullLocY = 25;
                lastInt = i;
            }

            Button lastBtn = this.Controls.OfType<Button>().Last();

            if (lastBtn != null)
            {
                Button settingsBtn = new Button();
                settingsBtn.Name = "tab_settings";
                settingsBtn.Text = "Settings";
                settingsBtn.Size = new Size(180, 35);
                settingsBtn.Location = new Point(15, lastBtn.Location.Y + 40);
                settingsBtn.Visible = true;
                settingsBtn.MouseDown += new MouseEventHandler(btn_MouseDown);
                settingsBtn.FlatAppearance.BorderSize = 1;
                settingsBtn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                settingsBtn.FlatStyle = FlatStyle.Flat;
                settingsBtn.Cursor = Cursors.Hand;

                Panel settingsPanel = new Panel();
                settingsPanel.Name = $"settings_panel_region";
                settingsPanel.Size = new Size(fullWidth, fullHeight);
                settingsPanel.BorderStyle = BorderStyle.FixedSingle;
                settingsPanel.Location = new Point(fullLocX, fullLocY);
                settingsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                settingsPanel.AutoScroll = false;
                settingsPanel.HorizontalScroll.Enabled = false;
                settingsPanel.HorizontalScroll.Visible = false;
                settingsPanel.HorizontalScroll.Maximum = 0;
                settingsPanel.AutoScroll = true;

                this.Controls.Add(settingsBtn);
                this.Controls.Add(settingsPanel);

                fillSettingsTab();
            }

            listTracks();
            selectFirstTab();
        }

        private void fillSettingsTab()
        {
            /*
             *   Filling the settings page;
             *   
             *   - Add combobox to hold the names of all the tabs
             *   - Add textbox to rename the selected tab
             *   - Add button to add new tab/delete selected tab
            */

            Panel settingsPanel = this.Controls.Find("settings_panel_region", false).FirstOrDefault() as Panel;

            ComboBox tabsBox = new ComboBox();
            tabsBox.Name = "settings_tabsBox";
            tabsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tabsBox.Font = new Font("Bahnschrift Light", 11, FontStyle.Regular);
            tabsBox.Size = new Size(230, 35);
            tabsBox.Location = new Point(15, 15);
            tabsBox.Visible = true;
            tabsBox.SelectedIndexChanged += new EventHandler(tabsBox_SelectedIndexChanged);
            tabsBox.Cursor = Cursors.Hand;

            // Settings

            Label textBarTextTitle = new Label();
            textBarTextTitle.Name = "settings_textBarTextTitle";
            textBarTextTitle.Text = "Tab display text";
            textBarTextTitle.AutoSize = true;
            textBarTextTitle.Size = new Size(180, 35);
            textBarTextTitle.Location = new Point(10, 55);
            textBarTextTitle.Visible = true;
            textBarTextTitle.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
            textBarTextTitle.ForeColor = Color.LightGray;
            textBarTextTitle.BackColor = listBackcolor;

            TextBox textBarText = new TextBox();
            textBarText.Name = "settings_textBarText";
            textBarText.BorderStyle = BorderStyle.FixedSingle;
            textBarText.Font = new Font("Bahnschrift Light", 11, FontStyle.Regular);
            textBarText.Size = new Size(230, 35);
            textBarText.Location = new Point(15, 75);
            textBarText.Visible = true;
            textBarText.KeyDown += new KeyEventHandler(textBar_KeyDown);
            textBarText.ForeColor = Color.LightGray;
            textBarText.BackColor = listBackcolor;


            Label textBarNameTitle = new Label();
            textBarNameTitle.Name = "settings_textBarNameTitle";
            textBarNameTitle.Text = "Tab ID";
            textBarNameTitle.AutoSize = true;
            textBarNameTitle.Size = new Size(180, 35);
            textBarNameTitle.Location = new Point(10, 110);
            textBarNameTitle.Visible = true;
            textBarNameTitle.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
            textBarNameTitle.ForeColor = Color.LightGray;
            textBarNameTitle.BackColor = listBackcolor;

            TextBox textBarName = new TextBox();
            textBarName.Name = "settings_textBarName";
            textBarName.BorderStyle = BorderStyle.FixedSingle;
            textBarName.Font = new Font("Bahnschrift Light", 11, FontStyle.Regular);
            textBarName.Size = new Size(230, 35);
            textBarName.Location = new Point(15, 130);
            textBarName.Visible = true;
            textBarName.KeyDown += new KeyEventHandler(textBar_KeyDown);
            textBarName.ForeColor = Color.LightGray;
            textBarName.BackColor = listBackcolor;


            Label textBarPathTitle = new Label();
            textBarPathTitle.Name = "settings_textBarPathTitle";
            textBarPathTitle.Text = "Path to search";
            textBarPathTitle.AutoSize = true;
            textBarPathTitle.Size = new Size(180, 35);
            textBarPathTitle.Location = new Point(10, 165);
            textBarPathTitle.Visible = true;
            textBarPathTitle.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
            textBarPathTitle.ForeColor = Color.LightGray;
            textBarPathTitle.BackColor = listBackcolor;

            TextBox textBarPath = new TextBox();
            textBarPath.Name = "settings_textBarPath";
            textBarPath.BorderStyle = BorderStyle.FixedSingle;
            textBarPath.Font = new Font("Bahnschrift Light", 11, FontStyle.Regular);
            textBarPath.Size = new Size(230, 35);
            textBarPath.Location = new Point(15, 185);
            textBarPath.Visible = true;
            textBarPath.KeyDown += new KeyEventHandler(textBar_KeyDown);
            textBarPath.ForeColor = Color.LightGray;
            textBarPath.BackColor = listBackcolor;

            // Buttons

            Button confirmBtn = new Button();
            confirmBtn.Name = "settings_confirmBtn";
            confirmBtn.Text = "Edit";
            confirmBtn.Size = new Size(180, 35);
            confirmBtn.Location = new Point(15, 225);
            confirmBtn.Visible = true;
            confirmBtn.MouseDown += new MouseEventHandler(confirmBtn_MouseDown);
            confirmBtn.FlatAppearance.BorderSize = 1;
            confirmBtn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            confirmBtn.FlatStyle = FlatStyle.Flat;
            confirmBtn.Cursor = Cursors.Hand;


            settingsPanel.Controls.Add(tabsBox);

            settingsPanel.Controls.Add(textBarTextTitle);
            settingsPanel.Controls.Add(textBarText);
            settingsPanel.Controls.Add(textBarNameTitle);
            settingsPanel.Controls.Add(textBarName);
            settingsPanel.Controls.Add(textBarPathTitle);
            settingsPanel.Controls.Add(textBarPath);

            settingsPanel.Controls.Add(confirmBtn);

            JArray tabsArray = readTabs() as JArray;
            foreach (JObject item in tabsArray)
            {
                tabsBox.Items.Add(item["Text"]);
            }
        }

        private void tabsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox tabsBox = (System.Windows.Forms.ComboBox)sender;

            if (tabsBox != null)
            {
                if (tabsBox.SelectedIndex > -1)
                {
                    JArray tabsArray = readTabs() as JArray;
                    JToken selectedItem = tabsArray[tabsBox.SelectedIndex];

                    TextBox textBarText = tabsBox.Parent.Controls.Find("settings_textBarText", false).FirstOrDefault() as TextBox;
                    TextBox textBarName = tabsBox.Parent.Controls.Find("settings_textBarName", false).FirstOrDefault() as TextBox;
                    TextBox textBarPath = tabsBox.Parent.Controls.Find("settings_textBarPath", false).FirstOrDefault() as TextBox;

                    textBarText.Text = selectedItem["Text"].ToString();
                    textBarName.Text = selectedItem["Name"].ToString();
                    textBarPath.Text = selectedItem["Path"].ToString();
                }
            }
        }

        private void textBar_KeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Forms.TextBox textBar = (System.Windows.Forms.TextBox)sender;

            if (textBar != null)
            {
                Button confirmBtn = textBar.Parent.Controls.Find("settings_confirmBtn", false).FirstOrDefault() as Button;

                TextBox textBarText = textBar.Parent.Controls.Find("settings_textBarText", false).FirstOrDefault() as TextBox;
                TextBox textBarName = textBar.Parent.Controls.Find("settings_textBarName", false).FirstOrDefault() as TextBox;
                TextBox textBarPath = textBar.Parent.Controls.Find("settings_textBarPath", false).FirstOrDefault() as TextBox;

                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;

                    if (textBar.Name == "settings_textBarText")
                    {
                        textBarName.Select();
                    }
                    else if (textBar.Name == "settings_textBarName")
                    {
                        textBarPath.Select();
                    }
                    else if (textBar.Name == "settings_textBarPath")
                    {
                        confirmBtn.Select();
                    }
                }
            }
        }

        private void confirmBtn_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button confirmBtn = (System.Windows.Forms.Button)sender;

            string jsonContent = File.ReadAllText(layoutConfig);
            JObject jsonObject = JObject.Parse(jsonContent);
            JArray tabsArray = (JArray)jsonObject["Tabs"];

            ComboBox tabsBox = confirmBtn.Parent.Controls.Find("settings_tabsBox", false).FirstOrDefault() as ComboBox;
            TextBox textBarText = confirmBtn.Parent.Controls.Find("settings_textBarText", false).FirstOrDefault() as TextBox;
            TextBox textBarName = confirmBtn.Parent.Controls.Find("settings_textBarName", false).FirstOrDefault() as TextBox;
            TextBox textBarPath = confirmBtn.Parent.Controls.Find("settings_textBarPath", false).FirstOrDefault() as TextBox;

            Label textBarNameTitle = confirmBtn.Parent.Controls.Find("settings_textBarNameTitle", false).FirstOrDefault() as Label;

            if (confirmBtn != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    switch (confirmBtn.Text.ToLower())
                    {
                        case "edit":
                            confirmBtn.Text = "Remove";
                            textBarNameTitle.Text = "Tab ID";
                            break;
                        case "remove":
                            confirmBtn.Text = "Add";
                            textBarNameTitle.Text = "Tab ID";
                            break;
                        case "add":
                            confirmBtn.Text = "Clear fields";
                            textBarNameTitle.Text = "Tab ID";
                            break;
                        case "clear fields":
                            confirmBtn.Text = "Edit";
                            textBarNameTitle.Text = "Tab ID  (used for checking and verification)";
                            break;
                    }
                }
                else if (e.Button == MouseButtons.Left)
                {
                    switch (confirmBtn.Text.ToLower())
                    {
                        case "edit":


                            break;

                        case "remove":

                            foreach (JObject tab in tabsArray)
                            {
                                string tabText = tab["Text"].ToString();
                                string tabName = tab["Name"].ToString();
                                string tabPath = tab["Path"].ToString();

                                string _text = textBarText.Text;
                                string _name = textBarName.Text;
                                string _path = textBarPath.Text;

                                if (tabText == _text &&
                                    tabName == _name &&
                                    tabPath == _path)
                                {
                                    tabsArray.Remove(tab);
                                    string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                                    File.WriteAllText(layoutConfig, output);
                                    break;
                                }
                            }

                            break;

                        case "add":

                            foreach (JObject tab in tabsArray)
                            {
                                string tabName = tab["Name"].ToString();
                                string _name = textBarName.Text;

                                bool detected = tabsArray.Any(obj => obj["Name"].ToString() == _name);
                                bool doesNotContain = !detected;

                                if (doesNotContain)
                                {
                                    JObject newTab = new JObject();
                                    newTab["Text"] = textBarText.Text;
                                    newTab["Name"] = textBarName.Text;
                                    newTab["Path"] = textBarPath.Text;

                                    tabsArray.Add(newTab);
                                    string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                                    File.WriteAllText(layoutConfig, output);
                                    break;
                                }
                            }

                            break;

                        case "clear fields":
                            textBarText.Clear();
                            textBarName.Clear();
                            textBarPath.Clear();

                            tabsBox.Text = "";

                            textBarText.Select();

                            break;
                    }
                }
            }
        }

        private void selectFirstTab()
        {
            string layout_content = File.ReadAllText(layoutConfig);
            JObject _layout = JObject.Parse(layout_content);
            JArray _tabs = (JArray)_layout["Tabs"];

            string firstMatch = _tabs[0]["Name"].ToString();
            Button btn = this.Controls.Find(firstMatch, false).FirstOrDefault() as Button;

            if (btn != null)
            {
                btn.FlatAppearance.BorderColor = Color.DodgerBlue;
                btn.PerformClick();
            }
        }

        private void clearAllPanels()
        {
            foreach (Control component in this.Controls)
            {
                if (component is Panel trackPanel)
                {
                    for (int i = trackPanel.Controls.Count - 1; i >= 0; i--)
                    {
                        Label selected = trackPanel.Controls[i] as Label;
                        if (selected != null)
                        {
                            try
                            {
                                trackPanel.Controls.RemoveAt(i);
                                selected.Dispose();
                            }
                            catch (Exception err)
                            {
                                Debug.WriteLine($"ERROR: {err.ToString()}");
                                MessageBox.Show($"Oops! It seems like we received an error. If you're uncertain what it\'s about, please message the developer with a screenshot:\n\n{err.ToString()}", this.Text, MessageBoxButtons.OK);
                            }
                        }
                    }
                }
            }
        }

        private void deselectTrackPanel(Panel panel)
        {
            foreach (Control component in panel.Controls)
            {
                if (component is Label lbl)
                {
                    if (lbl.BackColor != listHovercolor)
                    {
                        lbl.BackColor = listBackcolor;
                    }
                }
            }
        }

        private void deselectAllPanels()
        {
            foreach (Control component in this.Controls)
            {
                if (component is Panel trackPanel)
                {
                    foreach (Control track in trackPanel.Controls)
                    {
                        if (track is Label lbl)
                        {
                            lbl.BackColor = listBackcolor;
                        }
                    }
                }
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

                if (btn.Text == "Settings" && btn.Name.ToLower() == "tab_settings")
                {
                    Panel settingsPanel = this.Controls.Find("settings_panel_region", false).FirstOrDefault() as Panel;
                    settingsPanel.BringToFront();
                }
                else
                {
                    Dictionary<string, Panel> buttonPanelMap = ReadLayoutTabs(layoutConfig);
                    if (buttonPanelMap.TryGetValue(btn.Name, out Panel targetPanel))
                    {
                        targetPanel.BringToFront();
                    }
                }

            }

            b_placeholder.Select();
        }

        private Dictionary<string, Panel> ReadLayoutTabs(string layoutConfig)
        {
            string layoutContent = File.ReadAllText(layoutConfig);
            JObject layoutObject = JObject.Parse(layoutContent);
            JArray tabsArray = (JArray)layoutObject["Tabs"];

            Dictionary<string, Panel> buttonPanelMap = new Dictionary<string, Panel>();

            foreach (JObject item in tabsArray)
            {
                string buttonName = item["Name"].ToString();
                string panelName = buttonName + "_region"; // Assuming panel names have "_panel" suffix

                Panel panel = this.Controls.Find(panelName, true).FirstOrDefault() as Panel;
                if (panel != null)
                {
                    buttonPanelMap.Add(buttonName, panel);
                }
            }

            return buttonPanelMap;
        }

        private Dictionary<Button, (string Name, string Text, string Path, Panel Panel)> FindButtonProperties(string filePath, string buttonPrefix, string panelSuffix)
        {
            string jsonContent = File.ReadAllText(filePath);
            JObject jsonObject = JObject.Parse(jsonContent);
            JArray tabsArray = (JArray)jsonObject["Tabs"];

            Dictionary<Button, (string Name, string Text, string Path, Panel Panel)> buttonProperties = new Dictionary<Button, (string Name, string Text, string Path, Panel Panel)>();

            foreach (Button button in this.Controls.OfType<Button>().Where(b => b.Name.StartsWith(buttonPrefix)))
            {
                string buttonName = button.Name;
                JObject matchingItem = tabsArray.FirstOrDefault(item => item["Name"].ToString() == buttonName) as JObject;

                if (matchingItem != null)
                {
                    string name = matchingItem["Name"].ToString();
                    string text = matchingItem["Text"].ToString();
                    string path = matchingItem["Path"].ToString();

                    Panel panel = this.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == buttonName + panelSuffix);

                    buttonProperties.Add(button, (name, text, path, panel));
                }
            }

            return buttonProperties;
        }

        public async Task listTracks()
        {
            List<Label> tracks = new List<Label>();
            Panel foundPanel= null;

            string buttonPrefix = "tab_";
            string panelSuffix = "_region";
            Dictionary<Button, (string Name, string Text, string Path, Panel _panel)> buttonProperties =
                FindButtonProperties(layoutConfig, buttonPrefix, panelSuffix);

            foreach (var tabItem in buttonProperties)
            {
                Button btn = tabItem.Key;
                string name = tabItem.Value.Name;
                string text = tabItem.Value.Text;
                string path = tabItem.Value.Path;
                Panel panel = tabItem.Value._panel;

                foundPanel = panel;

                string[] acceptedExtensions = { ".mp3", ".wav", ".mp4" };
                DirectoryInfo pathInfo = new DirectoryInfo(path);
                FileInfo[] pathFiles = pathInfo.GetFiles()
                    .Where(file => acceptedExtensions.Contains(file.Extension.ToLower()))
                    .OrderByDescending(p => p.CreationTimeUtc)
                    .ToArray();

                int index = 0;

                foreach (var file in pathFiles)
                {
                    Label lbl = new Label();
                    lbl.Text = Path.GetFileName(pathFiles[index].FullName);
                    lbl.AutoSize = false;
                    lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Size = new Size(panel.Size.Width - 4, 25);
                    lbl.Location = new Point(1, 1 + (index * 28));
                    lbl.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
                    lbl.BackColor = listBackcolor;
                    lbl.ForeColor = Color.LightGray;
                    lbl.Margin = new Padding(1, 1, 1, 1);
                    lbl.Cursor = Cursors.Hand;
                    lbl.MouseEnter += new EventHandler(lbl_MouseEnter);
                    lbl.MouseLeave += new EventHandler(lbl_MouseLeave);
                    lbl.MouseDown += new MouseEventHandler(lbl_MouseDown);
                    lbl.MouseDoubleClick += new MouseEventHandler(lbl_MouseDoubleClick);
                    lbl.MouseUp += new MouseEventHandler(lbl_MouseUp);
                    tracks.Add(lbl);

                    index++;
                }

                Control[] allTracks = tracks.ToArray();
                foundPanel.Controls.AddRange(allTracks);

                tracks.Clear();

                /*
                string[] acceptedExtensions = { ".mp3", ".wav", ".mp4" };

                foreach (string extension in acceptedExtensions)
                {
                    DirectoryInfo pathInfo = new DirectoryInfo(path);
                    FileInfo[] pathFiles = pathInfo.GetFiles()
                        .Where(file => acceptedExtensions.Contains(file.Extension.ToLower()))
                        .OrderByDescending(p => p.CreationTimeUtc)
                        .ToArray();

                    for (int i = 0; pathFiles.Length > i; i++)
                    {
                        Label lbl = new Label();
                        lbl.Text = Path.GetFileName(pathFiles[i].FullName);
                        lbl.AutoSize = false;
                        lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                        lbl.Size = new Size(panel.Size.Width - 4, 25);
                        lbl.Location = new Point(1, 1 + (i * 28));
                        lbl.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
                        lbl.BackColor = listBackcolor;
                        lbl.ForeColor = Color.LightGray;
                        lbl.Margin = new Padding(1, 1, 1, 1);
                        lbl.Cursor = Cursors.Hand;
                        lbl.MouseEnter += new EventHandler(lbl_MouseEnter);
                        lbl.MouseLeave += new EventHandler(lbl_MouseLeave);
                        lbl.MouseDown += new MouseEventHandler(lbl_MouseDown);
                        lbl.MouseDoubleClick += new MouseEventHandler(lbl_MouseDoubleClick);
                        lbl.MouseUp += new MouseEventHandler(lbl_MouseUp);

                        await Task.Run(() =>
                        {
                            panel.Controls.Add(lbl);
                        });
                    }
                }
                */
            }

        }

        private void FindButtonAndPerformOperation(JArray tabsArray, Color borderColor, Button selectedButton, string selectedTrack)
        {
            if (selectedButton.FlatAppearance.BorderColor == borderColor)
            {
                string buttonName = selectedButton.Name;
                string buttonText = selectedButton.Text;

                JObject matchingItem = tabsArray.FirstOrDefault(item => item["Name"].ToString() == buttonName && item["Text"].ToString() == buttonText) as JObject;

                if (matchingItem != null)
                {
                    string path = matchingItem["Path"].ToString();
                    string fullPath = Path.Combine(path, selectedTrack);

                    bool pathExists = File.Exists(fullPath);
                    if (pathExists)
                    {
                        Process.Start(fullPath);
                    }
                }
            }
        }

        private Button GetAssociatedButton(Label label)
        {
            string associatedButtonName = "tab_" + label.Name; // Adjust the prefix to match your button names
            return this.Controls.OfType<Button>().FirstOrDefault(b => b.Name == associatedButtonName);
        }

        private JArray readTabs()
        {
            string layout_content = File.ReadAllText(layoutConfig);
            JObject layoutObject = JObject.Parse(layout_content);
            JArray tabs = (JArray)layoutObject["Tabs"];

            return tabs;
        }

        private void lbl_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.BackColor != listSelectedcolor)
                {
                    label.BackColor = listHovercolor;
                }
            }
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.BackColor != listSelectedcolor)
                {
                    label.BackColor = listBackcolor;
                }
            }
        }

        private void lbl_MouseDown(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;

            if (label.Text != "")
            {
                Panel parentPanel = label.Parent as Panel;
                deselectTrackPanel(parentPanel);

                if (label.BackColor == listHovercolor)
                {
                    label.BackColor = listSelectedcolor;
                }
                else
                {
                    label.BackColor = listHovercolor;
                }
            }
        }

        private void lbl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;

            if (label.Text != "")
            {
                Color activeColor = Color.DodgerBlue;
                Button associatedBtn = null;

                foreach (Control component in this.Controls)
                {
                    if (component is Button btn && btn.FlatAppearance.BorderColor == Color.DodgerBlue)
                    {
                        associatedBtn = btn;
                        break;
                    }
                }

                if (associatedBtn != null)
                {
                    JArray tabsArray = readTabs() as JArray;
                    FindButtonAndPerformOperation(tabsArray, activeColor, associatedBtn, label.Text);

                    Panel parentPanel = label.Parent as Panel;
                    deselectTrackPanel(parentPanel);
                    label.BackColor = listSelectedcolor;
                }
            }
        }

        private void lbl_MouseUp(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                // label.BackColor = listHovercolor;
            }
        }
    }
}
