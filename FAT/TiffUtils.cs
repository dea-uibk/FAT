using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using FAT;

namespace FAT
{
    public enum TiffTags
    {
        None = (ushort)0,
        ImageWidth = 256,
        ImageHeight = 257,
        BitsPerSample = 258,
        Compression = 259,
        PhotometricInterpretation = 262,
        StripOffsets = 273,
        SamplesPerPixel = 277,
        RowsPerStrip = 278,
        StripByteCounts = 279,
        XResolution = 282,
        YResolution = 283,
        ResolutionUnit = 296,
        ColorMap = 320 // only palette-color images
    }
    public enum TiffResolutionUnitType
    {
        RESUNIT_NONE = 1,
        RESUNIT_INCH = 2,
        RESUNIT_CENTIMETER = 3,
    }

    public enum TiffCompressionType
    {
        COMPRESSION_UNDEFINED = 0,
        COMPRESSION_NONE = 1,
        COMPRESSION_CCITTRLE = 2,
        COMPRESSION_CCITTFAX3 = 3,
        COMPRESSION_CCITT_T4 = 3,
        COMPRESSION_CCITTFAX4 = 4,
        COMPRESSION_CCITT_T6 = 4,
        COMPRESSION_LZW = 5,
        COMPRESSION_OJPEG = 6,
        COMPRESSION_JPEG = 7,
        COMPRESSION_NEXT = 32766,
        COMPRESSION_CCITTRLEW = 32771,
        COMPRESSION_PACKBITS = 32773,
        COMPRESSION_THUNDERSCAN = 32809,
        COMPRESSION_IT8CTPAD = 32895,
        COMPRESSION_IT8LW = 32896,
        COMPRESSION_IT8MP = 32897,
        COMPRESSION_IT8BL = 32898,
        COMPRESSION_PIXARFILM = 32908,
        COMPRESSION_PIXARLOG = 32909,
        COMPRESSION_DEFLATE = 32946,
        COMPRESSION_ADOBE_DEFLATE = 8,
        COMPRESSION_DCS = 32947,
        COMPRESSION_JBIG = 34661,
        COMPRESSION_SGILOG = 34676,
        COMPRESSION_SGILOG24 = 34677,
        COMPRESSION_JP2000 = 34712
    }

    class TiffUtils
    {
        public struct TiffIfDEntry
        {
            public ushort Tag;
            public ushort Type;
            public uint Count;
            public UInt32 OffsetOrValueUInt32;
            public UInt16 OffsetOrValueUInt16;
            //public int OffsetOrValueUInt32;
            
            /**
             * Checks whether the offset/value field contains an offset, ie the values do not fit into 4 bytes
             */
            public bool IsOffset()
            {
                return GetTifTypeSize(Type)*Count > 4;
            }
            /**
             * Checks whether the offset/value field contains a long, ie a UInt32 value
             */
            public bool IsLong()
            {
                return GetTifTypeSize(Type)*Count == 4;
            }
            /**
             * Checks whether the offset/value field contains a short, ie a UInt16 value
             */
            public bool IsShort()
            {
                return GetTifTypeSize(Type) * Count == 2;
            }

            public static TiffIfDEntry MakeXResEntry(uint offset)
            {
                var xresEntry = new TiffIfDEntry
                                    {Tag = (ushort) TiffTags.XResolution, Count = 1, Type = 5, OffsetOrValueUInt32 = offset};

                return xresEntry;
            }

            public static TiffIfDEntry MakeYResEntry(uint offset)
            {
                var yresEntry = new TiffIfDEntry
                                    {Tag = (ushort) TiffTags.YResolution, Count = 1, Type = 5, OffsetOrValueUInt32 = offset};

                return yresEntry;
            }

            public byte[] GetByteArray()
            {
               var tmp = new byte[12];
                
               BitConverter.GetBytes(Tag).CopyTo(tmp, 0);
               BitConverter.GetBytes(Type).CopyTo(tmp, 2);
               BitConverter.GetBytes(Count).CopyTo(tmp, 4);
               BitConverter.GetBytes(OffsetOrValueUInt32).CopyTo(tmp, 8);

                return tmp;
            }
        }

        public static TiffTags[] BilevelRequiredTifTags = new TiffTags[]
                                                             {
                                                             TiffTags.ImageWidth, TiffTags.ImageWidth,
                                                             TiffTags.Compression, TiffTags.PhotometricInterpretation, TiffTags.StripOffsets, 
                                                             TiffTags.RowsPerStrip, TiffTags.StripByteCounts, TiffTags.XResolution, TiffTags.YResolution, 
                                                             TiffTags.ResolutionUnit
                                                             };
        public static TiffTags[] GrayscaleRequiredTifTags = new TiffTags[]
                                                             {
                                                             TiffTags.ImageWidth, TiffTags.ImageWidth, TiffTags.BitsPerSample, 
                                                             TiffTags.Compression, TiffTags.PhotometricInterpretation, TiffTags.StripOffsets, 
                                                             TiffTags.RowsPerStrip, TiffTags.StripByteCounts, TiffTags.XResolution, TiffTags.YResolution, 
                                                             TiffTags.ResolutionUnit
                                                             };
        public static TiffTags[] PaletteColorRequiredTifTags = new TiffTags[]
                                                             {
                                                             TiffTags.ImageWidth, TiffTags.ImageWidth, TiffTags.BitsPerSample, 
                                                             TiffTags.Compression, TiffTags.PhotometricInterpretation, TiffTags.StripOffsets, 
                                                             TiffTags.RowsPerStrip, TiffTags.StripByteCounts, TiffTags.XResolution, TiffTags.YResolution, 
                                                             TiffTags.ResolutionUnit
                                                             };
        public static TiffTags[] FullColorRequiredTifTags = new TiffTags[]
                                                             {
                                                             TiffTags.ImageWidth, TiffTags.ImageWidth, TiffTags.BitsPerSample, 
                                                             TiffTags.Compression, TiffTags.PhotometricInterpretation, TiffTags.StripOffsets, 
                                                             TiffTags.RowsPerStrip, TiffTags.StripByteCounts, TiffTags.XResolution, TiffTags.YResolution, 
                                                             TiffTags.ResolutionUnit
                                                             };

        public static ushort GetTifTypeSize(int type)
        {
            switch (type)
            {
                case 1: // byte
                    return 1;
                case 2: // ascii
                    return 1;
                case 3: // short (16 bit uint)
                    return 2;
                case 4: // long (32 bit uint)
                    return 4;
                case 5: // rational (two longs: numerator denominator)
                    return 8;
                // NEW TYPES (as of tif 6.0):
                case 6: // sbyte
                    return 1;
                case 7: // undefined --> array of bytes...
                    return 1;
                case 8: // sshort
                    return 2;
                case 9: // slong
                    return 4;
                case 10: // srational
                    return 8;
                case 11: // float
                    return 4;
                case 12: // double
                    return 8;
                default:
                    return 0;
            }
        }

        private static void ReadTifIdfEntry(TiffIfDEntry entry, FileStream filestream, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        {
            // read IFD entry
            switch ((TiffTags)entry.Tag)
            {
                case TiffTags.ImageWidth:
                    //Trace.WriteLine("reading width... islong = "+entry.IsLong() + " count = "+entry.Count + " type = "+entry.Type);
                    metadata.Width = entry.IsLong() ? entry.OffsetOrValueUInt32 : entry.OffsetOrValueUInt16;
                    break;
                case TiffTags.ImageHeight:
                    //Trace.WriteLine("reading height... islong = " + entry.IsLong() + " count = " + entry.Count + " type = " + entry.Type);
                    metadata.Height = entry.IsLong() ? entry.OffsetOrValueUInt32 : entry.OffsetOrValueUInt16;
                    break;
                case TiffTags.Compression:
                    //Trace.WriteLine("reading Compression...");
                    //Trace.WriteLine("Tiff Compression found = " + entry.OffsetOrValueUInt32);
                    if (!Enum.IsDefined(typeof(TiffCompressionType), Convert.ToInt32(entry.OffsetOrValueUInt16)))
                        metadata.Compression = TiffCompressionType.COMPRESSION_UNDEFINED;
                    else
                        metadata.Compression = (TiffCompressionType)Convert.ToInt32(entry.OffsetOrValueUInt16);

                    break;
                case TiffTags.ResolutionUnit:
                    //Trace.WriteLine("reading ResolutionUnit...");
                    //Trace.WriteLine("Found ResolutionUnit");
                    metadata.ResolutionUnit = (TiffResolutionUnitType)entry.OffsetOrValueUInt16;
                    //Trace.WriteLine("res unit = " + metadata.ResolutionUnit);

                    break;
                case TiffTags.XResolution:
                    filestream.Seek(entry.OffsetOrValueUInt32, SeekOrigin.Begin);
                    var tmp1 = new byte[8];
                    filestream.Read(tmp1, 0, 8);
                    uint num1 = BitConverter.ToUInt32(tmp1, 0);
                    uint den1 = BitConverter.ToUInt32(tmp1, 4);
                    //Trace.WriteLine("num1 = " + num1 + " den1 = " + den1);
                    metadata.XRes = Math.Round((double)num1 / (double)den1);
                    //Console.WriteLine("XResolution, file="+filestream.Name+" XRes = "+metadata.XRes);
           
                    break;
                case TiffTags.YResolution:
                    filestream.Seek(entry.OffsetOrValueUInt32, SeekOrigin.Begin);
                    var tmp2 = new byte[8];
                    filestream.Read(tmp2, 0, 8);
                    uint num2 = BitConverter.ToUInt32(tmp2, 0);
                    uint den2 = BitConverter.ToUInt32(tmp2, 4);
                    //Trace.WriteLine("num2 = " + num2 + " den2 = " + den2);

                    metadata.YRes = Math.Round((double)num2 / (double)den2);
                    //Console.WriteLine("YResolution, file=" + filestream.Name + " YRes = " + metadata.YRes);
                    break;
                case TiffTags.BitsPerSample:
                    //Trace.WriteLine("reading bits per sample...");
                    if (entry.Count>=3)
                    {
                        //Trace.WriteLine("bps: count is 3");
                        filestream.Seek(entry.OffsetOrValueUInt32, SeekOrigin.Begin);
                        metadata.Bitdepth = new uint[entry.Count];
                        var tmp3 = new byte[entry.Count*2];
                        filestream.Read(tmp3, 0, (int) (entry.Count * 2));

                        for (int i = 0; i < entry.Count; ++i )
                        {
                            metadata.Bitdepth[i] = BitConverter.ToUInt16(tmp3, i*2);
                        }
                    }
                    else if (entry.Count==1)
                    {
                        //Trace.WriteLine("bps: count is 1");
                        metadata.Bitdepth = new uint[1];
                        metadata.Bitdepth[0] = entry.OffsetOrValueUInt16;
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid BitsPerSample!");
                    }
                    
                    break;
                case TiffTags.ColorMap:
                    metadata.Colormap = entry.OffsetOrValueUInt32;
                    break;

                default:
                    break;
            }

            //Console.WriteLine("IFD " + tag + ": type = " + type + ", count = " + count + ", offset = " + offsetOrValue + " isDefined = " + isDefined);
        }

        private static void DoResolutionPlausibilityCheck(TiffIfDEntry entry, FileStream filestream, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        {
            // read IFD entry
            switch ((TiffTags)entry.Tag)
            {
                case TiffTags.XResolution:
                    filestream.Seek(entry.OffsetOrValueUInt32, SeekOrigin.Begin);

                    //Console.WriteLine("XResolution, file="+filestream.Name+" XRes = "+metadata.XRes);
                    if (doPlausibilityCheck && metadata.XRes < thresholdResX)
                    {
                        bool isPlausible = ResolutionPlausibilityChecker.IsResolutionXPlausible(overwriteResX, metadata.Width);
                        //Trace.WriteLine("tif: is resx of " + overwriteResX + " plausible: " + isPlausible);
                        if (isPlausible)
                        {
                            filestream.Write(BitConverter.GetBytes((uint)overwriteResX), 0, 4);
                            filestream.Write(BitConverter.GetBytes((uint)1), 0, 4);
                            metadata.XRes = overwriteResX;
                            metadata.IsResOverwritten = true;
                        }
                    }

                    break;
                case TiffTags.YResolution:
                    filestream.Seek(entry.OffsetOrValueUInt32, SeekOrigin.Begin);
                    //Console.WriteLine("YResolution, file=" + filestream.Name + " YRes = " + metadata.YRes);
                    if (doPlausibilityCheck && metadata.YRes < thresholdResY)
                    {
                        bool isPlausible = ResolutionPlausibilityChecker.IsResolutionYPlausible(overwriteResY, metadata.Width);
                        //Trace.WriteLine("tif: is resy of " + overwriteResY + " plausible: " + isPlausible);
                        if (isPlausible)
                        {
                            //Trace.WriteLine("overwriteResX = " + overwriteResX);
                            filestream.Write(BitConverter.GetBytes((uint)overwriteResY), 0, 4);
                            filestream.Write(BitConverter.GetBytes((uint)1), 0, 4);
                            metadata.YRes = overwriteResY;
                            metadata.IsResOverwritten = true;
                        }
                    }
                    break;

                default:
                    break;
            }

            //Console.WriteLine("IFD " + tag + ": type = " + type + ", count = " + count + ", offset = " + offsetOrValue + " isDefined = " + isDefined);
        }


        public static void ReadTifHeader(string filename, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        {
            var filestream = new FileStream(filename, FileMode.Open);

            byte[] tmp = new byte[12];
            filestream.Read(tmp, 0, 8);

            string byteorder = new string((char)tmp[0], 1);
            byteorder += (char)tmp[1];

            if (!(byteorder.Equals("II") || byteorder.Equals("MM")))
            {
                throw new InvalidDataException("This is not a valid TIF file - invalid byteorder header!");
            }

            BitConverter.IsLittleEndian = byteorder.Equals("II");
            metadata.Endian = (BitConverter.IsLittleEndian) ? EndianType.LITTLE_ENDIAN : EndianType.BIG_ENDIAN; // set endian type

            //if (!BitConverter.IsLittleEndian)
            //{
            //    throw new InvalidDataException("BigEndian byteorder - have to reverse bytes - not supported yet!");
            //}

            Int16 versionNumber = BitConverter.ToInt16(tmp, 2); // must always be 42!!

            if (versionNumber != 42)
            {
                throw new InvalidDataException("This is not a valid TIF file - invalid second part of header!");
            }

            UInt32 idfOffset = BitConverter.ToUInt32(tmp, 4);
            //Console.WriteLine("First idf offset = "+idfOffset);

            //Console.WriteLine("TIF INFO:");
            //Console.WriteLine("byteorder = " + byteorder);
            //Console.WriteLine("version number = " + version_number);

            short resInsertPos = -1;
            short nEntriesWoRes = 0;
            uint countFiles = 0;
            //do
            //{
                countFiles++;
                //Console.WriteLine("countFiles = "+countFiles+" offset = " + idfOffset);
                filestream.Seek(idfOffset, SeekOrigin.Begin);
                filestream.Read(tmp, 0, 2);

                short dirEntries = BitConverter.ToInt16(tmp, 0);
                //Console.WriteLine("Nr. of dir entries: " + BitConverter.ToInt16(tmp, 0));

                var entries = new List<TiffIfDEntry>();
                for (short i = 0; i < dirEntries; ++i)
                {
                    var entry = new TiffIfDEntry();
                    // every entry has 12 bytes, first store tag, type and count:
                    filestream.Read(tmp, 0, 12);
                    entry.Tag = BitConverter.ToUInt16(tmp, 0);
                    entry.Type = BitConverter.ToUInt16(tmp, 2);
                    entry.Count = BitConverter.ToUInt32(tmp, 4);

                    // store the offset/value field both as uint32 and uint16, as the value may be of such types; note that this only makes a difference when confronted with big endian files!
                    entry.OffsetOrValueUInt32 = BitConverter.ToUInt32(tmp, 8);
                    entry.OffsetOrValueUInt16 = BitConverter.ToUInt16(tmp, 8);

                    if (entry.Tag < 282)
                        resInsertPos = i;

                    if (entry.Tag != 282 && entry.Tag != 283) nEntriesWoRes++;

                    entries.Add(entry);

                    //Console.WriteLine("Tag = " + entry.Tag + " Type: " + entry.Type + " Count = " + entry.Count + " entry.OffsetOrValueUInt32 = " + entry.OffsetOrValueUInt32);
                }

                // read offset of next IDF:
                filestream.Read(tmp, 0, 4);
                uint additionalIdfOffset = BitConverter.ToUInt32(tmp, 0);
                if (additionalIdfOffset > 0)
                {
                    throw new InvalidDataException("Multi-TIF Images are not supported!");
                }
                // read through all entries:
                foreach (TiffIfDEntry entry in entries)
                {
                    ReadTifIdfEntry(entry, filestream, ref metadata, doPlausibilityCheck, thresholdResX, thresholdResY,
                                    overwriteResX, overwriteResY);
                }
                // recalculate resolution according to resolution unit: 
                if (metadata.ResolutionUnit == TiffResolutionUnitType.RESUNIT_CENTIMETER)
                {
                    double factor = 2.54d;
                    metadata.XRes = Math.Round(metadata.XRes*factor);
                    metadata.YRes = Math.Round(metadata.YRes*factor);
                }
                if (doPlausibilityCheck)
                {
                    foreach (TiffIfDEntry entry in entries)
                    {
                        DoResolutionPlausibilityCheck(entry, filestream, ref metadata, doPlausibilityCheck, thresholdResX, thresholdResY,
                                        overwriteResX, overwriteResY);
                    }                    
                }
                // now check resolution for plausibility if there:

                    

                if (metadata.Bitdepth == null) // if bitdepth not parsed, it has to be a bilevel image!
                {
                    metadata.Colortype = ImageColorType.Binary;
                    metadata.Bitdepth = new uint[] {1};
                }
                else if (metadata.Bitdepth.Length > 1) // if length of array is larger 1, it has to be an RGB image
                {
                    metadata.Colortype = ImageColorType.RGB;
                }
                else if (metadata.Colormap > 0)
                {
                    metadata.Colortype = ImageColorType.IndexedColor;
                }
                else if (metadata.Bitdepth[0] > 1)
                {
                    metadata.Colortype = ImageColorType.Grayscale;
                }
                else
                {
                    metadata.Colortype = ImageColorType.Binary;
                }

                //Console.WriteLine("File " + (countFiles + 1) + ": " + metadata.ToString());
                //Console.WriteLine("next IFD offset = " + idfOffset);
            //} while (false);

            // add resolution info if it is not present (WARNING: this can damage the file --> TEST BEFORE USAGE!!!)
            if (doPlausibilityCheck && metadata.XRes<0 && metadata.YRes<0)
            {
                bool isPlausibleX = ResolutionPlausibilityChecker.IsResolutionXPlausible(overwriteResX, metadata.Width);
                bool isPlausibleY = ResolutionPlausibilityChecker.IsResolutionYPlausible(overwriteResY, metadata.Width);
                //Console.WriteLine("isPlausibleX = "+isPlausibleX + " isPlausibleY = "+isPlausibleY);
                //Console.WriteLine("resInsertPos = "+resInsertPos);

                bool DOITALWAYS = false;
                bool DOIT = true;
                if (isPlausibleX && isPlausibleY && DOIT || DOITALWAYS)
                {
                    // NOTE: what I basically do is to write a NEW IFD directory at the end of the file

                    long oldLength = filestream.Length;
                    // write new offset to the first ifd:
                    filestream.Position = 4;
                    filestream.Write(BitConverter.GetBytes((uint)oldLength), 0, 4);

                    int sizeOfNewIfd = 2 + (nEntriesWoRes + 2)*12 + 4; // the size of the new ifd
                    uint xOffset = 0, yOffset = 0;

                    var newIfdBytes = new List<Byte>(); // stores the bytes for the nw ifd

                    newIfdBytes.AddRange(BitConverter.GetBytes((short) (nEntriesWoRes+2))); // first store nr of ifd entries
                    // now write all ifds
                    for (int i=0; i<entries.Count; ++i)
                    {
                        var tag = (TiffTags)entries[i].Tag;
                        // write old ifd if not x- or y-resolution:
                        if (tag != TiffTags.XResolution && tag != TiffTags.YResolution)
                        {
                            //Console.WriteLine("Size of one idf = " + entries[i].GetByteArray().Count());
                            newIfdBytes.AddRange(entries[i].GetByteArray());
                        }
                        // write new resolution tags at the stored insert position:
                        if (i==resInsertPos)
                        {
                            xOffset = (uint)oldLength + (uint)sizeOfNewIfd;
                            newIfdBytes.AddRange(TiffIfDEntry.MakeXResEntry(xOffset).GetByteArray());
                            yOffset = (uint)oldLength + (uint)sizeOfNewIfd + 8;
                            newIfdBytes.AddRange(TiffIfDEntry.MakeYResEntry(yOffset).GetByteArray());
                            //Console.WriteLine("Wrote new fields, xoff, yoff =  " + xOffset + ", " + yOffset);
                        }
                    }
                    newIfdBytes.AddRange(BitConverter.GetBytes((uint)(0))); // write end of ifd, ie 0
                    //Console.WriteLine("size of newIfdBytes = " + newIfdBytes.Count + " calculated size = " + sizeOfNewIdf);
                    // write actual new res data: (at the offset that was stored into the res entries!):
                    newIfdBytes.AddRange(BitConverter.GetBytes((uint)overwriteResX));
                    newIfdBytes.AddRange(BitConverter.GetBytes((uint)1));
                    newIfdBytes.AddRange(BitConverter.GetBytes((uint)overwriteResY));
                    newIfdBytes.AddRange(BitConverter.GetBytes((uint)1));

                    // actually write stuff to the filestream:
                    filestream.SetLength(oldLength+newIfdBytes.Count);
                    filestream.Seek(oldLength, SeekOrigin.Begin);
                    filestream.Write(newIfdBytes.ToArray(), 0, newIfdBytes.Count);

                    // re-set the resolution in the metadata:
                    metadata.XRes = overwriteResX;
                    metadata.YRes = overwriteResY;
                } // end if new res is plausible
            } // end doPlausibilityCheck

            // fill up rest of metadata:
            filestream.Position = 0;
            metadata.Checksum = Checksummer.CheckSumString(filestream);
            metadata.Size = filestream.Length;
            metadata.Mimetype = "image/tiff";

            filestream.Close();
        }

    }
}
