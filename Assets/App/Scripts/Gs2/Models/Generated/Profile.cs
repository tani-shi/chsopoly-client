// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;
using Google.Protobuf;

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

        public override ByteString Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
            var n1 = BitConverter.GetBytes (Encoding.UTF8.GetByteCount (accountId));
            var d1 = Encoding.UTF8.GetBytes (accountId);
            size += 2 + d1.Length;
            var d2 = BitConverter.GetBytes (characterId);
            size += sizeof (System.UInt32);

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
            Buffer.BlockCopy (n1, 0, buffer, pos, 2);
            pos += 2;
            Buffer.BlockCopy (d1, 0, buffer, pos, d1.Length);
            pos += d1.Length;
            Buffer.BlockCopy (d2, 0, buffer, pos, d2.Length);
            pos += sizeof (System.UInt32);

            return ByteString.CopyFrom (buffer);
        }

        public Profile Deserialize (ByteString data)
        {
            var bytes = data.ToByteArray ();
            var pos = BitConverter.GetBytes (hashCode).Length;
            accountId = Encoding.UTF8.GetString (bytes, pos + 2, (int) BitConverter.ToUInt16 (bytes, pos));
            pos += Encoding.UTF8.GetByteCount (accountId) + 2;
            characterId = BitConverter.ToUInt32 (bytes, pos);
            pos += sizeof (System.UInt32);

            return this;
        }
    }
}