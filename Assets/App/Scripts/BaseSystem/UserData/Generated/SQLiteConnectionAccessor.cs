// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
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

            _connection.CreateTable<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ();
        }

        public T Load<T> () where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                return Decrypt<T> (_connection.Table<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ().FirstOrDefault ());
            throw new NotImplementedException ("The entity type is not supported to load. " + typeof (T).FullName);
        }

        public T Load<T> (int id) where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                return Decrypt<T> (_connection.Find<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> (id));
            throw new NotImplementedException ("The entity type is not supported to load. " + typeof (T).FullName);
        }

        public void Save<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                _connection.InsertOrReplace (Encrypt (entity));
            else
                throw new NotImplementedException ("The entity type is not supported to decrypt. " + typeof (T).FullName);
        }

        private T Decrypt<T> (object encrypted) where T : class, IUserDataEntity, new ()
        {
            if (encrypted == null)
            {
                return null;
            }

            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
            {
                var entity = new Chsopoly.UserData.Entity.Account ();
                var encryptedEntity = (e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.Gs2AccountId = Cipher.Decrypt (encryptedEntity.p9f8f4c20cf8b22b6dae34559daafc1c08ef63f3927c00d0b8be758cde29f9feb, "chsopoly");
                entity.Gs2Password = Cipher.Decrypt (encryptedEntity.pba88ac0aa5fd0c2c906d037104f1dcf0afb340684e87d90a871129910e96c215, "chsopoly");
                entity.Gs2CreatedAt = System.Int64.Parse (Cipher.Decrypt (encryptedEntity.p5498dd13b7f1904db60aa00a58303a8b97862ed05d965da3fdaf62d3d1ede812, "chsopoly"));
                return entity as T;
            }
            return null;
        }

        private object Encrypt<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return null;
            }

            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
            {
                var encrypted = new e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce ();
                var targetEntity = entity as Chsopoly.UserData.Entity.Account;
                encrypted.Id = targetEntity.Id;
                encrypted.p9f8f4c20cf8b22b6dae34559daafc1c08ef63f3927c00d0b8be758cde29f9feb = Cipher.Encrypt (targetEntity.Gs2AccountId.ToString (), "chsopoly");
                encrypted.pba88ac0aa5fd0c2c906d037104f1dcf0afb340684e87d90a871129910e96c215 = Cipher.Encrypt (targetEntity.Gs2Password.ToString (), "chsopoly");
                encrypted.p5498dd13b7f1904db60aa00a58303a8b97862ed05d965da3fdaf62d3d1ede812 = Cipher.Encrypt (targetEntity.Gs2CreatedAt.ToString (), "chsopoly");
                return encrypted;
            }
            return null;
        }
    }
}