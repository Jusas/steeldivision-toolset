using System;
using System.Collections.Generic;
using System.Linq;

namespace DeckToolbox.Utils
{
    public static class ByteExtensions
    {

        public static void ZeroPad(this List<byte> byteList, int numBytesToPadWith)
        {
            for (var i = 0; i < numBytesToPadWith; i++)
                byteList.Insert(0, 0);
        }

        public static ulong GetBitSection(this byte[] byteArray, int startBit, int lengthBits)
        {
            ulong mask = (ulong) (Math.Pow(2, lengthBits) - 1);
            int startByte = startBit/8;
            int endByte = (startBit + lengthBits - 1)/8;
            int bytesToTake = endByte - startByte + 1;

            if (bytesToTake > 8)
                throw new Exception("Can only read 8 bytes at a time");

            List<byte> bytes = byteArray.Skip(startByte).Take(bytesToTake).ToList();
            var localStartBit = startBit%8;
            var rightSkip = bytesToTake * 8 - (localStartBit + lengthBits);
            ulong result = 0;

            int containerByteSize = bytes.Count == 1
                ? 1
                : bytes.Count == 2
                    ? 2
                    : bytes.Count == 3 || bytes.Count == 4
                        ? 4
                        : 8;

            int padding = containerByteSize - bytes.Count;
            bytes.ZeroPad(padding);

            // When converting from bytes to numbers, if we're in a little endian system we
            // need to reverse the bytes.
            if (BitConverter.IsLittleEndian)
                bytes.Reverse();

            switch (containerByteSize)
            {
                case 1:
                    result = bytes[0];
                    break;
                case 2:
                    result = BitConverter.ToUInt16(bytes.ToArray(), 0);
                    break;
                case 4:
                    result = BitConverter.ToUInt32(bytes.ToArray(), 0);
                    break;
                case 8:
                    result = BitConverter.ToUInt64(bytes.ToArray(), 0);
                    break;
            }

            result = (result >> rightSkip) & mask;
            return result;
        }
    }

}
