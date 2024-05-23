using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LipFileManager;
using static LipFileManager.LipFile;

namespace WebviewAppShared.Operations
{
    internal class JsonObjectConverter
    {
        internal JsonObjectConverter() {}

        internal string ConvertToMarkersGraphJson(LipFile lipFile)
        {
            var phonomarkers = new object[lipFile.NumberOfMarkers];
            for (int i = 0; i < phonomarkers.Length; i++)
            {
                object entry;
                if (i != phonomarkers.Length - 1)
                {
                    entry = new { markerPos = lipFile.Markers[i].Sample, phonemPos = lipFile.Phonoframes[i] };
                }
                else
                {
                    entry = new { markerPos = lipFile.Markers[i].Sample, phonemPos = 0 };
                }
                phonomarkers[i] = entry;
            }
            var phonomarkersJson = JsonSerializer.Serialize(phonomarkers);
            return phonomarkersJson;
        }

        internal string ConvertToMarkersGraphJson(Region[] regions)
        {
            var phonomarkersList = new List<object>();
            var firstEntry = new { markerPos = 0, phonemPos = 0 }; //must be
            phonomarkersList.Add(firstEntry);

            for (int i = 0; i < regions.Length; i++)
            {
                var firstPos = GetOffsetPos(regions[i].Start);
                if (firstPos == 0) firstPos = 100;

                var offsetLength = GetOffsetPos(regions[i].End - regions[i].Start);
                var phonemCount = regions[i].Frames.Count();

                var firstPhonemEntry = new { markerPos = offsetLength, phonemPos = regions[i].Frames[0] };
                phonomarkersList.Add(firstPhonemEntry);

                if (phonemCount > 1)
                {
                    var distanceBetweenMarkers = offsetLength / phonemCount;
                    var lastPhonemPos = firstPos;
                    for (int j = 0; j < phonemCount; j++)
                    {
                        var newPos = lastPhonemPos + distanceBetweenMarkers;
                        lastPhonemPos = newPos;
                        var entry = new { markerPos = newPos, phonemPos = regions[i].Frames[j] };
                        phonomarkersList.Add(entry);
                    }
                }

                var lastWordSilenceEntry = new { markerPos = GetOffsetPos(regions[i].End), phonemPos = 0 };
                phonomarkersList.Add(lastWordSilenceEntry);
            }

            var phonomarkersJson = JsonSerializer.Serialize(phonomarkersList.ToArray());
            return phonomarkersJson;
        }

        private int GetOffsetPos(float pos)
        {
            return Convert.ToInt32(pos * 22100 * 4);
        }
    }
}
