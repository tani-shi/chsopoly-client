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
    public static class UserDataEncryptedEntityGenerator
    {
        private const string EncryptionPassword = "chsopoly";
        private const string EncryptedEntityPathFormat = "Assets/App/Scripts/BaseSystem/UserData/Entity/Generated/{0}.cs";
        private const string UserDataEntityHelperPath = "Assets/App/Scripts/BaseSystem/UserData/Generated/SQLiteConnectionAccessor.cs";
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
        private const string SQLiteConnectionAccessorTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Linq;
using System.Collections.Generic;
using Chsopoly.BaseSystem.UserData.Entity;
using Chsopoly.Libs;
using SQLite4Unity3d;

namespace Chsopoly.BaseSystem.UserData
{
    public class SQLiteConnectionAccessor
    {
        public SQLiteConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        private SQLiteConnection _connection;

        public SQLiteConnectionAccessor (string dbPath)
        {
            _connection = new SQLiteConnection (dbPath);

${INITIALIZERS}
        }

        public T Load<T> (int id) where T : class, IUserDataEntity, new ()
        {
${LOADERS_WITH_PK}
            throw new NotImplementedException (""The entity type is not supported to load. "" + typeof (T).FullName);
        }

        public T LoadFirst<T> () where T : class, IUserDataEntity, new ()
        {
${FIRST_LOADERS}
            throw new NotImplementedException (""The entity type is not supported to load. "" + typeof (T).FullName);
        }

        public List<T> LoadAll<T> () where T : class, IUserDataEntity, new ()
        {
${LOADERS}
            throw new NotImplementedException (""The entity type is not supported to load. "" + typeof (T).FullName);
        }

        public void Insert<T> (T entity) where T : class, IUserDataEntity, new ()
        {
${INSERTERS}
            ${INSERT_INDENT}throw new NotImplementedException (""The entity type is not supported to decrypt. "" + typeof (T).FullName);
        }

        public void InsertOrReplace<T> (T entity) where T : class, IUserDataEntity, new ()
        {
${REPLACERS}
            ${REPLACE_INDENT}throw new NotImplementedException (""The entity type is not supported to decrypt. "" + typeof (T).FullName);
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

        [MenuItem ("Project/User Data/Update UserData Encrypted Entity And SQLite Accessor")]
        private static void GenerateScripts ()
        {
            GenerateEncryptedEntityScripts ();
            GenerateSQLiteConnectionAccessorScript ();
        }

        private static void GenerateEncryptedEntityScripts ()
        {
            foreach (var type in Assembly.Load ("Assembly-CSharp").GetTypes ())
            {
                if (type.GetInterfaces ().Contains (typeof (IUserDataEntity)))
                {
                    GenerateEncryptedEntityScript (type);
                }
            }
        }

        private static void GenerateEncryptedEntityScript (Type type)
        {
            var builder = new StringBuilder ();
            foreach (var prop in type.GetProperties ())
            {
                if (prop.Name != "Id")
                {
                    builder.AppendLine ("public string " + prop.Name.ToEncryptedFieldName () + " { get; set; }");
                }
            }
            var content = EncryptedEntityTemplate
                .Replace ("${CLASS_NAME}", type.FullName.ToEncryptedClassName ())
                .Replace ("${PK}", "Id".ToEncryptedFieldName ())
                .Replace ("${FIELDS}", builder.ToString ().Indent (8));
            Directory.CreateDirectory (Path.GetDirectoryName (EncryptedEntityPathFormat));
            File.WriteAllText (string.Format (EncryptedEntityPathFormat, type.FullName.Replace ("Chsopoly.UserData.Entity", "").Replace (".", "")), content);

            AssetDatabase.Refresh ();
        }

        private static void GenerateSQLiteConnectionAccessorScript ()
        {
            var initializeBuilder = new StringBuilder ();
            var loadBuilder = new StringBuilder ();
            var loadFirstBuilder = new StringBuilder ();
            var loadWithPkBuilder = new StringBuilder ();
            var insertBuilder = new StringBuilder ();
            var replaceBuilder = new StringBuilder ();
            var decryptBuilder = new StringBuilder ();
            var encryptBuilder = new StringBuilder ();

            foreach (var type in Assembly.Load ("Assembly-CSharp").GetTypes ())
            {
                if (type.GetInterfaces ().Contains (typeof (IUserDataEntity)))
                {
                    initializeBuilder.AppendLine (string.Format ("_connection.CreateTable<{0}> ();", type.FullName.ToEncryptedClassName ()));
                    loadFirstBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    loadFirstBuilder.AppendLine (string.Format ("    return Decrypt<T> (_connection.Table<{0}> ().FirstOrDefault ());", type.FullName.ToEncryptedClassName ()));
                    loadWithPkBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    loadWithPkBuilder.AppendLine (string.Format ("    return Decrypt<T> (_connection.Find<{0}> (id));", type.FullName.ToEncryptedClassName ()));
                    loadBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    loadBuilder.AppendLine (string.Format ("    return _connection.Table<{0}> ().ToList ().ConvertAll (o => Decrypt<T> (o));", type.FullName.ToEncryptedClassName ()));
                    insertBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    insertBuilder.AppendLine (string.Format ("    _connection.Insert (Encrypt (entity));"));
                    replaceBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    replaceBuilder.AppendLine (string.Format ("    _connection.InsertOrReplace (Encrypt (entity));"));
                    decryptBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    decryptBuilder.AppendLine ("{");
                    decryptBuilder.AppendLine (string.Format ("    var entity = new {0} ();", type.FullName));
                    decryptBuilder.AppendLine (string.Format ("    var encryptedEntity = ({0}) encrypted;", type.FullName.ToEncryptedClassName ()));
                    encryptBuilder.AppendLine (string.Format ("else if (typeof (T) == typeof ({0}))", type.FullName));
                    encryptBuilder.AppendLine ("{");
                    encryptBuilder.AppendLine (string.Format ("    var encrypted = new {0} ();", type.FullName.ToEncryptedClassName ()));
                    encryptBuilder.AppendLine (string.Format ("    var targetEntity = entity as {0};", type.FullName));

                    var decryptFieldBuilder = new StringBuilder ();
                    var encryptFieldBuilder = new StringBuilder ();
                    foreach (var prop in type.GetProperties ())
                    {
                        if (prop.Name == "Id")
                        {
                            decryptFieldBuilder.AppendLine ("entity.Id = encryptedEntity.Id;");
                            encryptFieldBuilder.AppendLine ("encrypted.Id = targetEntity.Id;");
                        }
                        else
                        {
                            if (prop.PropertyType == typeof (string))
                            {
                                decryptFieldBuilder.AppendLine (string.Format ("entity.{0} = Cipher.Decrypt (encryptedEntity.{1}, \"{2}\");", prop.Name, prop.Name.ToEncryptedFieldName (), EncryptionPassword));
                            }
                            else
                            {
                                decryptFieldBuilder.AppendLine (string.Format ("entity.{0} = {1}.Parse (Cipher.Decrypt (encryptedEntity.{2}, \"{3}\"));", prop.Name, prop.PropertyType.FullName, prop.Name.ToEncryptedFieldName (), EncryptionPassword));
                            }
                            encryptFieldBuilder.AppendLine (string.Format ("encrypted.{0} = Cipher.Encrypt (targetEntity.{1}.ToString (), \"{2}\");", prop.Name.ToEncryptedFieldName (), prop.Name, EncryptionPassword));
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
            loadFirstBuilder.Remove (0, "else ".Count ());
            loadWithPkBuilder.Remove (0, "else ".Count ());
            loadBuilder.Remove (0, "else ".Count ());
            insertBuilder.Remove (0, "else ".Count ());
            replaceBuilder.Remove (0, "else ".Count ());
            encryptBuilder.Remove (0, "else ".Count ());
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

            var content = SQLiteConnectionAccessorTemplate
                .Replace ("${INITIALIZERS}", initializeBuilder.ToString ().Indent (12))
                .Replace ("${FIRST_LOADERS}", loadFirstBuilder.ToString ().Indent (12))
                .Replace ("${LOADERS_WITH_PK}", loadWithPkBuilder.ToString ().Indent (12))
                .Replace ("${LOADERS}", loadBuilder.ToString ().Indent (12))
                .Replace ("${INSERTERS}", insertBuilder.ToString ().Indent (12))
                .Replace ("${INSERT_INDENT}", insertIndent)
                .Replace ("${REPLACERS}", replaceBuilder.ToString ().Indent (12))
                .Replace ("${REPLACE_INDENT}", replaceIndent)
                .Replace ("${DECRYPTERS}", decryptBuilder.ToString ().Indent (12))
                .Replace ("${ENCRYPTERS}", encryptBuilder.ToString ().Indent (12));
            Directory.CreateDirectory (Path.GetDirectoryName (UserDataEntityHelperPath));
            File.WriteAllText (UserDataEntityHelperPath, content);

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
    }
}