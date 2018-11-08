using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hashing
{
    public class SHAOne
    {
        private string Text;

        //private uint h0 = 0x67452301;
        //private uint h1 = 0xEFCDAB89;
        //private uint h2 = 0x98BADCFE;
        //private uint h3 = 0x10325476;
        //private uint h4 = 0xC3D2E1F0;
        private BitWork h0 = new BitWork("01100111010001010010001100000001");
        private BitWork h1 = new BitWork("11101111110011011010101110001001");
        private BitWork h2 = new BitWork("10011000101110101101110011111110");
        private BitWork h3 = new BitWork("00010000001100100101010001110110");
        private BitWork h4 = new BitWork("11000011110100101110000111110000");


        public SHAOne(string text)
        {
            Text = text;
        }

        public byte[] NetFunction()
        {
            using (var sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(Text));
            }
        }

        public byte[] CustomFunction()
        {
            // Step 1 - The input string
            var inputString = Text;

            // Step 2 - Input string in ASCII codes
            var inputStringASCII = Encoding.ASCII.GetBytes(Text);

            // Step 3 - Convert into bit array
            var message = new BitWork(inputStringASCII);
            var originallength = (uint)message.Length;

            // Step 4 - Add '1' to the end
            message = message.AddAtEnd(1, 1);

            // Step 5 - Append '0's' to the end - count = 448 - bits length
            var zeroscount = 448 - message.Length;
            message = message.AddAtEnd(0, zeroscount);
            //TODO: some other case

            // Step 6 - Append original message length
            var originallengthBytes = BitConverter.GetBytes(originallength);
            var originallengthBits = new BitWork(originallengthBytes.Reverse().ToArray());
            var zerosToOriginalLengthBits = 64 - originallengthBits.Length;
            originallengthBits = originallengthBits.AddAtBegin(0, zerosToOriginalLengthBits);
            message = message.AddAtEnd(originallengthBits);

            if (message.Length != 512)
                throw new Exception();

            // Step 7 - 'Chunk' the message
            // split the message into 512 bit chunks

            // Step 8 - Break the 'Chunk' into 'Words'
            var chunks = message.Split(32);

            // Step 9 - 'Extend' into 80 words
            for (var i = 16; i < 80; i++)
            {
                // Step 9.1 - Get the chunks
                var chunk_16 = chunks[i - 16];
                var chunk_14 = chunks[i - 14];
                var chunk_8 = chunks[i - 8];
                var chunk_3 = chunks[i - 3];

                // Step 9.2 - XOR
                var newchunk = chunk_16 ^ chunk_14;
                newchunk ^= chunk_8;
                newchunk ^= chunk_3;

                // Step9.3 - Left rotate
                newchunk = newchunk.LeftRotate(1);

                chunks.Add(newchunk);
            }

            var A = h0.Clone();
            var B = h1.Clone();
            var C = h2.Clone();
            var D = h3.Clone();
            var E = h4.Clone();
            BitWork F = null, k = null;

            // Step 10 - Main loop
            for (var i = 0; i < 80; i++)
            {

                // Step 10.1 - Four choices
                if (i < 20) // 0 to 19 = function (B AND C) or (!B AND D)
                {
                    F = B & C;
                    var t = !B;
                    t &= D;
                    F |= t;
                    k = new BitWork("01011010100000100111100110011001");
                }
                else if (i > 19 && i < 40) // 20 to 39 = function B XOR C XOR D
                {
                    F = B ^ C;
                    F ^= D;
                    k = new BitWork("01101110110110011110101110100001");
                }
                else if (i > 39 && i < 60) // 40 to 59 = function (B AND C) OR (B AND D) OR (C AND D)
                {
                    F = B & C;
                    var t = B & D;
                    F |= t;
                    t = C & D;
                    F |= t;
                    k = new BitWork("10001111000110111011110011011100");
                }
                else if (i > 59 && i < 80) // 60 to 79 = function B XOR C XOR D
                {
                    F = B ^ C;
                    F ^= D;
                    k = new BitWork("11001010011000101100000111010110");
                }

                // Step 10.2 - Put them together
                // temp =  (A left rotate 5) + F + E + K + (the current word)
                var temp = A.LeftRotate(5);
                temp += F;
                temp += E;
                temp += k;
                temp += chunks[i];
                //var temp = A.LeftRotate(5) + F + E + k + chunks[i];

                // Step 10.3 - Trucate left for 32 length
                var final = temp.TruncateLeft(32);

                // Step 10.4 - Reset variables
                E = D;
                D = C;
                C = B.LeftRotate(30);
                B = A;
                A = final;
            }

            // Step 11 - Plus
            var eh0 = h0 + A;
            var eh1 = h1 + B;
            var eh2 = h2 + C;
            var eh3 = h3 + D;
            var eh4 = h4 + E;

            eh0 = eh0.TruncateLeft(32);
            eh1 = eh1.TruncateLeft(32);
            eh2 = eh2.TruncateLeft(32);
            eh3 = eh3.TruncateLeft(32);
            eh4 = eh4.TruncateLeft(32);

            var result = eh0.GetBytes().ToList();
            result.AddRange(eh1.GetBytes());
            result.AddRange(eh2.GetBytes());
            result.AddRange(eh3.GetBytes());
            result.AddRange(eh4.GetBytes());

            return result.ToArray();
        }
    }
}