using System.Net.Http.Json;
using EShop.Web.Models;

namespace EShop.Web.Clients;

public class ShoppingCartApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ProductResponse>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/products", cancellationToken);
        return await ReadJsonAsync<List<ProductResponse>>(response, cancellationToken) ?? [];
    }

    public async Task<ShoppingCartResponse> GetCartAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/cart", cancellationToken);
        return await ReadRequiredJsonAsync<ShoppingCartResponse>(response, cancellationToken);
    }

    public async Task<ShoppingCartResponse> AddItemAsync(
        int productId,
        int quantity = 1,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/cart/items",
            new AddCartItemRequest { ProductId = productId, Quantity = quantity },
            cancellationToken);

        return await ReadRequiredJsonAsync<ShoppingCartResponse>(response, cancellationToken);
    }

    public async Task RemoveItemAsync(int productId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/cart/items/{productId}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<OrderResponse> CheckoutAsync(string customerName, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/cart/checkout",
            new CheckoutRequest { CustomerName = customerName },
            cancellationToken);

        return await ReadRequiredJsonAsync<OrderResponse>(response, cancellationToken);
    }

    private static async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private static async Task<T> ReadRequiredJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var result = await ReadJsonAsync<T>(response, cancellationToken);
        return result ?? throw new HttpRequestException("The API returned an empty response.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(message)
                ? $"The API request failed with status code {(int)response.StatusCode}."
                : message,
            null,
            response.StatusCode);
    }
}
