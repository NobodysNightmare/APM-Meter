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
        private const int max_read_blocks = 64;

        private String log_file;

        public DateTime time;
        public List<APMLogEntry> entries;

        public TimeSpan total_time;
        public double average_apm
        {
            get
            {
                APMLogEntry entry = entries[entries.Count - 1];
                return (double)entry.actions/total_time.TotalMinutes;
            }
        }

        int prop_max_apm = 0;
        public int max_apm
        {
            get
            {
                if (prop_max_apm == 0)
                    foreach (APMLogEntry e in entries)
                        if (e.apm > prop_max_apm)
                            prop_max_apm = e.apm;
                return prop_max_apm;
            }
        }

        public APMLogData(String filename)
        {
            log_file = filename;
            entries = new List<APMLogEntry>();
            FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            //read head
            byte[] pre_buffer = new byte[8];
            readAtLeast(input, pre_buffer, 8);
            if (!(pre_buffer[0] == 'A' && pre_buffer[1] == 'P' && pre_buffer[2] == 'M'))
            {
                throw new Exception("Given file is no APM-Logfile");
            }
            
            Int32 head_len = BitConverter.ToInt32(pre_buffer, 4);

            readHeader(input, pre_buffer[3], head_len);
            readEntries(input, head_len);

            total_time = new TimeSpan(0, 0, 0, 0, entries[entries.Count - 1].time);
        }

        private void readHeader(FileStream input, byte version, int length)
        {
            input.Position = 0;
            byte[] head = new byte[length];
            readAtLeast(input, head, length);

            time = new DateTime(BitConverter.ToInt16(head, 8), head[10], head[11], head[12], head[13], 0);
        }

        private void readEntries(FileStream input, int header_size)
        {
            input.Position = header_size;
            byte[] buffer = new byte[APMLogEntry.entry_size * max_read_blocks];

            while (input.Position < input.Length)
            {
                int read_len = readAtLeast(input, buffer, APMLogEntry.entry_size);
                for (int i = 0; i < read_len; i += APMLogEntry.entry_size)
                    entries.Add(new APMLogEntry(buffer, i));
            }
        }

        private int readAtLeast(FileStream input, byte[] buffer, int min_len)
        {
            int read_len = 0;
            do
            {
                read_len += input.Read(buffer, read_len, buffer.Length - read_len);
            } while (read_len < min_len);
            return read_len;
        }
    }
}
