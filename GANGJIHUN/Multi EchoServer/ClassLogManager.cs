using System;
using System.Windows.Forms;

public class ClassLogManager
{
    private ListView _listViewLog;

    public ClassLogManager(ListView listViewLog)
    {
        _listViewLog = listViewLog;
        InitializeListView();
    }

    private void InitializeListView()
    {
        _listViewLog.View = View.Details;
        _listViewLog.Columns.Clear();
        _listViewLog.Columns.Add("시간", 100, HorizontalAlignment.Left);
        _listViewLog.Columns.Add("데이터", 300, HorizontalAlignment.Left);
    }

    public void addLog(string message)
    {
        if (_listViewLog.InvokeRequired)
        {
            _listViewLog.Invoke(new Action<string>(addLog), message);
        }
        else
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            ListViewItem item = new ListViewItem(time);
            item.SubItems.Add(message);
            _listViewLog.Items.Add(item);
            _listViewLog.EnsureVisible(_listViewLog.Items.Count - 1);
        }
    }
}