using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LipFileManager;

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
    }
}
