using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Chsopoly.Libs.Extensions;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.BaseSystem.Gs2.Editor
{
    public static class ModelSerializerGenerator
    {
        private const string ModelSerializerPathFormat = "Assets/App/Scripts/Gs2/Models/Generated/{0}.cs";
        private const string ModelDeserializerPath = "Assets/App/Scripts/BaseSystem/Gs2/Generated/ModelDeserializer.cs";
        private const string ModelSerializerScriptTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Text;
using Google.Protobuf;

namespace Chsopoly.Gs2.Models
{
    public partial class ${NAME}
    {
        public static uint hashCode
        {
            get
            {
                return ${HASH_CODE};
            }
        }

        public override ByteString Serialize ()
        {
            var size = 0;
            var d0 = BitConverter.GetBytes (hashCode);
            size += d0.Length;
${BYTES_CONVERTER}

            var pos = 0;
            var buffer = new byte [size];
            Buffer.BlockCopy (d0, 0, buffer, pos, d0.Length);
            pos += d0.Length;
${BYTES_WRITER}

            return ByteString.CopyFrom (buffer);
        }

        public ${NAME} Deserialize (ByteString data)
        {
            var bytes = data.ToByteArray ();
            var pos = BitConverter.GetBytes (hashCode).Length;
${DESERIALIZER}

            return this;
        }
    }
}";
        private const string ModelDeserializerScriptTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Google.Protobuf;

namespace Chsopoly.BaseSystem.Gs2
{
    public static class ModelDeserializer
    {
        public static Gs2PacketModel Deserialize (ByteString data)
        {
            switch (BitConverter.ToUInt32 (data.ToByteArray (), 0))
            {
${CASES}
                default:
                    return null;
            }
        }
    }
}";

        [MenuItem ("Project/Gs2/Update Model Serializer Scripts")]
        private static void GenerateScripts ()
        {
            GenerateSerializerScripts ();
            GenerateDeserializerScript ();

            AssetDatabase.Refresh ();
        }

        private static void GenerateSerializerScripts ()
        {
            foreach (var type in GetAllModelTypes ())
            {
                GenerateSerializerScript (type);
            }
        }

        private static void GenerateSerializerScript (Type type)
        {
            var converterBuilder = new StringBuilder ();
            var writerBuilder = new StringBuilder ();
            var deserializerBuilder = new StringBuilder ();
            var number = 1;

            foreach (var field in type.GetFields (BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType == typeof (int) ||
                    field.FieldType == typeof (uint) ||
                    field.FieldType == typeof (float) ||
                    field.FieldType == typeof (short) ||
                    field.FieldType == typeof (ushort) ||
                    field.FieldType == typeof (long) ||
                    field.FieldType == typeof (double) ||
                    field.FieldType == typeof (char) ||
                    field.FieldType == typeof (bool))
                {
                    converterBuilder.AppendLine (string.Format ("var d{0} = BitConverter.GetBytes ({1});", number, field.Name));
                    converterBuilder.AppendLine (string.Format ("size += sizeof ({0});", field.FieldType.FullName));
                    writerBuilder.AppendLine (string.Format ("Buffer.BlockCopy (d{0}, 0, buffer, pos, d{0}.Length);", number));
                    writerBuilder.AppendLine (string.Format ("pos += sizeof ({0});", field.FieldType.FullName));

                    if (field.FieldType == typeof (int))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToInt32 (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (uint))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToUInt32 (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (float))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToSingle (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (short))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToInt16 (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (ushort))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToUInt16 (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (long))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToInt64 (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (double))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToDouble (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (char))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToChar (bytes, pos);", field.Name));
                    if (field.FieldType == typeof (bool))
                        deserializerBuilder.AppendLine (string.Format ("{0} = BitConverter.ToBoolean (bytes, pos);", field.Name));

                    deserializerBuilder.AppendLine (string.Format ("pos += sizeof ({0});", field.FieldType.FullName));
                }
                else if (field.FieldType == typeof (string))
                {
                    converterBuilder.AppendLine (string.Format ("var n{0} = BitConverter.GetBytes (Encoding.UTF8.GetByteCount ({1}));", number, field.Name));
                    converterBuilder.AppendLine (string.Format ("var d{0} = Encoding.UTF8.GetBytes ({1});", number, field.Name));
                    converterBuilder.AppendLine (string.Format ("size += {0} + d{1}.Length;", sizeof (ushort), number));
                    writerBuilder.AppendLine (string.Format ("Buffer.BlockCopy (n{0}, 0, buffer, pos, {1});", number, sizeof (ushort)));
                    writerBuilder.AppendLine (string.Format ("pos += {0};", sizeof (ushort)));
                    writerBuilder.AppendLine (string.Format ("Buffer.BlockCopy (d{0}, 0, buffer, pos, d{0}.Length);", number));
                    writerBuilder.AppendLine (string.Format ("pos += d{0}.Length;", number));
                    deserializerBuilder.AppendLine (string.Format ("{0} = Encoding.UTF8.GetString (bytes, pos + {1}, (int) BitConverter.ToUInt16 (bytes, pos));", field.Name, sizeof (ushort), number));
                    deserializerBuilder.AppendLine (string.Format ("pos += Encoding.UTF8.GetByteCount ({0}) + {1};", field.Name, sizeof (ushort)));
                }
                else
                {
                    Debug.LogWarning ("A type that unable to serialize to bytes was found. " + type.FullName + "." + field.Name + "(" + field.FieldType.FullName + ")");
                }

                number++;
            }

            var path = string.Format (ModelSerializerPathFormat, type.Name);
            var content = ModelSerializerScriptTemplate
                .Replace ("${NAME}", type.Name)
                .Replace ("${HASH_CODE}", type.FullName.ToHashUInt32 ().ToString ())
                .Replace ("${BYTES_CONVERTER}", converterBuilder.ToString ().Indent (12))
                .Replace ("${BYTES_WRITER}", writerBuilder.ToString ().Indent (12))
                .Replace ("${DESERIALIZER}", deserializerBuilder.ToString ().Indent (12));
            Directory.CreateDirectory (Path.GetDirectoryName (path));
            File.WriteAllText (path, content);
        }

        private static void GenerateDeserializerScript ()
        {
            var builder = new StringBuilder ();

            foreach (var type in GetAllModelTypes ())
            {
                builder.AppendLine (string.Format ("case {0}:", type.FullName.ToHashUInt32 ()));
                builder.AppendLine (string.Format ("    return new {0} ().Deserialize (data);", type.FullName));
            }

            var content = ModelDeserializerScriptTemplate
                .Replace ("${CASES}", builder.ToString ().Indent (16));
            Directory.CreateDirectory (Path.GetDirectoryName (ModelDeserializerPath));
            File.WriteAllText (ModelDeserializerPath, content);
        }

        private static uint ToHashUInt32 (this string str)
        {
            return BitConverter.ToUInt32 (MD5.Create ().ComputeHash (Encoding.UTF8.GetBytes (str)), 0);
        }

        private static IEnumerable<Type> GetAllModelTypes ()
        {
            foreach (var type in Assembly.Load ("Assembly-CSharp").GetTypes ())
            {
                if (type.IsSubclassOf (typeof (Gs2PacketModel)))
                {
                    yield return type;
                }
            }
        }
    }
}