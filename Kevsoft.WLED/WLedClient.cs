﻿using System.Net.Http.Json;
using System.Text.Json;

namespace Kevsoft.WLED;

public sealed class WLedClient
{
    private readonly HttpClient _client;

    public WLedClient(HttpMessageHandler httpMessageHandler, string baseUri)
    {
        _client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri(baseUri, UriKind.Absolute)
        };

        // Add the keep-alive flag to the header
        _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    public WLedClient(string baseUri) : this(new HttpClientHandler(), baseUri)
    {
    }

    public async Task<WLedRootResponse> Get()
    {
        var message = await _client.GetAsync("json");

        message.EnsureSuccessStatusCode();
        WLedRootResponse response = null;
        var json = await message.Content.ReadAsStringAsync();
        if (json.StartsWith("{"))
        {
            try
            {
                response = JsonSerializer.Deserialize<WLedRootResponse>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return response;
    }

    public async Task<StateResponse> GetState()
    {
        var message = await _client.GetAsync("json/state");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<StateResponse>())!;
    }

    public async Task<InformationResponse> GetInformation()
    {
        var message = await _client.GetAsync("json/info");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<InformationResponse>())!;
    }

    public async Task<string[]> GetEffects()
    {
        var message = await _client.GetAsync("json/eff");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<string[]>())!;
    }

    public async Task<string[]> GetPalettes()
    {
        var message = await _client.GetAsync("json/pal");

        message.EnsureSuccessStatusCode();

        return (await message.Content.ReadFromJsonAsync<string[]>())!;
    }

    public async Task Post(WLedRootRequest request)
    {
        var stateString = JsonSerializer.Serialize(request);

        using var content = new StringContentWithoutCharset(stateString, "application/json");
        var result = await _client.PostAsync("/json", content);
        result.EnsureSuccessStatusCode();
    }

    public async Task Post(StateRequest request)
    {
        var stateString = JsonSerializer.Serialize(request);

        Console.WriteLine(stateString);

        using var content = new StringContentWithoutCharset(stateString, "application/json");
        var result = await _client.PostAsync("/json/state", content);
        result.EnsureSuccessStatusCode();
    }
}