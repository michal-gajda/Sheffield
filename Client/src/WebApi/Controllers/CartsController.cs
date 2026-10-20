namespace Sheffield.Client.WebApi.Controllers;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sheffield.Shared.Grains;

[ApiController, Route("api/[controller]")]
public sealed class CartsController(IGrainFactory grainFactory) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CartSnapshot>> Create()
    {
        var cartId = Guid.NewGuid().ToString("N");
        var cart = await grainFactory.GetGrain<ICartGrain>(cartId).CreateAsync();

        return CreatedAtAction(nameof(Get), new { cartId }, cart);
    }

    [HttpGet("{cartId}")]
    public async Task<ActionResult<CartSnapshot>> Get(string cartId)
    {
        var cart = await grainFactory.GetGrain<ICartGrain>(cartId).GetAsync();

        return cart is null ? NotFound() : Ok(cart);
    }

    [HttpPost("{cartId}/products/add")]
    public Task<ActionResult<CartSnapshot>> AddProducts(
        string cartId,
        QuantityRequest request) =>
        ChangeProductCount(cartId, request.Quantity, add: true);

    [HttpPost("{cartId}/products/subtract")]
    public Task<ActionResult<CartSnapshot>> SubtractProducts(
        string cartId,
        QuantityRequest request) =>
        ChangeProductCount(cartId, request.Quantity, add: false);

    private async Task<ActionResult<CartSnapshot>> ChangeProductCount(
        string cartId,
        int quantity,
        bool add)
    {
        var grain = grainFactory.GetGrain<ICartGrain>(cartId);
        var cart = add
            ? await grain.AddProductsAsync(quantity)
            : await grain.SubtractProductsAsync(quantity);

        if (cart is null)
        {
            return add
                ? NotFound()
                : Conflict(new { message = "Cart not found or does not contain enough products." });
        }

        return Ok(cart);
    }

    public sealed record QuantityRequest(
        [param: Range(1, int.MaxValue)] int Quantity);
}
