namespace Chronicle.Utility
{
    public sealed class Doublet<T1, T2>
    {
        public T1 First;
        public T2 Second;

        public Doublet(T1 pFirst, T2 pSecond)
        {
            First = pFirst;
            Second = pSecond;
        }
    }
}
