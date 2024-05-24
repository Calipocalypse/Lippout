using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LipFileManager
{
    public class WavFile
    {
        public double Length { get; set; } = 0;
        public string Base64 { get; set; } = String.Empty;
        public byte[] Bytes { get; set; }
        public float SampleRate { get; set; }
        public WavFile(byte[] bytes) {
            var dataLength = BitConverter.ToInt32(bytes, 40); 
            double audioLengthInSeconds = dataLength / GetBytesPerSecond(bytes);
            Length = audioLengthInSeconds;
            Base64 = Convert.ToBase64String(bytes);
            Bytes = bytes;
            SampleRate = GetSampleRate(bytes);
        }
        private double GetBytesPerSecond(byte[] bytes)
        {
            var bytesPerSecond = BitConverter.ToInt32(bytes, 28);
            return bytesPerSecond;
        }

        private float GetSampleRate(byte[] bytes)
        {
            var sampleRate = BitConverter.ToInt32(bytes, 24);
            return sampleRate;
        }
    }
}
