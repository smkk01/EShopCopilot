using System.Net.Http.Json;
using EShop.Web.Models;

namespace EShop.Web.Clients;

public class CustomerApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/customers", cancellationToken);
        return await ReadJsonAsync<List<CustomerResponse>>(response, cancellationToken) ?? [];
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/customers/{id}", cancellationToken);
        return await ReadRequiredJsonAsync<CustomerResponse>(response, cancellationToken);
    }

    public async Task<CustomerResponse> CreateAsync(
        CustomerRequest customer,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/customers", customer, cancellationToken);
        return await ReadRequiredJsonAsync<CustomerResponse>(response, cancellationToken);
    }

    public async Task UpdateAsync(
        Guid id,
        CustomerRequest customer,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"api/customers/{id}", customer, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/customers/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
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
