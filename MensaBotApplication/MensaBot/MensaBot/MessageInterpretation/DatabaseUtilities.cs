using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MensaBot.MessageInterpretation
{
    public class DatabaseUtilities
    {

        public const string DefaultMensaTag = "DefaultCanteen";
        public const string IgnoreTags = "IgnoreTags";

        public static bool RemoveKey(MensaBotEntities mensaBotEntities, string key, string channelId, string coversationId)
        {
            try
            {
                var chat = mensaBotEntities.Chats.FirstOrDefault(t => t.ConversationId == coversationId && t.ChannelId == channelId);

                if (chat == null || chat.Settings == null)
                    return false;

                if (chat.Settings.Any(s => s.Key == key))
                {
                    mensaBotEntities.Settings.Remove(chat.Settings.Single(s => s.Key == key));
                    mensaBotEntities.SaveChanges();

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }

        public static bool AddEntry(string key, string value, MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return false;

                var chat = mensaBotEntities.Chats.Single(t => t.ConversationId == conversationId && t.ChannelId == channelId);

                if (chat.Settings.Any(s => s.Key == key))
                {
                    chat.Settings.Single(s => s.Key == key).Value = value;
                    mensaBotEntities.SaveChanges();

                    return true;
                }
                else
                {
                    chat.Settings.Add(
                        new Setting
                        {
                            Key = key,
                            Value = value
                        });
                    mensaBotEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public static string GetValueBytKey(MensaBotEntities mensaBotEntities, string key, string channelId, string coversationId)
        {
            try
            {
                var chat = mensaBotEntities.Chats.FirstOrDefault(t => t.ConversationId == coversationId && t.ChannelId == channelId);

                if (chat == null || chat.Settings == null)
                    return null;

                if (chat.Settings.Any(s => s.Key == key))
                {

                    return chat.Settings.Single(s => s.Key == key).Value;

                }
            }
            catch (Exception e)
            {
                return null;
            }

            return null;

        }

        public static bool RemoveAllSettingsAndChat(MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            if ((mensaBotEntities.Chats.Any(t => t.ConversationId == conversationId && t.ChannelId == channelId)))
            {
                var chat = mensaBotEntities.Chats.FirstOrDefault(t => t.ConversationId == conversationId && t.ChannelId == channelId);

                if (chat.Settings != null)
                {
                    mensaBotEntities.Settings.RemoveRange((chat.Settings));
                    mensaBotEntities.SaveChanges();
                }

                mensaBotEntities.Chats.Remove(chat);
                mensaBotEntities.SaveChanges();
                return true;

            }
            return false;
        }

        public static bool CreateChatEntry(MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            if (!(mensaBotEntities.Chats.Any(t => t.ConversationId == conversationId && t.ChannelId == channelId)))
            {
                var tmp = mensaBotEntities.Chats.Add(
                    new Chat
                    {
                        ChannelId = channelId,
                        ConversationId = conversationId,
                        Settings = new List<Setting> { }
            });

                mensaBotEntities.SaveChanges();
                return true;
            }
            return false;
        }

        /*
         * new Setting
                                                {
                                                    Key = "Language",
                                                    Value = LanguageKey.EN.ToString()
                                                }
         * 
         * 
         * 
         */
    }
}
 