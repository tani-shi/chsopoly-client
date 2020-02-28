// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;
using Google.Protobuf;

namespace Chsopoly.Gs2.Models
{
    public partial class CharacterObjectJump
    {
        public static uint hashCode
        {
            get
            {
                return 4001559680;
            }
        }

        public override ByteString Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
            var d1 = BitConverter.GetBytes (x);
            size += sizeof (System.Single);
            var d2 = BitConverter.GetBytes (y);
            size += sizeof (System.Single);

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
            Buffer.BlockCopy (d1, 0, buffer, pos, d1.Length);
            pos += sizeof (System.Single);
            Buffer.BlockCopy (d2, 0, buffer, pos, d2.Length);
            pos += sizeof (System.Single);

            return ByteString.CopyFrom (buffer);
        }

        public CharacterObjectJump Deserialize (ByteString data)
        {
            var bytes = data.ToByteArray ();
            var pos = BitConverter.GetBytes (hashCode).Length;
            x = BitConverter.ToSingle (bytes, pos);
            pos += sizeof (System.Single);
            y = BitConverter.ToSingle (bytes, pos);
            pos += sizeof (System.Single);

            return this;
        }
    }
}