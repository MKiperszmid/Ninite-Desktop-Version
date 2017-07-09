using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            foreach (var program in _programs)
            {
                var called = false;
                var prog = progressBar1;
                var lbl = label1;

                if (_urls[0] == program)
                {
                    prog = progressBar1;
                    lbl = label1;
                }
                if (_urls[1] == program)
                {
                    prog = progressBar2;
                    lbl = label2;
                }
                if (_urls[2] == program)
                {
                    prog = progressBar3;
                    lbl = label3;
                }
                if (_urls[3] == program)
                {
                    prog = progressBar4;
                    lbl = label4;
                }
                if (_urls[4] == program)
                {
                    prog = progressBar5;
                    lbl = label5;
                }

                var wc = new WebClient();
                var name = Path.GetFileName(program);
                wc.DownloadFileAsync(new Uri(program), $"{name}");
                wc.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                {
                    prog.Value = e.ProgressPercentage;
                    lbl.Text = e.ProgressPercentage + "%";
                    if (e.ProgressPercentage != 100 || called) return;
                    called = true;
                    new Thread(() => { MessageBox.Show($@"Installing {program}", @"Installing"); }).Start();
                    AppInstaller(program);
                };
            }
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
            if(checkBox1.Checked)
                AddApp(_urls[0]);
            if(checkBox2.Checked)
                AddApp(_urls[1]);
            if(checkBox3.Checked)
                AddApp(_urls[2]);
            if(checkBox4.Checked)
                AddApp(_urls[3]);
            if(checkBox5.Checked)
                AddApp(_urls[4]);
            
            DownloadApps();
        }
    }
}