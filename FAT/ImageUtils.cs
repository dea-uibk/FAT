using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FAT
{
    public enum ImageColorType
    {
        Undefined,
        Binary,
        Grayscale,
        IndexedColor,
        RGB
    }

    public enum EndianType
    {
        LITTLE_ENDIAN,
        BIG_ENDIAN
    }

    public class ResolutionPlausibilityChecker
    {
        public static readonly int[,] DIN_A = new int[,]
                                          {
                                              {841, 1189}, // A0
                                              {594, 841}, // A1
                                              {420, 594}, // A2
                                              {297, 420}, // A3
                                              {210, 297}, // A4
                                              {148, 210}, // A5
                                              {105, 148}, // A6
                                          };

        public static readonly Dictionary<String, int> SIZES = new Dictionary<String, int>() {
                                                                                        { "Small", 4 },
                                                                                        { "Medium", 3 },
                                                                                        { "Large", 2 },
                                                                                     };

        public const double INCH_TO_M_FACTOR = (0.0254);
        public const double M_TO_INCH_FACTOR = 39.3700787;

        public static bool IsResolutionPlausible(double dpiX, double dpiY, uint width, uint height, double toleranceFactorLow = 0.0, double toleranceFactorHigh = 100)
        {
            //double wD = (double) width;
            //double hD = (double) height;

            double sizeXmm = ((double)width / (double)dpiX) * INCH_TO_M_FACTOR * 1e3;
            double sizeYmm = ((double)height / (double)dpiY) * INCH_TO_M_FACTOR * 1e3;

            //Trace.WriteLine("Plausibility check, sizeXmm = "+sizeXmm+", sizeYmm = "+sizeYmm);

            if (sizeXmm < toleranceFactorLow * DIN_A[4, 0])
                return false;
            if (sizeYmm < toleranceFactorLow * DIN_A[4, 1])
                return false;
            if (sizeXmm > toleranceFactorHigh * DIN_A[2, 0])
                return false;
            if (sizeYmm > toleranceFactorHigh * DIN_A[2, 1])
                return false;

            return true;
        }

        public static bool IsResolutionXPlausible(double dpiX, uint width, double toleranceFactorLow = 0.0, double toleranceFactorHigh = 100)
        {
            //double wD = (double) width;
            //double hD = (double) height;

            double sizeXmm = ((double)width / (double)dpiX) * INCH_TO_M_FACTOR * 1e3;

            //Trace.WriteLine("Plausibility check, sizeXmm = " + sizeXmm);

            if (sizeXmm < toleranceFactorLow * DIN_A[4, 0])
                return false;
            if (sizeXmm > toleranceFactorHigh * DIN_A[2, 0])
                return false;

            return true;
        }

        public static bool IsResolutionYPlausible(double dpiY, uint height, double toleranceFactorLow = 0.0, double toleranceFactorHigh = 100)
        {
            //double wD = (double) width;
            //double hD = (double) height;

            double sizeYmm = ((double)height / (double)dpiY) * INCH_TO_M_FACTOR * 1e3;

            //Trace.WriteLine("Plausibility check, sizeYmm = " + sizeYmm);

            if (sizeYmm < toleranceFactorLow * DIN_A[4, 1])
                return false;
            if (sizeYmm > toleranceFactorHigh * DIN_A[2, 1])
                return false;

            return true;
        }
    } // end class ResolutionPlausibilityChecker

    public class ImageMetadata
    {
        public uint Width=0;
        public uint Height = 0;
        public double XRes = -1.0f;
        public double YRes = -1.0f;
        public Boolean IsResOverwritten = false;
        //public uint[] Bitdepth = new uint[1]{1};
        public uint[] Bitdepth = null;
        public ImageColorType Colortype = ImageColorType.Undefined;
        public uint Colormap = 0;

        public string Checksum = "undefined";
        public long Size = 0;
        public string Mimetype = "undefined";
        public TiffCompressionType Compression = TiffCompressionType.COMPRESSION_UNDEFINED;
        public TiffResolutionUnitType ResolutionUnit = TiffResolutionUnitType.RESUNIT_NONE;
        public EndianType Endian = EndianType.LITTLE_ENDIAN;

        public new string ToString()
        {
            string str = "ImageMetadata: Width x Height: " + Width + " x " + Height;
            str += ", Xres x YRes = " + XRes + " x " + YRes;
            if (Bitdepth != null)
            {
                str += ", Bitdepth = ";

                str += "[";
                for (int i = 0; i < Bitdepth.Length; ++i )
                {
                    str += Bitdepth[i];
                    if (i + 1 == Bitdepth.Length) continue;
                    str += ", ";
                }
                str += "]";
            }

            str += ", Type = " + Colortype.ToString();
            str += ", Checksum = " + Checksum;
            str += ", Size = " + Size;
            str += ", Mimetype = " + Mimetype;
            str += ", Compression = " + Compression;
            str += ", ResolutionUnit = " + ResolutionUnit;
            str += ", IsResOverwritten = " + IsResOverwritten;
            str += ", Endian = " + Endian;

            return str;
        }
    }

    class ImageUtils
    {
        //enum PaletteFlags
        //{
        //    Alpha = 0x00000001,
        //    Grayscale = 0x00000002,
        //    Halftone= 0x00000004
        //}

        //static ImageUtils()
        //{
        //    // Check if FreeImage is available
        //    //if (!FreeImage.IsAvailable())
        //    //{
        //    //    throw new Exception("FreeImage is not available!");
        //    //}
        //    Console.WriteLine("FreeImag is available!");
        //}

        //public static void ReadImageMetaData(string filename)
        //{
        //    Image image = new Bitmap(filename);
        //    Console.WriteLine("Metadata for file " + filename);

        //    Console.WriteLine("Resolution horizontal " + image.HorizontalResolution);
        //    Console.WriteLine("Resolution vertical " + image.VerticalResolution);


        //    if ((image.Palette.Flags & 0x00000001) == 0x00000001)
        //    {
        //        Console.WriteLine("Image contains alpha info");
        //    }
        //    if ((image.Palette.Flags & 0x00000002) == 0x00000002)
        //    {
        //        Console.WriteLine("Image is grayscale");
        //    }
        //    if ((image.Palette.Flags & 0x00000004) == 0x00000001)
        //    {
        //        Console.WriteLine("Image has halftone color values");
        //    }

        //    //PropertyItem[] propItems = image.PropertyItems;
        //    //foreach (PropertyItem i in propItems)
        //    //{
        //    //    Console.WriteLine("Resolution horizontal "+image.HorizontalResolution);
        //    //    Console.WriteLine("Resolution vertical " + image.HorizontalResolution);

        //    //}
        //}


            //try
            //{
            //    // Error handling
            //    if (dib.IsNull)
            //    {
            //        // Chech whether FreeImage generated an error messe
            //        if (message != null)
            //        {
            //            MessageBox.Show("File could not be loaded!\nError:{0}", message);
            //        }
            //        else
            //        {
            //            MessageBox.Show("File could not be loaded!", message);
            //        }
            //        return;
            //    }

            //    // Read width
            //    lWidth.Text = String.Format("Width: {0}", FreeImage.GetWidth(dib));

            //    // Read height
            //    lHeight.Text = String.Format("Width: {0}", FreeImage.GetWidth(dib));

            //    // Read color depth
            //    lBPP.Text = String.Format("Color Depth: {0}", FreeImage.GetBPP(dib));

            //    // Read red bitmask (16 - 32 bpp)
            //    lRedMask.Text = String.Format("Red Mask: 0x{0:X8}", FreeImage.GetRedMask(dib));

            //    // Read green bitmask (16 - 32 bpp)
            //    lBlueMask.Text = String.Format("Green Mask: 0x{0:X8}", FreeImage.GetGreenMask(dib));

            //    // Read blue bitmask (16 - 32 bpp)
            //    lGreenMask.Text = String.Format("Blue Mask: 0x{0:X8}", FreeImage.GetBlueMask(dib));

            //    // Read image type (FI_BITMAP, FIT_RGB16, FIT_COMPLEX ect)
            //    lImageType.Text = String.Format("Image Type: {0}", FreeImage.GetImageType(dib));

            //    // Read x-axis dpi
            //    lDPIX.Text = String.Format("DPI X: {0}", FreeImage.GetResolutionX(dib));

            //    // Read y-axis dpi
            //    lDPIY.Text = String.Format("DPI Y: {0}", FreeImage.GetResolutionY(dib));

            //    // Read file format
            //    lFormat.Text = String.Format("File Format: {0}", FreeImage.GetFormatFromFIF(format));
            //}
            //catch
            //{
            //}

        //public static void ReadFreeImage(string filename, bool onlyHeader)
        //{

        //    FileInfo fi = new FileInfo(filename);
        //    long size = fi.Length;

        //    var filestream = new FileStream(fi.FullName, FileMode.Open);


        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    var sw = new Stopwatch();
        //    sw.Start();
        //    byte[] retVal = md5.ComputeHash(filestream);
        //    sw.Stop();
        //    Console.WriteLine("Elapsed time for checksum = "+sw.Elapsed.ToString());
        //    string checksum = System.BitConverter.ToString(retVal);
        //    //filestream.Close();

            

        //    // Format is stored in 'format' on successfull load.
        //    FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
        //    sw.Reset();
        //    sw.Start();
        //    // Try loading the file
        //    FREE_IMAGE_LOAD_FLAGS flags = 0;
            
        //    const int FIF_LOAD_NOPIXELS = 0x8000;
        //    if (onlyHeader)
        //    {
                
        //        flags = flags | (FREE_IMAGE_LOAD_FLAGS)FIF_LOAD_NOPIXELS;    
        //    }
            
        //    Console.WriteLine("flags are: "+flags);
        //    FIBITMAP dib = FreeImage.LoadFromStream(filestream, (FREE_IMAGE_LOAD_FLAGS)FIF_LOAD_NOPIXELS, ref format);
        //    //FreeImage.Load
            
            

            
            
        //    //FIBITMAP dib = FreeImage.LoadEx(filename, ref format);
        //    sw.Stop();
        //    Console.WriteLine("Elapsed time for load from stream = " + sw.Elapsed.ToString());

        //    sw.Reset();
        //    sw.Start();
        //    //Console.WriteLine("Image format is: "+format.ToString());
        //    uint width = FreeImage.GetWidth(dib);
        //    uint height = FreeImage.GetHeight(dib);
        //    uint resX = FreeImage.GetResolutionX(dib);
        //    uint resY = FreeImage.GetResolutionX(dib);
        //    string mime = FreeImage.GetFIFMimeType(format);
        //    PixelFormat pixelformat = FreeImage.GetPixelFormat(dib);
        //    sw.Stop();
        //    Console.WriteLine("Elapsed time for metadata = " + sw.Elapsed.ToString());
        //    //Console.WriteLine("PixelFormat is: " + pixelformat.ToString());
        //    //FreeImage.
            

        //    //uint height = FreeImage.GetHeight(dib);
        //    //uint height = FreeImage.GetHeight(dib);

        //    // Always unload bitmap
        //    FreeImage.UnloadEx(ref dib);
        //    filestream.Close();
        //}

        //public static void ReadFreeImageStream(string filename)
        //{
        //    FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
        //    FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        //    FreeImage.LoadFromStream(stream, ref format);
        //}

        public static void ReadImageHeader(string fn, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        {
            if (fn.EndsWith(".jpg") || fn.EndsWith(".jpeg"))
            {
                JpgUtils.ReadJpgHeader(fn, ref metadata, doPlausibilityCheck, thresholdResX, thresholdResY, overwriteResX, overwriteResY);
            }
            else if (fn.EndsWith(".tif") || fn.EndsWith(".tiff"))
            {
                TiffUtils.ReadTifHeader(fn, ref metadata, doPlausibilityCheck, thresholdResX, thresholdResY, overwriteResX, overwriteResY);
            }
            else if (fn.EndsWith(".jp2") || fn.EndsWith(".j2k"))
            {
                Jp2Utils.ReadJp2Header(fn, ref metadata, doPlausibilityCheck, thresholdResX, thresholdResY, overwriteResX, overwriteResY);
            }
            else
            {
                throw new NotSupportedException("ReadImageHeader: Filetype not supported: " + fn);
            }
        }

        public static void ReadSomeFiles(string [] files)
        {
            try
            {
                var sw = new Stopwatch();
                foreach (string fn in files)
                {
                    sw.Reset();
                    sw.Start();
                    var metadata = new ImageMetadata();
                    //ReadImageHeader(fn, ref metadata);
                    ReadImageHeader(fn, ref metadata); // with plausibility check
                    sw.Stop();
                    Trace.WriteLine("Metadata for file " + fn + ":");
                    Trace.WriteLine("\t" + metadata.ToString());
                    //Console.WriteLine("Elapsed time = " + sw.Elapsed.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Excption message: "+e.Message);
            }

        }


        public static void Main(string [] args)
        {
            Console.WriteLine("Starting ImageUtils!");

            var testfiles = new string[]
                                  {
                                      //@"external_resources/test_images/jpeg2000_test.jp2",

                                      // jpeg progressive dct files: (FIXED!)
                                      //@"E:\LFT\LFT_00006\1849\BZZ_18490608\1849_06_0001.jpg",
                                      //@"E:\LFT\LFT_00006\1849\BZZ_18490608\1849_06_0002.jpg",

                                      // tif files with resolution error but irfanview has 300x300:

                                       //@"E:\LFT\LFT_00006\1850\BZZ_18500202\1850_02_0005.tif",
                                       // @"E:\LFT\LFT_00006\1850\BZZ_18500717\1850_07_0034.tif",
                                       // @"E:\LFT\LFT_00006\1850\BZZ_18500904\1850_09_0005.tif",
                                       // @"E:\LFT\LFT_00006\1850\BZZ_18500911\1850_09_0015.tif",

                                        // tif image with low resolution (40x40):
                                        //@"C:\testimages\suedtirol_low_res_40_40.tif",

                                        // big endian files:
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0124.tif",
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0125.tif",
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0126.tif",
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0127.tif",
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0128.tif",
                                        //@"E:\LFT\LFT_00006\1907\BZZ_19070316\1907_03_0129.tif",

                                        @"c:\bnf\HH_00006\1882\Issue_18820102\00000001.jpg", // ICC profiles jpeg
                                        @"c:\bnf\HH_00006\1882\Issue_18820102\00000002.jpg", // ICC profiles jpeg
                                  };

            ReadSomeFiles(testfiles);
        }


    }
}
