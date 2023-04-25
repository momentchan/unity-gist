#define USE_GUI

#if USE_GUI
using System.Linq;
using mj.gist;
using PrefsGUI;
using PrefsGUI.RapidGUI;
using UnityEngine;

namespace Osc
{
    public class OscPortSocketGUI : OscPortSocket, IGUIUser
    {
        protected PrefsBool prefsSendToLocal;
        protected PrefsInt prefsLocalPort;
        protected PrefsString prefsDefaultRemoteHost;
        protected PrefsInt prefsDefaultRemotePort;
        protected PrefsInt prefsLimitReceiveBuffer;

        #region GUI
        public string GetName() => name;

        public void SetupGUI()
        {
        }

        public void ShowGUI()
        {
            prefsSendToLocal.DoGUI();
            prefsLocalPort.DoGUI();
            prefsDefaultRemoteHost.DoGUI();
            prefsDefaultRemotePort.DoGUI();
            prefsLimitReceiveBuffer.DoGUI();
        }
        #endregion

        protected override void OnEnable()
        {
            prefsSendToLocal = new PrefsBool($"{GetName()}_sendToLocal", false);
            prefsLocalPort = new PrefsInt($"{GetName()}_localPort", 8887);
            prefsDefaultRemoteHost = new PrefsString($"{GetName()}_defaultRemoteHost", "localhost");
            prefsDefaultRemotePort = new PrefsInt($"{GetName()}_defaultRemotePort", 8888);
            prefsLimitReceiveBuffer = new PrefsInt($"{GetName()}_limitReceiveBuffer", 30);

            localPort = prefsLocalPort;
            defaultRemoteHost = prefsDefaultRemoteHost;
            defaultRemotePort = prefsDefaultRemotePort;
            limitReceiveBuffer = prefsLimitReceiveBuffer;

            var users = FindObjectsOfType<MonoBehaviour>(true).OfType<IOscUser>().ToList();
            foreach (var user in users)
                user.Server = this;

            base.OnEnable();
        }

        protected override void Update()
        {
            base.Update();
            IsSendToLocal = prefsSendToLocal;
        }
    }
}
#else
#endif