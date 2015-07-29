using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace DF_BlobViewer.cs
{
    class BlobProcessor
    {
        
        byte[] LUT;
        private MainForm form;
        private bool _disconnected = false;
        private int i = 0;
        //Bitmap orig = null;
        int _maxBlobToShow = 0;
        PXCMPoint3DF32 contourMostLeft;
        PXCMPoint3DF32 contourMostRight;
        PXCMPoint3DF32 contourMostTop;
        PXCMPoint3DF32 contourMostBottom;

        public BlobProcessor(MainForm form)
        {
            this.form = form;
            LUT = Enumerable.Repeat((byte)0, 256).ToArray();
            LUT[255] = 255;
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



        /* Displaying Mask Images */
        private unsafe void DisplayPicture(PXCMImage depth, PXCMBlobData blobData)
        {
            if (depth == null)
                return;


            PXCMImage image = depth;
            PXCMImage.ImageInfo info = image.QueryInfo();

            int numOfBlobs = blobData.QueryNumberOfBlobs();

            if (_maxBlobToShow > numOfBlobs)
            {
                _maxBlobToShow = numOfBlobs;
            }

            PXCMBlobData.IBlob[] blobList = new PXCMBlobData.IBlob[_maxBlobToShow];
            PXCMPointI32[][] pointOuter = new PXCMPointI32[_maxBlobToShow][];
            PXCMPointI32[][] pointInner = new PXCMPointI32[_maxBlobToShow][];

            Bitmap picture = new Bitmap(image.info.width, image.info.height, PixelFormat.Format32bppRgb);

            PXCMImage.ImageData bdata;
            pxcmStatus results = pxcmStatus.PXCM_STATUS_NO_ERROR;
            PXCMBlobData.AccessOrderType accessOrder = PXCMBlobData.AccessOrderType.ACCESS_ORDER_LARGE_TO_SMALL;
            int accessOrderBy = form.GetAccessOrder();

            switch (accessOrderBy)
            {
                case 1:
                    accessOrder = PXCMBlobData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR;
                    break;
                case 2:
                    accessOrder = PXCMBlobData.AccessOrderType.ACCESS_ORDER_RIGHT_TO_LEFT;
                    break;
                case 0:
                default:
                    accessOrder = PXCMBlobData.AccessOrderType.ACCESS_ORDER_LARGE_TO_SMALL;
                    break;
            }

            Rectangle rect = new Rectangle(0, 0, image.info.width, image.info.height);
            BitmapData bitmapdata = picture.LockBits(rect, ImageLockMode.ReadWrite, picture.PixelFormat);

            for (int j = 0; j < _maxBlobToShow; j++)
            {
                byte tmp1 = (Byte)(255 - (255 / (_maxBlobToShow) * j));
                results = blobData.QueryBlobByAccessOrder(j, accessOrder, out blobList[j]);
                if (results == pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    bool isSegmentationImage = true;
                    results = blobList[j].QuerySegmentationImage(out image);
                    if (results != pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        PXCMImage.ImageInfo imgInfo = new PXCMImage.ImageInfo();
                        imgInfo.width = 640;
                        imgInfo.height = 480;
                        imgInfo.format = PXCMImage.PixelFormat.PIXEL_FORMAT_Y8;
                        PXCMSession session = PXCMSession.CreateInstance();
                        if (session != null)
                        {
                            image = session.CreateImage(info);
                            if (image == null) return;
                        }
                        image.AcquireAccess(PXCMImage.Access.ACCESS_WRITE, PXCMImage.PixelFormat.PIXEL_FORMAT_Y8, out bdata);
                        byte* numPtr = (byte*)bitmapdata.Scan0; //dst
                        byte* numPtr2 = (byte*)bdata.planes[0]; //row
                        int imagesize = image.info.width * image.info.height;

                        byte tmp;
                        for (int i = 0; i < imagesize; i++, numPtr += 4, numPtr2++)
                        {
                            tmp = (byte)(0);
                            numPtr[0] = tmp;
                            numPtr[1] = tmp;
                            numPtr[2] = tmp;
                            numPtr[3] = tmp;
                        }

                        image.ReleaseAccess(bdata);
                        isSegmentationImage = false;
                    }

                    results = image.AcquireAccess(PXCMImage.Access.ACCESS_READ_WRITE, PXCMImage.PixelFormat.PIXEL_FORMAT_Y8, out bdata);

                    if (form.GetBlobState() && isSegmentationImage == true)
                    {
                        byte* numPtr = (byte*)bitmapdata.Scan0; //dst
                        byte* numPtr2 = (byte*)bdata.planes[0]; //row
                        int imagesize = image.info.width * image.info.height;

                        for (int i = 0; i < imagesize; i++, numPtr += 4, numPtr2++)
                        {
                            byte tmp = (Byte)(numPtr2[0] == 0 ? 0 : tmp1);
                            tmp |= numPtr[0];
                            numPtr[0] = tmp;
                            numPtr[1] = tmp;
                            numPtr[2] = tmp;
                            numPtr[3] = 0xff;
                        }
                    }

                    if ((form.GetContourState()))
                    {
                        int contourNumber = blobList[j].QueryNumberOfContours();
                        if (contourNumber > 0)
                        {
                            for (int k = 0; k < contourNumber; ++k)
                            {
                                int contourSize = blobList[j].QueryContourSize(k);
                                if (blobList[j].IsContourOuter(k) == true)
                                    blobList[j].QueryContourPoints(k, out pointOuter[j]);
                                else
                                {
                                    blobList[j].QueryContourPoints(k, out pointInner[j]);
                                }
                            }

                            if (results == pxcmStatus.PXCM_STATUS_NO_ERROR && form.GetBlobState() == false)
                            {
                                byte* numPtr = (byte*)bitmapdata.Scan0; //dst
                                byte* numPtr2 = (byte*)bdata.planes[0]; //row
                                int imagesize = image.info.width * image.info.height;

                                byte tmp;
                                for (int i = 0; i < imagesize; i++, numPtr += 4, numPtr2++)
                                {
                                    tmp = (byte)(0);
                                    numPtr[0] = tmp;
                                    numPtr[1] = tmp;
                                    numPtr[2] = tmp;
                                    numPtr[3] = tmp;
                                }

                            }
                        }
                    }
                    image.ReleaseAccess(bdata);
                    image.Dispose();
                }
            }
            picture.UnlockBits(bitmapdata);
            form.DisplayBitmap(picture);

            ///////// that is my polygon zone

            Bitmap imageInstance2 = picture;
            i++;

            Bitmap croppedImage = null;
            if (contourMostRight.x > 0) {
                int rectWidth =  (int)(contourMostRight.x - contourMostLeft.x);
                int rectHeight = (int)(contourMostTop.y - contourMostBottom.y);
                Rectangle sourceRectangle = new Rectangle(new Point((int)contourMostLeft.x, 
                                                            (int)contourMostBottom.y), 
                                                                   new Size(rectWidth, rectHeight));

                croppedImage = CropImage(imageInstance2, sourceRectangle);
                
            }

            String[] origArray = {
                "d:\\origG.jpeg",
                "d:\\origX.jpeg",
                "d:\\origY.jpeg"
            };

            for (int i = 0; i < origArray.Length; i++)
            {
                Bitmap orig = null;
            if (File.Exists(origArray[i]))
                orig = new Bitmap(@origArray[i]);
            
            if (orig != null && croppedImage != null)
            {
                float diff = 0;
                orig            = ScaleImage(orig,          150, 100);
                croppedImage    = ScaleImage(croppedImage,  150, 100);
                bool isImageBlank = true;
                    
                for (int y = 0; y < orig.Height; y++)
                {
                    for (int x = 0; x < orig.Width; x++)
                    {
                        if(croppedImage.GetPixel(x, y).R > 1 && croppedImage.GetPixel(x, y).B > 1
                            && croppedImage.GetPixel(x, y).G > 1)
                        {
                            isImageBlank = false;
                            break;
                        }
                    }
                }

                if (!isImageBlank && orig.Size.Width == croppedImage.Size.Width)
                {
                    for (int y = 0; y < orig.Height; y++)
                    {
                        for (int x = 0; x < orig.Width; x++)
                        {
                            diff += (float)Math.Abs(orig.GetPixel(x, y).R -
                                                    croppedImage.GetPixel(x, y).R) / 255;
                            diff += (float)Math.Abs(orig.GetPixel(x, y).G -
                                                    croppedImage.GetPixel(x, y).G) / 255;
                            diff += (float)Math.Abs(orig.GetPixel(x, y).B -
                                                    croppedImage.GetPixel(x, y).B) / 255;
                        }
                    }
                }
                float percentMatch = 100 * diff / (orig.Width * orig.Height * 2);
                Console.WriteLine("diff: {0} %", percentMatch);
                
                if (percentMatch >= 90){
                    Console.WriteLine(origArray[i].ToString());
                        Environment.Exit(0);
                }
            }
           }
            /*
            if (croppedImage != null)
            {
                croppedImage.Save("d:\\cropedImage.jpeg",   System.Drawing.Imaging.ImageFormat.Jpeg);
             //   croppedImage.Save("d:\\origG.jpeg",         System.Drawing.Imaging.ImageFormat.Jpeg);

            }*/
            
            if (i == 100)
                Environment.Exit(0);
            
            picture.Dispose();
            
            for (int i = 0; i < _maxBlobToShow; i++)
            {
                if (form.GetContourState())
                {
                    if (pointOuter[i] != null && pointOuter[i].Length > 0)
                        form.DisplayContour(pointOuter[i], i);
                    if (pointInner[i] != null && pointInner[i].Length > 0)
                        form.DisplayContour(pointInner[i], i);
                }

                if (form.GetBlobDataPointsState())
                {
                    form.DisplayBlobDataPoints(blobList[i], i + 1);
                }
                
                PXCMPoint3DF32 point = blobList[i].QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_CENTER);
                contourMostRight         = blobList[i].QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_LEFT_MOST);
                contourMostLeft        = blobList[i].QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_RIGHT_MOST);
                contourMostBottom          = blobList[i].QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_TOP_MOST);
                contourMostTop       = blobList[i].QueryExtremityPoint(PXCMBlobData.ExtremityType.EXTREMITY_BOTTOM_MOST);
                

                form.DisplayBlobNumber(point, i + 1);
                
            }
        }

        public static pxcmStatus OnNewFrame(Int32 mid, PXCMBase module, PXCMCapture.Sample sample)
        {
            return pxcmStatus.PXCM_STATUS_NO_ERROR;
        }

        static Bitmap CropImage(Image originalImage, Rectangle sourceRectangle,
        Rectangle? destinationRectangle = null)
        {
            if (destinationRectangle == null)
            {
                destinationRectangle = new Rectangle(Point.Empty, sourceRectangle.Size);
            }

            var croppedImage = new Bitmap(destinationRectangle.Value.Width,
                destinationRectangle.Value.Height);
            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(originalImage, destinationRectangle.Value,
                    sourceRectangle, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        public static Bitmap ScaleImage(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /* Using PXCMSenseManager to handle data */
        public void SimplePipeline()
        {
        
            bool flag = true;
            PXCMSenseManager instance = null;
            _disconnected = false;
            instance = form.g_session.CreateSenseManager();
            if (instance == null)
            {
                form.UpdateStatus("Create SenseManager Failure");
                return;
            }
            if (form.GetPlaybackState())
            {
                if (instance.captureManager != null)
                {
                    instance.captureManager.SetFileName(form.GetFileName(), false);
                    instance.captureManager.SetRealtime(false);
                }
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
                    instance.captureManager.FilterByDeviceInfo(info);
                }
            }

            /* Set Module */
            pxcmStatus status = instance.EnableBlob();
            PXCMBlobModule blobModule = instance.QueryBlob();

            if (status != pxcmStatus.PXCM_STATUS_NO_ERROR || blobModule == null)
            {
                form.UpdateStatus("Failed Loading Module");
                return;
            }
            
            PXCMBlobConfiguration blobConfiguration = blobModule.CreateActiveConfiguration();
            PXCMBlobData blobData = blobModule.CreateOutput();

            blobConfiguration.SetContourSmoothing(1);
            
            if (blobConfiguration == null)
            {
                form.UpdateStatus("Failed Create Configuration");
                return;
            }
            if (blobData == null)
            {
                form.UpdateStatus("Failed Create Output");
                return;
            }

            if (form.worldFacingCamera)
            {
                instance.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 320, 240, 0);
            }

               
            PXCMSenseManager.Handler handler = new PXCMSenseManager.Handler();
            handler.onModuleProcessedFrame = new PXCMSenseManager.Handler.OnModuleProcessedFrameDelegate(OnNewFrame);
  
            form.UpdateStatus("Init Started");
            FPSTimer timer = new FPSTimer(form);
            if (instance.Init(handler) == pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                blobConfiguration.Update();
                PXCMCapture.DeviceInfo dinfo;
                PXCMCaptureManager captureManager = instance.QueryCaptureManager();
                if (captureManager != null)
                {
                    PXCMCapture.Device device = captureManager.QueryDevice();
                    if (device != null)
                    {
                        device.QueryDeviceInfo(out dinfo);

                        if (dinfo != null && dinfo.model == PXCMCapture.DeviceModel.DEVICE_MODEL_IVCAM)
                        {
                            device.SetMirrorMode(PXCMCapture.Device.MirrorMode.MIRROR_MODE_HORIZONTAL);
                        }

                        if (dinfo != null && dinfo.model == PXCMCapture.DeviceModel.DEVICE_MODEL_R200)
                        {
                            device.SetDepthConfidenceThreshold(1);
                        }
                    }
                }

                form.UpdateStatus("Streaming");
                while (!form.stop)
                {
                    if (instance.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        break;
                    }
                    if (!DisplayDeviceConnection(!instance.IsConnected()))
                    {                       
                        PXCMCapture.Sample sample = instance.QuerySample();
                        
                        if (sample != null && sample.depth != null)
                        {
                            PXCMImage.ImageInfo info = sample.depth.QueryInfo();
                           

                            float bSmooth = form.GetBlobSmoothing();
                            blobConfiguration.SetSegmentationSmoothing(bSmooth);

                            float maxDepth = form.GetMaxDepth();
                            blobConfiguration.SetMaxDistance(maxDepth);

                            int maxBlobs = 2;
                            _maxBlobToShow = maxBlobs;
                            blobConfiguration.SetMaxBlobs(_maxBlobToShow);

                            float contourSmooth = 1;
                            blobConfiguration.SetContourSmoothing(contourSmooth);

                            form.disableTxtForm(false);

                            blobConfiguration.EnableContourExtraction(true);
                            blobConfiguration.EnableSegmentationImage(true);
                            blobConfiguration.ApplyChanges();

                            if (blobData != null)
                            {
                                blobData.Update();
                            }

                            if (form.GetContourState() == false && form.GetBlobState() == false)
                            {
                                form.SetBlobState(true);
                                form.SetContourState(true);
                            }

                            DisplayPicture(sample.depth, blobData);
                            form.UpdatePanel();
                            timer.Tick();
                        }                                             
                    }
                    instance.ReleaseFrame();
                }
            
                form.disableTxtForm(true);

                // Clean Up
                if (blobData != null) blobData.Dispose();
                if (blobConfiguration != null) blobConfiguration.Dispose();
            }
            else
            {
                form.UpdateStatus("Init Failed");
                flag = false;
            }


          
            instance.Close();
            instance.Dispose();
            if (flag)
            {
                form.UpdateStatus("Stopped");
            }
        }

        
    }
}
