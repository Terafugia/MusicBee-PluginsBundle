﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Module.VkAudioDownloader.Exceptions;
using VkNet.Abstractions;
using VkNet.Exception;
using VkNet.Model;

namespace Module.VkAudioDownloader.Helpers
{
    public static class VkApiHelper
    {
        public static async Task<bool> TryAuthorizeWithValidationAsync(this IVkApi vkApi,
            string login, string password, Func<string> twoFACallback)
        {
            try
            {
                await vkApi.AuthorizeAsync(new ApiAuthParams()
                {
                    Login = login,
                    Password = password,
                    TwoFactorAuthorization = twoFACallback
                });

                return vkApi.IsAuthorizedWithCheck();
            }
            catch (VkApiException)
            {
                return false;
            }
            catch (VkAuthorizationException)
            {
                return false;
            }
        }

        /// <param name="vkApi">IVkApi service.</param>
        /// <param name="accessToken">Token for authorization.</param>
        /// <exception cref="ArgumentException">Invalid value of accessToken.</exception>
        /// <exception cref="VkApiAuthorizationException">Error on validation.</exception>
        public static void AuthorizeWithValidation(this IVkApi vkApi, string accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new ArgumentException(
                        $@"Invalid value of access token: ""{accessToken}"".",
                        nameof(accessToken)
                    );
                }

                vkApi.Authorize(new ApiAuthParams
                {
                    AccessToken = accessToken
                });

                if (!vkApi.IsAuthorizedWithCheck())
                {
                    throw new VkApiAuthorizationException(
                        "Vk api authorization passed, but validation is not succeeded.");
                }
            }
            catch (VkApiException e)
            {
                throw new VkApiAuthorizationException("Got internal exception on vk api authorization.", e);
            }
            catch (VkAuthorizationException e)
            {
                throw new VkApiAuthorizationException("Got internal exception on vk api authorization.", e);
            }
        }

        public static bool IsAuthorizedWithCheck(this IVkApi vkApi)
        {
            return vkApi.IsAuthorized
                   && vkApi.UserId is not null;
        }

        // static without this
        private static Regex _regex = new Regex(@"/[0-9a-f]+(/audios)?/([0-9a-f]+)/index.m3u8");

        // ReSharper disable once InconsistentNaming
        public static bool ConvertToMp3(string m3u8Url, out string mp3Url)
        {
            mp3Url = _regex.Replace(m3u8Url, @"$1/$2.mp3");
            return mp3Url.IndexOf("m3u8", StringComparison.OrdinalIgnoreCase) == -1;
        }
    }
}