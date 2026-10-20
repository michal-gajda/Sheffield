namespace Sheffield.Shared.Grains;

public interface ICartGrain : IGrainWithStringKey
{
    Task<CartSnapshot> CreateAsync();

    Task<CartSnapshot?> GetAsync();

    Task<CartSnapshot?> AddProductsAsync(int quantity);

    Task<CartSnapshot?> SubtractProductsAsync(int quantity);
}
