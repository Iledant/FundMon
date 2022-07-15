using FundMon.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.xUnit.Test;

public class ModeFixture
{
    public ModeFixture()
    {
        AppMode.IsTestRunning = true;
    }
}
