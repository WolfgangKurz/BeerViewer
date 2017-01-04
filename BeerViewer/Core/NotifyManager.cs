using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using DesktopToast;

namespace BeerViewer.Core
{
	#region Base Data
	internal static class Notification
	{
		public static class Types
		{
			public static string Test { get; } = nameof(Test);
			public static string BuildingCompleted { get; } = nameof(BuildingCompleted);
			public static string RepairingCompleted { get; } = nameof(RepairingCompleted);
			public static string ExpeditionReturned { get; } = nameof(ExpeditionReturned);
			public static string FleetRejuvenated { get; } = nameof(FleetRejuvenated);
		}

		public static INotification Create(string type, string header, string body, Action activated = null, Action<Exception> failed = null)
		{
			return new AnonymousNotification
			{
				Type = type,
				Header = header,
				Body = body,
				Activated = activated,
				Failed = failed,
			};
		}

		private class AnonymousNotification : INotification
		{
			public string Type { get; internal set; }
			public string Header { get; internal set; }
			public string Body { get; internal set; }

			public Action Activated { get; internal set; }
			public Action<Exception> Failed { get; internal set; }
		}
	}
	internal interface INotification
	{
		string Type { get; }
		string Header { get; }
		string Body { get; }

		Action Activated { get; }
		Action<Exception> Failed { get; }
	}
	internal abstract class NotifierBase : IDisposable
	{
		public bool Initialized { get; private set; }
		public abstract bool IsSupported { get; }

		protected abstract void InitializeCore();
		protected abstract void NotifyCore(string header, string body, Action activated, Action<Exception> failed);

		public void Initialize()
		{
			if (this.Initialized) return;

			this.InitializeCore();
			this.Initialized = true;
		}

		public void Notify(INotification notification)
		{
			if (!this.IsSupported) return;

			this.Initialize();
			this.NotifyCore(notification.Header, notification.Body, notification.Activated, notification.Failed);
		}

		public virtual void Dispose() { }
	}
	#endregion

	public class NotifyManager
	{

		private class Toast
		{
			public static bool IsSupported
			{
				get
				{
					var version = Environment.OSVersion.Version;
					return (version.Major == 6 && version.Minor >= 2) || version.Major > 6;
				}
			}

#if DEBUG
			public const string AppId = "WolfgangKurz.BeerViewer.Debug";
#else
		public const string AppId = "WolfgangKurz.BeerViewer"; 
#endif

			public event Action Activated;
			public event Action ToastFailed;
			private readonly ToastRequest request;

			public Toast(string header, string body)
			{
				this.request = new ToastRequest
				{
					ToastHeadline = header,
					ToastBody = body,
					ShortcutFileName = "맥주뷰어.lnk",
					ShortcutTargetFilePath = Assembly.GetEntryAssembly().Location,
					ShortcutIconFilePath = Assembly.GetEntryAssembly().Location,
					AppId = AppId,
				};
			}

			public void Show()
			{
				ToastManager.ShowAsync(this.request)
					.ContinueWith(t =>
					{
						if (t.Result == ToastResult.Activated)
							this.Activated?.Invoke();

						if (t.Result == ToastResult.Failed)
							this.ToastFailed?.Invoke();
					});
			}
		}

		private class BalloonNotifier : NotifierBase
		{
			private NotifyIcon notifyIcon;
			private EventHandler activatedAction;
			public override bool IsSupported => !Toast.IsSupported;

			protected override void InitializeCore()
			{
				var MainWindow = frmMain.Instance;
				var Icon = MainWindow.Icon;

				MainWindow.Invoke(new MethodInvoker(() =>
				{
					var ProductInfo = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(
						Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), false)
					);

					this.notifyIcon = new NotifyIcon
					{
						Text = ProductInfo.Product,
						Icon = Icon,
						Visible = true,
					};
				}));
			}

			protected override void NotifyCore(string header, string body, Action activated, Action<Exception> failed)
			{
				if (this.notifyIcon == null) return;

				var MainWindow = frmMain.Instance;
				MainWindow.Invoke(new MethodInvoker(() =>
				{
					if (activated != null)
					{
						this.notifyIcon.BalloonTipClicked -= this.activatedAction;

						this.activatedAction = (sender, args) => activated();
						this.notifyIcon.BalloonTipClicked += this.activatedAction;
					}

					this.notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
				}));
			}

			public override void Dispose()
			{
				this.notifyIcon?.Dispose();
			}
		}
		private class ToastNotifier : NotifierBase
		{
			public override bool IsSupported => Toast.IsSupported;
			protected override void InitializeCore() { }

			protected override void NotifyCore(string header, string body, Action activated, Action<Exception> failed)
			{
				var toast = new Toast(header, body);
				toast.Activated += activated;
				if (failed != null) toast.ToastFailed += () => failed(new Exception("Toast failed."));

				toast.Show();
			}
		}

		public static void Notify(string Type, string Title, string Message)
		{
			var MainWindow = frmMain.Instance;
			var Notify = Notification.Create(Type, Title, Message, () =>
			{
				MainWindow.Invoke(new MethodInvoker(() =>
				{
					if (MainWindow.WindowState == FormWindowState.Minimized)
						MainWindow.WindowState = FormWindowState.Normal;

					MainWindow.Activate();
				}));
			});

			if (Toast.IsSupported)
				new ToastNotifier().Notify(Notify);
			else
				new BalloonNotifier().Notify(Notify);
		}
	}
}
