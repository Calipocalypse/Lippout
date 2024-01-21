using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LipFileManager
{
    public class PhonemToFrame
    {
        public byte[] Frames;
        public PhonemToFrame(string phonemText)
        {
            var dictionary = GetPhonemesDictionary();

            var phonemonSplitted = phonemText.Split(' ');
            var nums = new List<byte>();

            for (int i = 0; i < phonemonSplitted.Length; i++)
            {
                var table = GetTableOfWord(phonemonSplitted[i], dictionary);
                foreach (var num in table)
                {
                    nums.Add(num);
                }
            }

            Frames = nums.ToArray();
        }

        Dictionary<string, byte> GetPhonemesDictionary()
        {
            var dictionary = new Dictionary<string, byte>();
            dictionary.Add("i", 3);
            dictionary.Add("ɪ", 1);
            dictionary.Add("eɪ", 1);
            dictionary.Add("e", 3);
            dictionary.Add("æ", 1);
            dictionary.Add("ɑ", 1);
            dictionary.Add("ɔ", 1);
            dictionary.Add("oʊ", 7);
            dictionary.Add("ʊ", 8);
            dictionary.Add("u", 7);
            dictionary.Add("ʊəʳ", 3);
            dictionary.Add("ɒ", 1);
            dictionary.Add("ʌ", 8);
            dictionary.Add("aɪ", 1);
            dictionary.Add("aʊ", 7);
            dictionary.Add("ɔɪ", 7);
            dictionary.Add("p", 6);
            dictionary.Add("b", 6);
            dictionary.Add("t", 2);
            dictionary.Add("d", 2);
            dictionary.Add("k", 2);
            dictionary.Add("g", 2);
            dictionary.Add("f", 4);
            dictionary.Add("v", 4);
            dictionary.Add("θ", 5);
            dictionary.Add("ð", 5);
            dictionary.Add("s", 2);
            dictionary.Add("z", 2);
            dictionary.Add("ʃ", 2);
            dictionary.Add("ʒ", 2);
            dictionary.Add("h", 2);
            dictionary.Add("m", 6);
            dictionary.Add("n", 2);
            dictionary.Add("ŋ", 2);
            dictionary.Add("l", 5);
            dictionary.Add("w", 8);
            dictionary.Add("j", 2);
            dictionary.Add("r", 2);
            dictionary.Add("tʃ", 2);
            dictionary.Add("dʒ", 2);

            dictionary.OrderBy(x => x.Key.Count());

            return dictionary;
        }

        byte[] GetTableOfWord(string word, Dictionary<string, byte> dictionary)
        {
            var list = new List<byte>();

            foreach (var entry in dictionary)
            {
                word = word.Replace(entry.Key, entry.Value.ToString());
            }

            word = new string(word.Where(c => Char.IsDigit(c)).ToArray());

            foreach (var number in word)
            {
                byte num = byte.Parse(number.ToString());
                list.Add(num);
            }

            return list.ToArray();
        }
    }
}