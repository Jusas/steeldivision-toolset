using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DeckToolbox.Utils
{
    public class AssemblyJsonFileReader : IJsonFileReader
    {
        private Assembly _assembly;

        public AssemblyJsonFileReader(Assembly asm)
        {
            _assembly = asm;
        }

        public string ReadFile(string path)
        {
            string json;
            using (var stream = _assembly.GetManifestResourceStream(path))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    json = sr.ReadToEnd();
                }
            }
            return json;
        }
    }
}
