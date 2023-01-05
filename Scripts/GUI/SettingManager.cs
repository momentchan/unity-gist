using System.Linq;
using PrefsGUI;
using PrefsGUI.RapidGUI;
using RapidGUI;
using UnityEngine;

namespace mj.gist
{
    public class SettingManager : MonoBehaviour
    {
        [SerializeField] private Vector2 position;
        [SerializeField] KeyCode settingKey = KeyCode.E;

        private bool show = false;

        private WindowLaunchers windowLaunchers;
        private Rect windowRect = new()
        {
            width = Screen.width * 0.5f
        };

        private void Start()
        {
            var users = FindObjectsOfType<MonoBehaviour>().OfType<IGUIUser>().ToList();

            windowLaunchers = new WindowLaunchers
            {
                isWindow = false
            };
            foreach (var user in users)
            {
                windowLaunchers.Add(user.GetName(), () => user.ShowGUI());
            }
            windowLaunchers.Add("PrefsSearch", PrefsSearch.DoGUI).SetWidth(600f).SetHeight(800f);

            windowRect.position = position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(settingKey))
                show = !show;
        }

        public void OnGUI()
        {
            if (!show) return;
            windowRect = RGUI.ResizableWindow(GetHashCode(), windowRect, (id) =>
            {
                windowLaunchers.DoGUI();
                GUILayout.Space(50f);
                GUILayout.Label($"file path: {PrefsGUI.Kvs.PrefsKvsPathSelector.path}");

                if (GUILayout.Button("Save")) Prefs.Save();
                GUI.DragWindow();
            },
            "Setting Manager");
        }
    }
}