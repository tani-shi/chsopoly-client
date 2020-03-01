// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;
using Google.Protobuf;

namespace Chsopoly.Gs2.Models
{
    public partial class CharacterObjectSync
    {
        public static uint hashCode
        {
            get
            {
                return 2415485155;
            }
        }

        public override ByteString Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
            var d1 = BitConverter.GetBytes (direction);
            size += sizeof (System.Int32);
            var d2 = BitConverter.GetBytes (state);
            size += sizeof (System.Int32);
            var d3 = BitConverter.GetBytes (x);
            size += sizeof (System.Single);
            var d4 = BitConverter.GetBytes (y);
            size += sizeof (System.Single);

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
            Buffer.BlockCopy (d1, 0, buffer, pos, d1.Length);
            pos += sizeof (System.Int32);
            Buffer.BlockCopy (d2, 0, buffer, pos, d2.Length);
            pos += sizeof (System.Int32);
            Buffer.BlockCopy (d3, 0, buffer, pos, d3.Length);
            pos += sizeof (System.Single);
            Buffer.BlockCopy (d4, 0, buffer, pos, d4.Length);
            pos += sizeof (System.Single);

            return ByteString.CopyFrom (buffer);
        }

        public CharacterObjectSync Deserialize (ByteString data)
        {
            var bytes = data.ToByteArray ();
            var pos = BitConverter.GetBytes (hashCode).Length;
            direction = BitConverter.ToInt32 (bytes, pos);
            pos += sizeof (System.Int32);
            state = BitConverter.ToInt32 (bytes, pos);
            pos += sizeof (System.Int32);
            x = BitConverter.ToSingle (bytes, pos);
            pos += sizeof (System.Single);
            y = BitConverter.ToSingle (bytes, pos);
            pos += sizeof (System.Single);

            return this;
        }
    }
}