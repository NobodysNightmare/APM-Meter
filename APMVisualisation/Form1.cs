using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APMVisualisation
{
    public partial class Form1 : Form
    {
        private APMLogData apm_log = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void loadLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLogFileDialog.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openLogFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            apm_log = new APMLogData(openLogFileDialog.FileName);
            updateStatusBar();
        }

        private void updateStatusBar()
        {
            String time_str = Math.Floor(apm_log.total_time.TotalMinutes).ToString() + ":";
            time_str += String.Format("{0:00}", apm_log.total_time.Seconds)+" min";
            totalTimeStatus.Text = time_str;

            averageAPMStatus.Text = String.Format("{0:0.##}", apm_log.average_apm);
        }
    }
}
