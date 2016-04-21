using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Twinkr2
{
	public class Tw2Url
	{
		public delegate void MethodContainer();

		public event MethodContainer onCount;

		private readonly string _userAgent;
		public Tw2Url Url;
		private readonly string _address;
		public Tw2Url()
		{
			Url = this;
			_userAgent = GetUserAgent();
			 _address = "http://eu.battle.net";

		}

		private static string GetUserAgent()
		{
			return
				" Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Win64; x64; Trident/4.0; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
		}

		public  HtmlDocument GetHtmlDocument(string url)
		{
			try
			{
				var web = new WebClient {Encoding = Encoding.UTF8};
				web.Headers.Set("UserAgent", _userAgent);

				var str = web.DownloadString(url);
				var doc = new HtmlDocument();
				doc.LoadHtml(str);
				return doc;
			}
			catch (WebException e)
			{
				MessageBox.Show(e.Message);
				
			}
			catch (NotSupportedException e)
			{
				MessageBox.Show(e.Message);
			}
			return null;
		}

		public Task<bool>  ParseMainForumPage()
		{
			var t = new TaskCompletionSource<bool>();
			t.SetResult(true);
			
			Program.UrlsForums=new List<string>();
			if (!File.Exists(Program.FileUrlsForums))
			{


				var doc = GetHtmlDocument(_address);
				const string xpath = @"//a[@class=""forum-link""]";
				var node = doc.DocumentNode.SelectNodes(xpath);

				if (node == null) return t.Task;
				var counti = node.Count/100;
				var countii = 0;
				foreach (var n in node)
				{
					
					var a = n.GetAttributeValue("href", String.Empty);
					countii++;
					if (counti <= countii)
					{
						countii = 0;
						onCount();
					}
					if (Program.UrlsForums.Contains(a)) continue;
					Program.UrlsForums.Add(a);

				}
			}
			else if(File.Exists(Program.FileUrlsForums))
			{
				Program.UrlsForums=ReadUrls(Program.FileUrlsForums);
				
				return t.Task;
			}

			SaveUrls(Program.FileUrlsForums,Program.UrlsForums);

			return t.Task;
		}

		public static void SaveUrls(string file, List<string> list)
		{
			using (var f = File.Create(file))
			{
				
				var writer = new XmlSerializer(list.GetType());
				f.Flush();
				writer.Serialize(f, list);
				f.Close();
			}
		}

		public static List<string> ReadUrls(string file)
		{
			var l = new List<string>();
			using (var f = new StreamReader(file))
			{
				var reader = new XmlSerializer(l.GetType());
				
				l = (List<string>) reader.Deserialize(f);
				f.Close();
			}

			return l;

		}

		public  Task<bool> ParseTopicPage()
		{
			var t = new TaskCompletionSource<bool>();
			t.SetResult(true);
			Program.Countpage = 0;
			if (Program.UrlsForums != null && Program.UrlsForums.Count == 0)
			{
				MessageBox.Show("Нет линков в UrlsForum");
				Program.UrlsForums=ReadUrls(Program.FileUrlsForums);
			}
			
			Program.UrlsTopic=new List<string>();
			Program.UrlsForumsPage=new List<string>();
			if (Program.UrlsForums == null)ParseMainForumPage();
			if (Program.UrlsForums != null)
				foreach (var forum in Program.UrlsForums)
				{
					var url = _address + forum;

					var doc = GetHtmlDocument(url);
					const string topicTitle = @"//a[@class=""topic-title""]";
					const string pagenum = @"//a[@data-pagenum]";
					var topic = doc.DocumentNode.SelectNodes(topicTitle);
					var pagen = doc.DocumentNode.SelectNodes(pagenum);

					if (topic != null)
					{
						var counti = topic.Count / 100;
						var countii = 0;
						foreach (var n in topic)
						{

							var a = n.GetAttributeValue("href", String.Empty);
							countii++;
							if (counti <= countii)
							{
								countii = 0;
								onCount();
							}
							if (Program.UrlsTopic.Contains(a)) continue;
							Program.UrlsTopic.Add(a);

						}
					}
				  
					if(pagen!=null)
					{

						foreach (var n in pagen)
						{

							var a = n.GetAttributeValue("data-pagenum", String.Empty);
						
							var ii = Int32.Parse(a);
							if (ii > Program.Countpage) Program.Countpage = ii;

						}
					}
					if (Program.Countpage > 0)
					{

						for (var i = 1; i <= Program.Countpage; i++)
						{
							var page = forum + @"?page=" + i;
							if (Program.UrlsForumsPage.Contains(page)) continue;
							Program.UrlsForumsPage.Add(page);
						}
						Program.Countpage = 0;
					}
				}


			SaveUrls(Program.FileUrlsForumsPage,Program.UrlsForumsPage);
			SaveUrls(Program.FileUrlsTopic,Program.UrlsTopic);
			return t.Task;
		}
		public  Task<bool> ParsePage()
		{
			var t = new TaskCompletionSource<bool>();
			t.SetResult(true);
			Program.Countpage = 0;
			if (Program.UrlsForumsPage != null && Program.UrlsForums.Count == 0)
			{
				MessageBox.Show("Нет линков в UrlsForum");
				Program.UrlsForumsPage=ReadUrls(Program.FileUrlsForumsPage);
			}
			
			Program.UrlsTopic=new List<string>();
			Program.UrlsPage=new List<string>();
			
			if (Program.UrlsForumsPage != null)

			{

				var counti = Program.UrlsForumsPage.Count / 100;
				var countii = 0;
				foreach (var forum in Program.UrlsForumsPage)
				{
					var url = _address + forum;

					var doc = GetHtmlDocument(url);
					const string topicTitle = @"//a[@class=""topic-title""]";
					
					var topic = doc.DocumentNode.SelectNodes(topicTitle);
					

					if (topic != null)
					{
						
						foreach (var n in topic)
						{

							var a = n.GetAttributeValue("href", String.Empty);
							
							if (Program.UrlsTopic.Contains(a)) continue;
							Program.UrlsTopic.Add(a);

						}
					}

					countii++;
					if (counti <= countii)
					{
						countii = 0;
						onCount();
					}
				  
					
				}
			}


			SaveUrls(Program.FileUrlsTopic,Program.UrlsTopic);
			//SaveUrls(Program.FileUrlsTopic,Program.UrlsTopic);
			return t.Task;
		}


		public Task<bool> ParseData()
		{
			var cahe = "cahe";
			var t = new TaskCompletionSource<bool>();
			t.SetResult(true);
			if (Program.UrlsTopic != null && Program.UrlsTopic.Count == 0)
			{
				MessageBox.Show("Нет линков в UrlsTopic");
				Program.UrlsTopic = ReadUrls(Program.FileUrlsTopic);
			}
			if (!Directory.Exists(cahe)) Directory.CreateDirectory(cahe);


			if (Program.UrlsTopic != null)
			{

				var counti = Program.UrlsTopic.Count / 100;
				var countii = 0;
				foreach (var forum in Program.UrlsTopic)
				{

					countii++;
					if (counti <= countii)
					{
						countii = 0;
						onCount();
					}
					var url = _address + forum;
					var fileurl = string.Format(@"{0}\{1}.{2}", cahe, forum.Replace("/","_"), ".txt");
					if (File.Exists(fileurl))continue;
					var doc = GetHtmlDocument(url);
					
					doc.Save(fileurl);
					

					


				}
			}


			SaveUrls(Program.FileUrlsTopic, Program.UrlsTopic);
			//SaveUrls(Program.FileUrlsTopic,Program.UrlsTopic);
			return t.Task;


		}
	}
}
