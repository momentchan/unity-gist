#define USE_GUI

#if USE_GUI
using mj.gist;
using PrefsGUI;
using PrefsGUI.RapidGUI;

namespace Osc
{
    public class OscPortSocketGUI : OscPortSocket, IGUIUser
    {
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
            prefsLocalPort.DoGUI();
            prefsDefaultRemoteHost.DoGUI();
            prefsDefaultRemotePort.DoGUI();
            prefsLimitReceiveBuffer.DoGUI();
        }
        #endregion

        protected override void OnEnable()
        {
            prefsLocalPort = new PrefsInt($"{GetName()}_localPort", 8887);
            prefsDefaultRemoteHost = new PrefsString($"{GetName()}_defaultRemoteHost", "localhost");
            prefsDefaultRemotePort = new PrefsInt($"{GetName()}_defaultRemotePort", 8888);
            prefsLimitReceiveBuffer = new PrefsInt($"{GetName()}_limitReceiveBuffer", 30);

            localPort = prefsLocalPort;
            defaultRemoteHost = prefsDefaultRemoteHost;
            defaultRemotePort = prefsDefaultRemotePort;
            limitReceiveBuffer = prefsLimitReceiveBuffer;

            base.OnEnable();
        }
    }
}
#else
#endif