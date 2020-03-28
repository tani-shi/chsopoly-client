// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
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
        [SerializeField] Account _Account = default;
        [SerializeField] List<Character> _Character = default;
        [SerializeField] List<Gimmick> _Gimmick = default;

        public Account Account { get { return _Account; } }
        public List<Character> Character { get { return _Character; } }
        public List<Gimmick> Gimmick { get { return _Gimmick; } }

        private SQLiteConnection _connection;

        public void Load ()
        {
            _connection = new SQLiteConnection (Application.persistentDataPath + "/d");

            _connection.CreateTable<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ();
            _connection.CreateTable<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> ();
            _connection.CreateTable<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> ();

            _Account = Decrypt<Chsopoly.UserData.Entity.Account> (_connection.Table<e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce> ().FirstOrDefault ());
            if (_Account == null) _Account = new Chsopoly.UserData.Entity.Account ();
            _Character = (_connection.Table<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> () as IEnumerable<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421>).Select (o => Decrypt<Chsopoly.UserData.Entity.Character> (o)).ToList ();
            _Gimmick = (_connection.Table<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> () as IEnumerable<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c>).Select (o => Decrypt<Chsopoly.UserData.Entity.Gimmick> (o)).ToList ();
        }

        public void Save ()
        {
            if (_Account.IsDirty) InsertOrReplace<Chsopoly.UserData.Entity.Account> (_Account);
            foreach (var e in _Character)
                if (e.IsNew) Insert<Chsopoly.UserData.Entity.Character> (e);
                else if (e.IsDirty) InsertOrReplace<Chsopoly.UserData.Entity.Character> (e);
                else if (e.IsDelete) Delete<Chsopoly.UserData.Entity.Character> (e);
            foreach (var e in _Gimmick)
                if (e.IsNew) Insert<Chsopoly.UserData.Entity.Gimmick> (e);
                else if (e.IsDirty) InsertOrReplace<Chsopoly.UserData.Entity.Gimmick> (e);
                else if (e.IsDelete) Delete<Chsopoly.UserData.Entity.Gimmick> (e);
        }

        private void Insert<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

            entity.IsDirty = false;
            entity.IsNew = false;

            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                _connection.Insert (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                _connection.Insert (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                _connection.Insert (Encrypt (entity));
            else
                throw new NotImplementedException ("The entity type is not supported to decrypt. " + typeof (T).FullName);
        }

        private void InsertOrReplace<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

            entity.IsDirty = false;
            entity.IsNew = false;

            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Account))
                _connection.InsertOrReplace (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
                _connection.InsertOrReplace (Encrypt (entity));
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
                _connection.InsertOrReplace (Encrypt (entity));
            else
                throw new NotImplementedException ("The entity type is not supported to decrypt. " + typeof (T).FullName);
        }

        private void Delete<T> (T entity) where T : class, IUserDataEntity, new ()
        {
            if (entity == null)
            {
                return;
            }

            if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
            {
                _connection.Delete<e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421> (entity.Id);
                _Character.Remove (entity as Chsopoly.UserData.Entity.Character);
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
            {
                _connection.Delete<ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c> (entity.Id);
                _Gimmick.Remove (entity as Chsopoly.UserData.Entity.Gimmick);
            }
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
                var entity = new Chsopoly.UserData.Entity.Account (false);
                var encryptedEntity = (e4c6c753edb7bb08a340a30a63b55a2974f5775a7af4ffb504b3254585da9a5ce) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.gs2AccountId = Cipher.Decrypt (encryptedEntity.pcbd7e12366094089e67e732de02567338ab87b434218b7ff65784cb06c0bbd12, "chsopoly");
                entity.gs2Password = Cipher.Decrypt (encryptedEntity.pdd2b431ac147a328da239a1e109d237516caa4fc7a143093fd1ec6a2e6cba1c0, "chsopoly");
                entity.gs2CreatedAt = System.Int64.Parse (Cipher.Decrypt (encryptedEntity.pb7b330e603a22cd9820cbea8e7307339f56683e9be3db3d351c10d2036a92d7c, "chsopoly"));
                entity.characterId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.peb1fe45bcc9bff0588bafcaeb44c21cce576505b5cecccfc6e299d27786b510d, "chsopoly"));
                return entity as T;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
            {
                var entity = new Chsopoly.UserData.Entity.Character (false);
                var encryptedEntity = (e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.characterId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.peb1fe45bcc9bff0588bafcaeb44c21cce576505b5cecccfc6e299d27786b510d, "chsopoly"));
                return entity as T;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
            {
                var entity = new Chsopoly.UserData.Entity.Gimmick (false);
                var encryptedEntity = (ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c) encrypted;
                entity.Id = encryptedEntity.Id;
                entity.gimmickId = System.UInt32.Parse (Cipher.Decrypt (encryptedEntity.pb870c2fa63cadedea9fbed5b519faa92c5a8b35787896c6b78a7d7d49e1c5337, "chsopoly"));
                entity.isActive = System.Boolean.Parse (Cipher.Decrypt (encryptedEntity.p9442331fe398b259a1a1dd4ddc062049fca67f4e6d6c783dd838394cb547cb05, "chsopoly"));
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
                encrypted.pcbd7e12366094089e67e732de02567338ab87b434218b7ff65784cb06c0bbd12 = Cipher.Encrypt (targetEntity.gs2AccountId.ToString (), "chsopoly");
                encrypted.pdd2b431ac147a328da239a1e109d237516caa4fc7a143093fd1ec6a2e6cba1c0 = Cipher.Encrypt (targetEntity.gs2Password.ToString (), "chsopoly");
                encrypted.pb7b330e603a22cd9820cbea8e7307339f56683e9be3db3d351c10d2036a92d7c = Cipher.Encrypt (targetEntity.gs2CreatedAt.ToString (), "chsopoly");
                encrypted.peb1fe45bcc9bff0588bafcaeb44c21cce576505b5cecccfc6e299d27786b510d = Cipher.Encrypt (targetEntity.characterId.ToString (), "chsopoly");
                return encrypted;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Character))
            {
                var encrypted = new e0535323a9a61b8c6d1f616466044d8548acf190cee5d12d5e8a8c94f64c07421 ();
                var targetEntity = entity as Chsopoly.UserData.Entity.Character;
                encrypted.Id = targetEntity.Id;
                encrypted.peb1fe45bcc9bff0588bafcaeb44c21cce576505b5cecccfc6e299d27786b510d = Cipher.Encrypt (targetEntity.characterId.ToString (), "chsopoly");
                return encrypted;
            }
            else if (typeof (T) == typeof (Chsopoly.UserData.Entity.Gimmick))
            {
                var encrypted = new ee4356908d2934e4f501867e6d216f4f5158f7d7faa11b178219363d15f19682c ();
                var targetEntity = entity as Chsopoly.UserData.Entity.Gimmick;
                encrypted.Id = targetEntity.Id;
                encrypted.pb870c2fa63cadedea9fbed5b519faa92c5a8b35787896c6b78a7d7d49e1c5337 = Cipher.Encrypt (targetEntity.gimmickId.ToString (), "chsopoly");
                encrypted.p9442331fe398b259a1a1dd4ddc062049fca67f4e6d6c783dd838394cb547cb05 = Cipher.Encrypt (targetEntity.isActive.ToString (), "chsopoly");
                return encrypted;
            }
            return null;
        }
    }
}