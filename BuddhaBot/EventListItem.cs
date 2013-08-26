using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BuddhaBot
{
    class EventListItem
    {
        public static readonly Color ErrorColor = Color.Red;
        public static readonly Color SuccessColor = Color.Green;
        public static readonly Color WarningColor = Color.SandyBrown;
        public static readonly Color DefaultColor = Color.Black;

        public Color ItemColor { get; set; }
        public string Message { get; set; }

        public EventListItem(string m, Color c)
        {
            ItemColor = c;
            Message = m;
        }
    }
}
