using Fusion;
using UnityEngine;

namespace CoreClash.Lesson02
{
    public enum Lesson02Button
    {
        Attack,
        Fire,
        Reload
    }

    public struct Lesson02InputData : INetworkInput
    {
        public Vector2 Move;
        public NetworkButtons Buttons;
    }
}
