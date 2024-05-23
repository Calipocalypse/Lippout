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
        public WavFile(byte[] bytes) {
            var dataLength = BitConverter.ToInt32(bytes, 40); 
            double audioLengthInSeconds = dataLength / GetBytesPerSecond(bytes);
            Length = audioLengthInSeconds;
            Base64 = Convert.ToBase64String(bytes);
        }
        private double GetBytesPerSecond(byte[] bytes)
        {
            var bytesPerSecond = BitConverter.ToInt32(bytes, 28);
            return bytesPerSecond;
        }
    }
}
