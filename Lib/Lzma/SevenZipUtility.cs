/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：March 04 2016
// 模块描述：
//----------------------------------------------------------------*/

using System;
using System.IO;
using Common;

namespace SevenZip
{
    public class SevenZipUtility : ICodeProgress
    {
        public Int64 inSize = -1;
        public Int64 outSize = -1;

        public bool Decompress(string intputPath, string outputPath)
        {
            return false;
        }

        public bool Compress(string outputPath, string zipInArchiveRoot, params string[] inputFiles)
        {
            return false;
        }

        /// <summary>
        /// Callback progress.
        /// </summary>
        /// <param name="inSize">
        /// input size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// output size. -1 if unknown.
        /// </param>
        public void SetProgress(Int64 inSize, Int64 outSize)
        {
            this.inSize = inSize;
            this.outSize = outSize;
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="OriPath">原文件地址</param>
        /// <param name="destPath">目标文件</param>
        public void Zip(string OriPath, string destPath)
        {
            FileStream inStream = new FileStream(OriPath, FileMode.Open);
            FileStream outStream = new FileStream(destPath, FileMode.Create);
            bool dictionaryIsDefined = false;
            Int32 dictionary = 1 << 21;

            if (!dictionaryIsDefined)
                dictionary = 1 << 23;

            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
            // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 128;
            string mf = "bt4";
            bool eos = false;

            CoderPropID[] propIDs =
                {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.Algorithm,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker
                };
            object[] properties =
                {
                    (Int32)(dictionary),
                    (Int32)(posStateBits),
                    (Int32)(litContextBits),
                    (Int32)(litPosBits),
                    (Int32)(algorithm),
                    (Int32)(numFastBytes),
                    mf,
                    eos
                };

            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            Int64 fileSize;

            fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));

            encoder.Code(inStream, outStream, -1, -1,this);

            //关闭文件流
            inStream.Close();
            outStream.Close();
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="OriPath">原文件地址</param>
        /// <param name="destPath">目标文件</param>
        public void Unzip(string OriPath, string destPath)
        {
            FileStream inStream = new FileStream(OriPath, FileMode.Open);
            FileStream outStream = new FileStream(destPath, FileMode.Create);
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(properties);

            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, this);

            //关闭文件流
            inStream.Close();
            outStream.Close();
        }
    }

    
}

