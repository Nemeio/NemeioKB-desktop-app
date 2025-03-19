namespace Nemeio.Tools.Testing.Update.Core.Update.Controller
{
    public class TestStartedEventArgs
    {
        public int TestCount { get; private set; }

        public TestStartedEventArgs(int count)
        {
            TestCount = count;
        }
    }
}
