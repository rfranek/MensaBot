using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Collections.Generic;

    using MensaBot.Resources.Language;

    public class CommandAlternatives
    {
        #region member vars

        private readonly Dictionary<string, string[]> _dictionary;
        private List<string> _commands;

        #endregion

        #region constructors and destructors

        public CommandAlternatives()
        {
            _dictionary = new Dictionary<string, string[]>();
            _commands = new List<string>();
        }

        public bool ContainsCommand(string possibleCommand)
        {
            string [] commands = new string[_commands.Count];

            for (int c = 0; c < commands.Length; c++)
            {
                commands[c] = Lang.ResourceManager.GetString(_commands[c]);
            }

            return Array.IndexOf(commands, possibleCommand) >= 0;

        }

        #endregion

        #region methods

        public void AddCommands(string[] alternatives)
        {
            _commands.AddRange(alternatives);
        }


        public string[] listCommands()
        {
            return _commands.ToArray();
        }

        public int IndexOf (string value)
        {
            string[] commands = new string[_commands.Count];

            for (int c = 0; c < commands.Length; c++)
            {
                commands[c] = Lang.ResourceManager.GetString(_commands[c]);
            }

            return Array.IndexOf(commands, value);

        }

        public void ReplaceCommands(string[] alternatives)
        {
            _commands = new List<string>();
            _commands.AddRange(alternatives);
        }

        #endregion
    }


}