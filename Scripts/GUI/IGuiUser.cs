namespace mj.gist
{
    public interface IGUIUser
    {
        string GetName();
        void ShowGUI();
        void SetupGUI();
    }

    public interface IGUIPartial
    {
        void PartialGUI();
    }
}