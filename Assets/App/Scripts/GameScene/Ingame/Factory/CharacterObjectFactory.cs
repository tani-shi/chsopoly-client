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
using UnityEngine.UI;

namespace Chsopoly.GameScene.Ingame.Factory
{
    public class CharacterObjectFactory
    {
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
            (obj.transform as RectTransform).pivot = new Vector2 (0.5f, 0);

            var animator = obj.SafeAddComponent<Animator> ();
            animator.runtimeAnimatorController = animHandle.Result;

            var image = obj.GetComponent<Image> ();
            image.SetNativeSize ();

            var rigidbody = obj.SafeAddComponent<Rigidbody2D> ();
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.mass = data.weight;

            var bodyCollider = obj.SafeAddComponent<CapsuleCollider2D> ();
            bodyCollider.size = new Vector2 (data.width, data.height);
            bodyCollider.offset = new Vector2 (0, data.height.Half ());
            bodyCollider.sharedMaterial = IngameSettings.Character.PhysicsMaterial;

            characterObject.Initialize (characterId);

            callback.SafeInvoke (obj);
        }

        private AsyncOperationHandle<GameObject> LoadCharacter (string assetName)
        {
            return Addressables.LoadAssetAsync<GameObject> (IngameSettings.Paths.CharacterPrefab (assetName));
        }

        private AsyncOperationHandle<RuntimeAnimatorController> LoadAnimation (string assetName)
        {
            return Addressables.LoadAssetAsync<RuntimeAnimatorController> (IngameSettings.Paths.CharacterAnimator (assetName));
        }
    }
}