
using static GameManager;

public class HooliganController : CharacterController
{
    public override void CharacterInit()
    {
        _characterType = CharacterType._Hooligan;
        SetAI(this._characterType);
    }
}
