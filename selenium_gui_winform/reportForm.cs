using System.ComponentModel;

namespace selenium_gui_winform;

public partial class reportForm : Form {
    private string _domain = "";
    private int _page;
    private string _path = "";

    private ScanPoint[] _pointArr;
    private bool _saved = false;
    private bool _result;
    private int _scan;
    private string _url = "";

    public reportForm(string url, string domain, bool result, string path) {
        InitializeComponent();
        _url    = url;
        _domain = domain;
        _result = result;
        _path   = path;
    }

    private void reportForm_Load(object sender, EventArgs e) {
        LangInit();
        SearchDetect();

        lbl_Result.ForeColor = _result ? Color.Red : default;

        list_Detail.Items.Add(res.text_0 + _url);
        list_Detail.Items.Add("");
        list_Detail.Items.Add(res.text_1);

        foreach (var cur in _pointArr) list_Detail.Items.Add($"{cur.File} -> <{cur.Tag}> <- {cur.Payload}");
        list_Detail.Items.Add(@"");
        list_Detail.Items.Add(res.text_2);
        list_Detail.Items.Add(@"");
        list_Detail.Items.Add(res.text_3);
        list_Detail.Items.Add(res.text_4);
    }

    private void reportForm_Closing(object sender, CancelEventArgs e) {
        if (!_saved &&
            MessageBox.Show(res.string_NotSave, res.error, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
            DialogResult.No) e.Cancel = true;
    }

    private struct ScanPoint {
        public readonly string File;
        public readonly string Tag;
        public readonly string Payload;

        public ScanPoint(string file, string tag, string payload) {
            File    = file;
            Tag     = tag;
            Payload = payload;
        }
    }

    /// <summary>
    ///     Apply localization
    /// </summary>
    private void LangInit() {
        Text                   = res.string_EndScanReport + @" - " + _url;
        lbl_Link.Text          = _url;
        lbl_Count.Text         = _scan.ToString();
        lbl_CounfOfFind.Text   = _page.ToString();
        lbl_CountOfDetect.Text = res.lbl_CountOfDetect + @":";
        lbl_Target.Text        = res.lbl_Target + @":";
        lbl_Status.Text        = res.lbl_Status + @":";
        gbDetailed.Text        = res.gb_Deatil;
        lbl_Link.Text          = res.link_ReadMore;
        btn_Save.Text          = res.btn_SaveAs;
        btn_Close.Text         = res.btn_Exit;
        lbl_Result.Text        = _result ? res.status_Found : res.status_NotFound;
    }

    /// <summary>
    ///     Get Count of find page & detect page
    /// </summary>
    private void SearchDetect() {
        _page = Directory.GetFiles(_path, $"{_domain}_*.txt", SearchOption.AllDirectories).Length;
        _scan = Directory.GetFiles(_path, @"_injectable_*.txt", SearchOption.AllDirectories).Length;

        _pointArr = new ScanPoint[_scan];
        for (var i = 0; i < _scan; i++)
            //TODO: Read Format of _injectable_*.txt
            _pointArr[i] = new ScanPoint("File", "Tag", "Payload");
    }

    private void btn_Close_Click(object sender, EventArgs e) {
        // Call Closing EventHandler
        Close();
    }

    /// <summary>
    /// Save list to text file
    /// </summary>
    private void btn_Save_Click(object sender, EventArgs e) {
        StreamWriter sw;
        sw = new StreamWriter(Path.Combine(_path, @"_savedReport_.txt"));

        for (var i = 0; i < list_Detail.Items.Count; i++) {
            list_Detail.Items[i] += "\r\n";

            sw.Write(list_Detail.Items[i]);
        }

        sw.Close();
        _saved = true;
    }
}