namespace mj.gist
{
    public interface IGUIUser
    {
        string GetName();
        void ShowGUI();
    }

    public interface IGUIPartial
    {
        void PartialGUI();
    }
}