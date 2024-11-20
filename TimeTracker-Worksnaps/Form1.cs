using System.Text;
using System.Xml;

namespace TimeTracker_Worksnaps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            this.Left = workingArea.Right - this.Width - 0;
            this.Top = workingArea.Bottom - this.Height - 0;

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                var timeEntries = GetTimeEntries().Where(timeEntry => timeEntry.ActivityLevel > 0);
                TimeSpan totalTime = TimeSpan.FromMinutes(timeEntries.Count() * 10);
                var remainingMinutes = totalTime.TotalMinutes < 450 ? 450 - totalTime.TotalMinutes : 0;
                var expectedOut = DateTime.Now.AddMinutes(remainingMinutes).RoundUpToNext10Minutes();
                this.Invoke((MethodInvoker)delegate
                {
                    lblTotalTime.Text = $"{totalTime.Hours}(hrs) {totalTime.Minutes} (mins)";
                    lblExpectedOut.Text = expectedOut.ToString("hh:mm tt");
                });

                Thread.Sleep((int)TimeSpan.FromMinutes(5).TotalMilliseconds);
            }
        }

        private IEnumerable<TimeEntry> GetTimeEntries()
        {
            using (HttpClient client = new HttpClient())
            {
                var fromTimestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).GetTimestamp();
                var toTimestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).GetTimestamp();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Configuration.GetApiKey()))
                );
                var response = client.GetAsync($"https://api.worksnaps.com/api/projects/{Configuration.GetProjectId()}/time_entries.xml?user_ids={Configuration.GetUserId()}&from_timestamp={fromTimestamp}&to_timestamp={toTimestamp}").Result.Content.ReadAsStringAsync().Result;
                
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(response);
                var rootDocument = xmlDocument.GetElementsByTagName("time_entries")[0];
                foreach (XmlElement timeEntry in rootDocument.SelectNodes("time_entry"))
                {
                    yield return new TimeEntry()
                    {
                        ActivityLevel = Convert.ToInt32(timeEntry.SelectSingleNode("activity_level").InnerText.ToString())
                    };
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
