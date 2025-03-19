namespace Nemeio.Core.Systems.Hid
{
    public class SystemHidKey
    {
        public string Data { get; set; }
        public bool Repeat { get; set; }

        public SystemHidKey(string data, bool repeat = false)
        {
            Data = data;
            Repeat = repeat;
        }
    }
}
