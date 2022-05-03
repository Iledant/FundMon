using FundMon.Repository;

namespace FundMon.Pages;

public record RepoAndSelectedPortfolio
{
    public Portfolio SelectedPortfolio;
    public Repo Repo;

    public void Deconstruct(out Portfolio portfolio, out Repo repo)
    {
        portfolio = SelectedPortfolio;
        repo = Repo;
    }
}
