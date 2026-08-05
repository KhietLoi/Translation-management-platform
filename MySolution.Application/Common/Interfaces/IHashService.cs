namespace MySolution.Application.Common.Interfaces;

public interface IHashService
{
    string ComputeHash(string value);
    bool Verify(string value, string hash);
    
}