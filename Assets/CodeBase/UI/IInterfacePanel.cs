using static Codebase.Utils.Enums;

namespace CodeBase.UI
{
    public interface IInterfacePanel
    {
        UIPanelType UIPanelType { get; }

        void Show();

        void Hide();

        void Init();
    }
}
