using Chsopoly.GameScene.Ingame.Event;
using UnityEngine;

namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObject : BaseObject<CharacterObject.State, CharacterObjectModel.Animation, CharacterObject>, IIngameLoadCompleteEvent
    {
        public enum State
        {
            None,
            Idle,
        }

        public override void Initialize (int id)
        {

        }

        void IIngameLoadCompleteEvent.OnIngameLoadComplete ()
        {
            var obj = GameObject.FindWithTag (IngameSettings.StartPointTag);
            if (obj != null)
            {
                transform.position = obj.transform.position;
            }
        }
    }
}