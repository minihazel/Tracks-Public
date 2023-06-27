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
using System.Management;
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

        splashscreen Splashscreen = new splashscreen();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetGetConnection(
        [MarshalAs(UnmanagedType.LPTStr)] string localName,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
        ref int length);

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
                JObject json = new JObject();
                json["Tabs"] = new JArray();
                string output = JsonConvert.SerializeObject(json, Formatting.Indented);
                File.WriteAllText(layoutConfig, output);

                Application.Restart();
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

            if (_tabs.Count > 0)
            {
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
                    btn.FlatStyle = FlatStyle.Flat;
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

                Button lastBtn = this.Controls.OfType<Button>().LastOrDefault();

                if (lastBtn != null)
                {
                    Button settingsBtn = new Button();
                    settingsBtn.Name = "tab_settings";
                    settingsBtn.Text = "SETTINGS";
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
            }
            else
            {
                Button settingsBtn = new Button();
                settingsBtn.Name = "tab_settings";
                settingsBtn.Text = "SETTINGS";
                settingsBtn.Size = new Size(180, 35);
                settingsBtn.Location = new Point(15, 25);
                settingsBtn.Visible = true;
                settingsBtn.MouseDown += new MouseEventHandler(btn_MouseDown);
                settingsBtn.FlatAppearance.BorderSize = 1;
                settingsBtn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                settingsBtn.FlatStyle = FlatStyle.Flat;
                settingsBtn.Cursor = Cursors.Hand;

                int spacing = 10;
                int panelWidth = this.ClientSize.Width - settingsBtn.Size.Width - spacing - 20;
                int panelHeight = this.ClientSize.Height - 30;

                fullWidth = panelWidth;
                fullHeight = panelHeight;

                fullLocX = settingsBtn.Location.X + settingsBtn.Size.Width + spacing;
                fullLocY = 25;

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
            textBarText.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
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
            textBarName.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
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
            textBarPath.Font = new Font("Bahnschrift Light", 10, FontStyle.Regular);
            textBarPath.Size = new Size(420, 35);
            textBarPath.Location = new Point(15, 185);
            textBarPath.Visible = true;
            textBarPath.KeyDown += new KeyEventHandler(textBar_KeyDown);
            textBarPath.DragEnter += new DragEventHandler(textBarPath_DragEnter);
            textBarPath.DragDrop += new DragEventHandler(textBarPath_DragDrop);
            textBarPath.ForeColor = Color.LightGray;
            textBarPath.BackColor = listBackcolor;
            textBarPath.AllowDrop = true;

            // Buttons

            Button browseForFolder = new Button();
            browseForFolder.Name = "settings_browseFolderBtn";
            browseForFolder.Text = "Browse";
            browseForFolder.Size = new Size(100, 30);
            browseForFolder.Location = new Point(textBarPath.Size.Width + 30, textBarPath.Location.Y - 3);
            browseForFolder.Visible = true;
            browseForFolder.MouseDown += new MouseEventHandler(browseForFolder_MouseDown);
            browseForFolder.FlatAppearance.BorderSize = 1;
            browseForFolder.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            browseForFolder.FlatStyle = FlatStyle.Flat;
            browseForFolder.Cursor = Cursors.Hand;
            browseForFolder.DragEnter += new DragEventHandler(browseForFolder_DragEnter);
            browseForFolder.DragDrop += new DragEventHandler(browseForFolder_DragDrop);
            browseForFolder.AllowDrop = true;

            Button confirmBtn = new Button();
            confirmBtn.Name = "settings_confirmBtn";
            confirmBtn.Text = "Add tab";
            confirmBtn.Size = new Size(180, 35);
            confirmBtn.Location = new Point(15, 225);
            confirmBtn.Visible = true;
            confirmBtn.MouseDown += new MouseEventHandler(confirmBtn_MouseDown);
            confirmBtn.FlatAppearance.BorderSize = 1;
            confirmBtn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            confirmBtn.FlatStyle = FlatStyle.Flat;
            confirmBtn.Cursor = Cursors.Hand;

            Button openConfig = new Button();
            openConfig.Name = "settings_openConfig";
            openConfig.Text = "Open configuration file";
            openConfig.Size = new Size(180, 35);
            openConfig.Location = new Point(15, confirmBtn.Location.Y + 100);
            openConfig.Visible = true;
            openConfig.MouseDown += new MouseEventHandler(openConfig_MouseDown);
            openConfig.FlatAppearance.BorderSize = 1;
            openConfig.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            openConfig.FlatStyle = FlatStyle.Flat;
            openConfig.Cursor = Cursors.Hand;

            Button resetConfig = new Button();
            resetConfig.Name = "settings_resetConfig";
            resetConfig.Text = "Reset configuration file";
            resetConfig.Size = new Size(180, 35);
            resetConfig.Location = new Point(15, openConfig.Location.Y + 40);
            resetConfig.Visible = true;
            resetConfig.MouseDown += new MouseEventHandler(resetConfig_MouseDown);
            resetConfig.FlatAppearance.BorderSize = 1;
            resetConfig.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            resetConfig.FlatStyle = FlatStyle.Flat;
            resetConfig.Cursor = Cursors.Hand;

            Button importConfig = new Button();
            importConfig.Name = "settings_importConfig";
            importConfig.Text = "Import";
            importConfig.Size = new Size(355, 35);
            importConfig.Location = new Point(15, resetConfig.Location.Y + 100);
            importConfig.Visible = true;
            importConfig.MouseDown += new MouseEventHandler(importConfig_MouseDown);
            importConfig.FlatAppearance.BorderSize = 1;
            importConfig.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            importConfig.FlatStyle = FlatStyle.Flat;
            importConfig.Cursor = Cursors.Hand;
            importConfig.DragEnter += new DragEventHandler(importConfig_DragEnter);
            importConfig.DragDrop += new DragEventHandler(importConfig_DragDrop);
            importConfig.AllowDrop = true;

            // Control generating

            settingsPanel.Controls.Add(tabsBox);

            settingsPanel.Controls.Add(textBarTextTitle);
            settingsPanel.Controls.Add(textBarText);
            settingsPanel.Controls.Add(textBarNameTitle);
            settingsPanel.Controls.Add(textBarName);
            settingsPanel.Controls.Add(textBarPathTitle);
            settingsPanel.Controls.Add(textBarPath);
            settingsPanel.Controls.Add(browseForFolder);

            settingsPanel.Controls.Add(confirmBtn);
            settingsPanel.Controls.Add(openConfig);
            settingsPanel.Controls.Add(importConfig);
            settingsPanel.Controls.Add(resetConfig);

            // After-gen action

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

        private void browseForFolder_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button browseForFolder = (System.Windows.Forms.Button)sender;
            TextBox textBarPath = browseForFolder.Parent.Controls.Find("settings_textBarPath", false).FirstOrDefault() as TextBox;

            if (browseForFolder != null)
            {
                var dlg = new FolderBrowser();
                dlg.InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (dlg.ShowDialog(this.Handle) == true)
                {
                    string selectedFolder = dlg.ResultPath;

                    textBarPath.Text = selectedFolder;
                }
            }

            b_placeholder.Select();
        }

        private void textBarPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void textBarPath_DragDrop(object sender, DragEventArgs e)
        {
            System.Windows.Forms.TextBox textBarPath = (System.Windows.Forms.TextBox)sender;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string firstFile = files[0];

                if (firstFile != null)
                {
                    textBarPath.Text = firstFile;
                }
            }
        }

        private void browseForFolder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void browseForFolder_DragDrop(object sender, DragEventArgs e)
        {
            System.Windows.Forms.Button browseForFolder = (System.Windows.Forms.Button)sender;
            TextBox textBarPath = browseForFolder.Parent.Controls.Find("settings_textBarPath", false).FirstOrDefault() as TextBox;

            if (browseForFolder != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    string firstFile = files[0];

                    if (firstFile != null)
                    {
                        textBarPath.Text = firstFile;
                    }
                }
            }
        }

        private void openConfig_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button openConfig = (System.Windows.Forms.Button)sender;

            if (openConfig != null)
            {
                bool layoutExists = File.Exists(layoutConfig);

                if (layoutExists)
                    Process.Start(layoutConfig);
            }

            b_placeholder.Select();
        }

        private void resetConfig_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button resetConfig = (System.Windows.Forms.Button)sender;

            if (resetConfig != null)
            {
                if (MessageBox.Show("Are you sure you want to reset your configuration file?\n\nThis is irreversible!", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string jsonContent = File.ReadAllText(layoutConfig);
                    JObject jsonObject = JObject.Parse(jsonContent);
                    JArray tabsArray = (JArray)jsonObject["Tabs"];

                    tabsArray.Clear();
                    string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    File.WriteAllText(layoutConfig, output);

                    clearAllPanels();
                }
            }

            b_placeholder.Select();
        }

        private void importConfig_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button importConfigBtn = (System.Windows.Forms.Button)sender;

            OpenFileDialog opener = new OpenFileDialog();
            opener.Filter = "JSON files (*.json)|*.json";
            opener.Title = "Select a configuration file (layout.json)";

            if (opener.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = opener.FileName;
                bool pathExists = File.Exists(selectedPath);
                string fileName = Path.GetFileName(selectedPath);

                if (pathExists)
                {
                    if (fileName == "layout.json")
                    {
                        importConfigBtn.Text = $"Importing {selectedPath} ...";
                        importConfigBtn.Size = new Size(350, 35);

                        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                        timer.Interval = 1000;
                        timer.Tick += ((_sender, _e) =>
                        {
                            File.Copy(selectedPath, layoutConfig, true);
                            clearAllPanels();

                            timer.Stop();
                            timer.Dispose();
                        });
                        timer.Start();
                    }
                }
            }
        }

        private void importConfig_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void importConfig_DragDrop(object sender, DragEventArgs e)
        {
            System.Windows.Forms.Button importConfigBtn = (System.Windows.Forms.Button)sender;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string firstFile = files[0];
                string layoutFile = Path.GetFileName(firstFile);

                if (layoutFile == "layout.json")
                {
                    bool layoutExists = File.Exists(layoutFile);
                    if (layoutExists)
                    {
                        if (MessageBox.Show($"Detected configuration file\n\n\"{firstFile}\"\n\nWould you like to import and refresh?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // File.Copy(firstFile, layoutConfig, true);
                            // clearAllPanels();

                            importConfigBtn.Text = $"Importing {firstFile} ...";
                            importConfigBtn.Size = new Size(350, 35);

                            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                            timer.Interval = 1000;
                            timer.Tick += ((_sender, _e) =>
                            {
                                File.Copy(firstFile, layoutConfig, true);
                                clearAllPanels();

                                timer.Stop();
                                timer.Dispose();
                            });
                            timer.Start();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("We only allow \"layout.json\" configurations, please try again!", this.Text, MessageBoxButtons.OK);
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
                        case "add tab":
                            confirmBtn.Text = "Remove";
                            break;

                        case "remove":
                            confirmBtn.Text = "Edit";
                            textBarNameTitle.Text = "Tab ID  (used for checking and verification)";

                            if (tabsBox.SelectedIndex < 0 && tabsBox.Text == "")
                            {
                                textBarText.Enabled = false;
                                textBarPath.Enabled = false;
                            }

                            textBarName.Enabled = false;

                            break;

                        case "edit":
                            confirmBtn.Text = "Clear fields";
                            textBarNameTitle.Text = "Tab ID";

                            textBarText.Enabled = true;
                            textBarPath.Enabled = true;
                            textBarName.Enabled = true;
                            break;

                        case "clear fields":
                            confirmBtn.Text = "Add tab";
                            break;
                    }
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (textBarText.Text == "" && textBarName.Text == "" && textBarPath.Text == "")
                    {
                        MessageBox.Show("Please select an item to edit or remove, or fill out the fields and add a new tab.", this.Text, MessageBoxButtons.OK);

                    }
                    else
                    {
                        switch (confirmBtn.Text.ToLower())
                        {
                            case "edit":
                                int selectedIndex = tabsBox.SelectedIndex;

                                if (selectedIndex >= 0 && selectedIndex < tabsArray.Count)
                                {
                                    JObject selectedItem = tabsArray[selectedIndex] as JObject;

                                    if (selectedItem != null)
                                    {
                                        if (selectedItem["Name"].ToString() == textBarName.Text)
                                        {
                                            selectedItem["Text"] = textBarText.Text;
                                            selectedItem["Path"] = textBarPath.Text;
                                            string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                                            File.WriteAllText(layoutConfig, output);
                                        }
                                    }
                                }

                                textBarNameTitle.Text = "Tab ID  (used for checking and verification)";
                                textBarName.Enabled = false;

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

                                        clearAllPanels();

                                        textBarText.Clear();
                                        textBarName.Clear();
                                        textBarPath.Clear();
                                        tabsBox.SelectedIndex = -1;
                                        textBarText.Select();

                                        break;
                                    }
                                }

                                break;

                            case "add tab":

                                if (tabsArray.Count == 0)
                                {
                                    JObject newTab = new JObject();
                                    newTab["Text"] = textBarText.Text;

                                    if (textBarName.Text.StartsWith("tab_"))
                                    {
                                        newTab["Name"] = textBarName.Text;
                                    }
                                    else
                                    {
                                        newTab["Name"] = $"tab_{textBarName.Text}";
                                    }

                                    newTab["Path"] = textBarPath.Text;

                                    tabsArray.Add(newTab);
                                    string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                                    File.WriteAllText(layoutConfig, output);

                                    clearAllPanels();

                                    break;
                                }
                                else
                                {

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
                                            newTab["Path"] = textBarPath.Text;

                                            if (textBarName.Text.StartsWith("tab_"))
                                            {
                                                newTab["Name"] = textBarName.Text;
                                            }
                                            else
                                            {
                                                newTab["Name"] = $"tab_{textBarName.Text}";
                                            }

                                            tabsArray.Add(newTab);
                                            string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                                            File.WriteAllText(layoutConfig, output);

                                            clearAllPanels();

                                            break;
                                        }
                                    }
                                }

                                break;

                            case "clear fields":
                                textBarText.Clear();
                                textBarName.Clear();
                                textBarPath.Clear();
                                tabsBox.SelectedIndex = -1;
                                textBarText.Select();

                                break;
                        }
                    }
                }
            }
        }

        private void selectFirstTab()
        {
            string layout_content = File.ReadAllText(layoutConfig);
            JObject _layout = JObject.Parse(layout_content);
            JArray _tabs = (JArray)_layout["Tabs"];

            if (_tabs.Count > 0)
            {
                string firstMatch = _tabs[0]["Name"].ToString();
                Button btn = this.Controls.Find(firstMatch, false).FirstOrDefault() as Button;

                if (btn != null)
                {
                    btn.FlatAppearance.BorderColor = Color.DodgerBlue;
                    btn.PerformClick();
                }
            }
            else
            {
                Button tabSettingsBtn = this.Controls.Find("tab_settings", false).FirstOrDefault() as Button;
                
                // Button btn = this.Controls.OfType<Button>().LastOrDefault();

                if (tabSettingsBtn != null)
                {
                    tabSettingsBtn.PerformClick();
                    tabSettingsBtn.FlatAppearance.BorderColor = Color.DodgerBlue;
                    tabSettingsBtn.ForeColor = Color.DodgerBlue;
                }
            }
        }

        private void clearAllPanels()
        {
            foreach (Control component in this.Controls)
            {
                for (int i = this.Controls.Count - 1; i >= 0; i--)
                {
                    Control selected = this.Controls[i] as Control;
                    if (selected != null)
                    {
                        try
                        {
                            this.Controls.RemoveAt(i);
                            selected.Dispose();
                        }
                        catch (Exception err)
                        {
                            Debug.WriteLine($"ERROR: {err.ToString()}");
                            MessageBox.Show($"Oops! It seems like we received an error. If you're uncertain what it\'s about, please message the developer with a screenshot:\n\n{err.ToString()}", this.Text, MessageBoxButtons.OK);
                        }
                    }
                }

                /*
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

                if (component is Button btn)
                {
                    for (int i = this.Controls.Count - 1; i >= 0; i--)
                    {
                        Button selected = this.Controls[i] as Button;

                        if (selected != null)
                        {
                            try
                            {
                                this.Controls.RemoveAt(i);
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
                */
            }

            drawLayout();
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

                if (btn.Text.ToLower() == "settings" && btn.Name.ToLower() == "tab_settings")
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

                string[] acceptedExtensions = { ".mp3", ".wav" };
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

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("", this.Text, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {

            }
        }
    }
}
