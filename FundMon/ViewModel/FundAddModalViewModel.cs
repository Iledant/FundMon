using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class FundAddModalViewModel : ObservableObject
{
    [ObservableProperty]
    List<MorningstarResponseLine> results;
    public async void FundSearch(string pattern)
    {
        Results = await MorningStarHelpers.FetchFunds(pattern);
    }
}
