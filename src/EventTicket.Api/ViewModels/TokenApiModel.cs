using System.Text.Json.Serialization;

namespace EventTicket.Api.ViewModels;

public record TokenApiModel(
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken
);