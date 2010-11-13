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
    public partial class APMVisMainWindow : Form
    {
        private static int MAX_ZOOM = 10000;

        private APMLogData apm_log = null;
        private String log_name = "";

        private int graph_margin_bot = 0;
        private int graph_margin_top = 5;
        private int graph_margin_left = 0;
        private int graph_margin_right = 0;

        private double y_caption_density = 0.333;
        private int min_y_step = 20;
        private int y_step;

        private double x_caption_density = 0.333;
        private int min_x_step = 10000;
        private int x_step;

        private int graph_height;
        private int graph_width;

        private int viewport_center;
        private int viewport_range;

        private int timeToX(double time)
        {
            int viewport_left = viewport_center - viewport_range / 2;
            double time_frac = (time - viewport_left) / viewport_range;
            return (int)(graph_margin_left + time_frac * graph_width);
        }

        private int XtoTime(int x)
        {
            double graph_frac = ((double)x - graph_margin_left) / graph_width;
            int viewport_left = viewport_center - viewport_range / 2;
            return (int)(viewport_left + graph_frac * viewport_range);
        }
        
        private int APMtoY(double apm)
        {
            if(apm_log == null)
                return 0;
            double apm_frac = apm / apm_log.max_apm;
            return (int)(graphBox.Height - graph_margin_bot - apm_frac * graph_height);
        }

        private void updateGraphDimensions()
        {
            graph_margin_bot = (int)SystemFonts.DefaultFont.GetHeight();
            graph_height = graphBox.Height - (graph_margin_top + graph_margin_bot) - 1;
            graph_width = graphBox.Width - (graph_margin_left + graph_margin_right) - 1;

            updateYStepSize();
            updateXStepSize();
        }

        private void updateYStepSize()
        {
            if(apm_log == null)
                return;
            double font_height = SystemFonts.DefaultFont.GetHeight();
            int i;
            double last_diff = -1;
            for (i = 1; i * min_y_step <= apm_log.max_apm; i++)
            {
                int count = apm_log.max_apm / (i * min_y_step);
                double diff = Math.Abs(count*font_height/graph_height-y_caption_density);

                if (last_diff > 0 && diff > last_diff)
                {
                    i--;
                    break;
                }
                last_diff = diff;
            }
            y_step = i * min_y_step;
        }

        private void updateXStepSize()
        {
            double text_width = 33;
            int i;
            double last_diff = -1;
            for (i = 1; i * min_x_step <= viewport_range; i++)
            {
                int count = viewport_range / (i * min_x_step);
                double diff = Math.Abs(count * text_width / graph_width - x_caption_density);

                if (last_diff > 0 && diff > last_diff)
                {
                    i--;
                    break;
                }
                last_diff = diff;
            }
            x_step = i * min_x_step;
        }

        private void resetViewport()
        {
            if(apm_log == null)
                return;

            viewport_range = (int)apm_log.total_time.TotalMilliseconds;
            viewport_center = viewport_range / 2;
            updateXStepSize();
        }

        private void viewportZoomIn(bool refresh)
        {
            viewport_range = Math.Max(viewport_range / 2, MAX_ZOOM);
            updateXStepSize();
            if(refresh)
                graphBox.Refresh();
        }

        private void viewportZoomOut(bool refresh)
        {
            if (apm_log == null)
                return;
            viewport_range = Math.Min(viewport_range * 2, (int)apm_log.total_time.TotalMilliseconds);
            viewport_center = Math.Max(viewport_range / 2, Math.Min(viewport_center, (int)apm_log.total_time.TotalMilliseconds - viewport_range / 2));
            updateXStepSize();
            if (refresh)
                graphBox.Refresh();
        }

        private void setViewportCenter(int center) {
            viewport_center = Math.Max(viewport_range / 2, Math.Min(center, (int)apm_log.total_time.TotalMilliseconds - viewport_range / 2));
        }

        public APMVisMainWindow()
        {
            InitializeComponent();
            updateGraphDimensions();
            this.MouseWheel += new MouseEventHandler(this.graphBox_mouseWheel);

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

        private void openLog_event(object sender, CancelEventArgs e)
        {
            apm_log = new APMLogData(openLogFileDialog.FileName);
            log_name = openLogFileDialog.SafeFileName;
            closeLogMenuItem.Enabled = true;

            updateStatusBar();
            resetViewport();
            updateYStepSize();
            graphBox.Refresh();
        }

        private void closeLog_event(object sender, EventArgs e)
        {
            apm_log = null;
            closeLogMenuItem.Enabled = false;
            updateStatusBar();
            graphBox.Refresh();
        }

        private void updateStatusBar()
        {
            if (apm_log == null)
            {
                totalTimeStatus.Text = "---";
                averageAPMStatus.Text = "---";
                logFilenameStatus.Text = "---";
                return;
            }

            logFilenameStatus.Text = log_name;
            String time_str = Math.Floor(apm_log.total_time.TotalMinutes).ToString() + ":";
            time_str += String.Format("{0:00}", apm_log.total_time.Seconds)+" min";
            totalTimeStatus.Text = time_str;

            averageAPMStatus.Text = String.Format("{0:0.##}", apm_log.average_apm);
        }

        private void graphBox_Paint(object sender, PaintEventArgs e)
        {
            //enable AA
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //draw surrounding
            e.Graphics.DrawRectangle(Pens.Black, graph_margin_left, graph_margin_top, graph_width, graph_height);
            
            if (apm_log == null)
                return;

            //left margin changes depending on max-apm
            graph_margin_left = (int)e.Graphics.MeasureString(apm_log.max_apm.ToString(), SystemFonts.DefaultFont).Width;

            //draw captions
            int font_offset = (int)SystemFonts.DefaultFont.GetHeight() / 2;
            for(int i = 0;i<=apm_log.max_apm;i+=y_step)
                e.Graphics.DrawString(i.ToString(), SystemFonts.DefaultFont, Brushes.Black, 0, APMtoY(i)-font_offset);

            font_offset = (int)e.Graphics.MeasureString("00:00", SystemFonts.DefaultFont).Width/2;
            int viewport_left = viewport_center - viewport_range / 2;
            int viewport_right = viewport_center + viewport_range / 2;
            TimeSpan time_step = new TimeSpan(0,0,0,0,x_step);
            for (TimeSpan i = new TimeSpan(); i.TotalMilliseconds <= apm_log.total_time.TotalMilliseconds; i = i.Add(time_step))
            {
                if(i.TotalMilliseconds < viewport_left || i.TotalMilliseconds > viewport_right)
                    continue;
                String time = String.Format("{0:00}", i.TotalMinutes) + ":" + String.Format("{0:00}", i.Seconds);
                int pos = timeToX(i.TotalMilliseconds) - font_offset;
                e.Graphics.DrawString(time, SystemFonts.DefaultFont, Brushes.Black, pos, graph_margin_top+graph_height);
            }

            //limit new draw-commands to graph-area
            e.Graphics.SetClip(new Rectangle(graph_margin_left+1, graph_margin_top+1, graph_width-1, graph_height-1));

            //draw visible graph
            APMLogEntry last_entry = null;
            foreach (APMLogEntry entry in apm_log.entries)
            {
                if (last_entry == null)
                {
                    last_entry = entry;
                    continue;
                }
                if (entry.time < viewport_left || entry.time > viewport_right)
                    continue;

                e.Graphics.DrawLine(Pens.Green, timeToX(last_entry.time), APMtoY(last_entry.apm), timeToX(entry.time), APMtoY(entry.apm));

                last_entry = entry;
            }

            //indicate average APM
            e.Graphics.DrawLine(Pens.Chocolate, graph_margin_left, APMtoY(apm_log.average_apm), graph_margin_left + graph_width, APMtoY(apm_log.average_apm));
        }

        private void graphBox_Resize(object sender, EventArgs e)
        {
            updateGraphDimensions();
            graphBox.Refresh();
        }
        
        private void graphBox_mouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int center = XtoTime(e.X);
                viewportZoomIn(false);
                setViewportCenter(center);
                graphBox.Refresh();
            }
            else
                viewportZoomOut(true);
        }

        private int last_x;
        private void graphBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.Button.HasFlag(MouseButtons.Left))
                return;
            if (apm_log == null)
                return;

            int screen_delta = last_x - e.X;
            int pixel_width = viewport_range / graph_width;

            setViewportCenter(viewport_center + pixel_width * screen_delta);

            last_x = e.X;
            graphBox.Refresh();
        }

        private void graphBox_MouseDown(object sender, MouseEventArgs e)
        {
            last_x = e.X;
        }

        private void graphBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int center = XtoTime(e.X);
            viewportZoomIn(false);
            setViewportCenter(center);
            graphBox.Refresh();
        }
    }
}
