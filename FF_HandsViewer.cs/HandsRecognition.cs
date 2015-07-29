using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace hands_viewer.cs
{
    class HandsRecognition
    {
        byte[] LUT;
        private MainForm form;
        private bool _disconnected = false;
        //Queue containing depth image - for synchronization purposes
        private Queue<PXCMImage> m_images;
        private const int NumberOfFramesToDelay = 3;
        private int _framesCounter = 0;
        private float _maxRange;
       


        public HandsRecognition(MainForm form)
        {
            m_images = new Queue<PXCMImage>();
            this.form = form;
            LUT = Enumerable.Repeat((byte)0, 256).ToArray();
            LUT[255] = 1;
        }

        /* Checking if sensor device connect or not */
        private bool DisplayDeviceConnection(bool state)
        {
            if (state)
            {
                if (!_disconnected) form.UpdateStatus("Device Disconnected");
                _disconnected = true;
            }
            else
            {
                if (_disconnected) form.UpdateStatus("Device Reconnected");
                _disconnected = false;
            }
            return _disconnected;
        }

        /* Displaying current frame gestures */
        private void DisplayGesture(PXCMHandData handAnalysis,int frameNumber)
        {

            int firedGesturesNumber = handAnalysis.QueryFiredGesturesNumber();
            string gestureStatusLeft = string.Empty;
            string gestureStatusRight = string.Empty;
            if (firedGesturesNumber == 0)
            {
              
                return;
            }
           
            for (int i = 0; i < firedGesturesNumber; i++)
            {
                PXCMHandData.GestureData gestureData;
                if (handAnalysis.QueryFiredGestureData(i, out gestureData) == pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    PXCMHandData.IHand handData;
                    if (handAnalysis.QueryHandDataById(gestureData.handId, out handData) != pxcmStatus.PXCM_STATUS_NO_ERROR)
                        return;
                   
                    PXCMHandData.BodySideType bodySideType = handData.QueryBodySide();
                    if (bodySideType == PXCMHandData.BodySideType.BODY_SIDE_LEFT)
                    {
                        gestureStatusLeft += "Left Hand Gesture: " + gestureData.name;
                    }
                    else if (bodySideType == PXCMHandData.BodySideType.BODY_SIDE_RIGHT)
                    {
                        gestureStatusRight += "Right Hand Gesture: " + gestureData.name;
                    }
                   
                }
                  
            }
            if (gestureStatusLeft == String.Empty)
                form.UpdateInfo("Frame " + frameNumber + ") " + gestureStatusRight + "\n", Color.SeaGreen);
            else
                form.UpdateInfo("Frame " + frameNumber + ") " + gestureStatusLeft + ", " + gestureStatusRight + "\n",Color.SeaGreen);
          
        }

        /* Displaying Depth/Mask Images - for depth image only we use a delay of NumberOfFramesToDelay to sync image with tracking */
        private unsafe void DisplayPicture(PXCMImage depth, PXCMHandData handAnalysis)
        {
            if (depth == null)
                return;

            PXCMImage image = depth;

            //Mask Image
            if (form.GetLabelmapState())
            {
                Bitmap labeledBitmap = null;
                try
                {
                    labeledBitmap = new Bitmap(image.info.width, image.info.height, PixelFormat.Format32bppRgb);
                    for (int j = 0; j < handAnalysis.QueryNumberOfHands(); j++)
                    {
                        int id;
                        PXCMImage.ImageData data;

                        handAnalysis.QueryHandId(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_TIME, j, out id);
                        //Get hand by time of appearance
                        PXCMHandData.IHand handData;
                        handAnalysis.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_TIME, j, out handData);
                        if (handData != null &&
                            (handData.QuerySegmentationImage(out image) >= pxcmStatus.PXCM_STATUS_NO_ERROR))
                        {
                            if (image.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_Y8,
                                out data) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                            {
                                Rectangle rect = new Rectangle(0, 0, image.info.width, image.info.height);

                                BitmapData bitmapdata = labeledBitmap.LockBits(rect, ImageLockMode.ReadWrite,
                                    labeledBitmap.PixelFormat);
                                byte* numPtr = (byte*) bitmapdata.Scan0; //dst
                                byte* numPtr2 = (byte*) data.planes[0]; //row
                                int imagesize = image.info.width*image.info.height;
                                byte num2 = (byte) handData.QueryBodySide();

                                byte tmp = 0;
                                for (int i = 0; i < imagesize; i++, numPtr += 4, numPtr2++)
                                {
                                    tmp = (byte) (LUT[numPtr2[0]]*num2*100);
                                    numPtr[0] = (Byte) (tmp | numPtr[0]);
                                    numPtr[1] = (Byte) (tmp | numPtr[1]);
                                    numPtr[2] = (Byte) (tmp | numPtr[2]);
                                    numPtr[3] = 0xff;
                                }

                                labeledBitmap.UnlockBits(bitmapdata);
                                image.ReleaseAccess(data);
                                
                            }
                        }
                    }
                    if (labeledBitmap != null)
                    {
                        form.DisplayBitmap(labeledBitmap);
                        labeledBitmap.Dispose();
                    }
                    image.Dispose();
                }
                catch (Exception)
                {
                    if (labeledBitmap != null)
                    {
                        labeledBitmap.Dispose();
                    }
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }

            }//end label image

            //Depth Image
            else
            {
                //collecting 3 images inside a queue and displaying the oldest image
                PXCMImage.ImageInfo info;
                PXCMImage image2;

                info = image.QueryInfo();
                image2 = form.g_session.CreateImage(info);
                if (image2 == null) { return; }
                image2.CopyImage(image);
                m_images.Enqueue(image2);
                if (m_images.Count == NumberOfFramesToDelay)
                {
                    Bitmap depthBitmap;
                    try
                    {
                        depthBitmap = new Bitmap(image.info.width, image.info.height, PixelFormat.Format32bppRgb);
                    }
                    catch (Exception)
                    {
                        image.Dispose();
                        PXCMImage queImage = m_images.Dequeue();
                        queImage.Dispose();
                        return;
                    }

                    PXCMImage.ImageData data3;
                    PXCMImage image3 = m_images.Dequeue();
                    if (image3.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH, out data3) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        float fMaxValue = _maxRange;
                        byte cVal;

                        Rectangle rect = new Rectangle(0, 0, image.info.width, image.info.height);
                        BitmapData bitmapdata = depthBitmap.LockBits(rect, ImageLockMode.ReadWrite, depthBitmap.PixelFormat);

                        byte* pDst = (byte*)bitmapdata.Scan0;
                        short* pSrc = (short*)data3.planes[0];
                        int size = image.info.width * image.info.height;

                        for (int i = 0; i < size; i++, pSrc++, pDst += 4)
                        {
                            cVal = (byte)((*pSrc) / fMaxValue * 255);
                            if (cVal != 0)
                                cVal = (byte)(255 - cVal);
                           
                            pDst[0] = cVal;
                            pDst[1] = cVal;
                            pDst[2] = cVal;
                            pDst[3] = 255;
                        }
                        try
                        {
                            depthBitmap.UnlockBits(bitmapdata);
                        }
                        catch (Exception)
                        {
                            image3.ReleaseAccess(data3);
                            depthBitmap.Dispose();
                            image3.Dispose();
                            return;
                        }

                        form.DisplayBitmap(depthBitmap);
                        image3.ReleaseAccess(data3);
                    }
                    depthBitmap.Dispose();
                    image3.Dispose();
                }
            }
        }

        protected float Distance(PXCMPoint3DF32 a, PXCMPoint3DF32 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;

            return (float) Math.Sqrt((double)(dx * dx + dy * dy + dz * dz));
        }

        //protected bool IsSognut(PXCMHandData.JointData[] nodes, PXCMHandData.JointType center, PXCMHandData.JointType mid, PXCMHandData.JointType tip)
        protected bool IsSognut(PXCMHandData.JointData[] nodes, PXCMHandData.JointType center, int mid, int tip)
        {
            float distanceTip = Distance(nodes[(int)center].positionWorld, nodes[(int)tip].positionWorld);
            float distanceMid = Distance(nodes[(int)center].positionWorld, nodes[(int)mid].positionWorld);

            return distanceMid > distanceTip;
        }

        protected string GetFolded(PXCMHandData.JointData[] nodes)
        {
            string info = ""; 
            for (int i = 0; i < 5; i++)
            {
                bool isSognut = IsSognut(nodes, PXCMHandData.JointType.JOINT_WRIST, 3 + i * 4, 5 + i * 4);
                if (isSognut)
                {
                    info += "_";
                }
                else
                {
                    info += "|";
                }
            }

            return info;
        }

        protected string GetFoldedCount(PXCMHandData.JointData[] nodes)
        {
            int count = 0;
            for (int i = 1; i < 5; i++)
            {
                bool isSognut = IsSognut(nodes, PXCMHandData.JointType.JOINT_WRIST, 3 + i * 4, 5 + i * 4);
                if (!isSognut)
                {
                    count += 1;
                }
            }

            return count.ToString();
        }

        protected int GetTip(PXCMHandData.JointData[] nodes)
        {
            for (int i = 1; i < 5; i++)
            {
                bool isSognut = IsSognut(nodes, PXCMHandData.JointType.JOINT_WRIST, 3 + i * 4, 5 + i * 4);
                if (!isSognut)
                {
                    return i;
                }
            }

            return -1;
        }

        protected string GetRoundFingers(PXCMHandData.JointData[] nodes)
        {
            float distanceThumb = Distance(nodes[(int)PXCMHandData.JointType.JOINT_INDEX_TIP].positionWorld, nodes[(int)PXCMHandData.JointType.JOINT_THUMB_TIP].positionWorld);
            float distanceMid = Distance(nodes[(int)PXCMHandData.JointType.JOINT_INDEX_TIP].positionWorld, nodes[(int)PXCMHandData.JointType.JOINT_MIDDLE_TIP].positionWorld);

            if (distanceMid > distanceThumb)
            {
                return "o";
            }
            else
            {
                return "-";
            }
        }


        protected string getProportions(PXCMHandData.JointData[] nodes)
        {
            string info = "";
            for (int i = 0; i < 5; i++)
            {
                float distance1 = Distance(nodes[(int)PXCMHandData.JointType.JOINT_WRIST].positionWorld, nodes[3 + i * 4].positionWorld);
                float distance2 = Distance(nodes[3 + i * 4].positionWorld, nodes[5 + i * 4].positionWorld);

                info += Math.Round((distance1 / distance2), 3).ToString() + "\t";
            }

            return info;
        }

        protected string GetLetter(string rounded, string folded)
        {
            if (rounded == "oo")
            {
                return "B";
            }
            if (rounded == "o")
            {
                return "C";
            }
            if (rounded == "-o")
            {
                return "D";
            }
            if (folded == "22")
            {
                return "F";
            }
            if (folded == "41")
            {
                return "L";
            }
            if (folded == "42")
            {
                return "N";
            }
            if (folded == "43")
            {
                return "M";
            }
            return "?";
        }

        public string findVovel(PXCMHandData.JointData[][] nodes, int tip)
        {
            PXCMPoint3DF32 tipPoint = nodes[1][5 + 4 * tip].positionWorld;

            float minTipDistance = 1000;
            int tipIndex = -1;
            for (int i = 0; i < 5; i++)
            {
                float tipDistance = Distance(tipPoint, nodes[0][5 + i * 4].positionWorld);
                if (tipDistance < minTipDistance)
                {
                    minTipDistance = tipDistance;
                    tipIndex = i;
                }
            }

            switch (tipIndex)
            {
                case 0: 
                    return "A";
                case 1:
                    return "E";
                case 2:
                    return "I";
                case 3:
                    return "O";
                case 4:
                    return "U";
                default:
                    return "?";
            }
        }

        public string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }

        protected void DisplayGesturesNew(PXCMHandData.JointData[][] nodes, int numOfHands)
        {
            string handsR = "";
            string handsC = "";

            string info = "";

            // reversed (?)
            bool reversed = false;
            if (numOfHands >= 2)
            {
                PXCMHandData.JointData joint1 = nodes[0][1];
                PXCMHandData.JointData joint2 = nodes[1][1];
                if (joint1 != null && joint2 != null)
                {
                    reversed = joint1.positionImage.x < joint2.positionImage.x;
                }
            }

            if (reversed) {
                PXCMHandData.JointData[] hand0 = nodes[0];
                nodes[0] = nodes[1];
                nodes[1] = hand0;
            }

            // joints
            for (int h = 0; h < numOfHands; h++)
            {
                PXCMHandData.JointData joint = nodes[h][0];
                if (joint == null)
                {
                    continue;
                }



                string handInfo = "Hand " + h.ToString() + "\r\n";
                string folded = GetFolded(nodes[h]);

                handInfo += folded + "\r\n";

                if (folded.Substring(1) == "||__")
                {
                    handInfo += "Victory";
                }
                if (folded == "||__|")
                {
                    handInfo += "Koza";
                }
                if (folded == "|_|__")
                {
                    handInfo += "Fuck";
                }

                
                handsC += GetFoldedCount(nodes[h]);
                handsR += GetRoundFingers(nodes[h]);

                string rounded = GetRoundFingers(nodes[h]);
                handInfo += "\r\n" + GetRoundFingers(nodes[h]);
                handInfo += "\r\n" + getProportions(nodes[h]);

                info += handInfo + "\r\n";
            }

            string letter = "?";
            if (handsC == "41")
            {
                letter = findVovel(nodes, GetTip(nodes[1]));
            }
            
            if (letter == "?")
            {
                letter = GetLetter(handsR, handsC);
            }
           
            info = reversed.ToString() + "\tLetter - " + letter + "\r\n" + "Hands - " + handsR + handsC + "\r\n" + info;
            form.UpdateLetter(letter);
            if (letter != "?")
            {
                form.UpdateKaraoke(letter);
            }
            form.UpdateGestureInfo(info);
        }

        /* Displaying current frames hand joints */
        private void DisplayJoints(PXCMHandData handOutput, long timeStamp = 0)
        {
            if (form.GetJointsState() || form.GetSkeletonState())
            {
                //Iterate hands
                PXCMHandData.JointData[][] nodes = new PXCMHandData.JointData[][] { new PXCMHandData.JointData[0x20], new PXCMHandData.JointData[0x20] };
                int numOfHands = handOutput.QueryNumberOfHands();
                for (int i = 0; i < numOfHands; i++)
                {
                    //Get hand by time of appearence
                    //PXCMHandAnalysis.HandData handData = new PXCMHandAnalysis.HandData();
                    PXCMHandData.IHand handData;
                    if (handOutput.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_TIME, i, out handData) == pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        if (handData != null)
                        {
                            //Iterate Joints
                            for (int j = 0; j < 0x20; j++)
                            {
                                PXCMHandData.JointData jointData;
                                handData.QueryTrackedJoint((PXCMHandData.JointType)j, out jointData);
                                nodes[i][j] = jointData;
                                

                               

                            } // end iterating over joints
                        }
                    }
                } // end itrating over hands

                form.DisplayJoints(nodes, numOfHands);
                DisplayGesturesNew(nodes, numOfHands);
            }
            else
            {
                form.DisplayJoints(null, 0);
            }
        }

        /* Displaying current frame alerts */
        private void DisplayAlerts(PXCMHandData handAnalysis, int frameNumber)
        {
            bool isChanged = false;
            string sAlert = "Alert: ";
            for (int i = 0; i < handAnalysis.QueryFiredAlertsNumber(); i++)
            {
                PXCMHandData.AlertData alertData;
                if (handAnalysis.QueryFiredAlertData(i, out alertData) != pxcmStatus.PXCM_STATUS_NO_ERROR)
                    continue;

                //See PXCMHandAnalysis.AlertData.AlertType for all available alerts
                switch (alertData.label)
                {
                    case PXCMHandData.AlertType.ALERT_HAND_DETECTED:
                        {

                            sAlert += "Hand Detected, ";
                            isChanged = true;
                            break;
                        }
                    case PXCMHandData.AlertType.ALERT_HAND_NOT_DETECTED:
                        {

                            sAlert += "Hand Not Detected, ";
                            isChanged = true;
                            break;
                        }
                    case PXCMHandData.AlertType.ALERT_HAND_CALIBRATED:
                        {

                            sAlert += "Hand Calibrated, ";
                            isChanged = true;
                            break;
                        }
                    case PXCMHandData.AlertType.ALERT_HAND_NOT_CALIBRATED:
                        {

                            sAlert += "Hand Not Calibrated, ";
                            isChanged = true;
                            break;
                        }
                    case PXCMHandData.AlertType.ALERT_HAND_INSIDE_BORDERS:
                        {

                            sAlert += "Hand Inside Border, ";
                            isChanged = true;
                            break;
                        }
                    case PXCMHandData.AlertType.ALERT_HAND_OUT_OF_BORDERS:
                        {

                            sAlert += "Hand Out Of Borders, ";
                            isChanged = true;
                            break;
                        }
                }
            }
            if (isChanged)
            {
                form.UpdateInfo("Frame " + frameNumber + ") " + sAlert + "\n", Color.RoyalBlue);
            }


        }

        public static pxcmStatus OnNewFrame(Int32 mid, PXCMBase module, PXCMCapture.Sample sample)
        {
            return pxcmStatus.PXCM_STATUS_NO_ERROR;
        }

      



        /* Using PXCMSenseManager to handle data */
        public void SimplePipeline()
        {   
            form.UpdateInfo(String.Empty,Color.Black);
            bool liveCamera = false;

            bool flag = true;
            PXCMSenseManager instance = null;
            _disconnected = false;
            instance = form.g_session.CreateSenseManager();
            if (instance == null)
            {
                form.UpdateStatus("Failed creating SenseManager");
                return;
            }

            PXCMCaptureManager captureManager = instance.captureManager;
            if (captureManager != null)
            {
                if (form.GetRecordState())
                {
                    captureManager.SetFileName(form.GetFileName(), true);
                    PXCMCapture.DeviceInfo info;
                    if (form.Devices.TryGetValue(form.GetCheckedDevice(), out info))
                    {
                        captureManager.FilterByDeviceInfo(info);
                    }

                }
                else if (form.GetPlaybackState())
                {
                    captureManager.SetFileName(form.GetFileName(), false);
                }
                else
                {
                    PXCMCapture.DeviceInfo info;
                    if (String.IsNullOrEmpty(form.GetCheckedDevice()))
                    {
                        form.UpdateStatus("Device Failure");
                        return;
                    }

                    if (form.Devices.TryGetValue(form.GetCheckedDevice(), out info))
                    {
                        captureManager.FilterByDeviceInfo(info);
                    }

                    liveCamera = true;
                }
            }
            /* Set Module */
            pxcmStatus status = instance.EnableHand(form.GetCheckedModule());
            PXCMHandModule handAnalysis = instance.QueryHand();

            if (status != pxcmStatus.PXCM_STATUS_NO_ERROR || handAnalysis == null)
            {
                form.UpdateStatus("Failed Loading Module");
                return;
            }

            PXCMSenseManager.Handler handler = new PXCMSenseManager.Handler();
            handler.onModuleProcessedFrame = new PXCMSenseManager.Handler.OnModuleProcessedFrameDelegate(OnNewFrame);


            PXCMHandConfiguration handConfiguration = handAnalysis.CreateActiveConfiguration();
            PXCMHandData handData = handAnalysis.CreateOutput();

            if (handConfiguration == null)
            {
                form.UpdateStatus("Failed Create Configuration");
                return;
            }
            if (handData==null)
            {
                form.UpdateStatus("Failed Create Output");
                return;
            }

            if (form.getInitGesturesFirstTime() == false)
            {
                int totalNumOfGestures = handConfiguration.QueryGesturesTotalNumber();
                if (totalNumOfGestures > 0)
                {
                    this.form.UpdateGesturesToList("", 0);
                    for (int i = 0; i < totalNumOfGestures; i++)
                    {
                        string gestureName = string.Empty;
                        if (handConfiguration.QueryGestureNameByIndex(i, out gestureName) ==
                            pxcmStatus.PXCM_STATUS_NO_ERROR)
                        {
                            
                                this.form.UpdateGesturesToList(gestureName, i + 1);
                            
                            
                        }


                    }
                  
                    form.setInitGesturesFirstTime(true);
                    form.UpdateGesturesListSize();
                }
            }


            FPSTimer timer = new FPSTimer(form);
            form.UpdateStatus("Init Started");
            if (handAnalysis != null && instance.Init(handler) == pxcmStatus.PXCM_STATUS_NO_ERROR)
            {

                PXCMCapture.DeviceInfo dinfo;

                PXCMCapture.Device device = instance.captureManager.device;
                if (device != null)
                {
                    device.QueryDeviceInfo(out dinfo);
                    _maxRange = device.QueryDepthSensorRange().max;

                }
               

                if (handConfiguration != null)
                {
                    handConfiguration.EnableAllAlerts();
                    handConfiguration.EnableSegmentationImage(true);

                    handConfiguration.ApplyChanges();
                    handConfiguration.Update();
                }

                form.UpdateStatus("Streaming");
                int frameCounter = 0;
                int frameNumber = 0;

                while (!form.stop)
                {

                    string gestureName = form.GetGestureName();
                    if (string.IsNullOrEmpty(gestureName) == false)
                    {
                        if (handConfiguration.IsGestureEnabled(gestureName) == false)
                        {
                            handConfiguration.DisableAllGestures();
                            handConfiguration.EnableGesture(gestureName, true);
                            handConfiguration.ApplyChanges();
                        }
                    }
                    else
                    {
                        handConfiguration.DisableAllGestures();
                        handConfiguration.ApplyChanges();
                    }

                    if (instance.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        break;
                    }

                    frameCounter++;
                    
                    if (!DisplayDeviceConnection(!instance.IsConnected()))
                    {

                        if (handData != null)
                        {
                            handData.Update();
                        }

                        PXCMCapture.Sample sample = instance.QueryHandSample();
                        if (sample != null && sample.depth != null)
                        {
                            DisplayPicture(sample.depth, handData);

                            if (handData != null)
                            {
                                frameNumber = liveCamera ? frameCounter : instance.captureManager.QueryFrameIndex(); 

                                DisplayJoints(handData);
                                DisplayGesture(handData,frameNumber);
                                DisplayAlerts(handData, frameNumber);
                            }
                            form.UpdatePanel();
                        }
                        timer.Tick();
                    }
                    instance.ReleaseFrame();
                }
            }
            else
            {
                form.UpdateStatus("Init Failed");
                flag = false;
            }
            foreach (PXCMImage pxcmImage in m_images)
            {
                pxcmImage.Dispose();
            }

            // Clean Up
            if (handData != null) handData.Dispose();
            if (handConfiguration != null) handConfiguration.Dispose();
            instance.Close();
            instance.Dispose();

            if (flag)
            {
                form.UpdateStatus("Stopped");
            }
        }
    }
}
