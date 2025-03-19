using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Nemeios.Runner
{
    public partial class NemeioRunner
    {
        /// <summary>
        /// Allow to add layout to keyboard
        /// </summary>
        /// <exception cref="AddConfigurationFailedException">Throw if can't add layout</exception>
        public async Task AddLayoutAsync(ILayout layout)
        {
            if (State == NemeioState.FactoryReset)
            {
                //  Can't add any layout on FactoryReset mode

                return;
            }

            if (layout == null)
            {
                return;
            }

            await Task.Yield();

            try
            {
                _addConfigurationMonitor.SendConfiguration(layout);

                LayoutIdWithHashs.Add(new LayoutIdWithHash(layout.LayoutId, layout.Hash));

                _logger.LogTrace($"Nemeio <{Identifier}> has added layout <{layout.Title}> with id <{layout.LayoutId}> and hash <{layout.Hash}>");
            }
            catch (KeyboardException exception)
            {
                var message = $"Failed to add configuration <{layout.Title}> with id <{layout.LayoutId}> and hash <{layout.Hash}> on keyboard <{Identifier}>";

                _logger.LogError(exception, message);

                throw new AddConfigurationFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to add configuration <{layout.Title}> with id <{layout.LayoutId}> and hash <{layout.Hash}> on keyboard <{Identifier}> because of communication error";

                _logger.LogError(communicationException, message);

                throw new AddConfigurationFailedException(message, communicationException);
            }
        }

        /// <summary>
        /// Allow to delete layout to keyboard
        /// </summary>
        /// <exception cref="DeleteConfigurationFailedException">Throw if can't delete layout</exception>
        public async Task DeleteLayoutAsync(LayoutId layoutId)
        {
            if (State == NemeioState.FactoryReset)
            {
                //  Can't remove any layout on FactoryReset mode

                return;
            }

            if (layoutId == null)
            {
                return;
            }

            await Task.Yield();

            try
            {
                _deleteConfigurationMonitor.Delete(layoutId);
                var target = LayoutIdWithHashs.FirstOrDefault(x => x.Id == layoutId);
                if (target != null)
                {
                    LayoutIdWithHashs.Remove(target);
                }

                _logger.LogTrace($"Nemeio <{Identifier}> has removed layout with id <{layoutId}>");
            }
            catch (KeyboardException exception)
            {
                //  Keyboard sent error code

                if (exception.ErrorCode == KeyboardErrorCode.ProtectedConfiguration)
                {
                    //  If try to delete protected configuration
                    //  we swallow exception.

                    _logger.LogTrace($"Try to remove protected layout on keyboard with id <{layoutId}>");
                }
                else if (exception.ErrorCode == KeyboardErrorCode.NotFound)
                {
                    //  We try to delete a configuration which not exists on keyboard
                    //  This error can't happend in theory except on develop side

                    _logger.LogWarning($"Layout with id <{layoutId}> not found on keyboard, can't delete it");
                }
                else
                {
                    var message = $"Failed to remove configuration with id <{layoutId}> on keyboard <{Identifier}>";

                    _logger.LogError(exception, message);

                    throw new DeleteConfigurationFailedException(message, exception);
                }
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to remove configuration with id <{layoutId}> on keyboard <{Identifier}> because of communication error";

                _logger.LogError(communicationException, message);

                throw new DeleteConfigurationFailedException(message, communicationException);
            }
        }

        /// <summary>
        /// Allow to apply layout to keyboard
        /// </summary>
        /// <exception cref="ApplyConfigurationFailedException">Throw if can't apply layout</exception>
        public async Task ApplyLayoutAsync(ILayout layout)
        {
            if (State == NemeioState.FactoryReset)
            {
                //  Can't apply any layout on FactoryReset mode

                return;
            }

            if (layout == null)
            {
                return;
            }

            await Task.Yield();

            try
            {
                _logger.LogTrace($"Apply layout <{layout.Title}> with id <{layout.LayoutId}> and hash <{layout.Hash}>");

                _applyConfigurationMonitor.Apply(layout.LayoutId);
            }
            catch (KeyboardException exception)
            {
                //  Keyboard sent error code

                var message = $"Fail apply layout with name <{layout?.Title}> and id <{layout?.LayoutId}> and hash <{layout?.Hash}>";

                _logger.LogError(exception, message);

                throw new ApplyConfigurationFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to apply layout with name <{layout?.Title}> and id <{layout?.LayoutId}> and hash <{layout?.Hash}> because of communication error";

                _logger.LogError(communicationException, message);

                throw new ApplyConfigurationFailedException(message, communicationException);
            }
        }
    }
}
