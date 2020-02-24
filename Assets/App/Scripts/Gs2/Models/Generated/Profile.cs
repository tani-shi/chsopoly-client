// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;

namespace Chsopoly.Gs2.Models
{
    public partial class Profile
    {
        public static uint hashCode
        {
            get
            {
                return 2559275900;
            }
        }

        public byte[] Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
            var d1 = BitConverter.GetBytes (characterId);
            size += sizeof (System.UInt32);

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
            Buffer.BlockCopy (d1, 0, buffer, pos, d1.Length);
            pos += sizeof (System.UInt32);

            return buffer;
        }

        public Profile Deserialize (byte[] data)
        {
            var pos = BitConverter.GetBytes (hashCode).Length;
            characterId = BitConverter.ToUInt32 (data, pos);
            pos += sizeof (System.UInt32);

            return this;
        }
    }
}