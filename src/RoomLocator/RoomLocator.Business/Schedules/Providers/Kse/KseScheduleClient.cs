using System.Net.Http.Json;

namespace RoomLocator.Business.Schedules.Providers.Kse;

public class KseScheduleClient : IKseScheduleClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://schedule.kse.ua/index";

    public KseScheduleClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<KseRoom>> GetRoomsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<GetKseRoomsResponse>($"{_baseUrl}/auditoriums");

        return response.Result;
    }

    public async Task<string> GetIcalContentByRoomAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/ical?id_grp=0&id_aud={id}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
