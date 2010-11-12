using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace APMVisualisation
{
    class APMLogEntry
    {
        public const int entry_size = 12;

        public Int32 actions;
        public Int32 time;
        public Int32 apm;

        public APMLogEntry(byte[] buffer, int offset)
        {
            actions = BitConverter.ToInt32(buffer, offset);
            time = BitConverter.ToInt32(buffer, offset+4);
            apm = BitConverter.ToInt32(buffer, offset+8);
        }
    }
    class APMLogData
    {
        private const int max_read_blocks = 32;

        private List<APMLogEntry> entries;

        public TimeSpan total_time;
        public double average_apm
        {
            get
            {
                APMLogEntry entry = entries[entries.Count - 1];
                return (double)entry.actions/total_time.TotalMinutes;
            }
        }

        public APMLogData(String filename)
        {
            entries = new List<APMLogEntry>();

            FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[APMLogEntry.entry_size*max_read_blocks];

            while (input.Position < input.Length)
            {
                int read_len = 0;
                do
                {
                    read_len += input.Read(buffer, read_len, buffer.Length);
                } while (read_len < APMLogEntry.entry_size);

                for (int i = 0; i < read_len; i += APMLogEntry.entry_size)
                    entries.Add(new APMLogEntry(buffer, i));
            }

            total_time = new TimeSpan(0, 0, 0, 0, entries[entries.Count - 1].time);
        }
    }
}
