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

        private CharacterObject _character = null;

        public IEnumerator CreateCharacter (uint characterId, Transform parent)
        {
            if (_character != null)
            {
                Debug.LogWarning ("The character object was already loaded. " + _character.name);
                yield break;
            }

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

            var obj = objHandle.Result.CreateInstance ();
            _character = obj.SafeAddComponent<CharacterObject> ();
            obj.SafeAddComponent<CharacterObjectModel> ();
            obj.SafeAddComponent<CharacterStateMachine> ();

            var collider = obj.SafeAddComponent<CapsuleCollider> ();
            collider.height = data.height;
            collider.radius = data.radius;

            var animator = obj.SafeAddComponent<Animator> ();
            animator.runtimeAnimatorController = animHandle.Result;
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