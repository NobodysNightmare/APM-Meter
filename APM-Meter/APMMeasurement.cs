using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Timers;

namespace APM_Meter
{
    public struct APMSnapshot
    {
        public long TotalTime { get; set; }
        public int TotalActions { get; set; }
        public int CurrentAPM { get; set; }
        public int AverageAPM { get; set; }
    }

    public class RefreshEventArgs : EventArgs
    {
        public APMSnapshot Snapshot { get; private set; }

        public RefreshEventArgs(APMSnapshot snap)
        {
            Snapshot = snap;
        }
    }

    public class APMMeasurement
    {
        private const int MemSize = sizeof(int);
        private const string MemName = "APMHook-Shared-Memory";

        private const int RefreshInterval = 500;
        private const int CurrentAPMRingSize = 20;

        private static MemoryMappedFile SharedMemory;
        private static MemoryMappedViewAccessor ViewAccessor;

        private static int TotalActions
        {
            get
            {
                return ViewAccessor.ReadInt32(0);
            }
            set
            {
                ViewAccessor.Write(0, value);
            }
        }

        private int AverageAPM
        {
            get
            {
                return ComputeAPM(TotalActions, ElapsedTime.ElapsedMilliseconds);
            }
        }

        private APMSnapshot[] CurrentAPMRing;
        private int CurrentAPMRingPos;

        private int CurrentActionsOffset;
        private long CurrentTimeOffset;

        private int CurrentAPM
        {
            get
            {
                int currentActions = 0;
                long currentTime = 0;
                foreach (APMSnapshot snap in CurrentAPMRing)
                {
                    currentActions += snap.TotalActions;
                    currentTime += snap.TotalTime;
                }
                return ComputeAPM(currentActions, currentTime);
            }
        }

        private Stopwatch ElapsedTime;

        private Timer RefreshTimer;

        public event EventHandler<RefreshEventArgs> Refresh;

        static APMMeasurement()
        {
            SharedMemory = MemoryMappedFile.CreateOrOpen(MemName, MemSize);
            ViewAccessor = SharedMemory.CreateViewAccessor();
        }

        public APMMeasurement()
        {
            ElapsedTime = new Stopwatch();
            RefreshTimer = new Timer(RefreshInterval);
            RefreshTimer.Elapsed += new ElapsedEventHandler(RefreshTimer_Elapsed);

            CurrentAPMRing = new APMSnapshot[CurrentAPMRingSize];
            ResetAllAPM();
        }

        public void StartMeasurement()
        {
            if (!ElapsedTime.IsRunning)
                StopMeasurement();

            ResetAllAPM();

            ElapsedTime.Restart();
            RefreshTimer.Start();
        }

        public void StopMeasurement()
        {
            ElapsedTime.Stop();
            RefreshTimer.Stop();
        }

        public void ResetAllAPM()
        {
            TotalActions = 0;
            CurrentActionsOffset = 0;
            CurrentTimeOffset = 0;

            ElapsedTime.Reset();
            for (int i = 0; i < CurrentAPMRing.Length; i++)
            {
                CurrentAPMRing[i].TotalTime = RefreshInterval;
                CurrentAPMRing[i].TotalActions = 0;
            }
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MoveCurrentAPMWindow();
            OnRaiseRefresh();
        }

        private void OnRaiseRefresh()
        {
            EventHandler<RefreshEventArgs> handler = Refresh;
            if (handler != null)
            {
                RefreshEventArgs eventArgs = new RefreshEventArgs(CreateSnapshot());
                handler(this, eventArgs);
            }
        }

        private APMSnapshot CreateSnapshot()
        {
            APMSnapshot s = new APMSnapshot();
            s.TotalTime = ElapsedTime.ElapsedMilliseconds;
            s.TotalActions = TotalActions;
            s.AverageAPM = AverageAPM;
            s.CurrentAPM = CurrentAPM;

            return s;
        }

        private void MoveCurrentAPMWindow()
        {
            CurrentAPMRingPos++;
            if (CurrentAPMRingPos >= CurrentAPMRingSize)
                CurrentAPMRingPos = 0;

            int actions = TotalActions;
            long time = ElapsedTime.ElapsedMilliseconds;
            CurrentAPMRing[CurrentAPMRingPos].TotalActions = actions - CurrentActionsOffset;
            CurrentAPMRing[CurrentAPMRingPos].TotalTime = time - CurrentTimeOffset;
            CurrentActionsOffset = actions;
            CurrentTimeOffset = time;
        }

        private int ComputeAPM(int actions, long timeSpanMilliseconds) {
	        double span = (double)(timeSpanMilliseconds)/1000.0;
	        return (int)(((double)actions/span)*60);
        }
    }
}
