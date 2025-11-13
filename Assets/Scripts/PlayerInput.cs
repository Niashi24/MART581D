using UnityEngine;
using UnityEngine.Serialization;

namespace Mart581d
{
    [System.Serializable]
    public struct PlayerInput
    {
        public Button jump;
        public Button bark;
        public Vector2 move;
        public Vector2 aim;

        public void Update(bool jump, bool bark, Vector2 move, Vector2 aim)
        {
            this.jump.Update(jump);
            this.bark.Update(bark);
            this.move = move;
            this.aim = aim;
        }

        public PlayerInput Take()
        {
            return new PlayerInput()
            {
                jump = jump.Take(),
                bark = bark.Take(),
                move = move,
                aim = aim,
            };
        }
    }
}