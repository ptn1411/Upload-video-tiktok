using AutoItX3Lib;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WinFormsApp1222
{

    public partial class Form1 : Form
    {
        
        IWebDriver chromeDriver;
        AutoItX3 autoIT;
        String? userDataDir = null;
        String? profileDirectory = null;
        String? forder = null;
        String? caption = null;
        String ip = "45.140.13.119:9132";
        String username = "username";
        String pass = "password";
        String? filePathJson = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            try
            {
              
                string appPath = Application.StartupPath;
                string filePath = "config.json";
                string fullpath = Path.Combine(appPath, filePath);
                filePathJson = fullpath;
                string json = File.ReadAllText(fullpath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                string user = jsonObj["user"];
                string profile = jsonObj["profile"];
                string caption_json = jsonObj["caption"];
                
                if (user != null && profile != null && caption_json != null)
                {
                    textBox2.Text = user;
                    textBox3.Text = profile;
                    richTextBox2.Text = caption_json;
                    string LOGIN_DATA_PATH= "\\..\\Local\\Google\\Chrome\\User Data\\";
                    var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);// APPDATA
                    var p = Path.GetFullPath(appdata + LOGIN_DATA_PATH);
                    userDataDir = p;

                    profileDirectory = profile;

                    caption = caption_json;
                }
                else
                {
                    MessageBox.Show("Please check config.json file");
                    this.Close();
                }
                 

            }
            catch (Exception ex)
            {
                //MessageBox.Show(""+ex);
                EditApp();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               

                var Lines = FormatUid();
                if (Lines != null)
                {
                    foreach (string Line in Lines)
                    {
                        forder = Line;
                        EditFolderOpen();
                        var driverService = ChromeDriverService.CreateDefaultService();

                        driverService.HideCommandPromptWindow = false;
                        ChromeOptions option = new ChromeOptions();
                        autoIT = new AutoItX3();
                        option.AddArgument(@"user-data-dir=" + userDataDir);
                        option.AddArgument("--profile-directory=" + profileDirectory);

                        option.AddArgument("start-maximized");
                        option.AddArgument("--disable-extensions");
                        // option.AddArgument("headless");
                        option.AddArgument("--no-sandbox");
                        option.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
                        //option.AddArgument("--log-level=3");
                        // option.AddArgument(string.Format("--proxy-server={0}", ip));
                        chromeDriver = new ChromeDriver(driverService, option);
                        chromeDriver.Url = "https://phamthanhnam.com/?utm_source=phamthanhnam.com&utm_medium=url_shortener";
                        IJavaScriptExecutor js = (IJavaScriptExecutor)chromeDriver;
                        js.ExecuteScript("javascript:(() => {function pageScroll() {window.scrollBy(0, 1000);window.scrollTo({ top: 0, behavior: 'smooth' });setTimeout(pageScroll, 2000);}pageScroll();})();");
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        chromeDriver.Url = "https://www.tiktok.com/upload";
                        chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(6);

                        var firstResult = chromeDriver.FindElement(By.TagName("iframe"));

                        chromeDriver.SwitchTo().Frame(firstResult);

                        foreach (string file in Directory.EnumerateFiles(Line, "*.mp4"))
                        {
                            string file_path = Path.GetFileName(file);
                            UploadFile(file_path);
                        }

                        chromeDriver.Dispose();
                        chromeDriver.Quit();
                    };
                }
                else
                {
                    MessageBox.Show("Please check config.json file");
                    this.Close();
                }
          
            }
            catch (Exception ex)
            {
               //
               //MessageBox.Show("" + ex);                
                EditApp();
            }

        }
        public void OpenNewTab(string url)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)chromeDriver;
            js.ExecuteScript(string.Format("window.open('{0}', '_blank');", url));
        }

        public string CoventUidToUrl(string uid)
        {
            string url = "https://www.facebook.com/groups/" + uid + "/about";
            return url;
        }
        public Array FormatUid()
        {
            String text = richTextBox1.Text.ToString();

            if (text != null)
            {
                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                return lines;
            }
            else
            {
                return null;
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public void EditFolderOpen()
        {
            try
            {
                string json = File.ReadAllText(userDataDir + profileDirectory + "\\Preferences");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["selectfile"]["last_directory"] = forder;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(userDataDir + profileDirectory + "/Preferences", output);
            }
            catch (Exception ex)
            {
              //  MessageBox.Show("" + ex);
                EditApp();
            }


        }
        public void UploadFile(string file_path)
        {
            try
            {
                chromeDriver.FindElement(By.ClassName("css-ku5jwq")).Click();
                IJavaScriptExecutor js = (IJavaScriptExecutor)chromeDriver;
                js.ExecuteScript("var element = document.getElementsByClassName('public-DraftStyleDefault-block')[0].children[0].getAttribute('data-offset-key');");
                var caption_elem = chromeDriver.FindElement(By.ClassName("public-DraftStyleDefault-block"));
                caption_elem.SendKeys(caption);
                autoIT.WinActivate("Open");
                autoIT.ControlFocus("Open", "", "Edit1");
                Thread.Sleep(TimeSpan.FromSeconds(2));
                autoIT.Send(file_path);
                Thread.Sleep(TimeSpan.FromSeconds(2));

                autoIT.Send("{ENTER}");
                Thread.Sleep(TimeSpan.FromSeconds(15));

                chromeDriver.FindElement(By.ClassName("css-15xm9lp")).Click();

                Thread.Sleep(TimeSpan.FromSeconds(7));
                chromeDriver.FindElement(By.ClassName("modal-btn")).Click();
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
              //  MessageBox.Show("" + ex);
                EditApp();
            }
          
        }

        private void button2_Click(object sender, EventArgs e)
            
        {
            EditApp();
        }
        public void EditApp() {

            try
            {
             

                Application.Exit();
            }
            catch (Exception e )
            {
                
            }

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string user = textBox2.Text;
          
                   

                    string json = File.ReadAllText(filePathJson);
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    jsonObj["user"] = user;
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                System.IO.File.WriteAllText(filePathJson, output);
              
            }
            catch (Exception ex)
            {
                //MessageBox.Show("" + ex);
                EditApp();
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string profile = textBox3.Text;
                profileDirectory = textBox3.Text.ToString();

                string json = File.ReadAllText(filePathJson);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["profile"] = profile;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(filePathJson, output);
            }
            catch (Exception ex)
            {
//MessageBox.Show("" + ex);
                EditApp();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                 
                caption = richTextBox2.Text;

                string json = File.ReadAllText(filePathJson);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["caption"] = caption;
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(filePathJson, output);
            }
            catch (Exception ex)
            {
               // MessageBox.Show("" + ex);
                EditApp();
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
