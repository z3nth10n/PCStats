using Lerp2Web;
using System;
using System.Windows.Forms;
using LiteProgram = LiteLerped_WF_API.Program;

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
        //internal static frmMain main;

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
            //LiteProgram.cred = new frmCredentials();
            //---------------------------------
            Run(new frmMain());
            //---
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Close();
        }

        private static void Run(frmMain main)
        {
            Console.WriteLine("Running program!");

            //Run LiteLerped-WF-API
            LiteProgram.Run("pcstats_", main);

            Application.Run(main);
        }

        public static void Close()
        {
            //Save config
            API.config.Save();

            //End session...
            LiteProgram.web.EndSession(LiteProgram.web.sessionSha);
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