using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;
using System.Drawing.Imaging;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;


namespace myALPR1
{
    class EdgNBlobALPRClass
    {


        private DifferenceEdgeDetector edgeDetector = new DifferenceEdgeDetector();
        private Threshold thresholdFilter = new Threshold(40);
        private BlobCounter blobCounter = new BlobCounter();
        private SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
        private QuadrilateralTransformation quadrilateralTransformation = new QuadrilateralTransformation();
        private OtsuThreshold otsuThresholdFilter = new OtsuThreshold();

        public void setSumCorrelation(int num)
        {
            EdgemOcr.setSumCorrelation(num);
        }

        public int getSumCorrealtion()
        {
            return EdgemOcr.getSumCorrelation();
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            int count = points.Count;
            System.Drawing.Point[] pointsArray = new System.Drawing.Point[count];

            for (int i = 0; i < count; i++)
            {
                pointsArray[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return pointsArray;
        }


        private void sortBlobs(Blob[] blobArr) // sort blobs by X coordinate
        {


            bool swapped = true;
            int j = 0;
            Blob tmbBlob;
            while (swapped)
            {
                swapped = false;
                j++;
                for (int i = 0; i < blobArr.Length - j; i++)
                {
                    if (blobArr[i].Rectangle.X > blobArr[i + 1].Rectangle.X)
                    {

                        tmbBlob = blobArr[i];
                        blobArr[i] = blobArr[i + 1];
                        blobArr[i + 1] = tmbBlob;
                        swapped = true;
                    }
                }
            }

        }
        public MyOCRClass2 EdgemOcr = new MyOCRClass2();

        public String OCR(Bitmap res)          
        {

            

            String plate = "";

            IFilter SISfilter = new SISThreshold();           
            IFilter medianFilter = new Median();
            IFilter erosion = new Erosion();

            GrayscaleBT709 grayFilter = new GrayscaleBT709();
           
            ResizeNearestNeighbor Resizefilter = new ResizeNearestNeighbor(200, 40);       //resize plate image
            Bitmap resized = Resizefilter.Apply(res);
            
            IFilter inv = new Invert();


               // check plates lines count , lines represent B\W edges on plate
           

                String number = "";

              
             
                res = Resizefilter.Apply(res);
              //  res = grayFilter.Apply(res);
                res = medianFilter.Apply(res);


                res = SISfilter.Apply(res);
                res = inv.Apply(res);


                BlobCounter blobCounter2 = new BlobCounter(res);   // ecxtract symbols blobs from plate
                Blob[] blobs2 = blobCounter2.GetObjects(res, false);

                bool flag = true;
                sortBlobs(blobs2);       // sort symbol blobs so we can create a proper License plate from characters

                int symbolCandidates = 0;

                for (int blobIndex2 = 0; blobIndex2 < blobs2.Length; blobIndex2++)
                {
                    if (blobs2[blobIndex2].Image.Height >= (int)(res.Height - res.Height * 0.5) &&
                        blobs2[blobIndex2].Image.Width >= (int)(res.Width * 0.02) &&
                        blobs2[blobIndex2].Image.Width <= 40 &&

                        flag == true)
                    {

                        Bitmap letter;
                        Rectangle biggerLetterRect = blobs2[blobIndex2].Rectangle;
                        letter = blobs2[blobIndex2].Image.ToManagedImage();
                        Bitmap letterWithCanvas = letter;//= addCanvas(letter,6);

                        ResizeNearestNeighbor Resizefilter2 = new ResizeNearestNeighbor(30, 40);  // resize charachter 25 x 40 is fine


                        letterWithCanvas = Resizefilter2.Apply(letterWithCanvas);
                        /// OCR
                        EdgemOcr.compareMatrix(EdgemOcr.createMatrix(letterWithCanvas));   // create binary matrix from extracted character blob   and compare with base of character matrix

                        // int n = mOcr.getIndexOfmaxLetter();


                        if (symbolCandidates < 3)   // compare firs 3 chars with letters base
                        {
                            number += EdgemOcr.getChar();

                        }
                        else
                        {
                            number += EdgemOcr.getNumeric();            // and next 3 chars with numbers
                        }

                        /// end OCR
                        // number += mOcr.getLetter();
                        symbolCandidates++;


                        if (number.Length == 3)
                        {
                            number += "-";         // just add defis between letters and numbers
                        }


                    }

                }

                if (number.Length == 7)     // if recognized number has 7 symbols, with defis. show it
                {

                    
                    {
                        plate = number;
                    }

                   
                }

            


            return plate;
        }



        public List<Bitmap> findPlates(UnmanagedImage image)
        {


            List<Bitmap> PlatesList = new List<Bitmap>();
            if ((image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (!Grayscale.CommonAlgorithms.BT709.FormatTranslations.ContainsKey(image.PixelFormat)))
            {
                throw new UnsupportedImageFormatException("Pixel format of the specified image is not supported.");
            }

            // 1 - grayscaling
            UnmanagedImage grayImage = null;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                grayImage = UnmanagedImage.Create(image.Width, image.Height, PixelFormat.Format8bppIndexed);
                Grayscale.CommonAlgorithms.BT709.Apply(image, grayImage);
            }

            // 2 - Edge detection
            UnmanagedImage edgesImage = edgeDetector.Apply(grayImage);

            // 3 - Threshold edges
            thresholdFilter.ApplyInPlace(edgesImage);

            // 4 - Blob Counter

            blobCounter.MinHeight = 35;
            blobCounter.MinWidth = 35;
            blobCounter.MaxHeight = 350;
            blobCounter.MaxWidth = 350;
            blobCounter.FilterBlobs = true;
            blobCounter.ProcessImage(edgesImage);
            Blob[] blobs = blobCounter.GetObjectsInformation();

          //  return edgesImage.ToManagedImage();

            Bitmap bitmap = image.ToManagedImage();
            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                List<IntPoint> corners = null;

                // does it look like a quadrilateral ?

                
                if (shapeChecker.IsQuadrilateral(edgePoints, out corners))
                {
                    // get edge points on the left and on the right side
                    List<IntPoint> leftEdgePoints, rightEdgePoints;
                    blobCounter.GetBlobsLeftAndRightEdges(blobs[i], out leftEdgePoints, out rightEdgePoints);
                    Graphics g = Graphics.FromImage(bitmap);
                    Pen pen = new Pen(Color.Red);
                            // highlight border
                    g.DrawPolygon(pen, ToPointsArray(corners));

                    QuadrilateralTransformation filter = new QuadrilateralTransformation(corners);//, 150, 50);
                    // apply the filter
                   // Bitmap newImage = filter.Apply(grayImage.ToManagedImage());

                    Bitmap mBitmap = filter.Apply(grayImage.ToManagedImage());
                    if (mBitmap.Height > mBitmap.Width)
                    {
                        RotateNearestNeighbor rotateFilter = new RotateNearestNeighbor(90);
                        mBitmap = rotateFilter.Apply(mBitmap);
                    }

                    PlatesList.Add(mBitmap);
                    
                    
                }
            }

            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                grayImage.Dispose();
            }
           // edgesImage.Dispose();

            PlatesList.Add(bitmap);

            return PlatesList;

        }
        
    }
}
