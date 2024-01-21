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
                        markers[i].Type = GetUInt32ByReversedBytes(br.ReadBytes(4));
                        markers[i].Sample = GetUInt32ByReversedBytes(br.ReadBytes(4));
                    }
                    Markers = markers;
                }
            }
        }

        private uint GetUInt32ByReversedBytes(byte[] bytes)
        {
            var bytesReversed = bytes.Reverse();
            var bytesArray = bytesReversed.ToArray();
            var int32 = BitConverter.ToUInt32(bytesArray);
            return int32;
        }

        private byte GetFrameForPhonem(byte phonem)
        {
            var frames0 = new byte[] { 0x00 };
            var frames1 = new byte[] { 0x02, 0x03, 0x05, 0x06, 0x07, 0x0C, 0x0E};
            var frames2 = new byte[] { 0x13, 0x14, 0x15, 0x16, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x21, 0x22, 0x25, 0x26, 0x27, 0x28};
            var frames3 = new byte[] { 0x01, 0x04, 0x0B};
            var frames4 = new byte[] { 0x17, 0x18};
            var frames5 = new byte[] { 0x19, 0x1A, 0x23};
            var frames6 = new byte[] { 0x11, 0x12, 0x20};
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

        public void RecreateByPhonems(PhonemToFrame phonemManager, int wavLengthOffset)
        {
            var phonoFrames = phonemManager.Frames;
            var framePerMarker = wavLengthOffset / phonoFrames.Length;

            var lastFrameOffset = 0;
            Phonoframes = new byte[phonoFrames.Length + 1];
            Markers = new Marker[phonoFrames.Length + 1];
            for (int i = 0; i < phonoFrames.Length; i++)
            {
                Phonoframes[i] = phonoFrames[i];
                Markers[i] = new Marker { Sample = Convert.ToUInt32(lastFrameOffset), Type = 0 };
                lastFrameOffset += framePerMarker;
            }
            //Last Marker empty push
            Markers[phonoFrames.Length] = new Marker { Sample = Convert.ToUInt32(lastFrameOffset), Type = 0 };
            Phonoframes[phonoFrames.Length] = 0;

            NumberOfMarkers = Convert.ToUInt32(Markers.Count());
            NumberOfPhonems = NumberOfMarkers - 1;
        }
    }

    public class Marker
    {
        public uint Type { get; set; }
        public uint Sample { get; set; }
    }
}
