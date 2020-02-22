using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Chsopoly.Libs;
using UnityEngine;

namespace Chsopoly.BaseSystem.UserData
{
    public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
    {
        private const string DatabaseName = "d";

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

        public T Load<T> () where T : class,
        IUserDataEntity,
        new ()
        {
            if (_connection == null)
            {
                Debug.LogWarning ("UserDataManager is not ready, do Initialize first.");
                return null;
            }
            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }
            if (_entityMap[typeof (T)].Count > 0)
            {
                return (T) _entityMap[typeof (T)][0];
            }

            var entity = _connection.Load<T> ();
            if (entity != null)
            {
                _entityMap[typeof (T)].Add (entity.Id, entity);
            }
            return entity;
        }

        public T Load<T> (int id) where T : class,
        IUserDataEntity,
        new ()
        {
            if (_connection == null)
            {
                Debug.LogWarning ("UserDataManager is not ready, do Initialize first.");
                return null;
            }
            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }
            if (_entityMap[typeof (T)].ContainsKey (id))
            {
                return (T) _entityMap[typeof (T)][id];
            }

            var entity = _connection.Load<T> (id);
            if (entity != null)
            {
                _entityMap[typeof (T)].Add (id, entity);
            }
            return entity;
        }

        public void Save<T> (T entity) where T : class,
        IUserDataEntity,
        new ()
        {
            if (_connection == null)
            {
                Debug.LogWarning ("UserDataManager is not ready, do Initialize first.");
            }
            if (!_entityMap.ContainsKey (typeof (T)))
            {
                _entityMap.Add (typeof (T), new Dictionary<int, IUserDataEntity> ());
            }
            if (_entityMap[typeof (T)].ContainsKey (entity.Id))
            {
                _entityMap[typeof (T)][entity.Id] = entity;
            }

            _connection.Save<T> (entity);
        }
    }
}