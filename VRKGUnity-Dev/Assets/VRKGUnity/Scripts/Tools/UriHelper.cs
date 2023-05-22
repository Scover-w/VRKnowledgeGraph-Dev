public static class UriHelper
{
    public static bool Split(string uri, out string prefix, out string suffix)
    {
        int id = uri.LastIndexOf('#');

        if (id == -1)
            id = uri.LastIndexOf('/');

        prefix = "";
        suffix = "";

        if (id == -1)
            return false;

        prefix = uri.Substring(0, id + 1);
        suffix = uri.Substring(id + 1);

        return true;
    }
}