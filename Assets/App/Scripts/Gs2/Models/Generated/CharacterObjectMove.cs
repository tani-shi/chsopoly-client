// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;
using Google.Protobuf;

namespace Chsopoly.Gs2.Models
{
    public partial class CharacterObjectMove
    {
        public static uint hashCode
        {
            get
            {
                return 502460492;
            }
        }

        public override ByteString Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
            var d1 = BitConverter.GetBytes (direction);
            size += sizeof (System.Boolean);

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
            Buffer.BlockCopy (d1, 0, buffer, pos, d1.Length);
            pos += sizeof (System.Boolean);

            return ByteString.CopyFrom (buffer);
        }

        public CharacterObjectMove Deserialize (ByteString data)
        {
            var bytes = data.ToByteArray ();
            var pos = BitConverter.GetBytes (hashCode).Length;
            direction = BitConverter.ToBoolean (bytes, pos);
            pos += sizeof (System.Boolean);

            return this;
        }
    }
}