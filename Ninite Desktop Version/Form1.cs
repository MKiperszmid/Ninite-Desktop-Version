using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace NiniteClone
{
    public partial class Form1 : Form
    {
        private readonly List<string> _programs = new List<string>();
        
        private readonly List<string> _urls = new List<string> { "https://the.earth.li/~sgtatham/putty/latest/w64/putty-64bit-0.70-installer.msi",
            "https://steamcdn-a.akamaihd.net/client/installer/SteamSetup.exe",
            "https://github.com/dewster/lol-mastery-manager-new-client/releases/download/1.2.2/LoLMasteryManagerSetup.msi",
        "https://desktop.githubusercontent.com/releases/0.6.2-e2d9e7b3/GitHubDesktopSetup.exe",
        "http://www.7-zip.org/a/7z1700-x64.exe"};

        private List<ProgressBar> _progressBars = new List<ProgressBar>();
        private List<Label> _labels = new List<Label>();
        private List<CheckBox> _checkBoxes = new List<CheckBox>();
        public Form1()
        {
            InitializeComponent();
            
        }
        
        public void AppInstaller(string path)
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "CMD.exe"
            };

            var extension = Path.GetExtension(path);
            var name = Path.GetFileName(path);
            
            if (extension != null && extension.ToLower() == ".msi")
            {
                startInfo.Arguments = $"/c msiexec /i {name} /qn+ /norestart";
            }
            else if (extension != null && extension.ToLower() == ".exe")
            {
                startInfo.Arguments = $"/c {name} /S /norestart";
            }
            else
            {
                MessageBox.Show($@"Extension {extension} is not supported.", @"Not Supported");
                return;
            }

            var p = new Process {StartInfo = startInfo};
            p.Start();
        }
        public void DownloadApps()
        {
            int index = 0;
            foreach (var program in _programs)
            {
                var called = false;
                var progressBar = progressBar1;
                var lbl = label1;

                while (_urls[index] != program)
                {
                    index++;
                }
                progressBar = _progressBars[index];
                lbl = _labels[index];
                Download(program, progressBar, lbl, called);
                index++;
            }
        }

        public void Download(string program, ProgressBar progressBar, Label lbl, bool called)
        {
            Log("Downloading " + program);
            var wc = new WebClient();
            var name = Path.GetFileName(program);
            wc.DownloadFileAsync(new Uri(program), $"{name}");
            wc.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
            {
                progressBar.Value = e.ProgressPercentage;
                lbl.Text = e.ProgressPercentage + "%";
                if (e.ProgressPercentage != 100 || called) return;
                called = true;
                new Thread(() => { Log($"Installing {program}"); }).Start();
                AppInstaller(program);
            };
        }

        #region List Functions

        public void AddApp(string path)
        {
            _programs.Add(path);
        }

        public void ClearList()
        {
            _programs.Clear();
        }

        #endregion List Functions
        
        private void button1_Click(object sender, EventArgs e)
        {
            ClearList();
            var index = 0;

            foreach (Control checkBox in this.Controls)
            {
                if (!(checkBox is CheckBox))
                    continue;
                var checkbox = checkBox as CheckBox;
                _checkBoxes.Add(checkbox);
                if(checkbox.Checked)
                {
                    AddApp(_urls[index]);
                }
                index++;
            }
            DownloadApps();
        }

        private void Log(string msg)
        {
            richTextBox1.Text += msg + Environment.NewLine;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var control in this.Controls)
            {
                if (control is ProgressBar)
                {
                    _progressBars.Add(control as ProgressBar);
                }
                else if (control is Label)
                {
                    _labels.Add(control as Label);
                }
                else if (control is CheckBox)
                {
                    _checkBoxes.Add(control as CheckBox);
                }
            }
        }
    }
}