using System;
using System.Collections.Generic;

namespace Nemeio.Core.Models.SystemKeyboardCommand
{
    public class SystemKeyboardCommandHandler : ISystemKeyboardCommandHandler
    {
        private ISet<SystemKeyboardCommand> _commands;

        public SystemKeyboardCommandHandler()
        {
            _commands = new HashSet<SystemKeyboardCommand>();
        }

        public void RegisterCommand(SystemKeyboardCommand keyboardCommand)
        {
            if (keyboardCommand == null)
            {
                throw new ArgumentNullException(nameof(keyboardCommand));
            }

            var succeed = _commands.Add(keyboardCommand);

            if (!succeed)
            {
                throw new InvalidOperationException($"Can't add <{keyboardCommand}>");
            }
        }

        public bool Handle(IList<string> keys)
        {
            if (keys == null)
            {
                return false;
            }

            if (keys.Count == 0)
            {
                return false;
            }

            foreach (var command in _commands)
            {
                if (command.IsTriggered(keys))
                {
                    command.Execute();

                    return true;
                }
            }

            return false;
        }
    }
}
