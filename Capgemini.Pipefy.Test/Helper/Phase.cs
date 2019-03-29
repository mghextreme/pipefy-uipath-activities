namespace Capgemini.Pipefy.Test.Helper
{
    internal class Phase
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public bool Done { get; protected set; }
        public Pipe Pipe { get; protected set; }

        internal void SetParentPipe(Pipe pipe)
        {
            Pipe = pipe;
        }
    }
}