﻿using System;
using System.ServiceProcess;
using System.Threading;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

namespace UpdateOrchestratorStop
{
    public partial class Form1 : Form
    {
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        String exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        ServiceInstaller SvcInstaller = new ServiceInstaller();
        String _SvcName = "UsoSvc";
        String wakeSourceCommand = @"qe System /q:"" *[System[Provider[@Name = 'Microsoft-Windows-Power-Troubleshooter']]]"" /rd:true /c:1 /f:xml";

        public Form1() {
            InitializeComponent();
            tmr_monitorWakeup.Enabled = cb_MonitorWake.Checked;
        }

        private string getLastWakeInfo() {
            String args = @"qe System /q:"" *[System[Provider[@Name = 'Microsoft-Windows-Power-Troubleshooter']]]"" /rd:true /c:1 /f:xml";
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "wevtutil.exe";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.Arguments = args;
            Process proc = Process.Start(psi);
            proc.WaitForExit(2000);
            String strOutput = proc.StandardOutput.ReadToEnd();
            return strOutput;
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/ChronSyn/UpdateOrchestratorStop");
        }

        private void btn_disclaimer_Click(object sender, EventArgs e) {
            MessageBox.Show(
                "BY USING, DOWNLOADING, INSTALLING, COMPILING, DECOMPILING, COPYING, REVERSE ENGINEERING, MODIFYING, EXECUTING OR TRANSFERRING THIS SOFTWARE, YOU AGREE TO THE FOLLOWING DISCLAIMER. IF YOU DO NOT AGREE, YOU MUST CEASE ALL CURRENT AND FUTURE USAGE OF THIS SOFTWARE PRODUCT IMMEDIATELY UNTIL YOU DO AGREE TO THESE TERMS.\n\n\n" +


                "The use of this software is done at your own discretion and risk and with agreement that you will be solely responsible for any damage to your computer system or loss of data that results from such activities.You are solely responsible for adequate protection and backup of the data and equipment used in connection with any of the software linked to this project, and we will not be liable for any damages that you may suffer connection with downloading, installing, using, modifying or distributing such software. No advice or information, whether oral or written, obtained by you from us or from the project or other website shall create any warranty for the software.\n\n\n" +
                

                "Additionally, we make no warranty that:\n\n" +
                " - The software will meet your requirements.\n" +
                " - The software will be uninterrupted, timely, secure or error - free.\n" +
                " - The results from the use of the software will be effective, accurate or reliable.\n" +
                " - The quality of the software will meet your expectations.\n\n\n" +
                
                
                " The links to software, websites or other materials, and the related documentation made available in this product are subject to the following conditions:\n\n" +
                " - The software could include technical or other mistakes, inaccuracies or typographical errors.\n" +
                " - The software may be out of date, and we make no commitment to update such materials.\n" +
                " - We assume no responsibility for errors or omissions in the software or documentation available from this software. In no event shall we be liable to you or any third parties for any special, punitive, incidental, indirect or consequential damages of any kind, or any damages whatsoever, including, without limitation, those resulting from loss of use, lost data or profits, or any liability, arising out of or in connection with the use of this software or any material linked to from this software.");
        }

        private void cb_ToggleTimer_CheckedChanged(object sender, EventArgs e) {

        }

        private void cb_MonitorWake_CheckedChanged(object sender, EventArgs e) {
            tmr_monitorWakeup.Enabled = cb_MonitorWake.Checked;
        }


        private void button1_Click_2(object sender, EventArgs e) {
            String xml = getLastWakeInfo();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            String path = "//*[local-name()='Event']/*[local-name()='EventData']/*[local-name()='Data'][@Name='WakeSourceText']";
            XmlNode root = doc.DocumentElement;
            XmlNode node = root.SelectSingleNode(path);
            MessageBox.Show(node.InnerText);
        }

        private void tmr_monitorWakeup_Tick(object sender, EventArgs e) {
            String WakeInfo = getLastWakeInfo();
            if (WakeInfo.Contains("UpdateOrchestrator")) {
                SetSuspendState(false, true, true);
            }
        }
    }
}