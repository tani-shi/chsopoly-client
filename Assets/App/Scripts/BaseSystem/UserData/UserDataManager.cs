using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Chsopoly.Libs;
using UnityEngine;

namespace Chsopoly.BaseSystem.UserData
{
    public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
    {
        private const string DatabaseName = "d";
        private const int FirstId = 1;

        private SQLiteConnectionAccessor _connection = null;
        private Dictionary<Type, Dictionary<int, IUserDataEntity>> _entityMap = new Dictionary<Type, Dictionary<int, IUserDataEntity>> ();

#if UNITY_EDITOR
        [UnityEditor.MenuItem ("Project/User Data/Delete User Data")]
        private static void DeleteUserData ()
        {
            if (File.Exists (Application.persistentDataPath + "/" + DatabaseName))
            {
                File.Delete (Application.persistentDataPath + "/" + DatabaseName);
            }
        }
#endif

        public void Initialize ()
        {
            _connection = new SQLiteConnectionAccessor (Application.persistentDataPath + "/" + DatabaseName);
        }

        public T LoadFirst<T> () where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }
            if (_entityMap[typeof (T)].ContainsKey (FirstId))
            {
                return _entityMap[typeof (T)][FirstId] as T;
            }

            var entity = _connection.LoadFirst<T> ();
            if (entity != null)
            {
                if (_entityMap[typeof (T)].ContainsKey (FirstId))
                {
                    _entityMap[typeof (T)][FirstId] = entity;
                }
                else
                {
                    _entityMap[typeof (T)].Add (FirstId, entity);
                }
            }
            return entity;
        }

        public T Load<T> (int id) where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }
            if (_entityMap[typeof (T)].ContainsKey (id))
            {
                return _entityMap[typeof (T)][id] as T;
            }

            var entity = _connection.Load<T> (id);
            if (entity != null)
            {
                if (_entityMap[typeof (T)].ContainsKey (id))
                {
                    _entityMap[typeof (T)][id] = entity;
                }
                else
                {
                    _entityMap[typeof (T)].Add (id, entity);
                }
            }
            return entity;
        }

        public List<T> LoadAll<T> () where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }

            var entities = _connection.LoadAll<T> ();
            foreach (var entity in entities)
            {
                if (_entityMap[typeof (T)].ContainsKey (entity.Id))
                {
                    _entityMap[typeof (T)][entity.Id] = entity;
                }
                else
                {
                    _entityMap[typeof (T)].Add (entity.Id, entity);
                }
            }
            return entities;
        }

        public void Save<T> (T entity) where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (entity.Id <= 0)
            {
                entity.Id = FirstId;
                _connection.Insert<T> (entity);
            }
            else
            {
                _connection.InsertOrReplace<T> (entity);
            }
        }

        public T GetFirst<T> () where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (_entityMap.ContainsKey (typeof (T)))
            {
                return _entityMap[typeof (T)].FirstOrDefault ().Value as T;
            }

            return null;
        }

        public T Get<T> (int id) where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (_entityMap.ContainsKey (typeof (T)))
            {
                return _entityMap[typeof (T)][id] as T;
            }

            return null;
        }

        public List<T> GetAll<T> () where T : class,
        IUserDataEntity,
        new ()
        {
            ValidateConnection ();

            if (_entityMap.ContainsKey (typeof (T)))
            {
                return _entityMap[typeof (T)].Values.ToList ().ConvertAll (o => o as T);
            }

            return null;
        }

        private void ValidateConnection ()
        {
            if (_connection == null)
            {
                throw new Exception ("UserDataManager is not ready, do Initialize first.");
            }
        }
    }
}