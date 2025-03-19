using System.IO;

namespace Nemeio.Core.Images
{
    public interface IBinaryConvertible
    {
        int ComputeSize();
        void Convert(BinaryWriter writer);
    }
}
