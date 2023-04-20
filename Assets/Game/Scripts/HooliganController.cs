using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    public class HooliganController : CharacterController
    {
        protected override void Start()
        {
            _characterType = CharacterType._Hooligan;
            base.Start();
        }

        public override void CharacterInit()
        {
            _characterType = CharacterType._Hooligan;
            _currentControlTheme = GSettings.RightPlayerTheme;

            base.CharacterInit();
        }
    }
}


