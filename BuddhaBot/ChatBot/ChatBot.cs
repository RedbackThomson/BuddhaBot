using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using Sharkbite.Irc;

namespace BuddhaBot.ChatBot
{
    /// <summary>
    /// Class necessary for handling incoming IRC connections and messages
    /// </summary>
    class ChatBot
    {
        #region Variable Declaration
        //Configuration Information
        private string _channel;
        private string _botUsername, _botPassword;

        private Connection _irc;

        private bool _running, _hasMod;
        private List<string> _users, _moderators;

        public List<string> PermaModerators;

        public event EventHandler HandleEvent;
        public delegate void EventHandler(string message, Color color);

        //Constants
        private const string IrcHost = "irc.twitch.tv";
        #endregion

        #region Getters and Setters
        public List<string> Moderators { get { return _moderators; } }
        public string Channel { get { return _channel; } set {_channel = value;} }
        public string BotUsername { get { return _botUsername; } }
        public string BotPassword { get { return _botPassword; } }
        #endregion

        /// <summary>
        /// Constructor that allows specification of the channel's name
        /// </summary>
        /// <param name="channel">The username of the channel (case insensitive)</param>
        public ChatBot(string channel)
        {
            _channel = channel;

            //Create lists for users
            _users = new List<string>();
            _moderators = new List<string>();
        }

        /// <summary>
        /// Starts the chat bot with pre-designated settings
        /// </summary>
        public void StartBot()
        {
            //Check to see that the bot has not already been started
            if (_running) return;

            //Check requirements
            if (string.IsNullOrEmpty(_botUsername)) throw new Exception("No bot account info defined");

            //Message about mod req
            if (!Commands.RequiresMod) HandleEvent("This bot does not require channel mod.", EventListItem.WarningColor);

            //Reset user lists
            ResetUsers();

            //Starts the Identd server - Not necessary, but may be implemented by Twitch at some point
            Identd.Start(_botUsername);
            _irc = new Connection(GetConnArgs(), false, false);

            //Setup irc listeners
            _irc.Listener.OnRegistered += IrcRegistered;
            _irc.Listener.OnNames += IrcOnNames;
            _irc.Listener.OnJoin += IrcJoined;
            _irc.Listener.OnPart += IrcPart;
            _irc.Listener.OnPublic += IrcPublic;
            _irc.Listener.OnPrivate += IrcPrivate;
            _irc.Listener.OnChannelModeChange += Listener_OnChannelModeChange;

            try
            {
                //Attempt a connection the server
                _irc.Connect();

                //Set running flag to true
                _running = true;
            }
            catch(SocketException ex)
            {
                throw new Exception("Unable to connect to the IRC server");
            }
        }

        /// <summary>
        /// Stops the chat bot
        /// </summary>
        public void StopBot()
        {
            //Check to see that the bot has not already been stopped
            if (!_running) return;

            //Turn off the Identd server
            if (Identd.IsRunning()) Identd.Stop();

            //Disconnects the IRC bot
            if(_irc.Connected) _irc.Disconnect("Bot Stopped");

            //Clear user lists
            ResetUsers();

            //Set running flag to false
            _running = false;
        }

        /// <summary>
        /// Clears and adds the default users and mods of the chat
        /// </summary>
        private void ResetUsers()
        {
            //Clear the lists
            _users = new List<string>();
            _moderators = new List<string>();

            //Hardcode the perma mods
            _moderators.AddRange(PermaModerators);
        }

        /// <summary>
        /// Sets the bot's connection information (specifically the bot login)
        /// </summary>
        /// <param name="username">The username of the bot account</param>
        /// <param name="password">The password of the bot account</param>
        public void SetBotAccountInfo(string username, string password)
        {
            _botUsername = username;
            _botPassword = password;
        }

        /// <summary>
        /// Get the connection args that have been specified by the configuration information
        /// </summary>
        /// <returns>The relevant connection arguments</returns>
        private ConnectionArgs GetConnArgs()
        {
            var cargs = new ConnectionArgs(_botUsername, IrcHost) { Nick = _botUsername, RealName = _botUsername, UserName = _botUsername, ServerPassword = _botPassword };
            return cargs;
        }

        #region BuddhaBot IRC Listeners
        /// <summary>
        /// Method called upon a public message being sent over the channel
        /// </summary>
        /// <param name="user">The information of the sender</param>
        /// <param name="channel">The channel name</param>
        /// <param name="message">The message</param>
        private void IrcPublic(UserInfo user, string channel, string message)
        {
            //todo: remove
            Debug.WriteLine(message);
            //Run message through command responder
            var response = Commands.CommandResponder(user.User, message, _moderators.Contains(user.User) || PermaModerators.Contains(user.User.ToUpper()));
            if (response == null) return;

            //Respond appropriately
            Respond(response, user.User);

            //Create a new event if it's not a censor
            if (response.Action == CommandResponse.ResponseAction.None)
            {
                var eventString = message + " {" + user.User + "}";
                HandleEvent(eventString, EventListItem.DefaultColor);
            }
        }

        /// <summary>
        /// Method called upon receiving a private message
        /// </summary>
        /// <param name="user">The information of the sender</param>
        /// <param name="message">The message</param>
        void IrcPrivate(UserInfo user, string message)
        {
            //todo: Set moderators/subs (get it working first)
        }

        /// <summary>
        /// Method called upon an irc connection joining the channel
        /// </summary>
        /// <param name="user">The user info of the connecting person</param>
        /// <param name="channel">The channel name</param>
        private void IrcJoined(UserInfo user, string channel)
        {
            //Check to see if the connecting user is the bot
            if (user.User.ToLower() == _botUsername)
            {
                _irc.Sender.Raw("JTVCLIENT"); //Allows the bot to receive OP/Mod and sub info
                HandleEvent(string.Format("Joined #", channel.ToLower()), EventListItem.SuccessColor);
            }

            _users.Add(user.User);
        }

        /// <summary>
        /// Method called upon an irc connection leaving the channel
        /// </summary>
        /// <param name="user">The user info of the disconnecting person</param>
        /// <param name="channel">Che channel name</param>
        /// <param name="reason">The reason for parting</param>
        void IrcPart(UserInfo user, string channel, string reason)
        {
            //Remove from user lists
            _users.Remove(user.User);

            if (_moderators.Contains(user.User)) _moderators.Remove(user.User);
        }

        /// <summary>
        /// Method called upon receiving the list of connected users
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="nicks">The list of connected users</param>
        /// <param name="last">Final list of connected users (no more afterwards)</param>
        void IrcOnNames(string channel, string[] nicks, bool last)
        {
            //Add list of names to the current user list.
            _users.AddRange(nicks);
        }

        /// <summary>
        /// Method called upon the irc connection registering to the IRC server
        /// </summary>
        private void IrcRegistered()
        {
            //Turn off the Identd Server
            if(Identd.IsRunning()) Identd.Stop();

            //Join the corresponding channel
            _irc.Sender.Join("#" + _channel.ToLower());
            HandleEvent(string.Format("Connected! Joining #{0}", _channel.ToLower()), EventListItem.SuccessColor);
        }

        /// <summary>
        /// Method called upon the channel mode changing (someone becoming mod)
        /// </summary>
        /// <param name="who">The sender of the message</param>
        /// <param name="channel">The channel name</param>
        /// <param name="modes">The mode being changed to/from</param>
        void Listener_OnChannelModeChange(UserInfo who, string channel, ChannelModeInfo[] modes)
        {
            foreach (var mode in modes)
            {
                //Parameter contains the user's name
                var user = mode.Parameter;

                //Determine if the mode is changing operators (mods)
                if (mode.Mode == ChannelMode.ChannelOperator)
                {
                    //Check to see if it's the bot
                    if (user.ToUpper() == _botUsername.ToUpper())
                    {
                        //Determine if the mode is adding or removing
                        _hasMod = (mode.Action == ModeAction.Add);
                    }
                    if (mode.Action == ModeAction.Add)
                    {
                        //Add to moderator list
                        _moderators.Add(user);
                    }
                    else
                    {
                        //Otherwise remove them
                        if (_moderators.Contains(user))
                            _moderators.Remove(user);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Send a message through the connected IRC channel
        /// </summary>
        /// <param name="message">The message to be sent</param>
        private void SendMessage(string message)
        {
            //Sends the public message
            _irc.Sender.PublicMessage("#" + _channel.ToLower(), message);
        }

        /// <summary>
        /// Responds to the response from the commands/censor
        /// </summary>
        /// <param name="response">The response to respond as</param>
        /// <param name="user">Name of the response user</param>
        private void Respond(CommandResponse response, string user)
        {
            //Check to see if there was any response
            if (response == null) return;

            //Check for response message
            if(!string.IsNullOrWhiteSpace(response.Message))
            {
                //Sends the required message
                SendMessage(response.Message);
            }

            //Create an action response only if you have moderator
            if(response.Action != CommandResponse.ResponseAction.None)
            {
                //Make sure it has the permissions
                if (Commands.RequiresMod && !_hasMod) return;

                //Message containing action info
                var message = "";
                //Contains event name for event handling
                var eventName = "";
                switch(response.Action)
                {
                    case(CommandResponse.ResponseAction.Timeout):
                        eventName = "Timed out " + user;
                        message = "/timeout " + user;
                        break;
                    case (CommandResponse.ResponseAction.Purge):
                        eventName = "Purged " + user;
                        message = "/timeout " + user + " 1";
                        break;
                    case (CommandResponse.ResponseAction.Ban):
                        eventName = "Banned " + user;
                        message = "/ban " + user;
                        break;
                }

                //Send the response message
                if(!string.IsNullOrWhiteSpace(message))
                {
                    SendMessage(message);
                    HandleEvent(eventName, EventListItem.DefaultColor);
                }
            }
        }
    }
}
