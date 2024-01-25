using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LipFileManager
{
    public class LipFile
    {
        public uint FileTypeVersion { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        /// <summary>
        /// The length of the unpacked ACM file. This value is read by fallout2.exe and divided by the number of phoneme markers but is not used.
        /// </summary>
        public uint FileLength { get; set; }
        /// <summary>
        /// NUM-OF-PHONEMS: Total number of phoneme codes used in this file.
        /// </summary>
        public uint NumberOfPhonems { get; set; }
        public uint Unknown4 { get; set; }
        //NUM-OF-MARKERS: Total number of position markers stored in this file. Always equal to(NUM-OF-PHONEMES + 1) because a 0 secondframe with the mouth closed is inserted to the beginning of each speech sequence.
        public uint NumberOfMarkers { get; set; }
        /// <summary>
        /// Name of the corresponding ACM file for this .LIP file. Must not be greater than 7 bytes and must be null terminated. (ie. 0x00)
        /// </summary>
        public string AcmFileName { get; set; }
        public string Voc { get; set; }
        public byte[] Phonems { get; set; }
        public byte[] Phonoframes { get; set; }
        public Marker[] Markers { get; set; }


        public LipFile(byte[] fileBytes)
        {
            using (MemoryStream ms = new MemoryStream(fileBytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    FileTypeVersion = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    Unknown1 = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    Unknown2 = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    Unknown3 = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    FileLength = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    NumberOfPhonems = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    Unknown4 = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    NumberOfMarkers = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    AcmFileName = Encoding.ASCII.GetString(br.ReadBytes(8));
                    Voc = Encoding.ASCII.GetString(br.ReadBytes(4));

                    var phonems = new byte[NumberOfPhonems];
                    for (int i = 0; i < phonems.Length; i++)
                    {
                        phonems[i] = br.ReadByte();
                    }
                    Phonems = phonems;

                    var phonoframes = new byte[NumberOfPhonems];
                    for (int i = 0; i < phonems.Length; i++)
                    {
                        phonoframes[i] = GetFrameForPhonem(phonems[i]);
                    }
                    Phonoframes = phonoframes;

                    var markers = new Marker[NumberOfMarkers];
                    for (int i = 0; i < markers.Length; i++)
                    {
                        markers[i] = new Marker();
                        var localBytes = br.ReadBytes(4);
                        markers[i].Type = GetUInt32ByReversedBytes(localBytes);
                        localBytes = br.ReadBytes(4);
                        markers[i].Sample = GetUInt32ByReversedBytes(localBytes);
                    }
                    Markers = markers;
                }
            }
        }

        public void ApplyRegions(Region[] regions, float wavLength)
        {
            var phonems = new List<string>();
            var markers = new List<Marker>();

            var offsetFileLength = wavLength * 22100 * 4;

            for (int i = 0; i < regions.Length; i++)
            {
                var region = regions[i];
                var word = region.Word;
                var startOffset = region.Start * 22100 * 4;
                var endOffset = region.End * 22100 * 4;
            }
        }

        private static uint GetUInt32ByReversedBytes(byte[] bytes)
        {
            var bytesReversed = bytes.Reverse();
            var bytesArray = bytesReversed.ToArray();
            var int32 = BitConverter.ToUInt32(bytesArray);
            return int32;
        }

        private byte GetFrameForPhonem(byte phonem)
        {
            var frames0 = new byte[] { 0x00 };
            var frames1 = new byte[] { 0x02, 0x03, 0x05, 0x06, 0x07, 0x0C, 0x0E };
            var frames2 = new byte[] { 0x13, 0x14, 0x15, 0x16, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x21, 0x22, 0x25, 0x26, 0x27, 0x28 };
            var frames3 = new byte[] { 0x01, 0x04, 0x0B };
            var frames4 = new byte[] { 0x17, 0x18 };
            var frames5 = new byte[] { 0x19, 0x1A, 0x23 };
            var frames6 = new byte[] { 0x11, 0x12, 0x20 };
            var frames7 = new byte[] { 0x08, 0x0A, 0x0F, 0x10 };
            var frames8 = new byte[] { 0x09, 0x0D, 0x24, 0x29 };

            if (frames0.Contains(phonem)) return 0;
            if (frames1.Contains(phonem)) return 1;
            if (frames2.Contains(phonem)) return 2;
            if (frames3.Contains(phonem)) return 3;
            if (frames4.Contains(phonem)) return 4;
            if (frames5.Contains(phonem)) return 5;
            if (frames6.Contains(phonem)) return 6;
            if (frames7.Contains(phonem)) return 7;
            if (frames8.Contains(phonem)) return 8;
            else return 0;
        }

        private static byte GetMockedPhonemForFrame(byte frame)
        {
            if (frame == 0) return 0x00;
            if (frame == 1) return 0x02;
            if (frame == 2) return 0x13;
            if (frame == 3) return 0x01;
            if (frame == 4) return 0x17;
            if (frame == 5) return 0x19;
            if (frame == 6) return 0x11;
            if (frame == 7) return 0x08;
            if (frame == 8) return 0x09;
            else return 0;
        }

        public void RecreateByPhonems(PhonemToFrame phonemManager, int wavLengthOffset)
        {
            var phonoFrames = phonemManager.Frames;
            var framePerMarker = wavLengthOffset / phonoFrames.Length;

            var lastFrameOffset = 0;
            Phonoframes = new byte[phonoFrames.Length + 1];
            Markers = new Marker[phonoFrames.Length + 1];
            for (int i = 0; i < phonoFrames.Length; i++)
            {
                Phonoframes[i] = phonoFrames[i][i];
                Markers[i] = new Marker { Sample = Convert.ToUInt32(lastFrameOffset), Type = 0 };
                lastFrameOffset += framePerMarker;
            }
            //Last Marker empty push
            Markers[phonoFrames.Length] = new Marker { Sample = Convert.ToUInt32(lastFrameOffset), Type = 0 };
            Phonoframes[phonoFrames.Length] = 0;

            NumberOfMarkers = Convert.ToUInt32(Markers.Count());
            NumberOfPhonems = NumberOfMarkers - 1;
        }

        public Region[] GetPrimitiveWordsRegions(PhonemToFrame phonemManager, float wavLengthTime)
        {
            var words = phonemManager.Words;
            var characters = string.Join("", words);
            var charactersCount = characters.Count();

            var offsetPerCharacter = wavLengthTime / charactersCount;

            var regions = new Region[words.Count()];
            var lastFrameOffset = 0f;
            for (int i = 0; i < words.Length; i++)
            {
                var word = words[i];
                var charactersInWord = word.Count();
                var charactersOffsetLength = charactersInWord * offsetPerCharacter;

                var frames = phonemManager.GetTableOfWord(phonemManager.Phonems[i]);

                var region = new Region() { Word = word, Frames = frames, Start = lastFrameOffset, End = lastFrameOffset + charactersOffsetLength };
                regions[i] = region;

                lastFrameOffset += charactersOffsetLength;
            }
            return regions;
        }

        public static byte[] GetNewFileAsBytes(ChartObject[] points, string outputFileName)
        {
            uint fileTypeVersion = 2;
            uint unknown1 = 0x00000000;
            uint unknown2 = 0x00000000;
            uint unknown3 = 0x00000000;
            uint unpackedAcmLength = 0;
            uint numOfPhonems = Convert.ToUInt32(points.Count() - 1);
            uint unknown4 = 0x00000000;
            uint numOfMarkers = numOfPhonems + 1;
            string acmFileName = outputFileName;
            string vocConversion = "VOC";
            
            byte[] phonemes;
            var phonemesList = new List<byte>();
            for(int i = 0; i < points.Length; i++)
            {
                var fakePhonem = GetMockedPhonemForFrame(Convert.ToByte(points[i].y));
                if (i != points.Length - 1) phonemesList.Add(fakePhonem); //Ignore last phonem
            }
            phonemes = phonemesList.ToArray();

            uint[] markerTypes = new uint[numOfMarkers];
            uint[] markerSamples = new uint[numOfMarkers];
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0) markerTypes[i] = 1;
                else markerTypes[i] = 0;
                markerSamples[i] = Convert.ToUInt32(points[i].x);
            }

                // Create a memory stream to write the data
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Write each field according to the provided structure
                    writer.Write(GetReversedUint32(fileTypeVersion));
                    writer.Write(GetReversedUint32(unknown1));
                    writer.Write(GetReversedUint32(unknown2));
                    writer.Write(GetReversedUint32(unknown3));
                    writer.Write(GetReversedUint32(unpackedAcmLength));
                    writer.Write(GetReversedUint32(numOfPhonems));
                    writer.Write(GetReversedUint32(unknown4));
                    writer.Write(GetReversedUint32(numOfMarkers));

                    // Write ACM file name
                    byte[] acmFileNameBytes = Encoding.ASCII.GetBytes(acmFileName);
                    writer.Write(acmFileNameBytes, 0, acmFileNameBytes.Count());
                    //writer.Write(new byte[acmFileNameBytes.Length]);
                    for (int i = 0; i < 8 - acmFileNameBytes.Length; i++)
                    {
                        byte zero = 0;
                        writer.Write(zero);
                    }

                    // Write VOC conversion
                    byte[] vocConversionBytes = Encoding.ASCII.GetBytes(vocConversion);
                    writer.Write(vocConversionBytes, 0, vocConversionBytes.Length);
                    //writer.Write(new byte[vocConversionBytes.Length]);
                    for (int i = 0; i < 4 - vocConversionBytes.Length; i++)
                    {
                        byte zero = 0;
                        writer.Write(zero);
                    }

                    // Write phonemes
                    var phonemesLip = new byte[phonemes.Length];
                    Array.Copy(phonemes, phonemesLip, phonemes.Length);
                    writer.Write(phonemesLip);

                    for(int i = 0; i < markerSamples.Length; i++)
                    {
                        if (i ==  markerSamples.Length - 1)
                        {
                            Console.WriteLine();
                        }
                        writer.Write(GetReversedUint32(markerTypes[i]));
                        writer.Write(GetReversedUint32(markerSamples[i]));
                    }
                }

                // Get the resulting byte array
                byte[] result = stream.ToArray();
                return result;
            }
        }

        private static byte[] GetReversedUint32(uint num)
        {
            var result = BitConverter.GetBytes(num);
            var reversed = result.Reverse().ToArray();
            return reversed;
        }

        public class Marker
        {
            public uint Type { get; set; }
            public uint Sample { get; set; }
        }

        public class Region
        {
            public string Word { get; set; }
            public byte[] Frames { get; set; }
            public float Start { get; set; }
            public float End { get; set; }
        }

        public class ChartObject
        {
            public int x { get; set; }
            public int y { get; set; }
            public ChartObject(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
