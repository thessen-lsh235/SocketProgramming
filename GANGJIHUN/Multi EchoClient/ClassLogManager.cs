using System;
using System.Windows.Forms;

public class ClassLogManager
{
    private ListView _listViewLog;

    public ClassLogManager(ListView listViewLog)
    {
        _listViewLog = listViewLog;
        initializeListView();
    }

    private void initializeListView()
    {
        _listViewLog.View = View.Details;
        _listViewLog.Columns.Add("시간", 100, HorizontalAlignment.Left);
        _listViewLog.Columns.Add("보낸 데이터", 200, HorizontalAlignment.Left);
    }

    //
    //  listview 데이터 추가 관련
    //  addLog, addLogLogic
    //
    #region log logic
    public void addLog(string sent)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");

        if (_listViewLog.InvokeRequired)
        {
            _listViewLog.Invoke(new Action(() =>
            {
                addLogLogic(time, sent);
            }));
        }
        else
        {
            addLogLogic(time, sent);
        }
    }

    private void addLogLogic(string time, string sent)
    {
        ListViewItem item = new ListViewItem(time);
        item.SubItems.Add(sent);
        _listViewLog.Items.Add(item);
        _listViewLog.EnsureVisible(_listViewLog.Items.Count - 1);
    }
    #endregion
}