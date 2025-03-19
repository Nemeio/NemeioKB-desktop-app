using System;

namespace Nemeio.Api.JsonConverters
{
    internal interface IGenericAccess
    {
        bool IsValuePresent { get; }
        Type GenericType { get; }
        object Value { set; get; }
    }
}
