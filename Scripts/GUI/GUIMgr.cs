using System.Collections.Generic;
using kmty.editor;
using UnityEngine;

namespace mj.gist
{
    public abstract class GUIMgr : MonoBehaviour
    {

        protected List<IGUIUser> GUIs;
        protected List<bool> openGUIs;

        protected abstract void SetupGUI();

        public virtual void OnWindow()
        {
            for (int i = 0, n = GUIs.Count; i < n; i++)
            {
                var gui = GUIs[i];
                var name = gui.GetName();
                var open = openGUIs[i];
                using (new FoldoutScope(ref open, name)) { if (open) gui.ShowGUI(); }
                openGUIs[i] = open;
            }
        }
    }
}
