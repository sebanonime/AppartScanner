namespace Appart.Adapter
{
    public interface IAppartAdapter
    {
        string GetDetailUrl(ref string webPage);
        void SetAppartInfo(Appartement appart, string webPage, string url);
        bool PageEnded(string webPage);
    }
}