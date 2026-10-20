namespace Sheffield.Shared.Services;

using Orleans;
using Orleans.Runtime;
using Sheffield.Shared.Grains;

public sealed class CartGrain(
    [PersistentState("cart", "Default")] IPersistentState<CartState> state)
    : Grain, ICartGrain
{
    public async Task<CartSnapshot> CreateAsync()
    {
        if (!state.RecordExists)
        {
            await state.WriteStateAsync();
        }

        return CreateSnapshot();
    }

    public Task<CartSnapshot?> GetAsync()
    {
        return Task.FromResult(
            state.RecordExists ? CreateSnapshot() : null);
    }

    public async Task<CartSnapshot?> AddProductsAsync(int quantity)
    {
        if (!state.RecordExists)
        {
            return null;
        }

        state.State.ProductCount = checked(state.State.ProductCount + quantity);
        await state.WriteStateAsync();

        return CreateSnapshot();
    }

    public async Task<CartSnapshot?> SubtractProductsAsync(int quantity)
    {
        if (!state.RecordExists || state.State.ProductCount < quantity)
        {
            return null;
        }

        state.State.ProductCount -= quantity;
        await state.WriteStateAsync();

        return CreateSnapshot();
    }

    private CartSnapshot CreateSnapshot() =>
        new()
        {
            Id = this.GetPrimaryKeyString(),
            ProductCount = state.State.ProductCount
        };
}
