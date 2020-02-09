using System.Collections.Generic;
using Chsopoly.Libs.Extensions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Chsopoly.Libs.Editor
{
    [CustomEditor (typeof (AnimatorController))]
    public class AnimatorControllerInspector : UnityEditor.Editor
    {
        private string _newClipName;
        private Vector2 _scrollPosition;
        private List<string> _clipNameList = new List<string> ();
        private AnimationClip _importClip;

        public override void OnInspectorGUI ()
        {
            base.DrawDefaultInspector ();

            var controller = target as AnimatorController;
            if (controller == null)
            {
                return;
            }

            using (new GUILayout.HorizontalScope ())
            {
                GUILayout.Label ("Animation Clip Name");
                _newClipName = EditorGUILayout.TextField (_newClipName);
                using (new EditorGUI.DisabledScope (string.IsNullOrEmpty (_newClipName)))
                {
                    if (GUILayout.Button ("Create New Clip"))
                    {
                        var clip = AnimatorController.AllocateAnimatorClip (_newClipName);
                        AssetDatabase.AddObjectToAsset (clip, controller);
                        ReimportController (controller);
                    }
                }
            }

            using (new GUILayout.HorizontalScope ())
            {
                _importClip = EditorGUILayout.ObjectField (_importClip, typeof (AnimationClip), false) as AnimationClip;
                using (new EditorGUI.DisabledScope (_importClip == null))
                {
                    if (GUILayout.Button ("Import Clip"))
                    {
                        AssetDatabase.AddObjectToAsset (_importClip.CreateInstance (), controller);
                        ReimportController (controller);
                    }
                }
            }

            using (var scope = new GUILayout.ScrollViewScope (_scrollPosition, GUILayout.Height (0f), GUILayout.ExpandHeight (true)))
            {
                _scrollPosition = scope.scrollPosition;

                var clipList = GetAllClipList (controller);
                if (clipList.Count != _clipNameList.Count)
                {
                    _clipNameList.Clear ();
                    for (int i = 0; i < clipList.Count; i++)
                    {
                        _clipNameList.Add (string.Empty);
                    }
                }

                if (clipList.Count > 0)
                {
                    for (int i = 0; i < clipList.Count; i++)
                    {
                        using (new GUILayout.HorizontalScope ("box"))
                        {
                            GUILayout.Label (string.Format ("{0}", clipList[i].name), GUILayout.Width (150f));
                            _clipNameList[i] = GUILayout.TextField (_clipNameList[i], GUILayout.Width (150f));
                            using (new EditorGUI.DisabledScope (string.IsNullOrEmpty (_clipNameList[i]) || _clipNameList[i].Equals (clipList[i].name)))
                            {
                                if (GUILayout.Button ("Rename"))
                                {
                                    clipList[i].name = _clipNameList[i];
                                    ReimportController (controller);
                                }
                            }
                            if (GUILayout.Button ("Delete"))
                            {
                                Object.DestroyImmediate (clipList[i], true);
                                ReimportController (controller);
                            }
                        }
                    }
                }
            }
        }

        private List<AnimationClip> GetAllClipList (AnimatorController controller)
        {
            var list = new List<AnimationClip> ();
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath (AssetDatabase.GetAssetPath (controller)))
            {
                if (asset is AnimationClip)
                {
                    list.Add (asset as AnimationClip);
                }
            }
            return list;
        }

        private void ReimportController (AnimatorController controller)
        {
            AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (controller), ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }
    }
}