﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;

using MyTesla.Core.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyTesla.Mobile
{
    public class TeslaAPI
    {
        HttpClient _client = null;


        public TeslaAPI(string accessToken)
        {
            _client = new HttpClient { BaseAddress = Constants.TESLA_API_BASEADDRESS };

            // Set access token in auth header.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }


        public async Task<AccessToken> GetAccessToken(string email, string password) {
            AccessToken accessToken = null;

            using (var content = new StringContent(String.Empty)) {
                var requestUri = $"oauth/token?grant_type=password&client_id={Constants.TESLA_CLIENT_ID}&client_secret={Constants.TESLA_CLIENT_SECRET}&email={email}&password={password}";

                using (var response = _client.PostAsync(requestUri, content).Result) {
                    string responseData = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<LoginResponse>(responseData);

                    accessToken = new AccessToken {
                        Token = model.access_token,
                        ExpirationDate = DateTime.Now.AddSeconds(Convert.ToInt32(model.expires_in))
                    };

                    // Set access token in auth header.
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                }
            }

            return accessToken;
        }


        public async Task<List<Vehicle>> GetVehicles()
        {
            List<Vehicle> vehicles = null;

            using (var response = await _client.GetAsync("api/1/vehicles"))
            {
                var responseString = response.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    vehicles = JsonConvert.DeserializeObject<TeslaResponse<List<Vehicle>>>(responseString).Content;
                }
            }

            return vehicles ?? new List<Vehicle>();
        }


        public async Task<ChargeState> GetChargeState(long vehicleId)
        {
            ChargeState chargeState = null;

            using (var response = await _client.GetAsync($"api/1/vehicles/{vehicleId}/data_request/charge_state"))
            {
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    chargeState = JsonConvert.DeserializeObject<TeslaResponse<ChargeState>>(responseString).Content;
                }
            }

            return chargeState;
        }
        

        public async Task<DriveState> GetDriveState(long vehicleId)
        {
            DriveState driveState = null;

            using (var response = await _client.GetAsync($"api/1/vehicles/{vehicleId}/data_request/drive_state"))
            {
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    driveState = JsonConvert.DeserializeObject<TeslaResponse<DriveState>>(responseString).Content;
                }
            }

            return driveState;
        }


        protected bool ValidateApiReponse(HttpResponseMessage response, string apiCallDescription) {
            if (!response.IsSuccessStatusCode) {
                var errorMessage = $"Failed to {apiCallDescription}. \nDetails: " + response.Content.ReadAsStringAsync().Result;
                return false;
            }

            return true;
        }


        ~TeslaAPI() {
            _client.Dispose();
        }

    }
}
