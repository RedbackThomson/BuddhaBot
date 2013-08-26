using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class MainForm : Form
    {
        private ChatBot.ChatBot _bot;
        private Settings _settings;

        private const string LogFile = "log.txt";
        private const string LogFormat = "[{0}] {1}";

        public MainForm()
        {
            InitializeComponent();

            //Load initial settings
            LoadSettings();
            Commands.SetUpMessages();

            //Initialize Chat bot
            _bot.HandleEvent += AddEventItem;

            //Start timers
            ListTimer.Start();
        }

        /// <summary>
        /// Initiates and loads the chat settings
        /// </summary>
        public void LoadSettings()
        {
            //Creates the container
            _settings = new Settings();

            _settings.LoadSettings(_bot);

            //Load the chat bot
            _bot = new ChatBot.ChatBot(_settings.Channel);
            _bot.SetBotAccountInfo(_settings.BotUsername, _settings.PlaintextPassword);
            _bot.PermaModerators = _settings.PermaMods;
        }

        /// <summary>
        /// Adds an event to the event list
        /// </summary>
        /// <param name="message">The event message</param>
        /// <param name="textColor">The colour of the event</param>
        public void AddEventItem(string message, Color textColor)
        {
            //Set up cross threading
            if (EventList.InvokeRequired)
            {
                EventList.Invoke(new MethodInvoker(() => AddEventItem(message, textColor)));
            }
            else
            {
                //Write to UI
                var newItem = new EventListItem(message, textColor);
                EventList.Items.Insert(0, newItem);

                //Write to log file
                using(var fs = new FileStream(LogFile, FileMode.Append))
                using(var sw = new StreamWriter(fs))
                {

                    sw.WriteLine(LogFormat, DateTime.Now, message);
                }
            }
        }

        private void ConnectBTN_Click(object sender, EventArgs e)
        {
            try
            {
                _bot.StartBot();
            }
            catch (Exception ex)
            {
                AddEventItem(ex.Message, EventListItem.ErrorColor);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Call disconnect
            DisconnectBTN_Click(sender, new EventArgs());
        }

        private void ListTimerTick(object sender, EventArgs e)
        {
            //Remove all current items in the lists
            CommandsList.Items.Clear();
            ModsList.Items.Clear();
            CensorsList.Items.Clear();

            //Add the list
            CommandsList.Items.AddRange(Commands.TextCommands.Keys.ToArray());
            ModsList.Items.AddRange(_bot.Moderators.ToArray());
            CensorsList.Items.AddRange(Commands.Censored.Keys.ToArray());
        }

        private void EventListDrawItem(object sender, DrawItemEventArgs e)
        {
            var item = EventList.Items[e.Index] as EventListItem; // Get the current item and cast it to MyListBoxItem

            if (item != null)
            {
                e.Graphics.DrawString( // Draw the appropriate text in the ListBox
                    item.Message, // The message linked to the item
                    EventList.Font, // Take the font from the listbox
                    new SolidBrush(item.ItemColor), // Set the color 
                    0, // X pixel coordinate
                    e.Bounds.Top // Y pixel coordinate.  Multiply the index by the ItemHeight defined in the listbox.
                );
            }
        }

        private void DisconnectBTN_Click(object sender, EventArgs e)
        {
            //Stop the bot
            _bot.StopBot();
            AddEventItem("Disconnected", EventListItem.SuccessColor);

            //Save the current settings
            _settings.WriteSettings();
        }

        private void ConfigureBTN_Click(object sender, EventArgs e)
        {
            //Configure new settings
            _settings.NewSettings(false, _bot);
        }
    }
}
