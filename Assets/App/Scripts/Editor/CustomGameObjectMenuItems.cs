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
            var obj = new GameObject ("SimpleButton");
            if (Selection.activeGameObject != null)
            {
                obj.transform.SetParent (Selection.activeGameObject.transform);
                obj.layer = Selection.activeGameObject.layer;
            }
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            var image = obj.AddComponent<Image> ();
            var button = obj.AddComponent<SimpleButton> ();

            Undo.RegisterCreatedObjectUndo (obj, obj.GetHashCode ().ToString ());
        }
    }
}