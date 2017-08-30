using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Media;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

/*

    I have to:

    frmCredentials.doLogin assignament
    //Append here language menu
    //Append also the logout menu

    Hacer que el sendmsg tenga una manera de comprobar los estilos (con un namevaluecollection Dictionary<string, object>), la cuestión está en pasarselos de forma que no se haga muy engorrosa la llamada, ni tampoco haya muchas sobrecargas...
    Hacer que haya un tiempo de expiración?
    Hacer que cuando pase el tiempo de expiración si el usuario habia marcado recordar, siga conectado volviendo a generar otra key (esta opcion es poco segura)
    Hacer que si el registro no se ha podido generar se vuelva a reenviar una captcha (success y errorcodes)
    Prosegir con los datos a guardar (tengo q hacer las nuevas tablas en la base de datos)
    Y luego... Con las nuevas interpretaciones de cpu, ram y gpu

     */

namespace PCStats
{
    public static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.ApplicationExit += Application_ApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Esta parte va para program creds
            ProgramCreds.cred = new frmCredentials();
            //---------------------------------
            Run();
            //---

            Application.Run(ProgramCreds.cred);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Close();
        }

        public static void Run()
        {
            Console.WriteLine("Running program!");

            //Do config stuff...
            API.LoadConfigCallback(() =>
            {
                Console.WriteLine("Config callback!!");
                if (!API.config.AppSettings.Settings.IsEmpty(API.usernameConfig))
                    ProgramCreds.cred.a_txtUsername.Text = API.config.AppSettings.Settings[API.usernameConfig].Value;

                if (!API.config.AppSettings.Settings.IsEmpty(API.passwordConfig))
                    ProgramCreds.cred.a_txtPassword.Text = API.config.AppSettings.Settings[API.passwordConfig].Value;

                ProgramCreds.cred.doingAutologin = !API.config.AppSettings.Settings.IsEmpty(API.usernameConfig) && !API.config.AppSettings.Settings.IsEmpty(API.passwordConfig);
                ProgramCreds.cred.a_chkRemember.Checked = API.RememberingAuth;
            });
            API.LoadConfig();

            //And later, do things that will require it...
            ProgramCreds.web = new Lerp2Web("pcstats_");
            ProgramCreds.web.sessionSha = ProgramCreds.web.StartSession();
        }

        public static void Close()
        {
            //Save config
            API.config.Save();

            //End session...
            ProgramCreds.web.EndSession(ProgramCreds.web.sessionSha);
        }

        public static void Load()
        {
        }

        public static void Save()
        {
        }
    }

    public class UserMetrics
    {
        public int UserId, CoinsBalance;
        public ulong PixelsTravel;
        public KeyUsage[] KeyboardUsage;
        public UserBadge[] Badges;
    }

    public class GUIBadge
    {
        public int Id;

        public string Name,
                      Description,
                      IconUrl;
    }

    public class UserBadge
    {
        public int Id;
        public bool Unlocked;
    }

    public class KeyUsage
    {
        public Keys Key;

        public int TimesPressed,
                   LastTimePressed;
    }

    #region "LiteLerped-WF-API"

    #region "Languages"

    //Esto va para la API (LiteLerped-WF-API)

    public enum LerpedLanguage { ES, EN }

    public class LanguageManager
    {
        public CultureInfo culture;

        private ResourceManager _rMan;
        private string baseName;

        public Action<LerpedLanguage> Switch;

        public ResourceManager ResMan
        {
            get
            {
                if (_rMan == null)
                    _rMan = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
                return _rMan;
            }
        }

        public LanguageManager(string baseName, Action<LerpedLanguage> act)
        {
            this.baseName = baseName;

            Switch = (lang) =>
            {
                culture = CultureInfo.CreateSpecificCulture(lang.ToString().ToLower());
                act(lang);
            };
        }

        public string GetString(string str)
        {
            return ResMan.GetString(str, culture);
        }
    }

    #endregion "Languages"

    #region "Custom Controls"

    [Designer(typeof(ExTextBoxDesigner))]
    public class ExTextBox : UserControl
    {
        public TextBox textBox;

        public ExTextBox()
        {
            textBox = new TextBox()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(-1, -1),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right
            };
            Control container = new ContainerControl()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(-1)
            };
            container.Controls.Add(textBox);
            this.Controls.Add(container);

            textBox.KeyUp += (obj, sender) =>
            {
                //if (DateTime.UtcNow.IsValid(LastTimePressed))
                //{
                if (ErrorFunc != null)
                {
                    if (ErrorFunc())
                        BackColor = ErrorBorderColor;
                    else
                        BackColor = ValidBorderColor;
                }
                //}
                LastTimePressed = TimeHelpers.UnixTimestamp;
            };

            DefaultBorderColor = SystemColors.ControlDark;
            FocusedBorderColor = Color.FromArgb(129, 190, 247);
            ValidBorderColor = Color.Green;
            ErrorBorderColor = Color.Red;
            BackColor = DefaultBorderColor;
            Padding = new Padding(1);
            Size = textBox.Size;
        }

        private ulong LastTimePressed;

        public Func<bool> ErrorFunc;

        public Color DefaultBorderColor { get; set; }
        public Color FocusedBorderColor { get; set; }
        public Color ValidBorderColor { get; set; }
        public Color ErrorBorderColor { get; set; }

        public override string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public char PasswordChar
        {
            get { return textBox.PasswordChar; }
            set { textBox.PasswordChar = value; }
        }

        public bool CheckError()
        {
            bool ret = ErrorFunc != null && ErrorFunc();
            BackColor = ret ? ErrorBorderColor : (ErrorFunc != null ? ValidBorderColor : DefaultBackColor);
            return ret;
        }

        protected override void OnEnter(EventArgs e)
        {
            BackColor = FocusedBorderColor;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                BackColor = DefaultBorderColor;
            else if (ErrorFunc != null && ErrorFunc())
                BackColor = ErrorBorderColor;
            else if (ErrorFunc != null && !ErrorFunc())
                BackColor = ValidBorderColor;
            base.OnLeave(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, textBox.PreferredHeight, specified);
        }
    }

    public class ExTextBoxDesigner : ControlDesigner
    {
        public override void Initialize(IComponent comp)
        {
            base.Initialize(comp);
            var uc = (ExTextBox) comp;
            EnableDesignMode(uc.textBox, "textBox");
        }
    }

    #region "Captcha"

    [Designer(typeof(CaptchaDesigner))]
    public class Captcha : UserControl, ISupportInitialize
    {
        public PictureBox pictureBox;
        public Button btnRefresh;

        public string Challenge, Key;

        private ResourceManager rMan;
        private bool onceLoad;

        public Captcha()
        {
            rMan = new ResourceManager(typeof(Program).Namespace + ".Resources.Credentials", Assembly.GetExecutingAssembly());
            Control container = new ContainerControl()
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.ControlLightLight
            };
            //Console.WriteLine(container.ClientRectangle.Width);
            pictureBox = new PictureBox()
            {
                Name = "pcbCaptcha",
                BorderStyle = BorderStyle.None,
                Location = new Point(1, 1),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right
            };
            btnRefresh = new Button()
            {
                Name = "btnCaptcha",
                Size = new Size(24, 24),
                BackColor = SystemColors.ControlLight,
                BackgroundImage = (Bitmap) rMan.GetObject("Refresh")
            };
            container.Controls.Add(pictureBox);
            container.Controls.Add(btnRefresh);
            this.Controls.Add(container);

            btnRefresh.Click += RegenCaptcha;
            pictureBox.Paint += (obj, sender) =>
            {
                if (!onceLoad)
                { //Quizás vaya un poco justo.
                    Rectangle rect = ((PictureBox) obj).Parent.ClientRectangle;
                    pictureBox.Size = new Size(container.Width - 25, container.Height);
                    btnRefresh.Location = new Point(container.Width - 25, 0);
                    onceLoad = true;
                }
            };
            /*Load += (a, b) =>
            {
                Console.WriteLine("aaaa");
            };*/
        }

        public void DownloadReCaptcha()
        {
            if (string.IsNullOrEmpty(Key)) throw new NotDefinedCaptchaKeyException();
            try
            {
                using (WebClient client = new WebClient())
                {
                    string response = client.DownloadString(string.Format("http://api.recaptcha.net/challenge?k={0}", Key));

                    Match match = Regex.Match(response, "challenge : '(.+?)'");

                    if (match.Captures.Count == 0)
                    {
                        Challenge = null;
                        return;
                    }

                    Challenge = match.Groups[1].Value;

                    WebRequest request = WebRequest.Create(string.Format("http://www.google.com/recaptcha/api/image?c={0}", Challenge));
                    using (WebResponse webResp = request.GetResponse())
                    using (Stream responseStream = webResp.GetResponseStream())
                        pictureBox.Image = new Bitmap(responseStream);
                }
            }
            catch
            {
                Challenge = null;
            }
        }

        private string GetCaptchaResponse(string userResponse)
        {
            if (string.IsNullOrEmpty(Key)) throw new NotDefinedCaptchaKeyException();
            string url = string.Format("https://www.google.com/recaptcha/api/noscript?k={0}&recaptcha_challenge_field={1}&recaptcha_response_field={2}", Key, Challenge, Uri.EscapeDataString(userResponse));
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            try
            {
                return doc.DocumentNode.Descendants("textarea").FirstOrDefault().InnerText;
            }
            catch
            {
                return "";
            }
        }

        private string VerifyCaptcha(string gResponse)
        {
            if (string.IsNullOrEmpty(Key)) throw new NotDefinedCaptchaKeyException();
            using (WebClient client = new WebClient())
            {
                string GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", Key, gResponse));

                //PHPResponses captchaResponse = JsonConvert.DeserializeObject<PHPResponses>(GoogleReply);

                return JsonConvert.DeserializeObject<JObject>(GoogleReply)["success"].ToObject<string>(); //captchaResponse.Success;
            }
        }

        public bool SolveCaptcha(string userResponse)
        {
            string res = GetCaptchaResponse(userResponse);
            return VerifyCaptcha(res) == "true" || !string.IsNullOrWhiteSpace(res);
        }

        public void RegenCaptcha()
        {
            RegenCaptcha(null, null);
        }

        private void RegenCaptcha(object obj, EventArgs sender)
        {
            DownloadReCaptcha();
        }

        void ISupportInitialize.BeginInit()
        {
            //throw new NotImplementedException();
        }

        void ISupportInitialize.EndInit()
        {
            //throw new NotImplementedException();
        }
    }

    public class CaptchaDesigner : ControlDesigner
    {
        public override void Initialize(IComponent comp)
        {
            base.Initialize(comp);
            var uc = (Captcha) comp;
            EnableDesignMode(uc.pictureBox, "pictureBox");
        }
    }

    public class NotDefinedCaptchaKeyException : Exception
    {
    }

    #endregion "Captcha"

    #region "Notify Label"

    [Designer(typeof(NotifyLabelDesigner))]
    public class NotifyLabel : UserControl, ICloneable, IDisposable
    {
        public static Dictionary<int, NotifyLabel> lbl = new Dictionary<int, NotifyLabel>();
        public static int CurOrder = 0;
        public static bool PlaySounds = true;
        internal NotifyLabel _cloned;

        public static NotifyLabel FindLabel(int key)
        {
            if (lbl.ContainsKey(key))
                return lbl[key];
            return null;
        }

        private int _order;

        public int MyOrder
        {
            get
            {
                return _order;
            }
            set
            {
                if (_order != value)
                {
                    if (lbl.ContainsKey(value))
                        lbl.Swap(_order, value);
                    else
                        lbl.Add(value, this);
                }
                _order = value;
            }
        }

        public Label lblNotify;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text
        {
            get { return lblNotify.Text; }
            set { lblNotify.Text = value; }
        }

        public ContentAlignment TextAlign
        {
            get { return lblNotify.TextAlign; }
            set { lblNotify.TextAlign = value; }
        }

        public NotifyLabel()
            : base()
        {
            lblNotify = new Label()
            {
                Name = "lblNotify",
                BackColor = SystemColors.ControlLightLight,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(lblNotify);
        }

        public void StoreDetails()
        {
            _cloned = (NotifyLabel) Clone();
        }

        public void RestoreDetails()
        {
            PropertyInfo[] properties = _cloned.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object obj = property.GetValue(_cloned, null);
                if (!obj.Equals(GetType().GetProperty(property.Name)))
                    GetType().GetProperty(property.Name).SetValue(this, obj);
            }
        }

        public static void SendInfo(string str)
        {
            SystemSounds.Question.Play();
            SendMsg(str, Color.AliceBlue);
        }

        public static void SendWarning(string str)
        {
            SystemSounds.Exclamation.Play();
            SendMsg(str, Color.Yellow);
        }

        public static void SendError(string str)
        {
            SystemSounds.Asterisk.Play();
            SendMsg(str, Color.Red);
        }

        private static void SendMsg(string str, Color? col = null)
        {
            if (col == null) col = SystemColors.ControlDark;

            string backupText = FindLabel(CurOrder).Text;

            NotifyLabel lblNotify = FindLabel(CurOrder);

            lblNotify.StoreDetails();

            lblNotify.Text = str;
            lblNotify.ForeColor = (Color) col;
            //Change font size, weight??

            //And later... restore it! (After 5 secs)
            Task.Delay(5000).ContinueWith(t => lblNotify.RestoreDetails());
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class NotifyLabelDesigner : ControlDesigner
    {
        public override void Initialize(IComponent comp)
        {
            base.Initialize(comp);
            var uc = (NotifyLabel) comp;
            EnableDesignMode(uc.lblNotify, "label");
        }
    }

    #endregion "Notify Label"

    #endregion "Custom Controls"

    #region "Extensions"

    public static class TimeHelpers
    {
        public const ulong validMillisecondsDelay = 300;

        public static ulong UnixTimestamp
        {
            get
            {
                return (ulong) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            }
        }

        public static bool IsValid(this DateTime tim, ulong lastTimestamp)
        {
            return tim.Diff(lastTimestamp) > validMillisecondsDelay;
        }

        public static ulong Diff(this DateTime tim, ulong lastTimestamp)
        {
            return (ulong) (tim.Subtract(UnixTimeStampToDateTime(lastTimestamp))).TotalMilliseconds;
        }

        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string ToSQLDateTime(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public static class DictionaryHelpers
    {
        public static void Swap<T>(this Dictionary<int, T> dict, int key1, int key2)
        {
            if (key1 != key2)
            {
                T swap = dict[key1];
                dict[key1] = dict[key2];
                dict[key2] = swap;
            }
        }
    }

    public static class ConfigurationHelpers
    {
        public static bool IsEmpty(this KeyValueConfigurationCollection settings, string key)
        {
            return settings[key] == null || (settings[key] != null && string.IsNullOrEmpty(settings[key].Value));
        }
    }

    public static class IdentifierHelpers
    {
        public static string GetMachineUniqueID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("SELECT * FROM Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
                id = mo["ProcessorID"].ToString();

            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string motherBoard = "";
            foreach (ManagementObject mo in moc)
                motherBoard = (string) mo["SerialNumber"];

            return id + motherBoard;
        }
    }

    public static class CryptHelpers
    {
        public static string CreateMD5(this string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString("X2"));

                return sb.ToString().ToLower();
            }
        }

        public static bool IsValidMD5(this string md5) => md5 != null && md5.Length == 32 && md5.All(x => (x >= '0' && x <= '9') || (x >= 'a' && x <= 'f') || (x >= 'A' && x <= 'F'));
    }

    public static class CollectionHelpers
    {
        public static string BuildQueryString(this NameValueCollection nvc, bool allValues = false)
        {
            if (allValues) // Create query string with all values
                return string.Join("&", nvc.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
            else // Omit empty values
                return string.Join("&", nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(nvc[key])).Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }

        public static void ForEach<T>(
                                        this IEnumerable<T> source,
                                        Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }

    public static class JsonUtil
    {
        public static string JsonPrettify(this string json)
        {
            if (!json.IsValidJson()) return json;
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static JProperty GetErrors(this JObject obj)
        {
            return obj != null ? obj.Property("error") : default(JProperty);
        }
    }

    #endregion "Extensions"

    #endregion "LiteLerped-WF-API"

    #region "Lerp2Web"

    #region "Ajax Requests"

    public enum PHPSuccess
    {
        Register,
        Login,
        Logout,
        GetProfile,
        GetTags,
        Getreee,
        GetAppId,
        RegenAuth,
        RememberAuth,
        CreateAuth,
        RegisterEntity,
        StartAppSession,
        EndAppSession
    }

    public enum PHPRequestMethod
    {
        GET,
        POST
    }

    public class Lerp2Web
    {
        //Esto va para la otra API Lerp2Web

        public static Lerp2Web instance;
        private static DateTime StartTime;

        internal const bool outputWebRequests = true;
        public const string APIServer = "http://localhost/lerp2php";

        public const int MIN_USERNAME_LENGTH = 4,
                         MAX_USERNAME_LENGTH = 16,
                         MIN_PASSWORD_LENGTH = 8,
                         MAX_PASSWORD_LENGTH = 31;

        internal static ActionResponse responses = new ActionResponse();

        public static string EntitySha = "",
                             TokenSha = "";

        public bool Notifications;
        private bool _off;

        public string appPrefix,
                      sessionSha;

        public int appId;

        internal delegate void ModifyFormTextCallback(string text);

        public bool OfflineMode
        {
            get
            {
                return _off;
            }
            set
            {
                //Si Notification es true, entonces mostrar un mensaje en la toolbar del menu o un alert diciendo que ha si activado o desactivado el offlinemode (ademas de cambiar el titulo)
                _off = value;
                if (Form.ActiveForm != null)
                {
                    if (_off)
                        SafeModifyActiveFormText(string.Format("[Offline Mode] {0}", Form.ActiveForm.Text));
                    else
                        SafeModifyActiveFormText(Form.ActiveForm.Text.Replace("[Offline Mode] ", ""));
                }
                //Mostrar alerta
            }
        }

        public static int AuthId
        {
            get
            {
                return responses["createAuth"]["id"].ToObject<int>();
            }
        }

        public string strId
        {
            get
            {
                return appId.ToString();
            }
        }

        public static string Username
        {
            get
            {
                try
                {
                    return responses["createAuth"]["data"]["user_data"]["username"].ToObject<string>();
                }
                catch
                {
                    return "Invitado";
                }
            }
        }

        public static string AuthTime
        {
            get
            {
                try
                {
                    return responses["createAuth"]["data"]["creation_date"].ToObject<string>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was a problem getting the authtime: {0}", ex.ToString());
                    return "";
                }
            }
        }

        public static int UserId
        {
            get
            {
                return responses["createAuth"]["data"]["user_data"]["id"].ToObject<int>();
            }
        }

        public Lerp2Web(string appPrefix)
        {
            StartTime = DateTime.Now;
            this.appPrefix = appPrefix;

            //Solicitar el appId
            JObject obj = JGet(new ActionRequest("getAppId")
                {
                    { "prefix", appPrefix }
                });

            this.appId = obj["data"].ToObject<int>();

            //And load everything...
            Load();
            instance = this;
        }

        internal void Load()
        {
            //Console.WriteLine(new ActionRequest("") { { } });
            if (RegisterEntity() == null)
                Console.WriteLine("Couldn't create a new entity!");
        }

        public JObject JGet(NameValueCollection col, bool thrExc = false)
        {
            JObject res = JsonConvert.DeserializeObject<JObject>(Get(col, thrExc));
            RequestMade(col, res);
            return res;
        }

        public T JGet<T>(NameValueCollection col, bool thrExc = false) where T : JObject
        {
            T res = JsonConvert.DeserializeObject<T>(Get(col, thrExc));
            RequestMade(col, res);
            return res;
        }

        public string Get(NameValueCollection col, bool thrExc = false)
        {
            Lerp2WebDebug lerpedDebug = new Lerp2WebDebug(PHPRequestMethod.GET, col);
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.QueryString = col;
                    string res = client.DownloadString(string.Format("{0}/AppAjax.php", APIServer));
                    lerpedDebug.DebugResponse(res);
                    return res;
                }
            }
            catch (Exception ex)
            {
                OfflineMode = true;
                lerpedDebug.DebugException(ex);
                if (thrExc)
                    throw new Exception(ex.ToString());
                return "";
            }
        }

        public JObject JPost(NameValueCollection col, bool thrExc = false)
        {
            JObject res = JsonConvert.DeserializeObject<JObject>(Post(col, thrExc));
            RequestMade(col, res);
            return res;
        }

        public T JPost<T>(NameValueCollection col, bool thrExc = false) where T : JObject
        {
            T res = JsonConvert.DeserializeObject<T>(Post(col, thrExc));
            RequestMade(col, res);
            return res;
        }

        public string Post(NameValueCollection col, bool thrExc = false)
        {
            Lerp2WebDebug lerpedDebug = new Lerp2WebDebug(PHPRequestMethod.POST, col);
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] val = client.UploadValues(string.Format("{0}/AppAjax.php", APIServer), col);
                    string res = Encoding.Default.GetString(val);
                    lerpedDebug.DebugResponse(res);
                    return res;
                }
            }
            catch (Exception ex)
            {
                OfflineMode = true;
                lerpedDebug.DebugException(ex);
                if (thrExc)
                    throw new Exception(ex.ToString());
                return "";
            }
        }

        internal static void RequestMade<T>(NameValueCollection col, T obj) where T : JObject
        {
            if (responses != null && col != null && col["action"] != null)
                responses[col["action"]] = obj;
        }

        public string StartSession()
        {
            if (!OfflineMode)
            {
                //Once we know we already loaded OfflineSession from settings, we can search if we have stored Offline Session and send them because we have now Internet
                if (OfflineSession.HasStoredSessions()) //This needs that the config loads before it, if not this will throw and excepcion... (Maybe not)
                    OfflineSession.SendStoredSessions();

                //Later, we create a new session
                JObject obj = null;

                try
                {
                    obj = JPost(new EActionRequest("startAppSession", true)
                        {
                            { "app_id", strId }
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was a problem starting the session! Message:\n\n{0}", ex.ToString());
                    return "null";
                }

                //Retrieve errors...
                JProperty errors = obj.GetErrors();
                if (obj != null && errors == null || (errors != null && !errors.HasValues))
                    return obj["data"]["sha"].ToObject<string>();
            }
            else
                OfflineSession.StartSession(GetNewSessionSha());
            return "null";
        }

        public bool EndSession(string sessionSha)
        {
            if (!OfflineMode)
            {
                JObject obj = JPost(new EActionRequest("endAppSession")
                        {
                            { "sha", sessionSha }
                        });
                JProperty errors = obj.GetErrors();
                return errors == null || (errors != null && !errors.HasValues);
            }
            else
            { //Puede que se llame lo de la offline session...
                try
                {
                    //Aquí tengo que diferenciar si se empezó o no con Sha
                    if (string.IsNullOrEmpty(sessionSha)) //Si no hay Sha, pues creamos un nuevo registro sin fecha de entrada (puesto que esta está en internet)
                        OfflineSession.AddUnstartedSession(GetNewSessionSha());
                    else //Si hay Sha, entonces añadimos la session con la Sha que teniamos guardada...
                        OfflineSession.AddSession(sessionSha, StartTime);
                    API.config.AppSettings.Settings[API.sessionTimeConfig].Value = OfflineSession.ToString();
                }
                catch
                {
                    Console.WriteLine("There was a problem saving offline session!");
                }
                return false;
            }
        }

        internal JObject RegisterEntity()
        {
            EntitySha = IdentifierHelpers.GetMachineUniqueID().CreateMD5();
            if (!OfflineMode)
                try
                {
                    return JPost(new EActionRequest("registerEntity"));
                }
                catch
                {
                    return null;
                }
            return null;
        }

        public int CreateAuth(string username, string md5password)
        {
            if (!OfflineMode)
            {
                try
                {
                    JObject obj = JPost(new EActionRequest("createAuth", true)
                    {
                        { "username", username },
                        { "password", md5password.IsValidMD5() ? md5password : md5password.CreateMD5() }
                    });
                    try
                    {
                        TokenSha = obj["data"]["token_sha"].ToObject<string>();
                        if (obj != null && obj["data"] != null && obj["data"]["id"] != null)
                            return obj["data"]["id"].ToObject<int>();
                        return -1;
                    }
                    catch
                    {
                        return -1;
                    }
                }
                catch
                {
                    return -1;
                }
            }
            else
                return -1;
        }

        public bool ValidatingAuth()
        {
            Console.WriteLine("Validating auth!");
            try
            {
                JObject obj = JPost(new ETActionRequest("rememberAuth", true)
                {
                    { "creation_date", AuthTime },
                    { "remember", API.RememberingAuth.ToString() }
                });
                JTokenType type = obj["data"].Type;
                if (type == JTokenType.Boolean)
                    Console.WriteLine("Auth is active yet.");
                else if (type == JTokenType.String)
                {
                    Form.ActiveForm.Hide();
                    ProgramCreds.cred.Show();
                    MessageBox.Show("Session timed out!");
                }
                else
                    TokenSha = obj["data"]["token_sha"].ToObject<string>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem checking if the auth is valid! Message:\n\n{0}", ex.ToString());
                OfflineMode = true;
                return false;
            }
        }

        internal static void SafeModifyActiveFormText(string text)
        {
            if (Form.ActiveForm.InvokeRequired)
            {
                ModifyFormTextCallback d = new ModifyFormTextCallback(SafeModifyActiveFormText);
                Form.ActiveForm.Invoke(d, new object[] { text });
            }
            else
            {
                Form.ActiveForm.Text = text;
            }
        }

        public static string GetNewSessionSha()
        {
            return string.Concat(IdentifierHelpers.GetMachineUniqueID(), DateTime.Now.ToSQLDateTime()).CreateMD5();
        }
    }

    public class Lerp2WebDebug
    {
        protected PHPRequestMethod Method;

        //protected string Response;
        protected string Query;

        protected bool Backtrace;

        public Lerp2WebDebug(PHPRequestMethod method, NameValueCollection col, bool trace = false)
        {
            Method = method;
            Query = col.BuildQueryString();
            Backtrace = trace;
        }

        public void DebugResponse(string res)
        {
            Debug(res);
        }

        public void DebugException(Exception ex)
        {
            Debug("", ex);
        }

        private void Debug(string res, Exception ex = null)
        {
            if (!Lerp2Web.outputWebRequests) return;
            Console.WriteLine(string.Format(
                "\n------------\n\n[{0}] Request made at {1} has returned the following:\n\n{2}\n\nQuery string: {3}{4}{5}\n\n------------\n",
                Method,
                DateTime.Now.ToSQLDateTime(),
                !string.IsNullOrEmpty(res) ? res.JsonPrettify() : "",
                Query,
                ex != null ? string.Format("\n\nException:\n\n{0}", ex.ToString()) : "",
                Backtrace ? string.Format("\n\nTrace:\n\n{0}", new StackTrace().ToString()) : ""));
        }
    }

    public class OfflineSession
    {
        internal static int lastId = 0;
        internal static List<OfflineSession> offlineSessions = new List<OfflineSession>();

        public int Id;
        public string Sha;

        public DateTime StartDate,
                        EndDate;

        internal OfflineSession(string sh, DateTime s, DateTime e)
        {
            Id = lastId;
            Sha = sh;
            StartDate = s;
            EndDate = e;
            ++lastId;
        }

        public OfflineSession(string sh, DateTime s)
            : this(sh, s, default(DateTime))
        { }

        public static void Load(string str)
        {
            offlineSessions = ToObject(str);
        }

        public static void StartSession(string sha)
        {
            StartSession(sha, DateTime.Now);
        }

        public static void AddSession(string sha, DateTime start)
        {
            AddSession(sha, start, DateTime.Now);
        }

        public static void AddUnstartedSession(string sha)
        {
            AddUnstartedSession(sha, DateTime.Now);
        }

        public static void StartSession(string sha, DateTime start = default(DateTime))
        {
            offlineSessions.Add(new OfflineSession(sha, start == default(DateTime) ? DateTime.Now : start));
        }

        public static void AddSession(string sha, DateTime start, DateTime end = default(DateTime))
        {
            offlineSessions.Add(new OfflineSession(sha, start, end == default(DateTime) ? DateTime.Now : end));
        }

        public static void AddUnstartedSession(string sha, DateTime end = default(DateTime))
        {
            offlineSessions.Add(new OfflineSession(sha, default(DateTime), end == default(DateTime) ? DateTime.Now : end));
        }

        private static void EndLastSession(bool forceNew = true)
        {
            EndLastSession(DateTime.Now, forceNew);
        }

        private static void EndLastSession(DateTime end, bool forceNew = true)
        {
            OfflineSession obj = offlineSessions.SingleOrDefault(x => x.Id == lastId);
            if (obj == null && forceNew)
            {
                obj = new OfflineSession(obj.Sha, default(DateTime), end);
                offlineSessions.Add(obj);
            }
            else if (obj != null && obj.EndDate == default(DateTime))
                obj.EndDate = end;
        }

        //Send the actual stored sessions
        public static bool SendStoredSessions()
        {
            //Finish last session if needed
            EndLastSession(false);
            try
            {
                if (offlineSessions.Count > 0)
                    foreach (OfflineSession off in offlineSessions)
                    {
                        if (off.StartDate == null && off.EndDate != null)
                        { //Este es para el caso de que empiezes con internet y te quedes sin internet
                            Lerp2Web.instance.Post(new EActionRequest("endStartedSession")
                            {
                                { "sha", off.Sha },
                                { "end_time", off.EndDate.ToSQLDateTime() }
                            });
                        }
                        else if (off.StartDate != null && off.EndDate != null)
                        { //Este es para el caso de una sesion que nunca ha tenido internet
                            Lerp2Web.instance.Post(new EActionRequest("recordNewSession")
                            {
                                { "sha", off.Sha },
                                { "app_id", Lerp2Web.instance.strId },
                                { "start_time", off.StartDate.ToSQLDateTime() },
                                { "end_time", off.EndDate.ToSQLDateTime() }
                            });
                        }
                    }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static bool HasStoredSessions()
        {
            return offlineSessions.Count > 0;
        }

        public static string ToString()
        {
            return JsonConvert.SerializeObject(offlineSessions.ToArray());
        }

        public static List<OfflineSession> ToObject(string str)
        {
            return JsonConvert.DeserializeObject<OfflineSession[]>(str).ToList();
        }
    }

    public class ActionRequest : IEnumerable
    {
        public string Action;
        public bool Detailed;

        protected readonly Dictionary<string, string> _dict = new Dictionary<string, string>();

        public string this[string key]
        {
            get { return _dict[key]; }
            set { _dict.Add(key, value); }
        }

        public ActionRequest(string act, bool det = false)
        {
            Action = act;
            Detailed = det;
        }

        public void Add(string key, string val)
        {
            _dict.Add(key, val);
        }

        public IEnumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public static implicit operator NameValueCollection(ActionRequest req)
        {
            NameValueCollection col = new NameValueCollection();
            col.Add("action", req.Action);
            if (req._dict.Count > 0)
                foreach (KeyValuePair<string, string> kv in req._dict)
                    col.Add(kv.Key, kv.Value);
            if (req.Detailed) col.Add("detailed", "");
            return col;
        }
    }

    public class EActionRequest : ActionRequest
    {
        public string EntityKey;

        public EActionRequest(string act, bool det = false) : this(act, Lerp2Web.EntitySha, det)
        {
        }

        public EActionRequest(string act, string entKey, bool det = false) : base(act, det)
        {
            EntityKey = entKey;
        }

        public static implicit operator NameValueCollection(EActionRequest req)
        {
            NameValueCollection col = new NameValueCollection();
            col.Add("action", req.Action);
            col.Add("ek", req.EntityKey);
            if (req._dict.Count > 0)
                foreach (KeyValuePair<string, string> kv in req._dict)
                    col.Add(kv.Key, kv.Value);
            if (req.Detailed) col.Add("detailed", "");
            return col;
        }
    }

    public class TActionRequest : ActionRequest
    {
        public string TokenKey;

        public TActionRequest(string act, string tokenKey, bool det = false) : base(act, det)
        {
            TokenKey = tokenKey;
        }

        public static implicit operator NameValueCollection(TActionRequest req)
        {
            NameValueCollection col = new NameValueCollection();
            col.Add("action", req.Action);
            col.Add("tk", req.TokenKey);
            if (req._dict.Count > 0)
                foreach (KeyValuePair<string, string> kv in req._dict)
                    col.Add(kv.Key, kv.Value);
            if (req.Detailed) col.Add("detailed", "");
            return col;
        }
    }

    public class ETActionRequest : EActionRequest
    {
        public string TokenKey;

        public ETActionRequest(string act, bool det = false) : this(act, Lerp2Web.EntitySha, Lerp2Web.TokenSha, det)
        {
        }

        public ETActionRequest(string act, string entKey, string tokenKey, bool det = false) : base(act, entKey, det)
        {
            //Console.WriteLine("Token: " + tokenKey);
            TokenKey = tokenKey;
        }

        public static implicit operator NameValueCollection(ETActionRequest req)
        {
            NameValueCollection col = new NameValueCollection();
            col.Add("action", req.Action);
            col.Add("ek", req.EntityKey);
            col.Add("tk", req.TokenKey);
            if (req._dict.Count > 0)
                foreach (KeyValuePair<string, string> kv in req._dict)
                    col.Add(kv.Key, kv.Value);
            if (req.Detailed) col.Add("detailed", "");
            return col;
        }
    }

    public class ActionResponse
    {
        protected static Dictionary<string, JObject> _dict = new Dictionary<string, JObject>();

        public JObject this[string key]
        {
            get { return _dict.ContainsKey(key) ? _dict[key] : null; }
            set
            {
                if (!_dict.ContainsKey(key))
                    _dict.Add(key, value);
                else
                    _dict[key] = value;
            }
        }
    }

    #endregion "Ajax Requests"

    #endregion "Lerp2Web"
}

#region "Junk code"

/*public class PHPResponses
{
    [JsonProperty("success")]
    public string Success
    {
        get { return m_Success; }
        set { m_Success = value; }
    }

    private string m_Success;

    [JsonProperty("error-codes")]
    public List<string> ErrorCodes
    {
        get { return m_ErrorCodes; }
        set { m_ErrorCodes = value; }
    }

    private List<string> m_ErrorCodes;
}

public class PHPRegister : PHPResponses
{
}

public class PHPLogin : PHPResponses
{
}*/

/*public OfflineSession(DateTime s, DateTime e)
{
Id = lastId;
StartDate = s;
EndDate = e;
++lastId;
}*/

/*

     System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
     {
         return this.GetEnumerator();
     }

 */

/*public class AActionRequest : ActionRequest
{
    public string AuthKey;

    public AActionRequest(string act, bool det = false) : this(act, Lerp2Web.AuthSha, det)
    {
    }

    public AActionRequest(string act, string authKey, bool det = false) : base(act, det)
    {
        AuthKey = authKey;
    }

    public static implicit operator NameValueCollection(AActionRequest req)
    {
        NameValueCollection col = new NameValueCollection();
        col.Add("action", req.Action);
        col.Add("ak", req.AuthKey);
        if (req._dict.Count > 0)
            foreach (KeyValuePair<string, string> kv in req._dict)
                col.Add(kv.Key, kv.Value);
        if (req.Detailed) col.Add("detailed", "");
        return col;
    }
}*/

/*public class AEActionRequest : EActionRequest
{
    public string AuthKey;

    public AEActionRequest(string act, bool det = false) : this(act, Lerp2Web.EntitySha, Lerp2Web.AuthSha, det)
    {
    }

    public AEActionRequest(string act, string entKey, string authKey, bool det = false) : base(act, entKey, det)
    {
        AuthKey = authKey;
    }

    public static implicit operator NameValueCollection(AEActionRequest req)
    {
        NameValueCollection col = new NameValueCollection();
        col.Add("action", req.Action);
        col.Add("ek", req.EntityKey);
        col.Add("ak", req.AuthKey);
        if (req._dict.Count > 0)
            foreach (KeyValuePair<string, string> kv in req._dict)
                col.Add(kv.Key, kv.Value);
        if (req.Detailed) col.Add("detailed", "");
        return col;
    }
}

public class ATActionRequest : AActionRequest
{
    public string TokenKey;

    public ATActionRequest(string act, bool det = false) : this(act, Lerp2Web.EntitySha, Lerp2Web.TokenSha, det)
    {
    }

    public ATActionRequest(string act, string authKey, string tokenKey, bool det = false) : base(act, authKey, det)
    {
        TokenKey = tokenKey;
    }

    public static implicit operator NameValueCollection(ATActionRequest req)
    {
        NameValueCollection col = new NameValueCollection();
        col.Add("action", req.Action);
        col.Add("tk", req.TokenKey);
        col.Add("ak", req.AuthKey);
        if (req._dict.Count > 0)
            foreach (KeyValuePair<string, string> kv in req._dict)
                col.Add(kv.Key, kv.Value);
        if (req.Detailed) col.Add("detailed", "");
        return col;
    }
}*/

/*public class AETActionRequest : AEActionRequest
{
    public string TokenKey;

    public AETActionRequest(string act, bool det = false) : this(act, Lerp2Web.EntitySha, Lerp2Web.AuthSha, Lerp2Web.TokenSha, det)
    {
    }

    public AETActionRequest(string act, string entKey, string authKey, string tokenKey, bool det = false) : base(act, entKey, authKey, det)
    {
        TokenKey = tokenKey;
    }

    public static implicit operator NameValueCollection(AETActionRequest req)
    {
        NameValueCollection col = new NameValueCollection();
        col.Add("action", req.Action);
        col.Add("ek", req.EntityKey);
        col.Add("ak", req.AuthKey);
        col.Add("tk", req.TokenKey);
        if (req._dict.Count > 0)
            foreach (KeyValuePair<string, string> kv in req._dict)
                col.Add(kv.Key, kv.Value);
        if (req.Detailed) col.Add("detailed", "");
        return col;
    }
}*/

#endregion "Junk code"

//Finish