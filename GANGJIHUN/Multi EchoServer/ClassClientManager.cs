using System;
using System.Windows.Forms;

public class ClassClientManager
{
    private ListView _listView;

    public ClassClientManager(ListView listView)
    {
        _listView = listView;
        InitializeListView();
    }

    private void InitializeListView()
    {
        _listView.View = View.Details;
        _listView.Columns.Clear();
        _listView.Columns.Add("클라이언트", 200, HorizontalAlignment.Left);
        _listView.FullRowSelect = true;
    }

    public string getSelectedClientKey()
    {
        if (_listView.InvokeRequired)
        {
            return (string)_listView.Invoke(new Func<string>(getSelectedClientKey));
        }
        else
        {
            if (_listView.SelectedItems.Count > 0)
            {
                return _listView.SelectedItems[0].Text;
            }
            else
            {
                return null;
            }
        }
    }

    #region manage Client State
    public void addClient(string clientKey)
    {
        if (_listView.InvokeRequired)
        {
            _listView.Invoke(new Action<string>(addClient), clientKey);
        }
        else
        {
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Text == clientKey)
                {
                    return;
                }
            }

            _listView.Items.Add(new ListViewItem(clientKey));
        }
    }

    public void removeClient(string clientKey)
    {
        if (_listView.InvokeRequired)
        {
            _listView.Invoke(new Action<string>(removeClient), clientKey);
        }
        else
        {
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Text == clientKey)
                {
                    _listView.Items.Remove(item);
                    break;
                }
            }
        }
    }

    public void clearAllClients()
    {
        if (_listView.InvokeRequired)
        {
            _listView.Invoke(new MethodInvoker(clearAllClients));
        }
        else
        {
            _listView.Items.Clear();
        }
    }
    #endregion    
}