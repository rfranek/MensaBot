using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Collections.Generic;

    public class CommandAlternatives
    {
        #region member vars

        private readonly Dictionary<string, string[]> _dictionary;

        #endregion

        #region constructors and destructors

        public CommandAlternatives()
        {
            _dictionary = new Dictionary<string, string[]>();
        }

        #endregion

        #region methods

        public void AddCommands(LanguageKey key, string[] alternatives)
        {
            string keys = key.ToString();
            string[] oldCommands;
            _dictionary.TryGetValue(keys, out oldCommands);

            if (oldCommands == null)
                oldCommands = new string[0];

            string[] newCommands = new string[oldCommands.Length + alternatives.Length];

            for (int i = 0; i < oldCommands.Length; i++)
            {
                newCommands[i] = oldCommands[i];
            }
            for (int i = 0; i < alternatives.Length; i++)
            {
                newCommands[oldCommands.Length + i] = alternatives[i];
            }

            _dictionary.Remove(keys);
            _dictionary.Add(keys, newCommands);
        }

        public bool Contains(string value, LanguageKey key)
        {
            string[] values;

            try
            {
                _dictionary.TryGetValue(key.ToString(), out values);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                return false;
            }

            return values != null && values.Contains(value);
        }

        public string[] listCommands(LanguageKey key)
        {
            string[] commands = null;
            try
            {
                _dictionary.TryGetValue(key.ToString(), out commands);
            }catch(Exception e) { }

            return commands;
        }

        public int IndexOf (string value, LanguageKey key)
        {
            string[] values;

            try
            {
                _dictionary.TryGetValue(key.ToString(), out values);
                if (values == null)
                    return -1;

                var index = values.ToList().IndexOf(value);
                if (index == null)
                    return -1;
                else
                    return index;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e);
                return -1;
            }

        }

        public void ReplaceCommands(LanguageKey key, string[] alternatives)
        {
            _dictionary.Add(key.ToString(), alternatives);
        }

        #endregion
    }

    public enum LanguageKey
    {
        DE,
        EN,

        none
    }
}