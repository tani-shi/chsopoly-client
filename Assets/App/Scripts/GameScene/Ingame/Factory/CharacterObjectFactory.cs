using System;
using System.Collections;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.Object.Character.State;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class CharacterObjectFactory
    {
        public IEnumerator CreateCharacter (uint characterId, uint connectionId, Transform parent, Action<GameObject> callback = null)
        {
            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (characterId);
            var handle = LoadCharacter (data.assetName);
            yield return handle;
            if (handle.Result == null)
            {
                Debug.LogError ("Failed to load a character object prefab. " + data.assetName);
                yield break;
            }

            var obj = handle.Result.CreateInstance (parent);
            obj.layer = IngameSettings.Layers.Character;

            var characterObject = obj.SafeAddComponent<CharacterObject> ();
            obj.SafeAddComponent<CharacterObjectModel> ();
            obj.SafeAddComponent<CharacterStateMachine> ().Initialize (characterObject);
            (obj.transform as RectTransform).pivot = new Vector2 (0.5f, 0);

            var image = obj.GetComponent<Image> ();
            image.SetNativeSize ();

            var rigidbody = obj.SafeAddComponent<Rigidbody2D> ();
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.mass = data.weight;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            characterObject.Initialize (characterId);
            characterObject.SetIdentity (connectionId);

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadCharacter (string assetName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.CharacterPrefab (assetName));
        }
    }
}