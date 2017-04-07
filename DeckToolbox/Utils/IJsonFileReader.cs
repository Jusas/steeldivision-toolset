using System;
using System.Collections.Generic;
using System.Text;

namespace DeckToolbox.Utils
{
    public interface IJsonFileReader
    {
        string ReadFile(string path);
    }
}
