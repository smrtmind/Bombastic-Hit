using System;
using System.Collections.Generic;
using UnityEngine;
using static Codebase.Utils.Enums;

namespace CodeBase.UI
{
    public class UIController : MonoBehaviour
    {
        public static List<IInterfacePanel> InterfacePanels { get; set; } = new List<IInterfacePanel>();
        public static Action<UIPanelType> ShowUIPanelAloneAction = default;
        public static Action<UIPanelType> ShowUIPanelAlongAction = default;

        private void OnEnable()
        {
            ShowUIPanelAloneAction += ShowPanelAlone;
            ShowUIPanelAlongAction += ShowPanelAlong;
        }

        private void OnDisable()
        {
            ShowUIPanelAloneAction -= ShowPanelAlone;
            ShowUIPanelAlongAction -= ShowPanelAlong;
        }

        private void ShowPanelAlone(UIPanelType uIPanelType)
        {
            InterfacePanels.Find(somePanel => somePanel.UIPanelType == uIPanelType)?.Show();
            var temp = InterfacePanels.FindAll(somePanel => somePanel.UIPanelType != uIPanelType);
            foreach (var item in temp)
            {
                item.Hide();
            }
        }

        private void ShowPanelAlong(UIPanelType uIPanelType)
        {
            InterfacePanels.Find(somePanel => somePanel.UIPanelType == uIPanelType)?.Show();
        }
    }
}
