using System;

namespace Chronicle.Utility
{
    public delegate bool InitializerCallback();

    public sealed class InitializerAttribute : Attribute
    {
        public readonly byte Stage;

        public InitializerAttribute(byte pStage) { Stage = pStage; }
    }
}
