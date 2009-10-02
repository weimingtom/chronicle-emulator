namespace Chronicle.Utility
{
    public sealed class Triplet<T1, T2, T3>
    {
        public T1 First;
        public T2 Second;
        public T3 Third;

        public Triplet(T1 pFirst, T2 pSecond, T3 pThird)
        {
            First = pFirst;
            Second = pSecond;
            Third = pThird;
        }
    }
}
