using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrefsGUI;
using PrefsGUI.RapidGUI;
using RapidGUI;
using UnityEngine;

namespace mj.gist
{
    public class StatusManager : MonoBehaviour
    {
        [SerializeField] private Vector2 position;
        [SerializeField] KeyCode settingKey = KeyCode.E;
        private List<IStatus> status;
        private bool show = false;

        private Rect windowRect = new()
        {
            width = Screen.width * 0.5f
        };

        private void Start()
        {
            status = FindObjectsOfType<MonoBehaviour>(true).OfType<IStatus>().ToList();
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
                foreach (var s in status)
                    s.ShowStatus();
                GUI.DragWindow();
            },
            "Status Manager");
        }
    }
}