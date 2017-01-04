using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.Win32;
using SHDocVw;
using mshtml;
using System.IO;
using System.Drawing.Imaging;
using WebBrowser = System.Windows.Forms.WebBrowser;
using Control = System.Windows.Forms.Control;
using IServiceProvider = BeerViewer.Win32.IServiceProvider;

using BeerViewer.Win32;

namespace BeerViewer.Core
{
	internal static class Helper
	{
		[DllImport("user32.dll")]
		private static extern long LockWindowUpdate(IntPtr Handle);

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		public enum DeviceCap
		{
			VERTRES = 10,
			DESKTOPVERTRES = 117
		}

		private static int UseGPUAcceleration => 1;
		private static int FeatureBrowserEmulation => 11000;

		private static string UserStyleSheet =>
			"body { margin:0; overflow:hidden }"
			+ "#game_frame { position:fixed; left:50%; top:-16px; margin-left:-450px; z-index:1 }"
			+ ".area-pickupgame, .area-menu { display: none !important }";

		private static WebBrowser Browser => frmMain.Instance?.Browser;
		private static bool BrowserFirstLoaded { get; set; } = false;

		/// <summary>
		/// 디자인 모드인가?
		/// </summary>
		public static bool IsInDesignMode => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

		/// <summary>
		/// HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
		/// IE 브라우저 동작 버전을 명시 (IE11 기준으로)
		/// </summary>
		public static void SetRegistryFeatureBrowserEmulation()
		{
			const string key = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
			const string GPUkey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING";

			try
			{
				var valueName = Process.GetCurrentProcess().ProcessName + ".exe";
				Registry.SetValue(key, valueName, Helper.FeatureBrowserEmulation, RegistryValueKind.DWord);

				if (Registry.GetValue(GPUkey, valueName, null) == null)
				{
					Registry.SetValue(GPUkey, valueName, UseGPUAcceleration, RegistryValueKind.DWord);
				}
				else
				{
					var rgv = Convert.ToInt32(Registry.GetValue(GPUkey, valueName, null));

					if (rgv == UseGPUAcceleration) return;
					else Registry.SetValue(GPUkey, valueName, UseGPUAcceleration, RegistryValueKind.DWord);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// 시작 전 브라우저 준비
		/// </summary>
		public static void PrepareBrowser(WebBrowser browser, bool withoutEventAttaching = false)
		{
			if (!BrowserFirstLoaded)
			{
				browser.DocumentCompleted += (o, e) =>
				{
					DataStorage.Instance.Ready = true;

					#region Enterance Page
					if (e.Url.OriginalString == "about:blank")
					{
						if (BrowserFirstLoaded) return;
						BrowserFirstLoaded = true;

						Assembly assembly = Assembly.GetExecutingAssembly();
						Version Version = assembly.GetName().Version;

						var style = browser.Document.CreateElement("style");
						style.SetAttribute("type", "text/css");
						style.InnerHtml = "body{ background:#2c3a48; font-family:sans-serif } div#root{ display:flex; background:rgba(0,0,0,0.38); height:100%; align-items:center; overflow:hidden }"
							+ " div#content{ margin:auto; text-align:center; text-shadow:0 1px 15px rgba(255, 255, 255, 0.21); color:#fff }"
							+ " h1,h3{ margin:0; font-weight:300 } h1{ font-size:56px } h3{ font-size:34px } h3 span { padding:0 12px }"
							+ " a{ position:relative; display:inline-block; margin:20px 28px 0; height:38px; line-height:42px; padding:0 22px; background:#505963;"
							+ "text-decoration:none;font-size:20px;color:#fff} a::before,a::after{ content:\"\"; position:absolute; display:inline-block;"
							+ "left:-14px; top:6px; width:27px; height:27px; background:#505963; transform:rotate(45deg); z-index:0; cursor:pointer }"
							+ " a::after{ left:auto; right:-14px } a:hover,a:hover::before,a:hover::after { background:#5C636B }"
							+ " @keyframes fadeInDown { from{ opacity:0; transform:translate3d(0,-100%,0) } to{ opacity:1; transform:none }}"
							+ " .fadeInDown { animation-delay:0.3s; animation-duration:1s; animation-fill-mode:both; animation-name:fadeInDown }";
						browser.Document.Body.AppendChild(style);

						var script = browser.Document.CreateElement("script");
						script.SetAttribute("type", "text/javascript");
						script.InnerHtml =
							"window.game_start = function(elem){{\n"
							+ "try{ " + Const.PatchCookie + Const.GameURL + " }catch(e){ alert(e) }\n"
							+ " elem.style.visibility=\"hidden\";\n"
							+ "}};";
						browser.Document.Body.AppendChild(script);

						var root = browser.Document.CreateElement("div");
						root.Id = "root";
						root.InnerHtml = string.Format(
							"<div id=\"content\">"
								+ "<div style=\"margin-bottom:8px\">"
									+ "<img class=\"fadeInDown\" src=\"data:image/png;base64,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\">"
								+ "</div>"
								+ "<h1>BeerViewer</h1>"
								+ "<h3><span>{0}</span><span>{1}</span></h3>"
								+ "<a href=\"#\" onclick=\"window.game_start(this);\">GAME START</a>"
								+ "<div id=\"update\"></div>"
							+ " </div>",
							$"ver {Version.ToString(3)}",
							$"rev.{Version.Revision}"
						);
						browser.Document.Body.AppendChild(root);

						var updateLayer = browser.Document.GetElementById("update");
						if (updateLayer != null)
						{
							var Checker = new VersionChecker();
							Checker.PropertyEvent(nameof(Checker.State), () =>
							{
								if (Checker.State == CheckState.Updatable)
								{
									updateLayer.InnerHtml = string.Format(
										"<a href=\"{1}\">New version! - {0}</a>",
										Checker.Version,
										Checker.UpdateURL
									);
								}
								else if ((int)Checker.State < 0) // Error
									updateLayer.InnerHtml = "<span style=\"padding:7px 0\">Update information pending failed</span>";
							});
							Checker.RequestCheck();
						}
						return;
					}
					#endregion

					try
					{
						var document = browser.Document;
						if (document == null) return;

						var gameFrame = document.GetElementById("game_frame");
						if (gameFrame == null)
						{
							if (document.Url.PathAndQuery.Contains(".swf?"))
								gameFrame = document.Body;
						}

						#region Apply Stylesheet
						var target = gameFrame?.Document;
						if (target != null)
						{
							var style = target.CreateElement("style");
							style.SetAttribute("type", "text/css");
							style.InnerHtml = Helper.UserStyleSheet;
							document.Body.AppendChild(style);
						}
						#endregion

						#region Apply FlashQuality
						var frames = (document.DomDocument as HTMLDocument).frames;
						for (var i = 0; i < frames.length; i++)
						{
							var item = frames.item(i);
							var provider = item as IServiceProvider;
							if (provider == null) continue;

							object ppvObject;
							provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
							var webBrowser = ppvObject as IWebBrowser2;

							var iframeDocument = webBrowser?.Document as HTMLDocument;
							if (iframeDocument == null) continue;

							if (!iframeDocument.location.href.Contains("/ifr?")) continue;

							string qualityString = "high";
							switch (Settings.FlashQuality.Value)
							{
								case 0: qualityString = "high"; break;
								case 1: qualityString = "medium"; break;
								case 2: qualityString = "low"; break;
							}

							string scriptContent =
								"window.kcsFlash_StartFlash = function(a){var b={id:'externalswf',width:'800',height:'480',wmode:'opaque',quality:'" + qualityString + "',bgcolor:'#000000',allowScriptAccess:'always'};document.getElementById('flashWrap').innerHTML=ConstMessageInfo.InstallFlashMessage,gadgets.flash.embedFlash(a+ConstURLInfo.MainFlashURL+'?api_token='+flashInfo.apiToken+'&api_starttime='+flashInfo.apiStartTime,document.getElementById('flashWrap'),6,b),document.getElementById('adFlashWrap').style.height='0px',document.getElementById('wsFlashWrap').style.height='0px',document.getElementById('flashWrap').style.height='480px',gadgets.window.adjustHeight(ConstGadgetInfo.height)};"
							;
							var elem = iframeDocument.createElement("SCRIPT");
							elem.setAttribute("type", "text/javascript");
							elem.innerHTML = scriptContent;
							iframeDocument.appendChild(elem as IHTMLDOMNode);
						}
						#endregion
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				};
			}

			BrowserFirstLoaded = false;
			browser.Navigate("about:blank");
		}

		/// <summary>
		/// 크리티컬 화면을 준비하는지 여부
		/// </summary>
		public static void SetCritical(bool bCritical)
		{
			Color color = bCritical
				? Color.FromArgb(132, 8, 12)
				: Color.FromArgb(44, 58, 72);

			frmMain.Instance.SetBackColor(color);
		}

		/// <summary>
		/// 게임 화면 캡쳐
		/// </summary>
		/// <param name="browser"></param>
		public static Image Capture(WebBrowser browser)
		{
			// Find swf element
			var document = browser.Document.DomDocument as HTMLDocument;
			if (document == null) return null;

			IViewObject gameObject = null;

			// ~~~.swf?~~~ in URL
			if (document.url.Contains(".swf?"))
			{
				// This is flash object
				var viewObject = document.getElementsByTagName("embed").item(0, 0) as IViewObject;
				if (viewObject == null) return null;

				var width = ((HTMLEmbed)viewObject).clientWidth;
				var height = ((HTMLEmbed)viewObject).clientHeight;
				gameObject = viewObject;
			}
			else
			{
				// #game_frame is parent of Flash object
				var gameFrame = document.getElementById("game_frame")?.document as HTMLDocument;
				if (gameFrame == null) return null;

				var frames = document.frames;
				var find = false;
				for (var i = 0; i < frames.length; i++)
				{
					var item = frames.item(i);
					var provider = item as BeerViewer.Win32.IServiceProvider;
					if (provider == null) continue;

					object ppvObject;
					provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
					var webBrowser = ppvObject as IWebBrowser2;

					var iframeDocument = webBrowser?.Document as HTMLDocument;
					if (iframeDocument == null) continue;

					// flash 요소가 <embed>인 것과 <object>인 것을 판별하여 추출
					IViewObject viewObject = null;

					var swf = iframeDocument.getElementById("externalswf");
					if (swf == null) continue;

					Func<dynamic, bool> function = target =>
					{
						if (target == null) return false;

						viewObject = target as IViewObject;
						if (viewObject == null) return false;

						return true;
					};
					if (!function(swf as HTMLEmbed) && !function(swf as HTMLObjectElement)) continue;

					find = true;
					gameObject = viewObject;
					break;
				}
				if (!find) return null;
			}
			if (gameObject == null) return null;

			var image = GetRealScreenshot(800, 480, gameObject);
			return image;
		}
		private static Image GetRealScreenshot(int width, int height, IViewObject viewObject)
		{
			try
			{
				var image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				var rect = new RECT { left = 0, top = 0, right = width, bottom = height };
				var tdevice = new DVTARGETDEVICE { tdSize = 0, };

				using (var graphics = Graphics.FromImage(image))
				{
					var hdc = graphics.GetHdc();
					viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect, null, IntPtr.Zero, IntPtr.Zero);
					graphics.ReleaseHdc(hdc);
				}

				return image;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// <paramref name="action"/>을 실행하는 동안 UI 업데이트를 중단
		/// </summary>
		public static void LockUpdate(Control control, Action action)
		{
			LockWindowUpdate(control.Handle);
			control.SuspendLayout();
			action?.Invoke();
			control.ResumeLayout(false);
			LockWindowUpdate(IntPtr.Zero);
		}

		public static double GetDPIFactor()
		{
			Graphics g = Graphics.FromHwnd(IntPtr.Zero);
			IntPtr desktop = g.GetHdc();
			int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
			int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

			double ScreenScalingFactor = (double)PhysicalScreenHeight / LogicalScreenHeight;
			return ScreenScalingFactor; // 1.25 = 125%
		}
	}
}

namespace System.Windows.Forms
{
	public static class ControlExtension
	{
		/// <summary>
		/// <see cref="Action"/>으로 Invoke 실행
		/// </summary>
		public static void Invoke(this Control control, Action action)
		{
			control?.Invoke(new MethodInvoker(action));
		}
	}
}
