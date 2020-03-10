using System;
using System.IO;
using System.Linq;
using Chsopoly.BaseSystem.MasterData;
using Chsopoly.GameScene.Ingame;
using Chsopoly.GameScene.Ingame.Object.Gimmick;
using Chsopoly.Libs.Extensions;
using Chsopoly.MasterData.Collection;
using Chsopoly.MasterData.DAO.Ingame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Chsopoly.Editor
{
    public static class GimmickModelCapture
    {
        private const int PixelSizeBuffer = 20;

        [MenuItem ("Project/Capture All Gimmick Models")]
        public static void CaptureAllObjectModel ()
        {
            var prevScene = EditorSceneManager.GetActiveScene ().path;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ())
            {
                return;
            }
            EditorSceneManager.OpenScene ("Assets/App/Editor/Scenes/GimmickModelCapture.unity");

            var camera = GameObject.Find ("Camera").GetComponent<UnityEngine.Camera> ();
            camera.aspect = 1;
            camera.backgroundColor = Color.clear;
            camera.clearFlags = CameraClearFlags.SolidColor;

            MasterDataManager.Instance.LoadLocal (new MasterDataAccessorObjectCollection ());
            int index = 0;
            int total = 0;
            foreach (var _ in MasterDataManager.Instance.Get<GimmickDAO> ()) total++;

            try
            {
                foreach (var data in MasterDataManager.Instance.Get<GimmickDAO> ())
                {
                    EditorUtility.DisplayProgressBar (string.Empty, string.Format ("Capturing all object models... {0} / {1}", index, total), (float) index / (float) total);

                    var path = IngameSettings.Paths.GimmickCapture (data.assetName);
                    Directory.CreateDirectory (Path.GetDirectoryName (path));

                    var canvas = GameObject.Find ("Canvas");
                    var model = CreateObjectModel (data.assetName, canvas.transform);
                    if (model == null)
                    {
                        Debug.LogError (string.Format ("CAPTURE: {0} -> {1}", data.assetName, path));
                        continue;
                    }

                    var sizeDelta = (model.transform as RectTransform).sizeDelta;
                    var pixelSize = Mathf.Max (sizeDelta.x, sizeDelta.y) + PixelSizeBuffer;
                    camera.orthographicSize = pixelSize / 2.0f;

                    Capture (camera, path, (int) pixelSize);
                    GameObject.DestroyImmediate (model.gameObject);

                    Debug.Log (string.Format ("CAPTURE: {0} -> {1}", data.assetName, path));
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar ();
                AssetDatabase.Refresh ();
                EditorSceneManager.OpenScene (prevScene);
            }
        }

        private static GimmickObjectModel CreateObjectModel (string assetName, Transform parent)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject> (IngameSettings.Paths.GimmickPrefab (assetName));
            var model = prefab.CreateInstance (parent).SafeAddComponent<GimmickObjectModel> ();
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            if (model.Animator != null)
            {
                var infos = model.Animator.GetCurrentAnimatorClipInfo (0);
                if (infos != null && infos.Length > 0)
                {
                    infos[0].clip.SampleAnimation (model.gameObject, 0);
                }
            }
            return model;
        }

        private static void Capture (UnityEngine.Camera camera, string outputPath, int pixelSize)
        {
            RenderTexture imageTemp = RenderTexture.GetTemporary (pixelSize, pixelSize, 32, RenderTextureFormat.ARGB32);
            var image = new Texture2D (pixelSize, pixelSize, TextureFormat.ARGB32, false);
            camera.targetTexture = imageTemp;
            camera.Render ();
            RenderTexture.active = imageTemp;
            image.ReadPixels (new Rect (0, 0, pixelSize, pixelSize), 0, 0);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary (imageTemp);
            File.WriteAllBytes (outputPath, image.EncodeToPNG ());
            GameObject.DestroyImmediate (image);
        }
    }
}