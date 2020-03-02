namespace Chsopoly.GameScene.Ingame.Object.Character
{
    public class CharacterObjectModel : BaseObjectModel<CharacterObjectModel.Animation>
    {
        public enum Animation
        {
            Idle,
            Run,
            Jump,
            Appeal,
            Dying,
            Guard,
        }
    }
}