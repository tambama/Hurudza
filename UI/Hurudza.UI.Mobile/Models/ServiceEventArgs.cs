namespace Hurudza.UI.Mobile.Models;

public class ServiceEventArgs<T> : EventArgs
    where T : class
{
    public ServiceEventArgs(ListAction action, T item)
    {
        Action = action;
        Item = item;
    }

    public ListAction Action { get; }
    public T Item { get; }

    public enum ListAction { Add, Delete, Update };
}
