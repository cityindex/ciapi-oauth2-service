﻿using System;
using System.Web;
using System.Web.Mvc;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Configuration;

namespace CIAUTH.Code
{
    public static class Utilities
    {
        public static JsonResult CreateErrorJson(string error, string errorDescription, string errorUri, int status)
        {
            var jsonResult = new JsonResult
                                 {
                                     Data = new Error
                                                {
                                                    status = status,
                                                    error = error,
                                                    error_description = errorDescription,
                                                    error_uri = errorUri
                                                }
                                 };
            return jsonResult;
        }

        public static JsonResult RefreshToken(string refreshToken)
        {
            string username;
            string password;

            try
            {

                string decryptPayload = DecryptPayload(refreshToken);
                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                username = parts[0];
                password = parts[1];
            }
            catch (Exception ex)
            {
                throw new Exception("invalid refresh token", ex);
            }

            JsonResult jsonResult;
            try
            {
                Client rpcClient = BuildClient();
                ApiLogOnResponseDTO result = rpcClient.LogIn(username, password);

                if (result.PasswordChangeRequired)
                {
                    jsonResult = CreateErrorJson("invalid_request", "Password change required", "", 401);
                }
                else
                {
                    string payload = BuildPayload(username, password, result.Session);
                    jsonResult = BuildToken(payload);
                }
            }
            catch (InvalidCredentialsException)
            {
                jsonResult = CreateErrorJson("invalid_request", "Invalid Username or Password", "", 401);
            }
            catch (Exception ex)
            {
                jsonResult = CreateErrorJson("invalid_request", ex.Message, "", 400);
            }

            return jsonResult;
        }

        public static JsonResult BuildToken(string code)
        {

            string encrypted;
            string accessToken;
            try
            {
                string decryptPayload = DecryptPayload(code);
                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                string username = parts[0];
                string session = parts[1];
                string password = parts[2];


                accessToken = username + ":" + session;
                // #TODO: expose un encoded encrypt/decrypt methods
                encrypted = new SimplerAes().Encrypt(username + ":" + password);
            }
            catch (Exception ex)
            {

                throw new Exception("Malformed access code", ex);
            }
            string refreshToken = HttpUtility.UrlDecode(encrypted);
            var tokenObj = new AccessToken
                               {
                                   access_token = accessToken,
                                   expires_in = (int)DateTime.Now.AddDays(1).ToEpoch(),
                                   refresh_token = refreshToken,
                                   token_type = "bearer"
                               };

            var jsonResult = new JsonResult { Data = tokenObj };
            return jsonResult;
        }

        public static string DecryptPayload(string payload)
        {
            string payloadDecrypted = new SimplerAes().Decrypt(payload);
            return payloadDecrypted;
        }

        public static string BuildPayload(string username, string password, string session)
        {
            string package = username + ":" + session + ":" + password;
            string encrypted = new SimplerAes().Encrypt(package);
            return encrypted;
        }

        public static Client BuildClient()
        {
            var client = new Client(new Uri(CIAUTHConfigurationSection.Instance.ApiUrl),
                                    new Uri("http://example.com"), CIAUTHConfigurationSection.Instance.AppKey);
            return client;
        }

        public static string ComposeUrl(string baseUrl, string query)
        {
            string url = baseUrl;
            if (baseUrl.IndexOf("?", StringComparison.Ordinal) > -1)
            {
                url = url + "&";
            }
            else
            {
                url = url + "?";
            }
            return url + query;
        }
    }
}