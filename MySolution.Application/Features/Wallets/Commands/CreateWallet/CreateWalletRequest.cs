namespace MySolution.Application.Features.Wallets.Commands.CreateWallet;

public class CreateWalletRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}