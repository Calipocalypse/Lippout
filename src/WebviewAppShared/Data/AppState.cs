using LipFileManager;
using static LipFileManager.LipFile;

namespace WebviewAppTest
{
    public class AppState
    {
        public LipFile LipFile { get; set; }
        public bool EditorView { get; set; }
        public bool SetupView { get; set; }
        public bool WordsView { get; set; }
        public string PhonemText { get; set; } = "lɛts si wɑt kaɪnd ʌv ˈnæsti ˈslændər ɪz bɪˈhaɪnd ðɪs roʊb";
        public string EnglishText { get; set; } = "Lets see what kind of nasty slander is behind this robe";
        public WavFile WavFile { get; set; }
        public RecognizedTextOutput RecognizedTextResult { get; set; }
        public Region[] GeneratedRegions { get; set; } = new Region[] { };
        public byte[] GeneratedBytesToSave { get; set; } = new byte[0]; 
        public string OutputFileName { get; set; }
    }
}
