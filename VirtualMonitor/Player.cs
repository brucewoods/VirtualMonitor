using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualMonitor
{
	public partial class Player : Form
	{
		public Player()
		{
			InitializeComponent();
		}

		private void Player_Leave(object sender, EventArgs e)
		{
			axWindowsMediaPlayer1.close();
		}

		private void axWindowsMediaPlayer1_EndOfStream(object sender, AxWMPLib._WMPOCXEvents_EndOfStreamEvent e)
		{
			axWindowsMediaPlayer1.close();
			axWindowsMediaPlayer1.Visible = false;
			
			this.Close();
		}

		private void Player_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			
			 
		}
	}
}
