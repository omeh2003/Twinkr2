using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twinkr2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			dataGridView1.Columns.Add("Number", "Num");
			dataGridView1.Columns.Add("Url", "Url");

			dataGridView1.AutoGenerateColumns = true;
			var i = 0;
			if (Program.UrlsForums != null)
			{
				foreach (var s in Program.UrlsForums)
				{
					object[] o = {i.ToString(CultureInfo.InvariantCulture),s};
					dataGridView1.Rows.Add(o);
					i++;

				}


				//dataGridView1.DataSource = o;
				dataGridView1.Update();
				label1.Text = Program.UrlsForums.Count.ToString(CultureInfo.InvariantCulture);
			}
			if (Program.UrlsForumsPage != null)
				label2.Text = Program.UrlsForumsPage.Count.ToString(CultureInfo.InvariantCulture);
			if (Program.UrlsTopic != null) label3.Text = Program.UrlsTopic.Count.ToString(CultureInfo.InvariantCulture);
		}

		private async void ForumsPage_Click(object sender, EventArgs e)
		{
			var web = new Tw2Url();
			var pr = new Program();
			progressBar1.Value = 0;
			web.onCount += pr.web_onCount;
			await web.ParseTopicPage();
		}

		public void UpdateLabel()
		{
			SetText();
			if(Forum.InvokeRequired)return;
			Forum.Text = Program.UrlsForums.Count().ToString(CultureInfo.InvariantCulture);
			PageForum.Text = Program.UrlsForumsPage.Count().ToString(CultureInfo.InvariantCulture);
			TopicForum.Text = Program.UrlsTopic.Count().ToString(CultureInfo.InvariantCulture);
			PageTopic.Text = Program.UrlsPage.Count().ToString(CultureInfo.InvariantCulture);
			ErrorPage.Text = Program.FileUrlsIsReady.Count().ToString(CultureInfo.InvariantCulture);
		}
		delegate void SetTextCallback();
		private void SetText()
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (progressBar1.InvokeRequired)
			{
				SetTextCallback d = SetText;
				Invoke(d,null);
			}
			else
			{
				progressBar1.PerformStep();
			}
		}
		private async void MainForum_Click(object sender, EventArgs e)
		{
			var web = new Tw2Url();
			var pr = new Program();
			progressBar1.Value = 0;
			web.onCount += pr.web_onCount;
			await Task.Run(() => web.Url.ParseMainForumPage());
			UpdateLabel();

		}

		private async void Topic_Click(object sender, EventArgs e)
		{
			var web = new Tw2Url();
			var pr = new Program();
			progressBar1.Value = 0;
			web.onCount += pr.web_onCount;
			await web.ParsePage();
			UpdateLabel();
		}

		private async void Page_Click(object sender, EventArgs e)
		{
			var web = new Tw2Url();
			var pr = new Program();
			progressBar1.Value = 0;
			web.onCount += pr.web_onCount;
			await Task.Run(() => web.Url.ParseData());
			UpdateLabel();
		}
	}
}
