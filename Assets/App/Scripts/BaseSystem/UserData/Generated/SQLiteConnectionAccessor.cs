// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
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

            _connection.CreateTable<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ();
            _connection.CreateTable<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> ();
            _connection.CreateTable<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> ();
        }

        public T Load<T> (int id) where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                return Decrypt<T> (_connection.Find<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> (id));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                return Decrypt<T> (_connection.Find<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> (id));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                return Decrypt<T> (_connection.Find<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> (id));
            throw new NotImplementedException ("The entity type is not supported to load. " + typeof (T).FullName);
        }

        public T LoadFirst<T> () where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                return Decrypt<T> (_connection.Table<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ().FirstOrDefault ());
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                return Decrypt<T> (_connection.Table<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> ().FirstOrDefault ());
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                return Decrypt<T> (_connection.Table<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> ().FirstOrDefault ());
            throw new NotImplementedException ("The entity type is not supported to load. " + typeof (T).FullName);
        }

        public List<T> LoadAll<T> () where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                return _connection.Table<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ().ToList ().ConvertAll (o => Decrypt<T> (o));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                return _connection.Table<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> ().ToList ().ConvertAll (o => Decrypt<T> (o));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                return _connection.Table<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> ().ToList ().ConvertAll (o => Decrypt<T> (o));
            throw new NotImplementedException ("The entity type is not supported to load. " + typeof (T).FullName);
        }

        public void Insert<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                _connection.Insert (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                _connection.Insert (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                _connection.Insert (Encrypt (entity));
            else
                throw new NotImplementedException ("The entity type is not supported to decrypt. " + typeof (T).FullName);
        }

        public void InsertOrReplace<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                _connection.InsertOrReplace (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                _connection.InsertOrReplace (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
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
                entity.CharacterId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.p6675390ca6b0c7cba333937d1c0f53c9a82e61911c5f7e97a8b526631912e25c, "chsopoly"));
                return entity as T;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
            {
                var entity = new Chsopoly.UserData.Entity.Character ();
                var encryptedEntity = (e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.CharacterId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.p6675390ca6b0c7cba333937d1c0f53c9a82e61911c5f7e97a8b526631912e25c, "chsopoly"));
                return entity as T;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
            {
                var entity = new Chsopoly.UserData.Entity.Gimmick ();
                var encryptedEntity = (ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.GimmickId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.pde4250749f5d4fb051b4f95bb2af086b6a7cc1e7c3b421f8f33dacb8d0f8150b, "chsopoly"));
                entity.IsActive = System.Boolean.Parse (Cipher.Decrypt (encryptedEntity.p57401e5981c1341e2a7a2af3d54adf02f1b7935f794d1a8bca9294a604bf8819, "chsopoly"));
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
                encrypted.p6675390ca6b0c7cba333937d1c0f53c9a82e61911c5f7e97a8b526631912e25c = Cipher.Encrypt (targetEntity.CharacterId.ToString (), "chsopoly");
                return encrypted;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
            {
                var encrypted = new e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421 ();
                var targetEntity = entity as Chsopoly.UserData.Entity.Character;
                encrypted.Id = targetEntity.Id;
                encrypted.p6675390ca6b0c7cba333937d1c0f53c9a82e61911c5f7e97a8b526631912e25c = Cipher.Encrypt (targetEntity.CharacterId.ToString (), "chsopoly");
                return encrypted;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
            {
                var encrypted = new ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c ();
                var targetEntity = entity as Chsopoly.UserData.Entity.Gimmick;
                encrypted.Id = targetEntity.Id;
                encrypted.pde4250749f5d4fb051b4f95bb2af086b6a7cc1e7c3b421f8f33dacb8d0f8150b = Cipher.Encrypt (targetEntity.GimmickId.ToString (), "chsopoly");
                encrypted.p57401e5981c1341e2a7a2af3d54adf02f1b7935f794d1a8bca9294a604bf8819 = Cipher.Encrypt (targetEntity.IsActive.ToString (), "chsopoly");
                return encrypted;
            }
            return null;
        }
    }
}