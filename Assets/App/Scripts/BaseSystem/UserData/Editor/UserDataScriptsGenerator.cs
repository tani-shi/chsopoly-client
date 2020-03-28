using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Chsopoly.Libs.Extensions;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.BaseSystem.UserData.Editor
{
    public static class UserDataScriptsGenerator
    {
        private const string EncryptionPassword = "chsopoly";
        private const string EncryptedEntityPathFormat = "Assets/App/Scripts/BaseSystem/UserData/Entity/Generated/{0}.cs";
        private const string UserDataManagerPath = "Assets/App/Scripts/BaseSystem/UserData/Generated/UesrDataManager.cs";
        private const string EntityAccessorPathFormat = "Assets/App/Scripts/UserData/Entity/Generated/{0}.cs";
        private const string EncryptedEntityTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using SQLite4Unity3d;

namespace Chsopoly.BaseSystem.UserData.Entity
{
    public class ${CLASS_NAME}
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

${FIELDS}
    }
}";
        private const string EncryptedSingleEntityTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using SQLite4Unity3d;

using SQLite4Unity3d;

namespace Chsopoly.BaseSystem.UserData.Entity
{
    public struct ${CLASS_NAME}
    {
        [PrimaryKey]
        public int Id { get { return 1; } set { } }

${FIELDS}
    }
}";
        private const string UserDataManagerTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Linq;
using System.Collections.Generic;
using Chsopoly.BaseSystem.UserData.Entity;
using Chsopoly.Libs;
using Chsopoly.UserData.Entity;
using SQLite4Unity3d;
using UnityEngine;

namespace Chsopoly.BaseSystem.UserData
{
    public partial class UserDataManager : SingletonMonoBehaviour<UserDataManager>
    {
${SERIALIZER}

${ACCESSORS}

        private SQLiteConnection _connection;

        public void Load ()
        {
            _connection = new SQLiteConnection (Application.persistentDataPath + ""/d"");

${INITIALIZERS}

${LOADERS}
        }

        public void Save ()
        {
${SAVERS}
        }

        private void Insert<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

            entity.IsDirty = false;
            entity.IsNew = false;

${INSERTERS}
            ${INSERT_INDENT}throw new NotImplementedException (""The entity type is not supported to decrypt. "" + typeof (T).FullName);
        }

        private void InsertOrReplace<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

            entity.IsDirty = false;
            entity.IsNew = false;

${REPLACERS}
            ${REPLACE_INDENT}throw new NotImplementedException (""The entity type is not supported to decrypt. "" + typeof (T).FullName);
        }

        private void Delete<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

${DELETES}
            throw new NotImplementedException (""The entity type is not supported to decrypt. "" + typeof (T).FullName);
        }

        private T Decrypt<T> (object encrypted) where T : class, IUserDataEntity, new ()
        {
            if (encrypted == null)
            {
                return null;
            }

${DECRYPTERS}
            return null;
        }

        private object Encrypt<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return null;
            }

${ENCRYPTERS}
            return null;
        }
    }
}";
        private const string EntityAccessorTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    [Serializable]
    public partial class ${NAME} : IUserDataEntity
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }

        public ${NAME} ()
        {
            IsNew = true;
        }

        public ${NAME} (bool isNew)
        {
            IsNew = isNew;
        }
    }
}";
        private const string SingleEntityAccessorTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using Chsopoly.BaseSystem.UserData;

namespace Chsopoly.UserData.Entity
{
    [Serializable]
    public partial class ${NAME} : IUserDataEntity
    {
        public int Id
        {
            get
            {
                return 1;
            }
            set { }
        }

        public bool IsDirty { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }

        public ${NAME} ()
        {
            IsNew = true;
        }

        public ${NAME} (bool isNew)
        {
            IsNew = isNew;
        }
    }
}";

        [MenuItem ("Project/User Data/Update UserData Encrypted Entity And Accessor")]
        private static void GenerateScripts ()
        {
            foreach (var type in Assembly.Load ("Assembly-CSharp").GetTypes ())
            {
                if (IsEntity (type) || IsSingleEntity (type))
                {
                    GenerateEntityAccessorScript (type);
                    GenerateEncryptedEntityScript (type);
                }
            }

            GenerateUserDataManagerScript ();
        }

        private static void GenerateEncryptedEntityScript (Type type)
        {
            var builder = new StringBuilder ();
            foreach (var field in type.GetFields ())
            {
                builder.AppendLine ("public string " + field.Name.ToEncryptedFieldName () + " { get; set; }");
            }
            var content = EncryptedEntityTemplate
                .Replace ("${CLASS_NAME}", type.FullName.ToEncryptedClassName ())
                .Replace ("${PK}", "Id".ToEncryptedFieldName ())
                .Replace ("${FIELDS}", builder.ToString ().Indent (8));
            Directory.CreateDirectory (Path.GetDirectoryName (EncryptedEntityPathFormat));
            File.WriteAllText (string.Format (EncryptedEntityPathFormat, type.FullName.Replace ("Chsopoly.UserData.Entity", "").Replace (".", "")), content);

            AssetDatabase.Refresh ();
        }

        private static void GenerateEntityAccessorScript (Type type)
        {
            var content = string.Empty;
            if (IsSingleEntity (type))
            {
                content = SingleEntityAccessorTemplate.Replace ("${NAME}", type.Name);
            }
            else
            {
                content = EntityAccessorTemplate.Replace ("${NAME}", type.Name);
            }
            Directory.CreateDirectory (Path.GetDirectoryName (EntityAccessorPathFormat));
            File.WriteAllText (string.Format (EntityAccessorPathFormat, type.FullName.Replace ("Chsopoly.UserData.Entity", "").Replace (".", "")), content);

            AssetDatabase.Refresh ();
        }

        private static void GenerateUserDataManagerScript ()
        {
            var serializeBuilder = new StringBuilder ();
            var accessorBuilder = new StringBuilder ();
            var initializeBuilder = new StringBuilder ();
            var loadBuilder = new StringBuilder ();
            var saveBuilder = new StringBuilder ();
            var insertBuilder = new StringBuilder ();
            var replaceBuilder = new StringBuilder ();
            var deleteBuilder = new StringBuilder ();
            var decryptBuilder = new StringBuilder ();
            var encryptBuilder = new StringBuilder ();

            foreach (var type in Assembly.Load ("Assembly-CSharp").GetTypes ())
            {
                if (IsEntity (type) || IsSingleEntity (type))
                {
                    if (IsSingleEntity (type))
                    {
                        serializeBuilder.AppendLine (string.Format ("[SerializeField] {0} _{0} = default;", type.Name));
                        accessorBuilder.AppendLine (string.Format ("public {0} {0} {1} get {1} return _{0}; {2} {2}", type.Name, "{", "}"));
                        loadBuilder.AppendLine (string.Format ("_{0} = Decrypt<{1}> (_connection.Table<{2}> ().FirstOrDefault ());", type.Name, type.FullName, type.FullName.ToEncryptedClassName ()));
                        loadBuilder.AppendLine (string.Format ("if (_{0} == null) _{0} = new {1} ();", type.Name, type.FullName));
                        saveBuilder.AppendLine (string.Format ("if (_{0}.IsDirty) InsertOrReplace<{1}> (_{0});", type.Name, type.FullName));
                    }
                    else
                    {
                        serializeBuilder.AppendLine (string.Format ("[SerializeField] List<{0}> _{0} = default;", type.Name));
                        accessorBuilder.AppendLine (string.Format ("public List<{0}> {0} {1} get {1} return _{0}; {2} {2}", type.Name, "{", "}"));
                        loadBuilder.AppendLine (string.Format ("_{0} = (_connection.Table<{1}> () as IEnumerable<{1}>).Select (o => Decrypt<{2}> (o)).ToList ();", type.Name, type.FullName.ToEncryptedClassName (), type.FullName));
                        saveBuilder.AppendLine (string.Format ("foreach (var e in _{0})", type.Name));
                        saveBuilder.AppendLine (string.Format ("if (e.IsNew) Insert<{0}> (e);", type.FullName).Indent (4));
                        saveBuilder.AppendLine (string.Format ("else if (e.IsDirty) InsertOrReplace<{0}> (e);", type.FullName).Indent (4));
                        saveBuilder.AppendLine (string.Format ("else if (e.IsDelete) Delete<{0}> (e);", type.FullName).Indent (4));
                        deleteBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                        deleteBuilder.AppendLine ("{");
                        deleteBuilder.AppendLine (string.Format ("_connection.Delete<{0}> (entity.Id);", type.FullName.ToEncryptedClassName ()).Indent (4));
                        deleteBuilder.AppendLine (string.Format ("_{0}.Remove (entity as {1});", type.Name, type.FullName).Indent (4));
                        deleteBuilder.AppendLine ("}");
                    }
                    initializeBuilder.AppendLine (string.Format ("_connection.CreateTable<{0}> ();", type.FullName.ToEncryptedClassName ()));
                    insertBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    insertBuilder.AppendLine (string.Format ("    _connection.Insert (Encrypt (entity));"));
                    replaceBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    replaceBuilder.AppendLine (string.Format ("    _connection.InsertOrReplace (Encrypt (entity));"));
                    decryptBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    decryptBuilder.AppendLine ("{");
                    decryptBuilder.AppendLine (string.Format ("    var entity = new {0} (false);", type.FullName));
                    decryptBuilder.AppendLine (string.Format ("    var encryptedEntity = ({0}) encrypted;", type.FullName.ToEncryptedClassName ()));
                    encryptBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    encryptBuilder.AppendLine ("{");
                    encryptBuilder.AppendLine (string.Format ("    var encrypted = new {0} ();", type.FullName.ToEncryptedClassName ()));
                    encryptBuilder.AppendLine (string.Format ("    var targetEntity = entity as {0};", type.FullName));

                    var decryptFieldBuilder = new StringBuilder ();
                    var encryptFieldBuilder = new StringBuilder ();
                    decryptFieldBuilder.AppendLine ("entity.Id = encryptedEntity.Id;");
                    encryptFieldBuilder.AppendLine ("encrypted.Id = targetEntity.Id;");
                    foreach (var field in type.GetFields ())
                    {
                        if (field.FieldType.IsPrimitive || field.FieldType == typeof (string))
                        {
                            if (field.FieldType == typeof (string))
                            {
                                decryptFieldBuilder.AppendLine (string.Format ("entity.{0} = Cipher.Decrypt (encryptedEntity.{1}, \"{2}\");", field.Name, field.Name.ToEncryptedFieldName (), EncryptionPassword));
                            }
                            else
                            {
                                decryptFieldBuilder.AppendLine (string.Format ("entity.{0} = {1}.Parse (Cipher.Decrypt (encryptedEntity.{2}, \"{3}\"));", field.Name, field.FieldType.FullName, field.Name.ToEncryptedFieldName (), EncryptionPassword));
                            }
                            encryptFieldBuilder.AppendLine (string.Format ("encrypted.{0} = Cipher.Encrypt (targetEntity.{1}.ToString (), \"{2}\");", field.Name.ToEncryptedFieldName (), field.Name, EncryptionPassword));
                        }
                    }

                    decryptBuilder.AppendLine (decryptFieldBuilder.ToString ().Indent (4));
                    decryptBuilder.AppendLine ("    return entity as T;");
                    decryptBuilder.AppendLine ("}");
                    encryptBuilder.AppendLine (encryptFieldBuilder.ToString ().Indent (4));
                    encryptBuilder.AppendLine ("    return encrypted;");
                    encryptBuilder.AppendLine ("}");
                }
            }

            if (insertBuilder.Length > 0)
                insertBuilder.Remove (0, "else ".Count ());
            if (replaceBuilder.Length > 0)
                replaceBuilder.Remove (0, "else ".Count ());
            if (deleteBuilder.Length > 0)
                deleteBuilder.Remove (0, "else ".Count ());
            if (encryptBuilder.Length > 0)
                encryptBuilder.Remove (0, "else ".Count ());
            if (decryptBuilder.Length > 0)
                decryptBuilder.Remove (0, "else ".Count ());

            var insertIndent = "";
            if (insertBuilder.Length > 0)
            {
                insertBuilder.AppendLine ("else");
                insertIndent = insertIndent.Indent (4);
            }

            var replaceIndent = "";
            if (replaceBuilder.Length > 0)
            {
                replaceBuilder.AppendLine ("else");
                replaceIndent = replaceIndent.Indent (4);
            }

            var content = UserDataManagerTemplate
                .Replace ("${SERIALIZER}", serializeBuilder.ToString ().Indent (8))
                .Replace ("${ACCESSORS}", accessorBuilder.ToString ().Indent (8))
                .Replace ("${LOADERS}", loadBuilder.ToString ().Indent (12))
                .Replace ("${INITIALIZERS}", initializeBuilder.ToString ().Indent (12))
                .Replace ("${LOADERS}", loadBuilder.ToString ().Indent (12))
                .Replace ("${SAVERS}", saveBuilder.ToString ().Indent (12))
                .Replace ("${INSERTERS}", insertBuilder.ToString ().Indent (12))
                .Replace ("${INSERT_INDENT}", insertIndent)
                .Replace ("${REPLACERS}", replaceBuilder.ToString ().Indent (12))
                .Replace ("${REPLACE_INDENT}", replaceIndent)
                .Replace ("${DELETES}", deleteBuilder.ToString ().Indent (12))
                .Replace ("${DECRYPTERS}", decryptBuilder.ToString ().Indent (12))
                .Replace ("${ENCRYPTERS}", encryptBuilder.ToString ().Indent (12));
            Directory.CreateDirectory (Path.GetDirectoryName (UserDataManagerPath));
            File.WriteAllText (UserDataManagerPath, content);

            AssetDatabase.Refresh ();
        }

        private static string ToEncryptedFieldName (this string self)
        {
            return "p" + self.ToSha256Hash ();
        }

        private static string ToEncryptedClassName (this string self)
        {
            return "e" + self.ToSha256Hash ();
        }

        private static bool IsEntity (Type type)
        {
            return type.FullName.StartsWith ("Chsopoly.UserData.Entity.");
        }

        private static bool IsSingleEntity (Type type)
        {
            return type.Name == "Account";
        }
    }
}