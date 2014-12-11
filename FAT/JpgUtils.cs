using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FAT
{
    class JpgUtils
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

        private static UInt32 ReadBigEndianUInt32(BinaryReader binaryReader)
        {
            byte[] bytes = new byte[sizeof(uint)];
            for (int i = 0; i < sizeof(int); i += 1) {
                bytes[sizeof(int) - 1 - i] = binaryReader.ReadByte();
            }
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static bool IsValidJpgMarker(byte marker)
        {
            if (marker >> 4 == 0x0c)
                return true;
            if (marker >> 4 == 0x0e)
                return true;
            if (marker == 0xd8 || marker == 0xdb || marker == 0xfe || marker == 0xda || marker == 0xd9)
                return true;

            return false;
        }

        public static void ReadJpgHeader(string filename, ref ImageMetadata metadata, bool doPlausibilityCheck = false, short thresholdResX = 300, short thresholdResY = 300, short overwriteResX = 300, short overwriteResY = 300)
        { // fixme: currently bytes are reversed manually inside read function!
            BitConverter.IsLittleEndian = System.BitConverter.IsLittleEndian; // FIXME: currently bytes are reversed manually inside read function!

            metadata.Endian = EndianType.BIG_ENDIAN; // jpeg files are always big endian

            var br = new BinaryReader(File.Open(filename, FileMode.Open));

            // parse header and check its validity:
            UInt16 soi = br.ReadUInt16();
            if (soi != 0xd8ff)
            {
                throw new InvalidDataException("No valid JPEG file given (no soi tag)!");
            }

            UInt16 jfif = br.ReadUInt16();

            if (jfif != 0xe0ff) // jfif tag
                throw new InvalidDataException("No valid JPEG file given (no jfif tag)!");

            byte marker = 0xe0;
            ushort chunkLength = ReadBigEndianUInt16(br); // NOTE: JPEG IS BIG ENDIAN!!
            byte[] identifier = new byte[5];
            br.Read(identifier, 0, 5);
            //Console.WriteLine("identifier = " + BitConverter.ToString(identifier));
            byte[] version = new byte[2];
            br.Read(version, 0, 2);
            //Console.WriteLine("version = " + BitConverter.ToString(version));
            byte[] units = new byte[1];
            br.Read(units, 0, 1);
            //Console.WriteLine("units = " + BitConverter.ToString(units));

            long densityPositionX = br.BaseStream.Position;
            short xdens = ReadBigEndianInt16(br);
            long densityPositionY = br.BaseStream.Position;
            short ydens = ReadBigEndianInt16(br);
            //Console.WriteLine("x-y dens = " + xdens + " - " + ydens);

            metadata.XRes = xdens;
            metadata.YRes = ydens;

            br.ReadBytes(chunkLength - 14);
            
            // parse through tags and collect metadata:
            int countE2 = 0;
            int countByteDiff = 0;
            while (br.ReadByte() == 0xff)
            {
                marker = br.ReadByte();
                countByteDiff++;

                chunkLength= ReadBigEndianUInt16(br);
                if (chunkLength < 3)
                    throw new InvalidDataException("Invalid JPEG tag - invalid chunk length!");

                //Trace.WriteLine("Marker = " + BitConverter.ToString(new byte[] { marker }) + ", chunkLength = " + chunkLength);
                //Trace.WriteLine("IsValidJpgMarker: " + IsValidJpgMarker(marker)+ " Position: "+br.BaseStream.Position);

                //while (br.ReadByte() != 0xff) {
                //}
                //br.BaseStream.Position--;
                //continue;

                //if (marker == 0xE0) // jfif tag
                //{
                //    identifier = new byte[5];
                //    br.Read(identifier, 0, 5);
                //    Console.WriteLine("identifier = " + BitConverter.ToString(identifier));
                //    version = new byte[2];
                //    br.Read(version, 0, 2);
                //    Console.WriteLine("version = " + BitConverter.ToString(version));
                //    units = new byte[1];
                //    br.Read(units, 0, 1);
                //    Console.WriteLine("units = " + BitConverter.ToString(units));

                //    xdens = ReadBigEndianInt16(br);
                //    ydens = ReadBigEndianInt16(br);

                //    Console.WriteLine("x-y dens = " + xdens + " - " + ydens);
                //    br.ReadBytes(chunkLength - 14);
                //    continue;
                //}

                //if ((marker >> 4) == 0x0c)
                //{
                //    Console.WriteLine("This is a c marker!");
                //}

                //if (marker == 0xc0 || marker == 0xc2) // start of frame for baseline dct and progressive dct based jpeg (includes width, height and nr of components info)
                if (marker >= 0xc0 && marker <= 0xcf && marker != 0xc4 && marker != 0xcc) // start of frame for baseline dct and progressive dct based jpeg (includes width, height and nr of components info)
                {
                    //Console.WriteLine("C0 or C2 marker!!");
                    uint bitdepth = br.ReadByte();
                    metadata.Height = ReadBigEndianUInt16(br);
                    metadata.Width = ReadBigEndianUInt16(br);
                    int noc = br.ReadByte();

                    metadata.Bitdepth = new uint[noc];
                    for (int i = 0; i < noc; ++i)
                    {
                        metadata.Bitdepth[i] = bitdepth;
                    }
                    if (noc == 1)
                    {
                        metadata.Colortype = ImageColorType.Grayscale;
                    }
                    else
                    {
                        metadata.Colortype = ImageColorType.RGB;
                    }

                    //Console.WriteLine("bitdepth = " + bitdepth);
                    //Console.WriteLine("height = " + height);
                    //Console.WriteLine("width = " + width);
                    //Console.WriteLine("noc = " + noc);

                    break;
                    //br.ReadBytes(chunkLength - 2 - 1 - 4 - 1);
                    //return new Size(width, height);
                }

                br.ReadBytes(chunkLength - 2);
            }

            // make plausibility check:
            if (doPlausibilityCheck)
            {
                if (metadata.XRes < thresholdResX)
                {
                    bool isPlausible = ResolutionPlausibilityChecker.IsResolutionXPlausible(overwriteResX,
                                                                                            metadata.Width);
                    //Trace.WriteLine("jpg: is resx of " + overwriteResX + " plausible: " + isPlausible);
                    if (isPlausible)
                    {
                        // jump to density positon:
                        br.BaseStream.Seek(densityPositionX, SeekOrigin.Begin);

                        var tmp = BitConverter.GetBytes(overwriteResX);
                        Array.Reverse(tmp);
                        br.BaseStream.Write(tmp, 0, 2);
                        metadata.XRes = overwriteResX;
                        metadata.IsResOverwritten = true;
                    }
                }
                if (metadata.YRes < thresholdResY)
                {
                    bool isPlausible = ResolutionPlausibilityChecker.IsResolutionYPlausible(overwriteResY,
                                                                                            metadata.Width);
                    //Trace.WriteLine("jpg: is resy of " + overwriteResY + " plausible: " + isPlausible);
                    if (isPlausible)
                    {
                        // jump to density positon:
                        br.BaseStream.Seek(densityPositionY, SeekOrigin.Begin);
                        var tmp = BitConverter.GetBytes(overwriteResY);
                        Array.Reverse(tmp);
                        br.BaseStream.Write(tmp, 0, 2);
                        metadata.YRes = overwriteResY;
                        metadata.IsResOverwritten = true;
                    }
                }
            } // end doPlausibilityCheck

            // fill up rest of metadata:
            br.BaseStream.Position = 0;
            metadata.Checksum = Checksummer.CheckSumString(br.BaseStream);

            metadata.Size = br.BaseStream.Length;
            metadata.Mimetype = "image/jpeg";
            metadata.Compression = TiffCompressionType.COMPRESSION_JPEG;

            br.Close();
        }

    }
}