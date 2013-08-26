using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using BuddhaBot.ChatBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BuddhaBot
{
    /// <summary>
    /// Manages the settings of the chatbot
    /// </summary>
    class Settings
    {
        private const string SettingsFile = "settings.json";

        #region Variables
        #region Private Variables

        private string _channel, _botUsername, _botPassword, _pwEntropy;
        private List<string> _permaMods = new List<string>();
        #endregion

        #region Getters and Setters
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public string BotUsername
        {
            get { return _botUsername; }
            set { _botUsername = value; }
        }

        public string BotPassword
        {
            get { return _botPassword; }
            set { _botPassword = EncryptPassword(value, PwEntropy); }
        }

        public string PwEntropy
        {
            get { return _pwEntropy; }
            set { _pwEntropy = value; }
        }

        public string PlaintextPassword
        {
            get { return DecryptPassword(BotPassword, PwEntropy); }
        }

        public List<string> PermaMods
        {
            get { return _permaMods; }
            set { _permaMods = value; }
        }
        #endregion
        #endregion

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <param name="bot">The chat bot to be configured</param>
        public void LoadSettings(ChatBot.ChatBot bot)
        {
            //Check that the settings file exists
            if (!File.Exists(SettingsFile))
            {
                //Notify user of the issue
                MessageBox.Show(string.Format("Unable to find settings file ({0}). Exiting.", SettingsFile), "BuddhaBot Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("Unable to find settings file");
            }

            //Load file into string
            var settingsContent = File.ReadAllText(SettingsFile);

            //Load string into json format
            dynamic settingsJson = JObject.Parse(settingsContent);

            //Set up variables
            _channel = settingsJson.channel.ToString();
            _botUsername = settingsJson.botusername.ToString();
            _botPassword = settingsJson.botpassword.ToString();
            _pwEntropy = settingsJson.pwentropy.ToString();

            //First time running the program
            if (string.IsNullOrEmpty(_channel) || string.IsNullOrEmpty(_botUsername) || string.IsNullOrEmpty(_botPassword) || string.IsNullOrEmpty(_pwEntropy))
            {
                //Configure new settings
                NewSettings(true, bot);
            }
            else
            {
                try
                {
                    //Test Decrypt base-64 strings
                    DecryptPassword(_botPassword, _pwEntropy);
                }
                catch (Exception) //Catch any exceptions - this means there was an error loading the password
                {
                    NewSettings(true, bot);
                }
            }

            //Load the censors
            foreach (var censor in settingsJson.censored)
            {
                var action = ToCensorAction(censor.action.ToString());

                //If the action was successfully loaded
                if (action != CommandResponse.ResponseAction.None)
                {
                    var message = censor.response;
                    //There is no custom message
                    Commands.Censored.Add(censor.search.ToString(),
                                  message == null
                                      ? new CommandResponse(action)
                                      : new CommandResponse(message.ToString(), action));
                }
            }

            //Load the commands
            foreach (var command in settingsJson.commands)
            {
                //Load the variables
                var name = command.name.ToString();
                var response = command.response.ToString();

                Commands.TextCommands.Add(name.ToUpper(), response);
            }

            //Load the perma-mods
            foreach (var moderator in settingsJson.permamods)
            {
                //Add to the list
                _permaMods.Add(moderator.ToString().ToUpper());
            }
        }

        /// <summary>
        /// Writes the current settings to the file
        /// </summary>
        public void WriteSettings()
        {
            //Creates connection to the settings file
            using (var fs = File.Open(SettingsFile, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw))
            {
                //Set up json file formatting
                jw.Formatting = Formatting.Indented;

                //Start the base object
                jw.WriteStartObject();

                jw.WritePropertyName("channel");
                jw.WriteValue(_channel);

                jw.WritePropertyName("botusername");
                jw.WriteValue(_botUsername);

                jw.WritePropertyName("botpassword");
                jw.WriteValue(_botPassword);

                jw.WritePropertyName("pwentropy");
                jw.WriteValue(_pwEntropy);

                //Start writing commands
                jw.WritePropertyName("commands");
                jw.WriteStartArray();

                foreach (var command in Commands.TextCommands)
                {
                    jw.WriteStartObject();
                    jw.WritePropertyName("name");
                    jw.WriteValue(command.Key);

                    jw.WritePropertyName("response");
                    jw.WriteValue(command.Value);
                    jw.WriteEndObject();
                }
                //Finish writing commands
                jw.WriteEnd();

                //Start writing censors
                jw.WritePropertyName("censored");
                jw.WriteStartArray();

                foreach (var command in Commands.Censored)
                {
                    jw.WriteStartObject();
                    jw.WritePropertyName("search");
                    jw.WriteValue(command.Key);

                    jw.WritePropertyName("action");
                    jw.WriteValue(ActionToString(command.Value.Action));

                    jw.WritePropertyName("response");
                    jw.WriteValue(command.Value.Message);
                    jw.WriteEndObject();
                }

                //Finish writing censors
                jw.WriteEnd();

                //Start writing the perma-mods
                jw.WritePropertyName("permamods");
                jw.WriteStartArray();

                foreach (var permamod in _permaMods)
                {
                    jw.WriteValue(permamod);
                }

                //Finish writing perma-mods
                jw.WriteEnd();

                //End base object
                jw.WriteEndObject();
            }
        }

        /// <summary>
        /// Encrypts a password with a specified byte entropy
        /// </summary>
        /// <param name="password">The password in plaintext</param>
        /// <param name="entropy">The entropy as a base-64 text</param>
        /// <returns>The encrypted password as base-64 text</returns>
        private string EncryptPassword(string password, string entropy)
        {
            var entropyArray = Convert.FromBase64String(entropy);
            return
                Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(password), entropyArray,
                                                             DataProtectionScope.LocalMachine));
        }

        /// <summary>
        /// Decrypts a base-64 text password with a specified byte entropy
        /// </summary>
        /// <param name="password">The password in base-64 text</param>
        /// <param name="entropy">The entropy as a base-64 text</param>
        /// <returns>The decrypted password in plaintext</returns>
        private string DecryptPassword(string password, string entropy)
        {
            var entropyArray = Convert.FromBase64String(entropy);
            return
                Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(password), entropyArray,
                                                                DataProtectionScope.LocalMachine));
        }

        /// <summary>
        /// Sets up a new configuration dialogue and configures the new bot
        /// </summary>
        /// <param name="firstTime">Indicating the first configuration</param>
        /// <param name="bot">The chat bot to be configured</param>
        public void NewSettings(bool firstTime, ChatBot.ChatBot bot)
        {
            //Configure new settings
            var configForm = new Configure();
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                //Generate new password entropy and hash
                _pwEntropy = NewPasswordEntropy();

                //Load the info
                _channel = configForm.ChannelBox.Text;
                _botUsername = configForm.UsernameBox.Text;
                _botPassword = EncryptPassword(configForm.PasswordBox.Text, _pwEntropy);

                if (firstTime)
                    bot = new ChatBot.ChatBot(_channel);
                else
                    bot.Channel = _channel;
                bot.SetBotAccountInfo(_botUsername, PlaintextPassword);

                WriteSettings();
            }
            else
            {
                if (firstTime)
                {
                    MessageBox.Show("Unable to configure the settings. Exiting", "BuddhaBot Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    throw new Exception("Cancelled configure dialogue");
                }
            }
        }

        /// <summary>
        /// Creates a new entropy byte array for password hashing
        /// </summary>
        /// <returns>The base-64 text containing the entropy</returns>
        private string NewPasswordEntropy()
        {
            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            return Convert.ToBase64String(entropy);
        }

        /// <summary>
        /// Converts an action string from the settings file into an action
        /// </summary>
        /// <param name="option">The string found in the settings file</param>
        /// <returns>The necessary action (null if not found)</returns>
        public static CommandResponse.ResponseAction ToCensorAction(string option)
        {
            switch (option.ToUpper())
            {
                case ("BAN"):
                    return CommandResponse.ResponseAction.Ban;
                case ("TIMEOUT"):
                    return CommandResponse.ResponseAction.Timeout;
                case ("PURGE"):
                    return CommandResponse.ResponseAction.Purge;
                case ("WARNING"):
                    return CommandResponse.ResponseAction.Warning;
                default:
                    return CommandResponse.ResponseAction.None;
            }
        }

        /// <summary>
        /// Converts an action to a string for the settings file
        /// </summary>
        /// <param name="action">The action for the settings string</param>
        /// <returns>The specific action</returns>
        public static string ActionToString(CommandResponse.ResponseAction action)
        {
            var message = "";
            switch (action)
            {
                case (CommandResponse.ResponseAction.None):
                    message = "none";
                    break;
                case (CommandResponse.ResponseAction.Ban):
                    message = "ban";
                    break;
                case (CommandResponse.ResponseAction.Purge):
                    message = "purge";
                    break;
                case (CommandResponse.ResponseAction.Timeout):
                    message = "timeout";
                    break;
                case (CommandResponse.ResponseAction.Warning):
                    message = "warning";
                    break;
            }
            return message;
        }
    }
}
