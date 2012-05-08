using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;


namespace myALPR1
{
    using AForge.Imaging.Filters;
    class ALPRclass

    {
        int regionsCount;
        Bitmap[] candidates = new Bitmap[30];
        int[] arrMagnitude;
        ContrastCorrection contFilter = new ContrastCorrection();
        int[] POIs;
        IFilter grayfilter = new GrayscaleBT709();
        int[,] kernel = new int[3, 3] {  	 {-1, 0, 1},
											 {-1, 0, 1},
											 {-1, 0, 1}};

        OtsuThreshold otsuThresholdFilter = new OtsuThreshold();

        /// main values here
        static int risingInterval = 4;
        static double ratio = 1.1;//1.1;
        //static double difference = 4.5;  
 
       // static int risingInterval = 4;
      //  static double ratio = 0.6;



        public class ReturnedValue
        {
            public String[] RecognizedPlates;  
            
            public Bitmap BMap = new Bitmap(640,480);
            public Bitmap GraphBMP;
            public   ReturnedValue()
            {
               RecognizedPlates = new String[3];
               RecognizedPlates[0] = "*";
               RecognizedPlates[1] = "*";
               RecognizedPlates[2] = "*";

            }
        }
        List<PointOfInterest> arrPInterest = new List<PointOfInterest>();
        PRegion[] POIregions = new PRegion[20];
        PictureBox[] croppedRegions;
        public MyOCRClass2 mOcr = new MyOCRClass2();


        public void setSumCorrelation(int num)
        {
            mOcr.setSumCorrelation(num);
        }

        public int getSumCorrealtion()
        {
            return mOcr.getSumCorrelation();
        }


        public void doTheShitWithBlobForm(Bitmap actImage)
        {
            arrMagnitude = new int[actImage.Height];
            POIs = new int[actImage.Height];
            regionsCount = 0;
            arrPInterest.Clear();
            Bitmap newImage = grayfilter.Apply(actImage);
            IFilter conv = new Convolution(kernel);//Correlation(kernel); 
            newImage = conv.Apply(newImage);

            /// clip plate line candidates
            /// 
            countMagnitudesHorizontalProj((Bitmap)newImage);
            clip();
            makeOneLineIteration();
            pairInRange();
            fillarrPInterest();
            pairsSequence(0);
            fillarrPInterest();
            /// end clip line  
            ///           
            cropBandVertical(newImage, actImage);  
            OLOLOLCandidates(actImage);
        }
        public ReturnedValue doTheShit2(Bitmap actImage)
        {
            ReturnedValue ret = new ReturnedValue();
            arrMagnitude = new int[actImage.Height];
            POIs = new int[actImage.Height];
            regionsCount = 0;
            arrPInterest.Clear();
            Bitmap newImage = grayfilter.Apply(actImage);
            IFilter conv = new Convolution(kernel);//Correlation(kernel); 
            newImage = conv.Apply(newImage);

            /// clip plate line candidates
            /// 
            countMagnitudesHorizontalProj((Bitmap)newImage);
            clip();
            makeOneLineIteration();
            pairInRange();
            fillarrPInterest();
            pairsSequence(0);
            fillarrPInterest();
            /// end clip line  
            ///           
            ret.GraphBMP = drawGraph();
            cropBandVertical(newImage, actImage);
            ret.RecognizedPlates = OCRcandidates(actImage);
            ret.BMap = drawPOIs(actImage);
            return ret;
        }
        public ReturnedValue doTheShit(Bitmap actImage)
        {
            ReturnedValue ret = new ReturnedValue();
            if (actImage != null)
            {
                arrMagnitude = new int[480];//actImage.Height];
                POIs = new int[640];//actImage.Height];
                regionsCount = 0;
                arrPInterest.Clear();
                Bitmap newImage = grayfilter.Apply(actImage);
                IFilter conv = new Convolution(kernel);//Correlation(kernel); 
                newImage = conv.Apply(newImage);

                /// clip plate line candidates
                /// 
                countMagnitudesHorizontalProj((Bitmap)newImage);
                clip();
                makeOneLineIteration();
                pairInRange();
                fillarrPInterest();
                pairsSequence(0);
                fillarrPInterest();
                /// end clip line  
                ///   
                ret.GraphBMP = drawGraph();
                cropBandVertical(newImage, actImage);
                ret.RecognizedPlates = OCRcandidates(actImage);
                ret.BMap = drawPOIs(actImage);
                return ret;
            }
            return ret;
        }
        private Bitmap draw_Matrix(MyOCRClass2.Matrix m)
        {
            Bitmap bmp = new Bitmap(m.Width, m.Height);//30, 40);

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    //if (m.matrix[i, j] == 1)
                    //{
                    //    bmp.SetPixel(i, j, Color.White);
                    //}

                    if (m.matrix[i, j] == 0)
                    {
                        bmp.SetPixel(i, j, Color.Black);
                    }
                }
            }
            return bmp;
        }
        private int blobDistance(Blob b1, Blob b2)
        {
            return b2.Rectangle.X - b1.Rectangle.Right;
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
        #region OLOLO Candidates old code stuff with form
        public void OLOLOLCandidates(Bitmap actBitmap)
        {
           
            for (int i = 0; i < regionsCount; i++)
            {

                //IFilter SISfilter = new SISThreshold();
                OtsuThreshold SISfilter = new OtsuThreshold();
                Bitmap tempBitmap = new Bitmap(candidates[i]);
                IFilter medianFilter = new Median();
                IFilter erosion = new Erosion();
                tempBitmap = grayfilter.Apply(candidates[i]);
                tempBitmap = SISfilter.Apply(tempBitmap);



                BlobCounter blobCounter = new BlobCounter(tempBitmap);
                Blob[] blobs = blobCounter.GetObjects(tempBitmap, false);

                for (int blobIndex = 0; blobIndex < blobs.Length; blobIndex++)
                {

                    if (blobs[blobIndex].Image.Height >= tempBitmap.Height - 10 &&
                        blobs[blobIndex].Image.Width / blobs[blobIndex].Image.Height >= 3 &&
                        blobs[blobIndex].Image.Width / blobs[blobIndex].Image.Height <= 6)
                    {
                        BlobForm f = new BlobForm();
                        Rectangle actualPlateRect = blobs[blobIndex].Rectangle;
                        Rectangle plateRect = blobs[blobIndex].Rectangle;
                        actualPlateRect.Y = actualPlateRect.Y + POIregions[i].top.pos - 4;
                        plateRect.Y = plateRect.Y + plateRect.Height / 3;
                        plateRect.Height = plateRect.Height - plateRect.Height / 2;

                        Bitmap cloned = tempBitmap.Clone(plateRect, PixelFormat.Format32bppArgb);
                        ResizeNearestNeighbor Resizefilter = new ResizeNearestNeighbor(200, 40);
                        Bitmap resized = Resizefilter.Apply(cloned);
                        int[] arr = countMagnitudesVertical(resized, f);//tempBitmap.Clone(plateRect, PixelFormat.Format32bppArgb), f);
                        IFilter inv = new Invert();


                        int lines = monotonus(arr);
                        if (lines > 15)                 // if Plate found (more than 15 balck - white lines)
                        {
                            drawRectangle(actualPlateRect,actBitmap);
                            Bitmap res2 = Resizefilter.Apply(actBitmap.Clone(actualPlateRect, PixelFormat.Format24bppRgb));
                            actualPlateRect.Y = actualPlateRect.Y + 3;
                            actualPlateRect.Height = actualPlateRect.Height - 5;
                            actualPlateRect.X = (int)(actualPlateRect.X + actualPlateRect.Height * 0.7);
                            actualPlateRect.Width = (int)(actualPlateRect.Width - actualPlateRect.Height * 0.7);

                            Bitmap res = Resizefilter.Apply(actBitmap.Clone(actualPlateRect, PixelFormat.Format24bppRgb));
                            res = grayfilter.Apply(res);
                            res = medianFilter.Apply(res);


                            res = SISfilter.Apply(res);
                            res = inv.Apply(res);

                            f.loadPicture(res);
                            f.Show();
                            BlobCounter blobCounter2 = new BlobCounter(res); // blob counter for letters, trying to find letter and ocr it
                            Blob[] blobs2 = blobCounter2.GetObjects(res, false);

                            bool flag = true;
                            sortBlobs(blobs2);
                            int symbolCandidates = 0;
                            for (int blobIndex2 = 0; blobIndex2 < blobs2.Length; blobIndex2++)
                            {
                                if (blobs2[blobIndex2].Image.Height >= (int)(res.Height - res.Height * 0.3) &&
                                    blobs2[blobIndex2].Image.Width >= (int)(res.Width * 0.02) &&
                                    blobs2[blobIndex2].Image.Width <= 40 &&
                                    
                                    flag == true)                              
                                {
                                   
                                    Bitmap letter;
                                    Rectangle biggerLetterRect = blobs2[blobIndex2].Rectangle;
                                    letter = blobs2[blobIndex2].Image.ToManagedImage();
                                    ResizeNearestNeighbor Resizefilter2 = new ResizeNearestNeighbor(30, 40);  // resize letter , 30 x 40 is fine
                                    letter = Resizefilter2.Apply(letter);

                                    f.addPicture(letter);

                                    mOcr.compareMatrix(mOcr.createMatrix(letter));    // create matrix from image of character and copare it with base

                                    f.addPicture2(draw_Matrix(mOcr.createMatrix(letter)));

                                    int n = mOcr.getIndexOfmaxLetter();    // get index of character that has max match "points"

                                    if (symbolCandidates < 3)
                                    {
                                        f.addLetter(mOcr.getChar());

                                    }
                                    else
                                    {
                                        f.addLetter(mOcr.getNumeric());
                                    }
                                    symbolCandidates++;



                                    f.addLetter2(mOcr.getIndexOfmaxCorrelation().ToString());
                                    f.addLetter3(mOcr.getPREMaxLetter());


                                    //Image<Bgr, Byte> image = new Image<Bgr, byte>(letterWithCanvas);
                                    //using (Image<Gray, byte> gray = image.Convert<Gray, Byte>())
                                    //{

                                    //    _ocr.Recognize(gray);
                                    //    Tesseract.Charactor[] charactors = _ocr.GetCharactors();
                                    //    String text = _ocr.GetText();
                                    //    f.addLetter(text);

                                    //}
                                }

                            }

                        }
                        f.setText1(lines.ToString());




                    }


                }
            }





        }
        #endregion
        private String[] OCRcandidates(Bitmap actBitmap)          
        {

            String[] plates = new String[3] { "-", "-", "-" };
            int recognizedNumbersCount = 0;
            for (int i = 0; i < regionsCount; i++)     // for each region ,extract plate-like blob
            {

                IFilter SISfilter = new SISThreshold();
                Bitmap tempBitmap = new Bitmap(candidates[i]);
                IFilter medianFilter = new Median();
                IFilter erosion = new Erosion();
                tempBitmap = grayfilter.Apply(candidates[i]);
                tempBitmap = SISfilter.Apply(tempBitmap);
                
                //  BlobCounter blobCounter;


                BlobCounter blobCounter = new BlobCounter(tempBitmap);
                Blob[] blobs = blobCounter.GetObjects(tempBitmap, false);

                for (int blobIndex = 0; blobIndex < blobs.Length; blobIndex++)  // for each plate-like blob check size and proportions
                {

                    if (blobs[blobIndex].Image.Height >= tempBitmap.Height - 10 &&
                        blobs[blobIndex].Image.Width / blobs[blobIndex].Image.Height >= 3 &&
                        blobs[blobIndex].Image.Width / blobs[blobIndex].Image.Height <= 6)
                    {
                        BlobForm f = new BlobForm();
                        Rectangle actualPlateRect = blobs[blobIndex].Rectangle;
                        Rectangle plateRect = blobs[blobIndex].Rectangle;
                        actualPlateRect.Y = actualPlateRect.Y + POIregions[i].top.pos - 4;
                        plateRect.Y = plateRect.Y + plateRect.Height / 3;
                        plateRect.Height = plateRect.Height - plateRect.Height / 2;


                        //QuadrilateralTransformation quadrilateralTransformation =
                        //new QuadrilateralTransformation(quadrilateral, 100, 100);

                        Bitmap cloned = tempBitmap.Clone(plateRect, PixelFormat.Format32bppArgb);
                        ResizeNearestNeighbor Resizefilter = new ResizeNearestNeighbor(200, 40);       //resize plate image
                        Bitmap resized = Resizefilter.Apply(cloned);
                        int[] arr = countMagnitudesVertical(resized, f);
                        IFilter inv = new Invert();
                       

                        int lines = monotonus(arr);       // check plates lines count , lines represent B\W edges on plate
                        if (lines > 15)
                        {

                            String number = "";

                            drawRectangle(actualPlateRect, actBitmap);
                            if (actualPlateRect.Bottom > actBitmap.Height)
                            {
                                actualPlateRect.Height = actualPlateRect.Height - (actualPlateRect.Bottom - actBitmap.Height);
                            }
                            Bitmap res2 = Resizefilter.Apply(actBitmap.Clone(actualPlateRect, PixelFormat.Format24bppRgb));
                            actualPlateRect.Y = actualPlateRect.Y + 3;
                            actualPlateRect.Height = actualPlateRect.Height - 5;
                            actualPlateRect.X = (int)(actualPlateRect.X + actualPlateRect.Height * 0.7);
                            actualPlateRect.Width = (int)(actualPlateRect.Width - actualPlateRect.Height * 0.7);

                            Bitmap res = Resizefilter.Apply(actBitmap.Clone(actualPlateRect, PixelFormat.Format24bppRgb));
                            res = grayfilter.Apply(res);
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
                                if (blobs2[blobIndex2].Image.Height >= (int)(res.Height - res.Height * 0.3) &&
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
                                    mOcr.compareMatrix(mOcr.createMatrix(letterWithCanvas));   // create binary matrix from extracted character blob   and compare with base of character matrix

                                    // int n = mOcr.getIndexOfmaxLetter();


                                    if (symbolCandidates < 3)   // compare firs 3 chars with letters base
                                    {
                                        number += mOcr.getChar();
                                    }
                                    else
                                    {
                                        number += mOcr.getNumeric();            // and next 3 chars with numbers
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

                                if (recognizedNumbersCount == 0)
                                {
                                    plates[0]= number;
                                }

                                if (recognizedNumbersCount == 1)
                                {
                                    plates[1]= number;
                                }

                                if (recognizedNumbersCount == 2)
                                {
                                    plates[2]= number;
                                }
                                recognizedNumbersCount++;
                            }

                        }

                    }

                }
            }
            return plates;
        }
        private int monotonus(int[] arr)      // count B\W changeover
        {
            int lines = 0;
            int lastIndex = 0;
            int col = arr[0];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != col)
                {
                    lines++;
                    col = arr[i];
                    lastIndex = i;
                }
            }
            return lines;
        }
        public Bitmap drawRectangle(Rectangle rect, Bitmap sourceBMP)  // Draw  Yellow rectangles around License plate
        {
            Bitmap myBitmap2 = sourceBMP;
            if (rect.Bottom > sourceBMP.Height)
            {
                rect.Height = rect.Height - (rect.Bottom - sourceBMP.Height);
            }

            for (int x = rect.X; x < rect.Right && x<sourceBMP.Width ; x++)
            {
                myBitmap2.SetPixel(x, rect.Y, Color.Yellow);
                myBitmap2.SetPixel(x, rect.Bottom - 1, Color.Yellow);
            }

            for (int y = rect.Y; y < rect.Bottom && y < sourceBMP.Height && y>0; y++)
            {
                myBitmap2.SetPixel(rect.X, y, Color.Yellow);
                myBitmap2.SetPixel(rect.Right - 1, y, Color.Yellow);
            }
            return myBitmap2;
            
        }
        public void cropBandVertical(Bitmap img, Bitmap actBitmap)   // crop initial image to bands with plate candidates
        {
            croppedRegions = new PictureBox[regionsCount];
            for (int i = 0; i < regionsCount; i++)
            {
               
                Rectangle rect = new Rectangle(0, POIregions[i].top.pos - 4, img.Width, ((POIregions[i].bottom.pos + 10) - POIregions[i].top.pos));
                if (rect.Bottom > actBitmap.Height)
                {
                   // rect.Height = rect.Height-(rect.Bottom - actBitmap.Height);
                    regionsCount--;
                }

                if (rect.Bottom < actBitmap.Height)
                {
                    candidates[i] = actBitmap.Clone(rect, PixelFormat.Format32bppArgb);
                }

            }
        }
        public Bitmap addCanvas(Bitmap small, int canvasSize)  // add canvas to picture NO MORE NEEDED
        {
            Bitmap withCanvas;
            unsafeBitmapClass fastPictureSmall = new unsafeBitmapClass(small);
            fastPictureSmall.LockBitmap();

            if (small.Width < 6)
            {
                withCanvas = new Bitmap(small.Width + canvasSize * 4, small.Height + canvasSize * 2);
            }
            else
            {
                withCanvas = new Bitmap(small.Width + canvasSize * 2, small.Height + canvasSize * 2);

            }

            //withCanvas = new Bitmap(small.Width + canvasSize*2, small.Height + canvasSize*2);
            unsafeBitmapClass fastPictureBig = new unsafeBitmapClass(withCanvas);
            fastPictureBig.LockBitmap();



            for (int x = 0; x < small.Width; x++)
            {

                for (int y = 0; y < small.Height; y++)
                {


                    PixelData myPixel = fastPictureSmall.GetPixel(x, y);

                    fastPictureBig.SetPixel(x + canvasSize, y + canvasSize, myPixel);

                }
            }




            fastPictureBig.UnlockBitmap();
            fastPictureSmall.UnlockBitmap();
            withCanvas = fastPictureBig.Bitmap;

            return withCanvas;
        }
        public struct PRegion                // Region for clipping image
        {
            public PointOfInterest top;
            public PointOfInterest bottom;
        };
        public class PointOfInterest              // Points of interest on graph ( rising and lowering parts)
        {
            public int col;
            public int pos;
            public PointOfInterest(int c, int p)
            {
                this.col = c;
                this.pos = p;
            }

        }
        private Bitmap drawPOIs(Bitmap sourceBMP)               // Draw lines for POIS
        {

            Bitmap BitM = sourceBMP;
            for (int i = 3; i < POIs.Length; i++)
            {
                if (POIs[i] == 1)
                {
                  BitM =  drawLineOnImage(i, Color.Red, sourceBMP);

                } if (POIs[i] == 2)
                {
                  BitM =  drawLineOnImage(i, Color.Green, sourceBMP);
                }

            }
            return BitM;
        }
        private void makeOneLineIteration()// sort POIS like rising\lowering\rising etc. and fill arrPInterest
        {
            for (int i = 0; i < POIs.Length; i++)
            {
                makeOneLinesRecursive(i);
            }
        }
        private void makeOneLinesRecursive(int i)
        {
            if (i < POIs.Length - 1)
            {
                if (POIs[i] == 1)
                {
                    makeOneLinesRecursive(i + 1);
                    POIs[i + 1] = 0;

                }
                if (POIs[i] == 2)
                {
                    makeOneLinesRecursive(i + 1);
                    POIs[i + 1] = 0;

                }
                if (POIs[i] == 0)
                {
                    return;
                }
            }

        }
        private int nextGreen(int redPos)               //distance to next green POI
        {
            for (int i = redPos; i < POIs.Length; i++)
            {
                if (POIs[i] == 2)
                {
                    return i;
                }

            }
            return -1;
        }
        private void fillarrPInterest()
        {
            for (int i = 1; i < POIs.Length - 1; i++)
            {


                if (this.POIs[i] > 0)
                {
                    arrPInterest.Add(new PointOfInterest(POIs[i], i));
                }
            }
        }
        private void pairInRange()                        // Find red-green pairs in range
        {

            for (int i = 0; i < POIs.Length; i++)
            {
                if (POIs[i] == 1)
                {

                    if (nextGreen(i) - i < 10 && nextGreen(i) > 0)
                    {
                        POIs[nextGreen(i)] = 0;
                        // arrPInterest.RemoveAt(nextGreen(i));

                    }

                    if (nextGreen(i) - i > 30)
                    {
                        POIs[i] = 0;
                        //arrPInterest.RemoveAt(i);

                    }

                }

            }

        }
        private void pairsSequence(int i)
        {
            if (i == arrPInterest.Count - 1)
            {
                POIs[arrPInterest[i].pos] = 0;
                return;
            }
            if (i <= arrPInterest.Count - 1)
            {
                if (arrPInterest[i].col == 1 && arrPInterest[i + 1].col == 2)
                {
                    POIregions[regionsCount].top = arrPInterest[i];
                    POIregions[regionsCount].bottom = arrPInterest[i + 1];
                    regionsCount++;

                    pairsSequence(i + 2);
                }
                else
                {
                    POIs[arrPInterest[i].pos] = 0;
                    pairsSequence(i + 1);
                }
            }
        }
        private void clip() ////////////////////////////////////////////////     wierd stuff here    find POIs on graph
        {
            int min = arrMagnitude.Min();
            int max = arrMagnitude.Max();

            double difference = (Math.Log(max) + Math.Log(min)) * (Math.Log10(max - min) * 0.035);  //4.5;

            for (int i = risingInterval; i < arrMagnitude.Length; i++)
            {
                float a = arrMagnitude[i];
                float b = arrMagnitude[i - risingInterval];
                //function rising

                float range = b - a;
                float range2 = b / a;
                if (range > (max - min) / difference && range2 > ratio)
                {
                    //  drawLineOnGraph(i, Color.Green);
                    POIs[i] = 2;
                }

                //function lowering 
                float range3 = a - b;
                float range4 = a / b;
                if (range3 > (max - min) / difference && range4 > ratio)
                {
                    //  drawLineOnGraph(i, Color.Red);
                    POIs[i] = 1;
                }

            }
        }
        private Bitmap drawLineOnImage(int y, Color col,Bitmap sourceBMP)
        {

            Bitmap myBitmap2 = sourceBMP;


            for (int i = 0; i < sourceBMP.Width; i++)
            {
                myBitmap2.SetPixel(i, y, col);

            }
            return  myBitmap2;

           


        }
        private Bitmap drawLineOnGraph(int y, Color col,Bitmap sourceBMP)
        {
            Bitmap myBitmap = sourceBMP;


            for (int i = 0; i < sourceBMP.Width; i++)
            {
                myBitmap.SetPixel(i, y, col);

            }



           return myBitmap;

          

        }
        private Bitmap drawGraph()
        {
            Bitmap myBitmap = new Bitmap(100, 480);
            int min = arrMagnitude.Min();
            for (int i = 0; i < myBitmap.Height; i++)
            {

                try
                {
                    myBitmap.SetPixel(((arrMagnitude[i] - min) / 1000), i, Color.Black);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }



            }

             return myBitmap;
          
        }
        //private void drawGraphCandidates()
        //{
        //    Bitmap myBitmap = new Bitmap(pictureBox5.Width,pictureBox5.Height);
        //    int min = vertMagnitude1.Min();
        //    for (int i = 0; i < pictureBox5.Width; i++)
        //    {
        //        myBitmap.SetPixel(i, ((vertMagnitude1[i] / 500)), Color.Black); // ((vertMagnitude1[i] - min / 10000))
        //    }

        //    pictureBox5.Image = myBitmap;
        //    pictureBox5.Invalidate();
        //}
        private void countMagnitudesHorizontalProj(Bitmap bmp)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            unsafeBitmapClass fastPicture = new unsafeBitmapClass(bmp);
            fastPicture.LockBitmap();

            for (int y = 0; y < height; y++)
            {


                for (int x = 0; x < width; x++)
                {

                    PixelData myPixel = fastPicture.GetPixel(x, y);
                    arrMagnitude[y] += (myPixel.red + myPixel.green + myPixel.blue);


                }
            }

            fastPicture.UnlockBitmap();
        }
        private int[] countMagnitudesVertical(Bitmap bmp, BlobForm f)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            int area = height * width;
            int sum = 0;
            int[] vertMagnitude = new int[width];
            unsafeBitmapClass fastPicture = new unsafeBitmapClass(bmp);
            fastPicture.LockBitmap();
            Bitmap zebra = new Bitmap(width + 1, height + 1);

            for (int x = 0; x < width; x++)
            {
                //bool black = false;//1


                int black = 0;
                int white = 0;
                for (int y = 0; y < height; y++)
                {


                    PixelData myPixel = fastPicture.GetPixel(x, y);
                    if ((myPixel.red + myPixel.green + myPixel.blue) < 100)
                    {

                        //black = true;
                        black++;
                    }
                    else
                    {
                        white++;
                    }
                }
                //for (int h = 0; h < black; h++)
                //{
                //    zebra.SetPixel(x, h, Color.Black);
                //}
                // zebra.SetPixel(x, black, Color.Black);
                //vertMagnitude[x] = black;
                //sum += black;
                if (black > white / 3)//black ==true)
                {
                    vertMagnitude[x] = 1;
                    for (int h = 0; h < 20; h++)
                    {
                        zebra.SetPixel(x, h, Color.Black);
                    }

                }
                else
                {
                    vertMagnitude[x] = 0;

                }
            }

            // f.loadPicture2(zebra);            
            fastPicture.UnlockBitmap();
            return vertMagnitude;


        }
    }
}
