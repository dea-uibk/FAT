using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FAT
{
    class Jp2Utils
    {
        private static short ReadBigEndianInt16(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(short)];
            for (int i = 0; i < sizeof(short); i += 1)
            {
                bytes[sizeof(short) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        private static ushort ReadBigEndianUInt16(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(ushort)];
            for (int i = 0; i < sizeof(ushort); i += 1)
            {
                bytes[sizeof(ushort) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToUInt16(bytes, 0);
        }

        private static int ReadBigEndianInt32(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(int)];
            for (int i = 0; i < sizeof(int); i += 1)
            {
                bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        private static uint ReadBigEndianUInt32(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(uint)];
            for (int i = 0; i < sizeof(uint); i += 1)
            {
                bytes[sizeof(uint) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        private static long ReadBigEndianInt64(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(long)];
            for (int i = 0; i < sizeof(long); i += 1)
            {
                bytes[sizeof(long) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToInt64(bytes, 0);
        }

        private static ulong ReadBigEndianUInt64(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(ulong)];
            for (int i = 0; i < sizeof(ulong); i += 1)
            {
                bytes[sizeof(ulong) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static void ReadJp2Header(string filename, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        {
            BitConverter.IsLittleEndian = System.BitConverter.IsLittleEndian; // fixme: currently bytes are reversed manually inside read function!

            metadata.Endian = EndianType.BIG_ENDIAN; // jpeg2000 files are always big endian

            var br = new BinaryReader(File.Open(filename, FileMode.Open));

            // parse signature box: ----------------
            UInt32 sig1 = ReadBigEndianUInt32(br);
            UInt32 sig2 = ReadBigEndianUInt32(br);
            UInt32 sig3 = ReadBigEndianUInt32(br);
            if (sig1 != 0xc || sig2 != 0x6a502020 || sig3 != 0xd0a870a)
            {
                throw new InvalidDataException("No valid JPEG-2000 file given (no valid signature box)!");
            }
            //Console.WriteLine("Valid header!");

            // skip file type box: ----------
            Int32 l = ReadBigEndianInt32(br);
            //Console.WriteLine("lenght of file type box = " + l);
            br.ReadBytes(l - 4);

            // image header box: -----------
            
            l = ReadBigEndianInt32(br);
            UInt32 type = ReadBigEndianUInt32(br);

            long headerBoxPosition = 0;
            long captureResBoxPosition = 0;
            long displayResBoxPosition = 0;

            Int32 lHeader = l;
            if (type == 0x6a703268) // header box 
            {
                headerBoxPosition = br.BaseStream.Position - 8;
                int lSum = 8;

                //Trace.WriteLine("This is the jp2 header box, length = " + lHeader);

                while (lSum < lHeader) // while we are still inside the header box
                {
                    int lAct = ReadBigEndianInt32(br);
                    type = ReadBigEndianUInt32(br);
                    int bytesRead = 8;

                    //Trace.WriteLine("Actual box l = " + lAct + " lSum = " + lSum + " type = " + type.ToString("x"));

                    if (lAct == 0)
                    {
                        throw new InvalidDataException("No valid JPEG-2000 file given (box of length 0 inside header box)!");
                        Console.WriteLine("breaking out!!");
                        break;
                    }
                    else if (lAct == 1) // extended length
                    {
                        throw new InvalidDataException("No valid JPEG-2000 file given (extended length box inside header box)!");
                        ulong extendedLength = ReadBigEndianUInt64(br);
                        bytesRead += 8;
                    }

                    if (type == 0x69686472) // image header box 
                    {
                        metadata.Height = ReadBigEndianUInt32(br);
                        metadata.Width = ReadBigEndianUInt32(br);
                        ushort nc = ReadBigEndianUInt16(br); // number of channels
                        metadata.Colortype = nc == 1 ? ImageColorType.Grayscale : ImageColorType.RGB;

                        metadata.Bitdepth = new uint[nc];
                        byte bpc = br.ReadByte();
                        if (bpc != 255) // if bpc==255, it is defined in bits per component box!!!
                        {
                            for (int i = 0; i < nc; ++i)
                            {
                                metadata.Bitdepth[i] = bpc;
                                metadata.Bitdepth[i]++;
                            }
                        }

                        bytesRead += 11;

                        //Console.WriteLine("This is the image header box, nc = " + nc + ", bpc = " + bpc);
                    }
                    if (type == 0x62706363) // bits per component box
                    {
                        //Console.WriteLine("This is the bits per component box, nc = " + metadata.Bitdepth.Length);
                        for (int i = 0; i < metadata.Bitdepth.Length; ++i)
                        {
                            metadata.Bitdepth[i] = br.ReadByte();
                            bytesRead++;
                        }
                    }
                    if (type == 0x72657320) // Resolution box
                    {
                        //Console.WriteLine("This is the resolution box, length = "+lAct);
                        int lRes = lAct;
                        int lResCount = 8;

                        while (lResCount < lRes)
                        {
                            int lActRes = ReadBigEndianInt32(br);
                            uint typeRes = ReadBigEndianUInt32(br);

                            //Console.WriteLine("resolution box type = " + type.ToString("x") + " and length = " + lAct2);

                            if (lActRes == 0)
                            {
                                throw new InvalidDataException("No valid JPEG-2000 file given (box of length 0 inside resolution box)!");
                                Console.WriteLine("breaking out 2!!");
                                br.ReadBytes(lRes - lResCount - 8);
                                break;
                            }

                            if (typeRes == 0x72657363) // capture resolution box
                            {
                                captureResBoxPosition = br.BaseStream.Position;
                                //Console.WriteLine("This is the capture resolution box!!");
                                uint VRcN = ReadBigEndianUInt16(br);
                                uint VRcD = ReadBigEndianUInt16(br);
                                uint HRcN = ReadBigEndianUInt16(br);
                                uint HRcD = ReadBigEndianUInt16(br);
                                byte VRcE = br.ReadByte();
                                byte HRcE = br.ReadByte();

                                //Console.WriteLine("VRcN = " + VRcN);
                                //Console.WriteLine("VRcD = " + VRcD);
                                //Console.WriteLine("HRcN = " + HRcN);
                                //Console.WriteLine("HRcD = " + HRcD);
                                //Console.WriteLine("VRcE = " + VRcE);
                                //Console.WriteLine("HRcE = " + HRcE);

                                metadata.YRes = (VRcD!=0) ? ((double)VRcN / (double)VRcD) * Math.Pow(10, VRcE) / 39.3700787 : 0;
                                metadata.XRes = (HRcD!=0) ? ((double)HRcN / (double)HRcD) * Math.Pow(10, HRcE) / 39.3700787 : 0;
                                metadata.YRes = Math.Round(metadata.YRes);
                                metadata.XRes = Math.Round(metadata.XRes);

                                //Console.WriteLine("capture resolution: " + metadata.XRes);
                                //Console.WriteLine("capture resolution: " + metadata.YRes);

                                lResCount += 18;

                                // break out for capture res box:
                                br.ReadBytes(lRes - lResCount);
                                break;
                            }
                            else if (typeRes == 0x72657364) // display resolution box
                            {
                                displayResBoxPosition = br.BaseStream.Position;
                                //Console.WriteLine("This is the display resolution box!!");
                                uint VRdN = ReadBigEndianUInt16(br);
                                uint VRdD = ReadBigEndianUInt16(br);
                                uint HRdN = ReadBigEndianUInt16(br);
                                uint HRdD = ReadBigEndianUInt16(br);
                                byte VRdE = br.ReadByte();
                                byte HRdE = br.ReadByte();

                                //Console.WriteLine("VRdn = " + VRdN);
                                //Console.WriteLine("VRdD = " + VRdD);
                                //Console.WriteLine("HRdN = " + HRdN);
                                //Console.WriteLine("HRdD = " + HRdD);
                                //Console.WriteLine("VRdE = " + VRdE);
                                //Console.WriteLine("HRdE = " + HRdE);

                                metadata.YRes = (VRdD!=0) ? ((double)VRdN / (double)VRdD) * Math.Pow(10, VRdE) / 39.3700787 : 0;
                                metadata.XRes = (HRdD!=0) ? ((double)HRdN / (double)HRdD) * Math.Pow(10, HRdE) / 39.3700787 : 0;
                                metadata.YRes = Math.Round(metadata.YRes);
                                metadata.XRes = Math.Round(metadata.XRes);

                                //Console.WriteLine("display resolution x: " + metadata.XRes);
                                //Console.WriteLine("display resolution y: " + metadata.YRes);

                                lResCount += 18;
                            }

                        }
                    }

                    lSum += lAct;
                    br.ReadBytes(lAct - bytesRead);

                } // end while
            } // end of header box
            else
            {
                throw new InvalidDataException("No valid JPEG-2000 file given (no valid JP2 header box)!");
            }

            // make plausibility check:
            if (doPlausibilityCheck)
            {
                // check if there is a resolution box present and choose one of them, favoring the capture resolution box:
                long actResPosition = displayResBoxPosition;
                if (captureResBoxPosition > 0) actResPosition = captureResBoxPosition;

                //Console.WriteLine("JPEG2000 Plausibility check: actResPosition = " + actResPosition + " captureResBoxPosition = "+captureResBoxPosition + " displayResBoxPosition = "+displayResBoxPosition);

                if (actResPosition > 0) // means that a resolution box is present
                {
                    if (metadata.XRes < thresholdResX || metadata.YRes < thresholdResY)
                    {
                        bool isPlausibleX = ResolutionPlausibilityChecker.IsResolutionXPlausible(overwriteResX, metadata.Width);
                        bool isPlausibleY = ResolutionPlausibilityChecker.IsResolutionYPlausible(overwriteResY, metadata.Width);
                        if (isPlausibleX && isPlausibleY)
                        {
                            // jump to resolution positon:
                            br.BaseStream.Seek(actResPosition, SeekOrigin.Begin);
                            // write new resolution:
                            short nomX = (short)(overwriteResX * ResolutionPlausibilityChecker.M_TO_INCH_FACTOR);
                            short nomY = (short)(overwriteResY * ResolutionPlausibilityChecker.M_TO_INCH_FACTOR);
                            short denX = 1;
                            short denY = 1;
                            byte expX = 0;
                            byte expY = 0;

                            br.BaseStream.Write(BitConverter.GetBytes(nomY).Reverse().ToArray(), 0, 2);
                            br.BaseStream.Write(BitConverter.GetBytes(denY).Reverse().ToArray(), 0, 2);
                            br.BaseStream.Write(BitConverter.GetBytes(nomX).Reverse().ToArray(), 0, 2);
                            br.BaseStream.Write(BitConverter.GetBytes(denX).Reverse().ToArray(), 0, 2);
                            br.BaseStream.WriteByte(expY);
                            br.BaseStream.WriteByte(expX);

                            metadata.XRes = overwriteResX;
                            metadata.YRes = overwriteResY;
                        } // end is resolution is plausible
                    }
                } // end if there is a resolution box
                else // there is not resolution box --> create one and insert it
                {
                    //Console.WriteLine("THERE IS NO RESOLUTION BOX!!");
                    
                    bool isPlausibleX = ResolutionPlausibilityChecker.IsResolutionXPlausible(overwriteResX, metadata.Width);
                    bool isPlausibleY = ResolutionPlausibilityChecker.IsResolutionYPlausible(overwriteResY, metadata.Height);
                    bool DOITALWAYS = false;
                    bool DOIT = true;
                    if (isPlausibleX && isPlausibleY && headerBoxPosition > 0 && DOIT || DOITALWAYS)
                    {
                        // write new length of header box:
                        br.BaseStream.Position = headerBoxPosition;
                        //Console.WriteLine("headerBoxPosition = " + headerBoxPosition);
                        Int32 newHeaderLength = lHeader + 26;
                        br.BaseStream.Write(BitConverter.GetBytes(newHeaderLength).Reverse().ToArray(), 0, 4);
                        // get all bytes of stream as list:
                        br.BaseStream.Position = 0;
                        var bytes = new byte[br.BaseStream.Length];
                        br.BaseStream.Read(bytes, 0, bytes.Length);
                        var bytesList = new List<Byte>(bytes);
                        bytesList.Capacity += 26;
                        // create the byteslist for the resolution box:
                        short nomX = (short)(overwriteResX * ResolutionPlausibilityChecker.M_TO_INCH_FACTOR);
                        short nomY = (short)(overwriteResY * ResolutionPlausibilityChecker.M_TO_INCH_FACTOR);
                        short denX = 1;
                        short denY = 1;
                        byte expX = 0;
                        byte expY = 0;
                        var tmp = new List<byte>();
                        tmp.AddRange(BitConverter.GetBytes((Int32)26).Reverse().ToArray());// length of resolution box
                        tmp.AddRange(BitConverter.GetBytes((Int32)0x72657320).Reverse().ToArray());// type of resolution box
                        tmp.AddRange(BitConverter.GetBytes((Int32)18).Reverse().ToArray());// length of capture resolution box
                        tmp.AddRange(BitConverter.GetBytes((Int32)0x72657363).Reverse().ToArray());// type of capture resolution box
                        tmp.AddRange(BitConverter.GetBytes(nomY).Reverse().ToArray());
                        tmp.AddRange(BitConverter.GetBytes(denY).Reverse().ToArray());
                        tmp.AddRange(BitConverter.GetBytes(nomX).Reverse().ToArray());
                        tmp.AddRange(BitConverter.GetBytes(denX).Reverse().ToArray());
                        tmp.Add(expY);
                        tmp.Add(expX);
                        // insert the byteslist of the box into the file byteslist:

                        int insertPos = (int) headerBoxPosition + lHeader;
                        //Console.WriteLine("insert pos = "+insertPos);
                        bytesList.InsertRange(insertPos, tmp);

                        //Console.WriteLine("size of stream = " + br.BaseStream.Length + " size of bytesList: "+bytesList.Count);

                        // write to the file:
                        br.Close(); // first close old stream
                        File.WriteAllBytes(filename, bytesList.ToArray()); // write all bytes to the file
                        br = new BinaryReader(File.Open(filename, FileMode.Open)); // reopen stream

                        metadata.XRes = overwriteResX;
                        metadata.YRes = overwriteResY;
                    }
                }
            } // end doPlausibilityCheck

            // fill up rest of metadata:
            br.BaseStream.Position = 0;
            metadata.Checksum = Checksummer.CheckSumString(br.BaseStream);

            metadata.Size = br.BaseStream.Length;
            metadata.Mimetype = "image/jpeg2000";
            metadata.Compression = TiffCompressionType.COMPRESSION_JP2000;

            br.Close();
        }

    }
}
