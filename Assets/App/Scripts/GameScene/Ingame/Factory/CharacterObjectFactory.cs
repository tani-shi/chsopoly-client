using System;
using System.Collections;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame.Object.Character;
using Chsopoly.GameScene.Ingame.Object.State.Character;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class CharacterObjectFactory
    {
        private const string PrefabPathFormat = "Assets/App/AddressableAssets/Prefabs/Character/{0}.prefab";
        private const string AnimatorControllerPathFormat = "Assets/App/AddressableAssets/Animations/Character/{0}.controller";

        public IEnumerator CreateCharacter (uint characterId, Transform parent, Action<GameObject> callback = null)
        {
            var data = MasterDataManager.Instance.Get<CharacterDAO> ().Get (characterId);
            var objHandle = LoadCharacter (data.assetName);
            yield return objHandle;
            if (objHandle.Result == null)
            {
                Debug.LogError ("Failed to load a character object prefab. " + data.assetName);
                yield break;
            }
            var animHandle = LoadAnimation (data.assetName);
            yield return animHandle;
            if (animHandle.Result == null)
            {
                Debug.LogError ("Failed to load a character animator controller. " + data.assetName);
                yield break;
            }

            var obj = objHandle.Result.CreateInstance (parent);
            var characterObject = obj.SafeAddComponent<CharacterObject> ();
            obj.SafeAddComponent<CharacterObjectModel> ();
            obj.SafeAddComponent<CharacterStateMachine> ();

            var animator = obj.SafeAddComponent<Animator> ();
            animator.runtimeAnimatorController = animHandle.Result;

            var rigidbody = obj.SafeAddComponent<Rigidbody2D> ();
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.gravityScale = 10.0f;
            rigidbody.mass = data.weight;

            characterObject.Initialize (characterId);

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadCharacter (string assetName)
        {
            return Addressables.LoadAssetAsync<GameObject> (string.Format (PrefabPathFormat, assetName));
        }

        private AsyncOperationHandle<RuntimeAnimatorController> LoadAnimation (string assetName)
        {
            return Addressables.LoadAssetAsync<RuntimeAnimatorController> (string.Format (AnimatorControllerPathFormat, assetName));
        }
    }
}