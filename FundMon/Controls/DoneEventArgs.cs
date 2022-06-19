using System;

namespace FundMon.Controls;

public class DoneEventArgs : EventArgs
{
    public bool Escaped { get; set; }
    public DoneEventArgs(bool escaped)
    {
        Escaped = escaped;
    }
}
