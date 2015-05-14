using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JimCarrey
{
    public class JimBot
    {
        public void Preform()
        {
            _config = Config.Load();

            while(true)
            {
                Console.WriteLine("Started");

                if (string.IsNullOrEmpty(_config.AppId)
                    || string.IsNullOrEmpty(_config.AppSecret))
                {
                    PromptForAppToken();
                    continue;
                }

                if (string.IsNullOrEmpty(_config.UserToken)
                    || DateTime.UtcNow >= _config.UserTokenExpiresAt)
                {
                    PromptForUserToken();
                    continue;
                }

                var post = GetLatestPost("thesamephotoofjimcarreyeveryday");

                if(post != null)
                {
                    if(post.likes.data.Any(u => u.id == _config.UserId))
                    {
                        Console.WriteLine("Already liked");
                    }
                    else if (LikePost(post, _config.UserToken))
                    {
                        Console.WriteLine("Liked todays photo!");
                        SendNotification("Liked Jim Carrey's at " + DateTime.UtcNow);
                    }
                }
                
                Sleep();
            }
        }

        FB.Post GetLatestPost(string page)
        {
            var postUrl = string.Format("{0}{1}/feed?limit=1&access_token={2}",
                                        OpenGraph.BaseUrl, page, _config.UserToken);
            var result = OpenGraph.RequestEdge<FB.Post>(OpenGraph.Get, postUrl);

            if(result.data != null && result.data.Length > 0)
            {
                return result.data[0];
            }
            else
            {
                return null;
            }
        }

        bool LikePost(FB.Post post, string accessToken)
        {
            var likeUrl = string.Format("{0}{1}/likes?access_token={2}", 
                                    OpenGraph.BaseUrl, post.id, accessToken);
            var likeResult = OpenGraph.Request<FB.Result>(OpenGraph.Post, likeUrl);

            return likeResult.success;
        }

        bool SendNotification(string msg)
        {
            var notifyUrl = string.Format("{0}me/notifications?template={1}&access_token={2}",
                                            OpenGraph.BaseUrl, _config.UserId, _config.GetAppToken());
            var notifyResult = OpenGraph.Request<FB.Result>(OpenGraph.Post, notifyUrl);
            Console.WriteLine("Sent notification " + notifyResult.success);
            return notifyResult.success;
        }

        void Sleep()
        {
            Console.WriteLine("Sleep() " + DateTime.UtcNow);
            var todaysJim = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day).AddHours(6);
            
            if (Math.Abs((todaysJim - DateTime.UtcNow).TotalMinutes) < 15)
            {
                //Check every 6 seconds
                const int sixSeconds = 6 * 1000;
                Thread.Sleep(sixSeconds);
            }
            else
            {
                //Check every 6 minutes
                const int thirtyMinutes = 6 * 60 * 1000;
                Thread.Sleep(thirtyMinutes);
            }
        }

        bool PromptForAppToken()
        {
            Console.WriteLine("Press enter token required!");

            Console.WriteLine("Get app id:");
            var appId = Console.ReadLine();

            Console.WriteLine("Get app secret:");
            var appSecret = Console.ReadLine();

            var token = appId + "|" + appSecret;

            var tokenInfoUrl = string.Format("{0}debug_token?input_token={1}&access_token={2}",
                                    OpenGraph.BaseUrl, token, token);
            var tokenInfo = OpenGraph.Request<FB.Data<FB.Token>>(OpenGraph.Get, tokenInfoUrl);
            if (tokenInfo.error != null)
            {
                Console.WriteLine(tokenInfo.error);
                return false;
            }
            else if(tokenInfo.data.is_valid 
                && string.IsNullOrEmpty(tokenInfo.data.user_id)) //Check there is user
            {
                Console.WriteLine("Valid token for " + tokenInfo.data.application);
                _config.AppId = appId;
                _config.AppSecret = appSecret;
                _config.Save();
                return true;
            }
            
            return false;
        }

        bool PromptForUserToken()
        {

            Console.WriteLine("Press [enter] to start oAuth in browser");
            Console.ReadLine();

            //Open browser to oAuth flow
            var redirectUrl = "https://s3-ap-southeast-2.amazonaws.com/sleepybucket/jimcarrey/OAuthPage.html";
            var oauthUrl = string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&scope={1}&response_type=token&redirect_uri={2}",
                                         "1664657133762257", string.Join(",", _requiredPermissions), Uri.EscapeUriString(redirectUrl));
            System.Diagnostics.Process.Start(oauthUrl);

            Console.WriteLine("Enter token (right+click border/edit/pase)");

            var token = Console.ReadLine();

            var extendUrl = string.Format("{0}oauth/access_token?grant_type=fb_exchange_token&client_id={1}&client_secret={2}&fb_exchange_token={3}",
                                            OpenGraph.BaseUrl, _config.AppId, _config.AppSecret, token);
            var extendResult = OpenGraph.Request<FB.TokenExtended>(OpenGraph.Get, extendUrl);
            if (extendResult.error != null)
            {
                Console.WriteLine("Failed to extend token, a 3 hour session is no good");
                Console.WriteLine(extendResult.error.ToString());
                return false;
            }

            Console.WriteLine("Token extended");
            token = extendResult.access_token;

            //Debug token
            var tokenInfoUrl = string.Format("{0}debug_token?input_token={1}&access_token={2}",
                                    OpenGraph.BaseUrl, token, token);
            var tokenInfo = OpenGraph.Request<FB.Data<FB.Token>>(OpenGraph.Get, tokenInfoUrl);
            if(tokenInfo.error != null)
            {
                Console.WriteLine(tokenInfo.error);
                return false;
            }
            
            var expiresAt = tokenInfo.data.GetExpireTime();
            Console.WriteLine("Token expires at " + expiresAt);

            //Got permissions?
            foreach(var permission in _requiredPermissions)
            {
                if (!tokenInfo.data.scopes.Contains(permission))
                {
                    Console.WriteLine("Permission " + permission + " has not been not granted");
                    return false;
                }
            }

            _config.UserId = tokenInfo.data.user_id;
            _config.UserToken = token;
            _config.UserTokenExpiresAt = expiresAt;
            _config.Save();

            return true;
        }

        string[] _requiredPermissions = new[] { "user_likes", "publish_actions", "manage_notifications" };
        Config _config;
    }
}
