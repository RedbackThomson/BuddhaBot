using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BuddhaBot.ChatBot
{
    /// <summary>
    /// Class necessary for sorting through messages 
    /// and acting on the commands
    /// </summary>
    class Commands
    {
        //Declare Variables
        #region Command Variables

        public static Dictionary<string, string> TextCommands = new Dictionary<string, string>();

        private const string CommandChar = "!";

        private const string CensorCommand = "censor";

        private const string CommandCommand = "command";
        private const string CommandEdit = "edit";
        private const string CommandAdd = "add";
        private const string CommandRemove = "delete";

        private const string CommandAdded = "Command added [{0}]";
        private const string CommandExists = "Command already exists [{0}]";
        private const string CommandRemoved = "Command deleted [{0}]";
        private const string CommandEdited = "Command edited [{0}]";
        #endregion

        #region Censored Variables
        public static Dictionary<string, CommandResponse> Censored = new Dictionary<string,CommandResponse>();

        private static readonly Dictionary<string, int> Warned = new Dictionary<string, int>();
        private const string FirstWarning = "You have been warned, {0}";
        private const string LastWarning = "You ignored the first warning; last one, {0}.";
        private const string BanWarning = "You ignored both warnings, {0}. {{banned}}";

        private static string[] _banMessages;
        private static string[] _purgeMessages;
        private static string[] _timeoutMessages;

        private static readonly Random Randomizer = new Random();
        private static bool _requiresMod = false;
        private static DateTime _lastCensorMessage = DateTime.Now;

        private const int _timeoutDelay = 30;
        #endregion

        public static bool RequiresMod { get { return _requiresMod; } }

        /// <summary>
        /// Adds all ban, timeout and purge messages to the arrays
        /// </summary>
        public static void SetUpMessages()
        {
            _banMessages = new []{"Consider carefully the ethics of your actions, {0}. {{banned:spam}}", 
                "The path to enlightenment leads through silence, {0}. {{banned:spam}}", 
                "Reflect on your words, {0}. {{banned:spam}}", 
                "Mind wipe successful, {0}. {{banned:spam}}"};

            _purgeMessages = new[] { "Please don't say that, {0}. {{purged}}" };

            _timeoutMessages = new[] {"That's a paddlin'. {{timeout}} [{0}]"};
        }

        /// <summary>
        /// Reads the message and commands the bot if necessary
        /// </summary>
        /// <param name="user">The user's name</param>
        /// <param name="message">The sent message</param>
        /// <param name="isMod">Is the user currently a channel op</param>
        public static CommandResponse CommandResponder(string user, string message, bool isMod)
        {
            //Check for censored phrases
            var censorResponse = CensorCheck(user, message, isMod);
            if(censorResponse != null)
            {
                //XX Second Censor Delay
                if (DateTime.Now.Subtract(_lastCensorMessage).TotalSeconds >= _timeoutDelay)
                {
                    _lastCensorMessage = DateTime.Now;
                    return censorResponse;
                }

                //Else Don't send a message
                censorResponse.Message = string.Empty;
                return censorResponse;
            }

            //Only mods can use the commands
            if(isMod)
            {
                //Make sure it starts with the command character
                if (message.StartsWith(CommandChar))
                {
                    var commandResponse = CommandCheck(user, message);
                    if (commandResponse != null) return commandResponse;
                }
            }

            //No commands/censored words were detected
            return null;
        }

        private static CommandResponse CommandCheck(string user, string message)
        {
            //Remove the command character and trim it
            var cleanMessage = message.Remove(0, 1).Trim();
            var cleanSplit = cleanMessage.Split(' ');

            #region Command Commands
            //Check through the static commands
            if (cleanSplit[0] == CommandCommand && cleanSplit.Length >= 3)
            {
                //Check for add
                if(cleanSplit[1] == CommandAdd && cleanSplit.Length >= 4)
                {
                    //Take a new split to allow for spaces in response
                    var newSplit = cleanMessage.Split(new []{' '}, 4);
                    var name = newSplit[2];
                    var responseMessage = newSplit[3];

                    //Determine if it already exists
                    if (!TextCommands.ContainsKey(name.ToUpper()))
                    {
                        //Add the new message to the command list
                        TextCommands.Add(name.ToUpper(), responseMessage);
                    }
                    else
                    {
                        return new CommandResponse(string.Format(CommandExists, name),
                                                   CommandResponse.ResponseAction.None);
                    }

                    //Respond to the user
                    return new CommandResponse(string.Format(CommandAdded, name), CommandResponse.ResponseAction.None);
                }
                //Check for deleting
                if(cleanSplit[1] == CommandRemove && cleanSplit.Length == 3)
                {
                    var name = cleanSplit[2];

                    //Removes appropriate key
                    if (TextCommands.ContainsKey(name.ToUpper())) TextCommands.Remove(name.ToUpper());

                    //Respond to the user
                    return new CommandResponse(string.Format(CommandRemoved, name), CommandResponse.ResponseAction.None);
                }
                //Check for editing
                if(cleanSplit[1] == CommandEdit && cleanSplit.Length >= 4)
                {
                    //Take a new split to allow for spaces in response
                    var newSplit = cleanMessage.Split(new[] {' '}, 4);
                    var name = newSplit[2];
                    var responseMessage = newSplit[3];

                    //Check to see if the command already exists
                    if(TextCommands.ContainsKey(name.ToUpper()))
                    {
                        //Edit appropriately
                        TextCommands[name.ToUpper()] = responseMessage;

                        //Respond to the user
                        return new CommandResponse(string.Format(CommandEdited, name),
                                                   CommandResponse.ResponseAction.None);
                    }

                    //Otherwise add it
                    TextCommands.Add(name.ToUpper(), responseMessage);

                    //Respond to the user
                    return new CommandResponse(string.Format(CommandAdded, name), CommandResponse.ResponseAction.None);
                }
            }
            #endregion

            #region Censor Commands
            //Check to see if censoring command
            if (cleanSplit[0] == CensorCommand && cleanSplit.Length >= 3)
            {
                //Detect censor add or delete
                if (cleanSplit[1].ToUpper() == CommandAdd.ToUpper() && cleanSplit.Length >= 4)
                {
                    //Do a new split to allow for spaces in censor message
                    var newSplit = cleanMessage.Split(new[] { ' ' }, 4);
                    var censorMessage = newSplit[3];
                    var type = newSplit[2];

                    //Make sure it doesn't already exist
                    if (!Censored.ContainsKey(censorMessage))
                    {
                        //Sort by censor type
                        var censorType = Settings.ToCensorAction(type);
                        if (censorType != CommandResponse.ResponseAction.None)
                        {
                            Censored.Add(censorMessage, new CommandResponse(censorType));
                            return new CommandResponse(string.Format("Added the new censor [type: {0}]", type),
                                                       CommandResponse.ResponseAction.None);
                        }
                        return new CommandResponse(string.Format("Could not identify action type [{0}]", type),
                                                   CommandResponse.ResponseAction.None);
                    }
                    return new CommandResponse("Censored command already exists", CommandResponse.ResponseAction.None);
                }
                if(cleanSplit[1].ToUpper() == CommandRemove.ToUpper())
                {
                    var newSplit = cleanMessage.Split(new[] { ' ' }, 3);
                    var censorMessage = newSplit[2];

                    //Check to see if it already exists
                    if(Censored.ContainsKey(censorMessage))
                    {
                        //Remove it
                        Censored.Remove(censorMessage);
                        //Respond to the user
                        return new CommandResponse("Removed the censor", CommandResponse.ResponseAction.None);
                    }
                    return new CommandResponse("No censor by that name exists", CommandResponse.ResponseAction.None);
                }
                //Respond with an unidentified message
                return new CommandResponse(string.Format("Could not identify add/delete [{0}]", cleanSplit[1]),
                                           CommandResponse.ResponseAction.None);
            }
            #endregion

            //Check through text commands
            foreach(var name in TextCommands.Keys)
            {
                //If it matches
                if(name.ToUpper() == cleanMessage.ToUpper())
                {
                    //Respond appropriately
                    return new CommandResponse(TextCommands[name], CommandResponse.ResponseAction.None);
                }
            }

            //No commands were found
            return null;
        }

        /// <summary>
        /// Checks for censored phrases
        /// </summary>
        /// <param name="user">The sender's username</param>
        /// <param name="message">The message</param>
        /// <param name="isMod">Is the sender a moderator</param>
        /// <returns>The appropriate command response</returns>
        private static CommandResponse CensorCheck(string user, string message, bool isMod)
        {
            //Mods do not get censored
            if (isMod) return null;

            //Sort through the censored keys
            foreach (var key in Censored.Keys)
            {
                //Determine whether the message contains the phrase
                if (!message.ToUpper().Trim().Contains(key.ToUpper().Trim())) continue;

                //Sort by response type
                if (Censored[key].Action == CommandResponse.ResponseAction.Warning)
                {
                    if (Warned.ContainsKey(user))
                    {
                        //If the user has had only 1 warning 
                        if (Warned[user] == 1)
                        {
                            //Increment warning number
                            Warned[user]++;

                            //Send the timeout response
                            if (string.IsNullOrEmpty(Censored[key].Message))
                            {
                                var responseMessage = string.Format(LastWarning, user);
                                var response = new CommandResponse(responseMessage, CommandResponse.ResponseAction.Timeout);
                                return response;
                            }
                            return Censored[key];
                        }

                        //If the user has had 2 warnings
                        if (Warned[user] == 2)
                        {
                            //Reset warning number
                            Warned.Remove(user);

                            //Send the ban
                            if (string.IsNullOrEmpty(Censored[key].Message))
                            {
                                var responseMessage = string.Format(BanWarning, user);
                                var response = new CommandResponse(responseMessage, CommandResponse.ResponseAction.Ban);
                                return response;
                            }
                            return Censored[key];
                        }
                    }
                    else
                    {
                        //Add to warning list
                        Warned.Add(user, 1);

                        if (string.IsNullOrEmpty(Censored[key].Message))
                        {
                            //Send the warning response
                            var responseMessage = string.Format(FirstWarning, user);
                            var response = new CommandResponse(responseMessage, CommandResponse.ResponseAction.Purge);
                            return response;
                        }
                        return Censored[key];
                    }
                }
                else if (Censored[key].Action == CommandResponse.ResponseAction.Ban)
                {
                    //Send the ban message if not defined
                    if (string.IsNullOrEmpty(Censored[key].Message))
                    {
                        var responseMessage = string.Format(_banMessages[Randomizer.Next(0, _banMessages.Length - 1)],
                                                            user);
                        var response = new CommandResponse(responseMessage, Censored[key].Action);
                        return response;
                    }
                    return Censored[key];
                }
                else if(Censored[key].Action == CommandResponse.ResponseAction.Purge)
                {
                    if (string.IsNullOrEmpty(Censored[key].Message))
                    {
                        //Send the purge message
                        var responseMessage =
                            string.Format(_purgeMessages[Randomizer.Next(0, _purgeMessages.Length - 1)], user);
                        var response = new CommandResponse(responseMessage, Censored[key].Action);
                        return response;
                    }
                    return Censored[key];
                }
                else if(Censored[key].Action == CommandResponse.ResponseAction.Timeout)
                {
                    if (string.IsNullOrEmpty(Censored[key].Message))
                    {
                        //Timeout message
                        var responseMessage =
                            string.Format(_timeoutMessages[Randomizer.Next(0, _timeoutMessages.Length - 1)], user);
                        var response = new CommandResponse(responseMessage, Censored[key].Action);
                        return response;
                    }
                    return Censored[key];
                }
            }

            //No censored words were detected
            return null;
        }
    }

    /// <summary>
    /// Class for passing command responses
    /// </summary>
    public class CommandResponse
    {
        //Declare Variables
        public string Message;
        public ResponseAction Action;

        /// <summary>
        /// Constructor for default response (without custom message)
        /// </summary>
        /// <param name="action">The response action</param>
        public CommandResponse(ResponseAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Constructor for default response
        /// </summary>
        /// <param name="message">The message for the chat</param>
        /// <param name="action">The reponse action</param>
        public CommandResponse(string message, ResponseAction action)
        {
            Message = message;
            Action = action;
        }

        /// <summary>
        /// Enum for handling the actions for responding
        /// </summary>
        public enum ResponseAction
        {
            None = 0,
            Warning = 1,
            Timeout = 2,
            Ban = 3,
            Purge = 4
        }   
    }
}
