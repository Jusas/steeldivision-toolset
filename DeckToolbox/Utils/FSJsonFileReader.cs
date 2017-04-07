using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeckToolbox.Utils
{
    public class FSJsonFileReader : IJsonFileReader
    {
        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
