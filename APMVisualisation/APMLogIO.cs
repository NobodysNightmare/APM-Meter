﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace APMLogIO
{
    enum APMLogGameOutcome : byte {
        undefined = 0x0,
        lose = 0x1,
        invalid = 0x2,
        win = 0x3
    }

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

    class APMLog
    {
        private const int max_read_blocks = 64;

        private FileStream file_stream;

        private DateTime priv_time;
        public DateTime time
        {
            get
            {
                if(priv_time == DateTime.MinValue)
                {
                    byte[] time_buffer = new byte[6];
                    file_stream.Position = 8;
                    readAtLeast(file_stream, time_buffer, 6);
                    priv_time = new DateTime(BitConverter.ToInt16(time_buffer, 0), time_buffer[2], time_buffer[3], time_buffer[4], time_buffer[5], 0);
                }
                return priv_time;
            }
        }

        private APMLogGameOutcome priv_outcome = (APMLogGameOutcome)2; //this is an invalid value (unspecified+win)
        public APMLogGameOutcome outcome {
            get
            {
                if (priv_outcome == (APMLogGameOutcome)2)
                {
                    file_stream.Position = 14;
                    priv_outcome = (APMLogGameOutcome)file_stream.ReadByte();
                }
                return priv_outcome;
            }
            set
            {
                file_stream.Position = 14;
                file_stream.WriteByte((byte)value);
                file_stream.Flush();
                priv_outcome = value;
            }
        }

        public List<APMLogEntry> entries;

        public TimeSpan total_time;

        public double average_apm
        {
            get
            {
                APMLogEntry entry = entries.Last();
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

        public APMLog(String filename)
        {
            entries = new List<APMLogEntry>();
            file_stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            //read head
            byte[] pre_buffer = new byte[8];
            readAtLeast(file_stream, pre_buffer, 8);
            if (!(pre_buffer[0] == 'A' && pre_buffer[1] == 'P' && pre_buffer[2] == 'M'))
            {
                throw new Exception("Given file is no APM-Logfile");
            }
            
            Int32 head_len = BitConverter.ToInt32(pre_buffer, 4);

            readEntries(file_stream, head_len);

            total_time = new TimeSpan(0, 0, 0, 0, entries[entries.Count - 1].time);
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

        public void Close()
        {
            file_stream.Close();
        }
    }
}
