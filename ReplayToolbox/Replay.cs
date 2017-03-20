using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReplayToolbox.Models;

namespace ReplayToolbox
{
    public class Replay
    {

        private const long HeaderDataLengthPos = 24;
        private const long HeaderDataPos = 32;

        private bool _ownsStream = true;
        private Stream _dataStream;

        public ReplayHeader ReplayHeader { get; private set; }

        public static async Task<Replay> FromFile(string filename)
        {
            Replay r = new Replay(filename);
            try
            {
                await r.ValidateAndReadHeader();
            }
            catch (Exception)
            {
                return null;
            }
            return r;
        }

        /// <summary>
        /// Initializes a replay from a stream.
        /// Note: the creator of the stream is also responsible for disposing it.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<Replay> FromStream(Stream stream)
        {
            Replay r = new Replay(stream);
            try
            {
                await r.ValidateAndReadHeader();
            }
            catch (Exception e)
            {
                throw new Exception("Header read and validation failed", e);
            }

            return r;
        }

        private Replay(Stream dataStream)
        {
            _ownsStream = false;
            _dataStream = dataStream;
        }

        public Replay(string filename)
        {
            _dataStream = File.Open(filename, FileMode.Open);
        }

        private async Task ValidateAndReadHeader()
        {
            if (_dataStream == null)
                throw new InvalidDataException("Data stream was null!");

            byte[] buf = new byte[4];
            int bytesRead = await _dataStream.ReadAsync(buf, 0, 4);
            if (bytesRead != 4 || Encoding.ASCII.GetString(buf) != "ESAV")
                throw new Exception("Invalid replay file header!");

            _dataStream.Seek(HeaderDataLengthPos, SeekOrigin.Begin);
            bytesRead = await _dataStream.ReadAsync(buf, 0, 4);
            if (bytesRead != 4)
                throw new Exception("Unexpected number of bytes read while reading header!");

            if (BitConverter.IsLittleEndian)
                buf = buf.Reverse().ToArray();

            int headerDataLength = BitConverter.ToInt32(buf, 0);
            if (headerDataLength <= 0 || headerDataLength > 50000)
                throw new Exception("Unexpected header data length read, expected a value between 0 and 50 000!");

            _dataStream.Seek(HeaderDataPos, 0);
            buf = new byte[headerDataLength];
            bytesRead = await _dataStream.ReadAsync(buf, 0, headerDataLength);
            if (bytesRead != headerDataLength)
                throw new Exception(string.Format("Could not read the full header data section: expected {0} bytes, read {1} bytes.",
                    headerDataLength, bytesRead));

            string headerJson = Encoding.UTF8.GetString(buf);

            _dataStream.Seek(0, 0);

            if (_ownsStream)
                _dataStream.Dispose();

            ReplayHeader = JsonConvert.DeserializeObject<ReplayHeader>(headerJson);
        }

    }
}
