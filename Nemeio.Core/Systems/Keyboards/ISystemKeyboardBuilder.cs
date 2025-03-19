using System;
using System.IO;
using Nemeio.Core.Enums;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Systems.Keyboards
{
    public interface ISystemKeyboardBuilder
    {
        string GetKeyValue(uint sc, KeyboardModifier modifier, OsLayoutId layout);

        void ForEachModifiers(Action<KeyboardModifier> action);

        Stream LoadEmbeddedResource(string name);

        KeyDisposition GetKeyDisposition(uint scanCode, bool isRequiredKey, bool isFunctionKey);
    }
}
