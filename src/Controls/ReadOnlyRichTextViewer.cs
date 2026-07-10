public sealed class ReadOnlyRichTextViewer : RichTextBox
{
    public void SuppressCaretAndSelection()
    {
        SelectionStart = 0;
        SelectionLength = 0;
        HideCaret(Handle);
    }

    protected override void WndProc(ref Message m)
    {
        const int wmSetFocus = 0x0007;
        const int wmLButtonDown = 0x0201;
        const int wmLButtonUp = 0x0202;
        const int wmLButtonDblClk = 0x0203;
        const int wmMouseMove = 0x0200;
        const int wmSetCursor = 0x0020;

        if (m.Msg is wmSetFocus)
        {
            base.WndProc(ref m);
            SuppressCaretAndSelection();
            return;
        }

        if (m.Msg is wmLButtonDown or wmLButtonUp or wmLButtonDblClk)
        {
            SuppressCaretAndSelection();
            return;
        }

        if (m.Msg == wmMouseMove && (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
        {
            SuppressCaretAndSelection();
            return;
        }

        base.WndProc(ref m);

        if (m.Msg == wmSetCursor)
            Cursor.Current = Cursors.Arrow;
    }

    protected override void OnSelectionChanged(EventArgs e)
    {
        if (SelectionLength > 0)
            SuppressCaretAndSelection();

        base.OnSelectionChanged(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        SuppressCaretAndSelection();
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool HideCaret(IntPtr hWnd);
}
