using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeerViewer.Core
{
	public class MessageProvider
	{
		public static MessageProvider Instance { get; } = new MessageProvider();
		private static Random randomObj { get; } = new Random();

		private string TargetOriginal { get; set; }
		private Control Target { get; set; }
		private int LastMessage { get; set; }

		public MessageProvider()
		{
			this.Target = null;
		}

		public void SetProvider(Control Provider)
		{
			this.Target = Provider;
			this.TargetOriginal = this.Target?.Text;
		}

		public async void Submit(string Message, string OriginalText = "", bool SetForever = false)
		{
			if (Target == null || Target.IsDisposed) return;

			int id = randomObj.Next();
			this.LastMessage = id;

			Target.Text = (OriginalText.Length > 0 ? OriginalText + " :: " : "") + Message;
			if (SetForever) return;

			await Task.Delay(5000);
			if (this.LastMessage == id)
				Target.Text = this.TargetOriginal ?? "";
		}
	}
}
