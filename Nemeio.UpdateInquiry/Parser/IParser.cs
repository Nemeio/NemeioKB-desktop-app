namespace Nemeio.UpdateInquiry.Parser
{
    public interface IParser<T>
    {
        T Parse(string data);
        string Parse(T data);
    }
}
