using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chsopoly.BaseSystem.UserData.Editor
{
    [CustomEditor (typeof (UserDataManager))]
    public class UserDataManagerCustomInspector : UnityEditor.Editor
    {
        [UnityEditor.MenuItem ("Project/User Data/Delete User Data")]
        private static void DeleteUserData ()
        {
            if (File.Exists (Application.persistentDataPath + "/d"))
            {
                File.Delete (Application.persistentDataPath + "/d");
            }
        }

        public override void OnInspectorGUI ()
        {
            base.DrawDefaultInspector ();

            if (GUILayout.Button ("Save"))
            {
                (target as UserDataManager).Save ();
            }
        }
    }
}