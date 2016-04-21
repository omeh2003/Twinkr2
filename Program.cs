using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Twinkr2
{
	public class Program
	{

		#region Parametr

		#endregion

		private static Form1 _gui;
		private static int _countpage;

		private static void Main()
		{
		UrlsForums=new List<string>();
		UrlsForumsPage=new List<string>();
		UrlsPage=new List<string>();
		UrlsTopic=new List<string>();
		UrlsIsReadyParse=new List<string>();
			//if (UrlsForums == null) GetUrlsForums();
			var web =new Tw2Url();
			var pr = new Program();
			if (GUI==null)GUI=new Form1();
			GUI.ShowDialog();
			web.onCount += pr.web_onCount;
			web.ParseMainForumPage();
			//web.ParseTopicPage();
		}

		public  void web_onCount()
		{
			//GUI.progressBar1.PerformStep();
			GUI.UpdateLabel();
		}

		public static string FileUrlsForums { get { return "UrlsForums.xml"; } }
		public static string FileUrlsForumsPage { get { return "UrlsForumsPage.xml"; } }
		public static string FileUrlsTopic { get { return "UrlsTopic.xml"; } }
		public static string FileUrlsPage { get { return "UrlsPage.xml"; } }
		public static string FileUrlsIsReady { get { return "UrlsRedy.xml"; } }

		public static List<string> UrlsForums { get; set; }
		public static List<string> UrlsForumsPage { get; set; }
		public static List<string> UrlsTopic { get; set; }
		public static List<string> UrlsPage { get; set; }
		public static List<string> UrlsIsReadyParse { get; set; }

		public static Form1 GUI
		{
			get { return _gui; }
			set { _gui = value; }
		}

		public static int Countpage
		{
			get { return _countpage; }
			set { _countpage = value; }
		}
	}
}