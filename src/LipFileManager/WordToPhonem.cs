using Microsoft.PhoneticMatching;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace LipFileManager
{
    public static class WordToPhonem
    {
        public static string recoPhonemes;
        public static string GetPronunciationFromText(string sentence)
        {
            var pronouncer = EnPronouncer.Instance;

            var spllitedSentence = sentence.Split(' ');

            var ipa = String.Empty;

            foreach (var word in spllitedSentence)
            {
                var output = pronouncer.Pronounce(word);
                ipa += output.Ipa + " ";
            }

            ipa = ipa.Trim();

            return ipa;
        }


    }
}
