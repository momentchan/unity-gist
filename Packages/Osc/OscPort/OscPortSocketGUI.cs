#define USE_GUI

#if USE_GUI
using mj.gist;
using PrefsGUI;
using PrefsGUI.RapidGUI;

namespace Osc
{
    public class OscPortSocketGUI : OscPortSocket, IGUIUser
    {
        public override int LocalPort => prefsLocalPort;
        public override string DefaultRemoteHost => prefsDefaultRemoteHost;
        public override int DefaultRemotePort => prefsDefaultRemotePort;
        public override int LimitReceiveBuffer => prefsLimitReceiveBuffer;

        protected PrefsInt prefsLocalPort;
        protected PrefsString prefsDefaultRemoteHost;
        protected PrefsInt prefsDefaultRemotePort;
        protected PrefsInt prefsLimitReceiveBuffer;

#region GUI
        public string GetName() => name;

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

            base.OnEnable();
        }
    }
}
#else
#endif