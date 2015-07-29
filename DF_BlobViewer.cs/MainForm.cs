using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DF_BlobViewer.cs
{
    public partial class MainForm : Form
    {
        public PXCMSession g_session;
        private volatile bool closing = false;
        public volatile bool stop = false;
        private Bitmap bitmap = null;
        public bool worldFacingCamera = false;
      
        private Timer timer = new Timer();
        private string filename = null;
        public Dictionary<string,PXCMCapture.DeviceInfo> Devices { get; set; }

        private System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 14,FontStyle.Bold);
        private System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

        public MainForm(PXCMSession session)
        {
            InitializeComponent();

            this.g_session = session;
            PopulateDeviceMenu();
          //  PopulateModuleMenu();
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            Panel2.Paint += new PaintEventHandler(Panel_Paint);
           
           
        }
        private bool PopulateDeviceFromFileMenu()
        {
            Devices.Clear();

            PXCMSession.ImplDesc desc = new PXCMSession.ImplDesc();
            desc.group = PXCMSession.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = PXCMSession.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;

            PXCMSession.ImplDesc desc1;
            PXCMCapture.DeviceInfo dinfo;
            PXCMSenseManager pp = PXCMSenseManager.CreateInstance();
            if (pp == null)
            {
                UpdateStatus("Init Failed");
                return false;
            }
            try
            {
                if (g_session.QueryImpl(desc, 0, out desc1) < pxcmStatus.PXCM_STATUS_NO_ERROR) throw null;
                if (pp.captureManager.SetFileName(filename, false) < pxcmStatus.PXCM_STATUS_NO_ERROR) throw null;
                if (pp.QueryCaptureManager().LocateStreams() < pxcmStatus.PXCM_STATUS_NO_ERROR) throw null;
                pp.QueryCaptureManager().QueryDevice().QueryDeviceInfo(out dinfo);
            }
            catch
            {
                pp.Dispose();
                UpdateStatus("Init Failed");
                return false;
            }

            ToolStripMenuItem sm = new ToolStripMenuItem("Device");
            ToolStripMenuItem sm1 = new ToolStripMenuItem(dinfo.name, null, new EventHandler(Device_Item_Click));
            sm.DropDownItems.Add(sm1);
            if (sm.DropDownItems.Count > 0)
                (sm.DropDownItems[0] as ToolStripMenuItem).Checked = true;
            MainMenu.Items.RemoveAt(0);
            MainMenu.Items.Insert(0, sm);

            if (!Devices.Any())
            {
                if (dinfo.model == PXCMCapture.DeviceModel.DEVICE_MODEL_R200)
                {
                    txtMaxDepth.Text = "3000";
                    worldFacingCamera = true;
                }
                else
                {
                    txtMaxDepth.Text = "550";
                    worldFacingCamera = false;
                }
            }

            PXCMCapture.Device device = pp.QueryCaptureManager().QueryDevice();
            if (device == null)
            {
                pp.Dispose();
                UpdateStatus("Init Failed");
                return false;
            }

            pp.Close();
            pp.Dispose();

            StatusLabel.Text = "Ok";
            return true;
        }
        private void PopulateDeviceMenu()
        {
            Devices = new Dictionary<string, PXCMCapture.DeviceInfo>();

            PXCMSession.ImplDesc desc = new PXCMSession.ImplDesc();
            desc.group = PXCMSession.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = PXCMSession.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;
            ToolStripMenuItem sm = new ToolStripMenuItem("Device");
            for (int i = 0; ; i++)
            {
                PXCMSession.ImplDesc desc1;
                if (g_session.QueryImpl(desc, i, out desc1) < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
                PXCMCapture capture;
                if (g_session.CreateImpl<PXCMCapture>(desc1, out capture) < pxcmStatus.PXCM_STATUS_NO_ERROR) continue;
                for (int j = 0; ; j++)
                {
                    PXCMCapture.DeviceInfo dinfo;
                    if (capture.QueryDeviceInfo(j, out dinfo) < pxcmStatus.PXCM_STATUS_NO_ERROR) break;
                    if (!Devices.Any())
                    {
                        if (dinfo.model == PXCMCapture.DeviceModel.DEVICE_MODEL_R200)
                        {
                            txtMaxDepth.Text = "3000";
                            worldFacingCamera = true;
                        }
                        else
                        {
                            txtMaxDepth.Text = "550";
                            worldFacingCamera = false;
                        }
                    }

                    string name = dinfo.name;
                    if (Devices.ContainsKey(dinfo.name))
                    {
                        name += j;
                    }
                    Devices.Add(name,dinfo);
                    ToolStripMenuItem sm1 = new ToolStripMenuItem(dinfo.name, null, new EventHandler(Device_Item_Click));
                    sm.DropDownItems.Add(sm1);
                }
                capture.Dispose();
            }
            if (sm.DropDownItems.Count > 0)
                (sm.DropDownItems[0] as ToolStripMenuItem).Checked = true;
            MainMenu.Items.RemoveAt(0);
            MainMenu.Items.Insert(0, sm);
            Blob.Checked = true;
            AccessBySize.Checked = true;
        }

        private void RadioCheck(object sender, string name)
        {
            txtMaxDepth.Text = "550";
            foreach (ToolStripMenuItem m in MainMenu.Items)
            {
                if (!m.Text.Equals(name)) continue;
                foreach (ToolStripMenuItem e1 in m.DropDownItems)
                {
                    e1.Checked = (sender == e1);
                    
                    if (Devices.Count != 0 && e1.Checked)
                    {
                        PXCMCapture.DeviceInfo info;
                        if (Devices.TryGetValue(e1.Text, out info))
                        {
                            if (info.model == PXCMCapture.DeviceModel.DEVICE_MODEL_R200)
                            {
                                txtMaxDepth.Text = "3000";
                                worldFacingCamera = true;
                            }
                            else
                            {
                                txtMaxDepth.Text = "550";
                                worldFacingCamera = false;
                            }
                        }
                    }
                }
            }
        }

        private void Device_Item_Click(object sender, EventArgs e)
        {
            RadioCheck(sender, "Device");
        }

        bool isStopStartClick = false;
        private void Start_Click(object sender, EventArgs e)
        {
            isStopStartClick = true;
            MainMenu.Enabled = false;
            Stop.Enabled = true;

            stop = false;
            System.Threading.Thread thread = new System.Threading.Thread(DoRecognition);
            thread.Start();
            System.Threading.Thread.Sleep(5);

            StartBtn.Enabled = false;
        }

        delegate void DoRecognitionCompleted();
        private void DoRecognition()
        {
            BlobProcessor gr = new BlobProcessor(this);
            gr.SimplePipeline();
            this.Invoke(new DoRecognitionCompleted(
                delegate
                {
                    StartBtn.Enabled = true;
                    Stop.Enabled = false;
                    MainMenu.Enabled = true;
                    if (closing) Close();
                }
            ));
        }

        public string GetCheckedDevice()
        {
            foreach (ToolStripMenuItem m in MainMenu.Items)
            {
                if (!m.Text.Equals("Device")) continue;
                foreach (ToolStripMenuItem e in m.DropDownItems)
                {
                    if (e.Checked) return e.Text;
                }
            }
            return null;
        }

        public int GetAccessOrder()
        {
            if (AccessBySize.Checked == true)
            {
                return 0;
            }
            else if (AccessByDistance.Checked == true)
            {
                return 1;
            }
            else if (AccessByDirection.Checked == true)
            {
                return 2;
            }
            return 0;
        }

        public bool GetBlobState()
        {
            return Blob.Checked;
        }

        private delegate void UpdateFPSStatusDelegate(string status);
        public void UpdateFPSStatus(string status)
        {
            labelFPS.Invoke(new UpdateFPSStatusDelegate(delegate(string s) { labelFPS.Text = s; }), new object[] { status });
        }

        private delegate void UpdateBlobStateDelegate(bool value);
        public void UpdateBlobState(bool value)
        {
            Blob.Invoke(new UpdateBlobStateDelegate(delegate(bool flag) { Blob.Checked = flag; }), new object[] { value });
        }
        public void SetBlobState(bool flag)
        {
            UpdateBlobState(flag);
        }

        public bool GetContourState()
        {
            return Contour.Checked;
        }

        private delegate void UpdateContourStateDelegate(bool value);
        public void UpdateContourState(bool value)
        {
            Contour.Invoke(new UpdateContourStateDelegate(delegate(bool flag) { Contour.Checked = flag; }), new object[] { value });
        }
        public void SetContourState(bool flag)
        {
            UpdateContourState(flag);
        }

        public bool GetBlobDataPointsState()
        {
            return BlobDataPoints.Checked;
        }

        private delegate void UpdateBlobSmoothingDelegate(string value);
        public void UpdateBlobSmoothing(string value)
        {
            blobSmooth.Invoke(new UpdateBlobSmoothingDelegate(delegate(string s) { blobSmooth.Text = s; }), new object[] { value });
        }

        public float GetBlobSmoothing()
        {
            float bSmooth = 1.0f;
            if (Single.TryParse(blobSmooth.Text, out bSmooth))
            {
                if (bSmooth < 0 || bSmooth > 1)
                {
                    UpdateBlobSmoothing("1");
                    bSmooth = 1.0f;
                }
                return bSmooth;
            }
            else
            {
                UpdateBlobSmoothing("1");
                return 1.0f;
            }
        }

        public float GetMaxDepth()
        {
            float maxDepth = 550.0f;
            if (Single.TryParse(txtMaxDepth.Text, out maxDepth))
            {
                return maxDepth;
            }
            else
            {
                return 550.0f;
            }
        }

        private delegate void UpdateBlobMaxBlobsDelegate(string value);
        public void UpdateMaxBlobs(string value)
        {
            txtMaxBlobs.Invoke(new UpdateBlobMaxBlobsDelegate(delegate(string s) { txtMaxBlobs.Text = s; }), new object[] { value });
        }

        public int GetMaxBlobs()
        {
            int maxBlobs = 1;
            if (int.TryParse(txtMaxBlobs.Text, out maxBlobs))
            {
                if (maxBlobs < 1 || maxBlobs > 4)
                {
                    UpdateMaxBlobs("1");
                    maxBlobs = 1;
                }
                return maxBlobs;
            }
            else
            {
                UpdateMaxBlobs("1");
                return 1;
            }
        }

        private delegate void UpdateContourSmoothingDelegate(string value);
        public void UpdateContourSmoothing(string value)
        {
            ContourSmooth.Invoke(new UpdateContourSmoothingDelegate(delegate(string s) { ContourSmooth.Text = s; }), new object[] { value });
        }

        public float GetContourSmoothing()
        {
            float bSmooth = 1;
            if (Single.TryParse(ContourSmooth.Text, out bSmooth))
            {
                if (bSmooth < 0 || bSmooth > 1)
                {
                    UpdateContourSmoothing("1");
                    bSmooth = 1.0f;
                }
                return bSmooth;
            }
            else
            {
                UpdateContourSmoothing("1");
                return 1.0f;
            }
        }

        private delegate void UpdateTxtFormDelegate(bool enable);
        public void UpdateTxtForm(bool enable)
        {
            blobSmooth.Invoke(new UpdateTxtFormDelegate(delegate(bool flag) { blobSmooth.Enabled = flag; }), new object[] { enable });
            ContourSmooth.Invoke(new UpdateTxtFormDelegate(delegate(bool flag) { ContourSmooth.Enabled = flag; }), new object[] { enable });
            txtMaxBlobs.Invoke(new UpdateTxtFormDelegate(delegate(bool flag) { txtMaxBlobs.Enabled = flag; }), new object[] { enable });
            txtMaxDepth.Invoke(new UpdateTxtFormDelegate(delegate(bool flag) { txtMaxDepth.Enabled = flag; }), new object[] { enable });
        }
        public void disableTxtForm(bool enable)
        {
            UpdateTxtForm(enable);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;
            e.Cancel = Stop.Enabled;
            closing = true;
        }

        private delegate void UpdateStatusDelegate(string status);
        public void UpdateStatus(string status)
        {
            Status2.Invoke(new UpdateStatusDelegate(delegate(string s) { StatusLabel.Text = s; }), new object[] { status });
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            isStopStartClick = true;
            stop = true;
        }

        public void DisplayBitmap(Bitmap picture)
        {
            lock (this)
            {
                if (bitmap != null)
                    bitmap.Dispose();
                bitmap = new Bitmap(picture);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            lock (this)
            {
                if (bitmap == null) return;
                Bitmap bitmapNew = new Bitmap(bitmap);
                /* Keep the aspect ratio */
                Rectangle rc = (sender as PictureBox).ClientRectangle;
                float xscale = (float)rc.Width / (float)bitmap.Width;
                float yscale = (float)rc.Height / (float)bitmap.Height;
                float xyscale = (xscale < yscale) ? xscale : yscale;
                int width = (int)(bitmap.Width * xyscale);
                int height = (int)(bitmap.Height * xyscale);
                rc.X = (rc.Width - width) / 2;
                rc.Y = (rc.Height - height) / 2;
                rc.Width = width;
                rc.Height = height;
                e.Graphics.DrawImage(bitmapNew, rc);
                bitmapNew.Dispose();
            }
        }

        public void DisplayBlobNumber(PXCMPoint3DF32 centerPoint, int index)
        {
            if (bitmap == null) return;
            lock (this)
            {
                Graphics g = Graphics.FromImage(bitmap);
                var rect = new RectangleF(centerPoint.x, centerPoint.y, 20, 20);
                //Filling a rectangle before drawing the string.
                g.FillRectangle(Brushes.White, rect);
                g.DrawString(index.ToString(), drawFont, drawBrush, centerPoint.x, centerPoint.y);
                g.Dispose();
            }
        }

        public void DisplayBlobDataPoints(PXCMBlobData.IBlob bData,int index)
        {
            if (bitmap == null) return;
            lock (this)
            {
            
            int sz = 10;
            int szPoint = 5;
            Graphics g = Graphics.FromImage(bitmap); 
            using (Pen red = new Pen(Color.Red, 3.0f),
                    green = new Pen(Color.Green, 3.0f),
                    darkRed = new Pen(Color.DarkRed, 3.0f))
                    {
                        using (SolidBrush brush = new SolidBrush(Color.Green))
                        {
                            PXCMPoint3DF32 point =
                                bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_BOTTOM_MOST);

                            g.DrawEllipse(green, point.x - szPoint, point.y - szPoint, sz, sz);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                            point = bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_TOP_MOST);
                            g.DrawEllipse(green, point.x - szPoint, point.y - szPoint, sz, sz);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                            point = bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_LEFT_MOST);
                            g.DrawEllipse(green, point.x - szPoint, point.y - szPoint, sz, sz);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                            point = bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_RIGHT_MOST);
                            g.DrawEllipse(green, point.x - szPoint, point.y - szPoint, sz, sz);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                            point = bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_CENTER);
                            brush.Color = Color.DarkRed;
                            g.DrawEllipse(darkRed, point.x - szPoint, point.y - szPoint, sz, sz);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                            point = bData.QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_CLOSEST);
                            brush.Color = Color.Red;
                            g.DrawEllipse(red, point.x - szPoint/2, point.y - szPoint/2, szPoint, szPoint);
                            g.FillEllipse(brush, point.x - szPoint, point.y - szPoint, sz, sz);

                        }

                    }

                    g.Dispose();
            }
        }


        public void DisplayContour(PXCMPointI32[] contour, int blobNumber)
        {
            if (bitmap == null) return;
            lock (this)
            {
                Graphics g = Graphics.FromImage(bitmap);
                using (Pen contourColor = new Pen(Color.Blue, 3.0f))
                {
                    for (int i = 0; i < contour.Length; i++)
                    {
                        int baseX = (int) contour[i].x;
                        int baseY = (int) contour[i].y;

                        if (i + 1 < contour.Length)
                        {
                            int x = (int) contour[i + 1].x;
                            int y = (int) contour[i + 1].y;

                            g.DrawLine(contourColor, new Point(baseX, baseY), new Point(x, y));
                          
                        }
                        else
                        {
                            int x = (int)contour[0].x;
                            int y = (int)contour[0].y;
                            g.DrawLine(contourColor, new Point(baseX, baseY), new Point(x, y));
                        }


                    }
                }

                g.Dispose();

            }
          
        }

        private delegate void UpdatePanelDelegate();
        public void UpdatePanel()
        {

            Panel2.Invoke(new UpdatePanelDelegate(delegate()
            { Panel2.Invalidate();
            }));

        }

        private void Live_Click(object sender, EventArgs e)
        {
            Playback.Checked = false;
            Live.Checked = true;
            PopulateDeviceMenu();
            
        }

        private void Playback_Click(object sender, EventArgs e)
        {
            Live.Checked = false;
            Playback.Checked = true;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "RSSDK clip|*.rssdk|Old format clip|*.pcsdk|All files|*.*";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                filename = (ofd.ShowDialog() == DialogResult.OK) ? ofd.FileName : null;
            }

            PopulateDeviceFromFileMenu();
        }

        public bool GetPlaybackState()
        {
            return Playback.Checked;
        }

        public string GetFileName()
        {
            return filename;
        }

        bool isCheckedContour = false;

    }
}
