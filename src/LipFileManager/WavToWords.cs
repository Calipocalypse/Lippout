using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vosk;

namespace LipFileManager
{
    public class WavToWords
    {
        private string _pathToModel;

        public WavToWords(string pathToModel)
        {
            _pathToModel = pathToModel;
        }

        public RecognizedTextOutput GetWords(byte[] wavBytes, float wavFileSampleRate)
        {
            Vosk.Vosk.SetLogLevel(-1);

            var model = new Model(_pathToModel);

            //Get wav file sample rate
            

  
            var output = GetJsonWords(model, wavBytes, wavFileSampleRate);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var recognizedTextOutput = JsonSerializer.Deserialize<RecognizedTextOutput>(output, options);

            return recognizedTextOutput;
        }

        private string GetJsonWords(Model model, byte[] wavBytes, float wavFileSampleRate)
        {
            // Demo byte buffer
            VoskRecognizer rec = new VoskRecognizer(model, wavFileSampleRate);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);
            using (Stream source = new MemoryStream(wavBytes))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                       
                    }
                }
                return rec.FinalResult();
            }
        }
    }
    public class WordResult
    {
        public double Conf { get; set; }
        public double End { get; set; }
        public double Start { get; set; }
        public string Word { get; set; }
    }

    public class RecognizedTextOutput
    {
        public List<WordResult> Result { get; set; }
        public string Text { get; set; }
    }
}
