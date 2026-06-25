namespace PartnerManagement.Data;

public interface IDatabaseInitializer
{
    Task EnsureCreatedAsync();
}