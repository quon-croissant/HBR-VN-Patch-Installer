using System.ComponentModel;

public sealed class CustomProgressBar : ProgressBar
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color TrackColor { get; set; } = Color.FromArgb(118, 96, 136);

    public CustomProgressBar()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(TrackColor);

        if (Maximum <= Minimum || Value <= Minimum)
            return;

        var progress = (Value - Minimum) / (double)(Maximum - Minimum);
        var fillWidth = (int)Math.Round(progress * Width);
        if (fillWidth <= 0)
            return;

        using var brush = new SolidBrush(ForeColor);
        e.Graphics.FillRectangle(brush, 0, 0, fillWidth, Height);
    }
}
