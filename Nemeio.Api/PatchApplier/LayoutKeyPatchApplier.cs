using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Exceptions;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;

namespace Nemeio.Api.PatchApplier
{
    public enum LayoutKeyError
    {
        SelectedFileNotExecutable = 1,
        InvalidShortcut = 2,
        InvalidPath = 3,
        InvalidUrl = 4,
        InvalidFont = 5,
        InvalidModifier = 6,
        MissingSubaction = 7,
    }

    public class LayoutKeyPatchApplier : BasePatchApplier<PutLayoutKeyInDto, Key>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFontProvider _fontProvider;
        private readonly ILayoutImageGenerator _layoutGenService;
        private readonly KeyboardMap _keyboardMap;

        public LayoutKeyPatchApplier(ILoggerFactory loggerFactory, IFontProvider fontProvider, ILayoutImageGenerator layoutGenService, KeyboardMap keyboardMap)
            : base(loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            _layoutGenService = layoutGenService ?? throw new ArgumentNullException(nameof(layoutGenService));
            _keyboardMap = keyboardMap ?? throw new ArgumentNullException(nameof(keyboardMap));
        }

        public override Key Patch(PutLayoutKeyInDto input, Key currentValue)
        {
            if (input.Actions == null)
            {
                return currentValue;
            }

            _transaction.Run(ref currentValue, () =>
            {
                currentValue.Edited = true;
                currentValue.Disposition = input.Disposition;

                if (input.Font != null)
                {
                    var newFont = input.Font;
                    if (newFont != null)
                    {
                        if (!_fontProvider.FontExists(newFont.Name))
                        {
                            throw new PatchFailedException((int)LayoutKeyError.InvalidFont, $"Font <{newFont.Name}> doesn't exists");
                        }

                        currentValue.Font = newFont.ToDomainModel();
                    }
                    else
                    {
                        //  If "null" is specified, we want to reset value

                        currentValue.Font = null;
                    }
                }

                var isModifierKey = _keyboardMap.Buttons[currentValue.Index].IsModifier;

                foreach (var inDtoAction in input.Actions)
                {
                    var currentAction = currentValue.Actions.FirstOrDefault(x => x.Modifier == inDtoAction.Modifier);
                    if (currentAction == null)
                    {
                        var newAction = CreateAction(inDtoAction, isModifierKey);

                        currentValue.Actions.Add(newAction);
                    }
                    else
                    {
                        foreach (PutLayoutSubactionInDto sub in inDtoAction.Subactions)
                        {
                            if (sub.Type == KeyActionType.Application)
                            {
                                sub.Data = sub.Data.Replace(">>", @":\").Replace(">", @"\").Replace(".zip", string.Empty);
                            }
                        }
                        currentAction = UpdateAction(inDtoAction, currentAction, isModifierKey);
                    }
                }

                //  Remove not specified actions
                var modifiers = input.Actions.Select(x => x.Modifier);
                currentValue.Actions = currentValue.Actions.Where(x => modifiers.Contains(x.Modifier)).ToList();
            });

            return currentValue;
        }



        private KeyAction CreateAction(PutLayoutKeyActionInDto input, bool isModifierKey)
        {
            var newAction = new KeyAction();
            newAction.Display = input.Display ?? string.Empty;
            newAction.Modifier = input.Modifier;
            newAction.IsGrey = input.IsGrey;
            ComputeSubactionIfNeeded(input, newAction, isModifierKey);
            return newAction;
        }

        private KeyAction UpdateAction(PutLayoutKeyActionInDto input, KeyAction action, bool isModifierKey)
        {
            if (input.Display != null)
            {
                action.Display = input.Display;
            }
            action.IsGrey = input.IsGrey;
            ComputeSubactionIfNeeded(input, action, isModifierKey);
            return action;
        }

        private void ComputeSubactionIfNeeded(PutLayoutKeyActionInDto input, KeyAction keyAction, bool isModifierKey)
        {
            if (!isModifierKey)
            {
                AssertSubactionsIsNotPresent(input);
                AssertSubactionsDataIfInvalidFormat(input);

                keyAction.Subactions = input.Subactions
                                        .Select((subaction) => subaction.ToDomainModel())
                                        .ToList();
            }
        }

        #region Assert methods

        private void AssertSubactionsIsNotPresent(PutLayoutKeyActionInDto input)
        {
            if (input.Subactions == null)
            {
                throw new PatchFailedException((int)LayoutKeyError.MissingSubaction, $"Missing subaction");
            }
        }

        private void AssertSubactionsDataIfInvalidFormat(PutLayoutKeyActionInDto input)
        {
            //  Check all requirement for incoming data
            foreach (var subaction in input.Subactions)
            {
                switch (subaction.Type)
                {
                    case KeyActionType.Url:
                        AssertSubactionUrlIfInvalid(subaction);
                        break;
                    case KeyActionType.Application:
                        AssertApplicationPathIfInvalid(subaction);
                        break;
                }
            }
        }

        private void AssertSubactionUrlIfInvalid(PutLayoutSubactionInDto subactionInDto)
        {
            try
            {
                var url = subactionInDto.Data;
                var isValid = UrlHelper.IsValidHttpUrl(url);
                if (!isValid)
                {
                    throw new PatchFailedException((int)LayoutKeyError.InvalidUrl, $"<{url}> is invalid");
                }
            }
            catch (InvalidOperationException exception)
            {
                throw new PatchFailedException((int)LayoutKeyError.InvalidUrl, exception.Message);
            }
        }

        private void AssertApplicationPathIfInvalid(PutLayoutSubactionInDto subactionInDto)
        {
            //  Check file is application
            var filePath = subactionInDto.Data;

            try
            {
                var isValid = FileHelpers.IsValidExecutableExtension(filePath);
                if (!isValid)
                {
                    throw new PatchFailedException((int)LayoutKeyError.SelectedFileNotExecutable, $"<{filePath}> isn't an executable");
                }
            }
            catch (InvalidOperationException exception)
            {
                throw new PatchFailedException((int)LayoutKeyError.SelectedFileNotExecutable, exception.Message);
            }

            //  Path exists
            var exists = File.Exists(filePath);
            if (!exists)
            {
                throw new PatchFailedException((int)LayoutKeyError.InvalidPath, $"<{filePath}> doesn't exists on this computer");
            }
        }

        #endregion
    }
}
