
namespace Bloodymary.Game
{
    public class CharacterSystem : ButtonActionSystem
    {
        private CharacterController character;

        protected override void Start()
        {
            character = GetComponent<CharacterController>();
        }

        protected override void ActionMethod(bool intersect)
        {
            character.Death();
        }
    }
}


