using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keys
{
    public class KeystrokeInterpreter
    {
        public List<KeySubAction> GetActions(ILayout currentConfiguration, NemeioIndexKeystroke[] keystrokes)
        {
            if (currentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(currentConfiguration));
            }

            var keys = KeystrokeFromIndexs(currentConfiguration, keystrokes);

            var actions = ActionFromKeystroke(keys);

            return actions;
        }

        private IEnumerable<NemeioKeystroke> KeystrokeFromIndexs(ILayout currentCnfiguration, NemeioIndexKeystroke[] keystrokes)
            => keystrokes.Join(currentCnfiguration.Keys,
                keyStroke => keyStroke.Index,
                keyModel => keyModel.Index,
                (keyStroke, keyModel) => new NemeioKeystroke { IndexKeystroke = keyStroke, Key = keyModel });

        // TODO Très grande complexité cognitive et cyclomatique, à réécrire
        private List<KeySubAction> ActionFromKeystroke(IEnumerable<NemeioKeystroke> keystroke)
        {
            var result = new List<KeySubAction>();

            if (keystroke == null)
            {
                return result;
            }

            bool shiftIsPressed = false;
            bool altGrIsPressed = false;
            bool ctrlIsPressed = false;
            bool functionIsPressed = false;
            bool combinaisonFound = false;
            bool functionCombinaisonFound = false;

            foreach (var key in keystroke)
            {
                var modifier = KeyboardModifier.None;

                if (shiftIsPressed && !altGrIsPressed)
                {
                    modifier = KeyboardModifier.Shift;
                }
                else if (!shiftIsPressed && altGrIsPressed)
                {
                    modifier = KeyboardModifier.AltGr;
                }
                else if (shiftIsPressed && altGrIsPressed)
                {
                    modifier = KeyboardModifier.Shift | KeyboardModifier.AltGr;
                }
                else if (functionIsPressed)
                {
                    modifier = KeyboardModifier.Function;
                }

                var customActions = key.Key.Actions;

                if (customActions.Count > 0)
                {
                    var action = customActions.FirstOrDefault(x => x.Modifier == modifier);
                    if (action != null)
                    {
                        //Order CTRL Key first
                        List<KeySubAction> subactions = new List<KeySubAction>();
                        subactions.AddRange(action.Subactions.Where(x => x.IsCtrl()));
                        subactions.AddRange(action.Subactions.Where(x => !x.IsCtrl()));



                        if (subactions == null || subactions.Count <= 0)
                        {
                            modifier = KeyboardModifier.None;
                        }
                        else
                        {
                            combinaisonFound = modifier != KeyboardModifier.None;

                            foreach (var subaction in subactions)
                            {
                                if (functionIsPressed && functionCombinaisonFound)
                                {
                                    //  After a function key found
                                    //  we bypass every other keys

                                    break;
                                }

                                if (functionIsPressed)
                                {
                                    functionCombinaisonFound = true;
                                }

                                if (subaction.IsShift())
                                {
                                    shiftIsPressed = true;
                                }

                                if (subaction.IsAltGr())
                                {
                                    altGrIsPressed = true;
                                }

                                if (subaction.IsFunction())
                                {
                                    functionIsPressed = true;
                                }
                                if (subaction.IsCtrl())
                                {
                                    ctrlIsPressed = true;
                                    combinaisonFound = action.Subactions.Count > 1;
                                }

                                result.Add(subaction);
                            }
                        }
                    }
                }
            }

            if (combinaisonFound)
            {
                bool shiftDeleted = false;
                bool altGrDeleted = false;

                foreach (var action in result.ToList())
                {
                    var isShiftAndNotDeleted = action.IsShift() && !shiftDeleted;
                    var isAltGrAndNotDeleted = action.IsAltGr() && !altGrDeleted;

                    if (isShiftAndNotDeleted || isAltGrAndNotDeleted)
                    {
                        shiftDeleted = action.IsShift();
                        altGrDeleted = action.IsAltGr();

                        result.Remove(action);
                    }
                }
            }

            if (result.Count == 0)
            {
                if (shiftIsPressed)
                {
                    result.Add(KeySubAction.CreateModifierAction(KeyboardLiterals.Shift));
                }

                if (altGrIsPressed)
                {
                    result.Add(KeySubAction.CreateModifierAction(KeyboardLiterals.AltGr));
                }
                if (ctrlIsPressed)
                {
                    result.Add(KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl));
                }
            }
            else
            {
                //  Clean "Function" keys
                //  Function key is never sent to OS

                result.RemoveAll(x => x.IsFunction());
            }

            return result;
        }
    }
}
