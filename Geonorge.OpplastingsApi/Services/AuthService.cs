﻿using Geonorge.OpplastingsApi.Models.Api.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Geonorge.OpplastingsApi.Services
{
    public class AuthService : IAuthService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly AuthConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IOptions<AuthConfiguration> options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            _config = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<User> GetUser()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authTokens);

            var authToken = authTokens.SingleOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(authToken))
                return null;

            return await GetUserFromToken(authToken);
        }

        private async Task<User> GetUserFromToken(string authToken)
        {
            var byteArray = Encoding.ASCII.GetBytes(_config.IntrospectionCredentials);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var formUrlEncodedContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("token", authToken),
                new KeyValuePair<string, string>("client_id", _config.ClientId),
                new KeyValuePair<string, string>("client_secret", _config.ClientSecret)
            }
            );

            try
            {
                using var response = await _httpClient.PostAsync(_config.IntrospectionUrl, formUrlEncodedContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                var isActiveToken = json["active"]?.Value<bool>() ?? false;

                if (isActiveToken)
                {
                    var username = json["username"]?.Value<string>();

                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        if (username.Contains('@'))
                            username = username.Split('@')[0];

                        return await GetUserInfo(username, authToken);
                    }
                }

                _logger.LogError($"Could not get user info from token.");
                return null;

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Could not get user info from token.");
                return null;
            }
        }

        private async Task<User> GetUserInfo(string username, string authToken)
        {
            var userViewModel = new User();

            await SetOrganizationInfo(userViewModel, username, authToken);

            if (string.IsNullOrWhiteSpace(userViewModel.OrganizationName))
                return null;

            userViewModel.Roles = await GetRoles(username, authToken);

            return userViewModel.Roles != null ? userViewModel : null;
        }

        private async Task SetOrganizationInfo(User userViewModel, string username, string authToken)
        {
            var geoIdUserInfoUrl = $"{_config.BaatAuthzApiUrl}info/{username}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            try
            {
                _logger.LogInformation($"Request geoIdUserInfoUrl: {geoIdUserInfoUrl}");
                using var response = await _httpClient.GetAsync(geoIdUserInfoUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(responseBody);
                var json = JObject.Parse(responseBody);
                var organization = json["baat_organization"];

                if (organization == null)
                {
                    _logger.LogError($"Could not load organization info for user '{username}'.");
                    return;
                }

                userViewModel.OrganizationName = organization["name"].ToString();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Could not load organization info for user '{username}'.");
                return;
            }
        }

        private async Task<List<string>> GetRoles(string username, string authToken)
        {
            var geoIdUserInfoUrl = $"{_config.BaatAuthzApiUrl}info/{username}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            try
            {
                using var response = await _httpClient.GetAsync(geoIdUserInfoUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                return json["baat_services"]?.ToObject<List<string>>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Could not get roles for user '{username}'.");
                return null;
            }
        }
    }

    public interface IAuthService
    {
        Task<User> GetUser();
    }
}