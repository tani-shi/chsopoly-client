using Chsopoly.Components.Button;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Chsopoly.Editor
{
    public class CustomGameObjectMenuItems
    {
        [MenuItem ("GameObject/UI/Button - Chsopoly", false, 2031)]
        private static void GenerateSimpleButton ()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            var obj = new GameObject ("SimpleButton");
            obj.transform.SetParent (Selection.activeGameObject.transform);
            obj.layer = Selection.activeGameObject.layer;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            var image = obj.AddComponent<Image> ();
            var button = obj.AddComponent<SimpleButton> ();
            var animator = obj.AddComponent<Animator> ();
            animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController> ("Assets/App/AddressableAssets/Animations/Components/SimpleButton.controller");

            Undo.RegisterCreatedObjectUndo (obj, obj.GetHashCode ().ToString ());
            Selection.activeGameObject = obj;
        }
    }
}