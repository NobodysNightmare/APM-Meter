using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace APM_Meter
{
    public partial class APMMeterMainWindow : Form
    {
        private APMMeasurement Measure;

        public APMMeterMainWindow()
        {
            InitializeComponent();
            Measure = new APMMeasurement();
            Measure.Refresh += new EventHandler<RefreshEventArgs>(Measure_Refresh);
        }

        private void Measure_Refresh(object sender, RefreshEventArgs e)
        {
            CurrentAPMStatus.Text = e.Snapshot.CurrentAPM.ToString();
            AverageAPMStatus.Text = e.Snapshot.AverageAPM.ToString();
        }

        private void ClearStatusBar()
        {
            CurrentAPMStatus.Text = "---";
            AverageAPMStatus.Text = "---";
        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                KeyHookInterface.ToogleHook();
            }
            catch { }

            if (KeyHookInterface.Started)
            {
                HooksStarted();
            }
            else
            {
                HooksStopped();
            }
        }

        private void HooksStarted()
        {
            StartStopButton.Text = "Stop";
            Measure.StartMeasurement();
        }

        private void HooksStopped()
        {
            StartStopButton.Text = "Start";
            Measure.StopMeasurement();
        }
    }
}
