using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CiklometarBLL.Services
{
    public class StravaService : IStravaService
    {
        private readonly IConfiguration _config;
        private static readonly HttpClient client = new HttpClient();
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ContextDb _save;
        private readonly ILoginService _loginService;
        private readonly IBasicRepository<StravaTokens> _stravaRepository;
        private readonly IBasicRepository<Activity> _activitiesRepository;
        private readonly IRepository<CiklometarDAL.Models.Location> _locationRepository;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly UserFactory _userFactory;
       


        public StravaService(IConfiguration config, IRepository<User> userRepository, IMapper mapper,
            ContextDb save, ILoginService loginService, IBasicRepository<StravaTokens> stravaRepository,
            IBasicRepository<Activity> activitiesRepository,
            IRepository<CiklometarDAL.Models.Location> locationRepository, IBasicRepository<Role> roleRepository,
            UserFactory userFactory)
        {
            _config = config;
            _userRepository = userRepository;
            _mapper = mapper;
            _save = save;
            _loginService = loginService;
            _stravaRepository = stravaRepository;
            _activitiesRepository = activitiesRepository;
            _locationRepository = locationRepository;
            _roleRepository = roleRepository;
            _userFactory = userFactory;
        }
        

        public ChallengeDTO VerifySubscription(string link)
        {
            var challenge = new ChallengeDTO
            {
                Challenge = link
            };

            return challenge;
        }

        public async Task<AccessTokenDataDTO> ConnectWithStrava(string codeString, Guid userId)
        {

            var userDB = _userRepository.GetAllAsReadOnly().FirstOrDefault(x => x.Id == userId);

            if (userDB == null)
            {
                throw new Exception("Your user id is invalid.");
            }

            string client_id = "?client_id=" + _config["Strava:client_id"];
            string client_secret = "&client_secret=" + _config["Strava:client_secret"];
            string grant_type = "&grant_type=authorization_code";
            string code = "&code=" + codeString;

            string requestUrl = "https://www.strava.com/oauth/token" + client_id + client_secret + code + grant_type;

            HttpResponseMessage response = await client.PostAsync(requestUrl, null);
            var responseString = await response.Content.ReadAsStringAsync();

            string accessToken = responseString.Substring(responseString.IndexOf("\"access_token\":\"") + "\"access_token\":\"".Length).Split("\",".ToCharArray()).First();
            string refreshToken = responseString.Substring(responseString.IndexOf("\"refresh_token\":\"") + "\"refresh_token\":\"".Length).Split("\",".ToCharArray()).First();

            string stravaId = responseString.Substring(responseString.IndexOf("\"id\":") + "\"id\":".Length).Split(",".ToCharArray()).First();
            string expiresAt = responseString.Substring(responseString.IndexOf("\"expires_at\":") + "\"expires_at\":".Length).Split(",".ToCharArray()).First();
            string expiresIn = responseString.Substring(responseString.IndexOf("\"expires_in\":") + "\"expires_in\":".Length).Split(",".ToCharArray()).First();

            if (userDB.StravaId == stravaId)
            {
                throw new Exception("You've already connected this Strava account to this Ciklometar account.");
            }

            bool alreadyConnectedUsers = _userRepository.GetAllAsReadOnly().Where(user => user.StravaId == stravaId && user.Id != userDB.Id).Any();

            if (alreadyConnectedUsers)
            {
                throw new Exception("There is already a user connected with this Strava account.");
            }

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt));
            var token = new StravaTokens()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = dateTimeOffset.DateTime,
                AthleteId = int.Parse(stravaId)
            };
            _stravaRepository.Insert(token);

            userDB.StravaId = stravaId;

            _userRepository.Update(userDB);
            _save.SaveChanges();

            var userDTO = _loginService.CreateAccessTokenSignature(userDB);
            return userDTO;
        }
        public async Task<AccessTokenDataDTO> LoginWithStrava(string codeString)
        {
            string client_id = "?client_id=" + _config["Strava:client_id"];
            string client_secret = "&client_secret=" + _config["Strava:client_secret"];
            string grant_type = "&grant_type=authorization_code";
            string code = "&code=" + codeString;

            string requestUrl = "https://www.strava.com/oauth/token" + client_id + client_secret + code + grant_type;

            HttpResponseMessage response = await client.PostAsync(requestUrl, null);
            var responseString = await response.Content.ReadAsStringAsync();

            string accessToken = responseString.Substring(responseString.IndexOf("\"access_token\":\"") + "\"access_token\":\"".Length).Split("\",".ToCharArray()).First();
            string refreshToken = responseString.Substring(responseString.IndexOf("\"refresh_token\":\"") + "\"refresh_token\":\"".Length).Split("\",".ToCharArray()).First();

            string stravaId = responseString.Substring(responseString.IndexOf("\"id\":") + "\"id\":".Length).Split(",".ToCharArray()).First();
            string expiresAt = responseString.Substring(responseString.IndexOf("\"expires_at\":") + "\"expires_at\":".Length).Split(",".ToCharArray()).First();
            string expiresIn = responseString.Substring(responseString.IndexOf("\"expires_in\":") + "\"expires_in\":".Length).Split(",".ToCharArray()).First();

            var user = _userRepository.GetAllAsReadOnly().FirstOrDefault(x => x.StravaId == stravaId);

            if (user == null) throw new Exception("No user is connected to this Strava account.");

            var userDTO = _loginService.CreateAccessTokenSignature(user);

            return userDTO;
        }
        public async Task<AccessTokenDataDTO> RegisterWithStrava(string codeString)
        {
            string client_id = "?client_id=" + _config["Strava:client_id"];
            string client_secret = "&client_secret=" + _config["Strava:client_secret"];
            string grant_type = "&grant_type=authorization_code";
            string code = "&code=" + codeString;

            string requestUrl = "https://www.strava.com/oauth/token" + client_id + client_secret + code + grant_type;

            HttpResponseMessage response = await client.PostAsync(requestUrl, null);
            var responseString = await response.Content.ReadAsStringAsync();

            string username = responseString.Substring(responseString.IndexOf("\"username\":\"") + "\"username\":\"".Length).Split("\",".ToCharArray()).First();
            string firstName = responseString.Substring(responseString.IndexOf("\"firstname\":\"") + "\"firstname\":\"".Length).Split("\",".ToCharArray()).First();
            string lastName = responseString.Substring(responseString.IndexOf("\"lastname\":\"") + "\"lastname\":\"".Length).Split("\",".ToCharArray()).First();
            string accessToken = responseString.Substring(responseString.IndexOf("\"access_token\":\"") + "\"access_token\":\"".Length).Split("\",".ToCharArray()).First();
            string refreshToken = responseString.Substring(responseString.IndexOf("\"refresh_token\":\"") + "\"refresh_token\":\"".Length).Split("\",".ToCharArray()).First();

            string stravaId = responseString.Substring(responseString.IndexOf("\"id\":") + "\"id\":".Length).Split(",".ToCharArray()).First();
            string expiresAt = responseString.Substring(responseString.IndexOf("\"expires_at\":") + "\"expires_at\":".Length).Split(",".ToCharArray()).First();
            string expiresIn = responseString.Substring(responseString.IndexOf("\"expires_in\":") + "\"expires_in\":".Length).Split(",".ToCharArray()).First();


            var user = _userRepository.GetAllAsReadOnly().FirstOrDefault(x => x.StravaId == stravaId);
            if (user != null) throw new Exception("There already is a user connected to this Strava account.");

            UserRegisterDTO newUser = new UserRegisterDTO()
            {
                Nickname = username,
                FirstName = firstName,
                LastName = lastName,
                StravaId = stravaId,
            };
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt));
            var token = new StravaTokens()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = dateTimeOffset.DateTime,
                AthleteId = int.Parse(stravaId)
            };
            _stravaRepository.Insert(token);
            _save.SaveChanges();

            AccessTokenDataDTO userDTO = _loginService.CreateUser(newUser);

            return userDTO;
        }

        public async Task CreateWebhook()
        {
            if (_userFactory.IsSuperAdmin() == true)
            {
                string webhookUrl = "https://www.strava.com/api/v3/push_subscriptions";
                string client_id = "?client_id=" + _config["Strava:client_id"];
                string client_secret = "&client_secret=" + _config["Strava:client_secret"];
                string callbackUrl = "&callback_url=" + _config["Strava:callback_url"];
                string verifyToken = "&verify_token=STRAVA";
                string requestUrl = webhookUrl + client_id + client_secret + callbackUrl + verifyToken;
                await client.PostAsync(requestUrl, null);
            }
            else { throw new UnauthorizedAccessException(); }
        }

        public async Task DeleteWebhook()
        {
            if (_userFactory.IsSuperAdmin() == true)
            {
                string webhookUrl = "https://www.strava.com/api/v3/push_subscriptions/";
                string client_secret = "&client_secret=" + _config["Strava:client_secret"];
                string client_id = "?client_id=" + _config["Strava:client_id"];
                HttpResponseMessage response = await client.GetAsync(webhookUrl + client_id + client_secret);
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<List<WebhookSubscriptionDTO>>(json);
                string requestUrl = webhookUrl + content.First().id + client_id + client_secret;
                await client.DeleteAsync(requestUrl);
            }
            else { throw new UnauthorizedAccessException(); }
        }
        public async Task<bool> HandleWebhookEvent(StravaWebhookDTO stravaWebhookDTO)
        {
            var activity = _activitiesRepository.GetAllAsReadOnly().Where(i => i.ActivityId == stravaWebhookDTO.Object_id).FirstOrDefault();
            if (activity == null)
            {
                var accessToken = _stravaRepository.GetAll().Where(i => i.AthleteId == stravaWebhookDTO.Owner_id).FirstOrDefault();

                if (accessToken == null)
                {
                    throw new Exception("Access Token Not Found");
                }
                if (accessToken.Expiration <= DateTime.Now)
                {
                    var newAccessToken = await RefreshToken(accessToken.AthleteId);
                    accessToken = newAccessToken;

                }
                var activities = await GetActivity(accessToken.AccessToken, stravaWebhookDTO.Object_id, stravaWebhookDTO.Event_time);
                if (activities != null)
                {
                    return true;
                }
                return false;

            }
            return false;
        }
        private async Task<Activity> GetActivity(string accessToken, long objectId, long event_time)
        {
            HttpClient request = new HttpClient();
            string url = "https://www.strava.com/api/v3/activities/";
            string include_all_efforts = "?include_all_efforts=true";
            string requestUrl = url + objectId + include_all_efforts;
            request.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            HttpResponseMessage response = await request.GetAsync(requestUrl);
            var content = JsonConvert.DeserializeObject<ActivitiesDTO>(await response.Content.ReadAsStringAsync());
            if (content.Type == "Ride") { return null; }
            double maxAverageSpeed = Convert.ToDouble(_config["Strava:maxAverageSpeed"]);
            var coordinates = DecodePolyline(content.Map.polyline);
            var orgid = CheckOrganizationLocation(coordinates.Last(), content.Athlete.Id);
            if (
                Convert.ToDouble(content.Average_Speed) > maxAverageSpeed
                && _locationRepository.GetAll().Where(i => i.OrganizationId == orgid).Any() == false
                && _roleRepository.GetAllAsReadOnly().Where(id => id.User.StravaId == content.Athlete.Id.ToString() && id.UserType == UserType.Cyclist).Any() == false
                && Convert.ToDateTime(content.Start_date) < _userFactory.UsersLastBan(content.Athlete.Id, orgid)
              ) 
            {
                return null; 
            }
            var activities = new Activity()
            {
                ActivityId = objectId,
                AthleteId = content.Athlete.Id.ToString(),
                Distance = content.Distance,
                Type = content.Type,
                AvgSpeed = (long)Convert.ToDouble(content.Average_Speed),
                EndLocation = new NetTopologySuite.Geometries.Point(coordinates.Last().Lat, coordinates.Last().Lng) { SRID = 4326 },
                OrganizationId = orgid,
                Event_time = DateTimeOffset.FromUnixTimeSeconds(event_time).DateTime,
                Moving_time = Convert.ToInt32(content.Moving_time),
                Elapsed_time = Convert.ToInt32(content.Elapsed_time)
            };
            _activitiesRepository.Insert(activities);
            _save.SaveChanges();

            return activities;
        }

        private Guid CheckOrganizationLocation(LocationDTO coordinates, int athleteId)
        {
            var orgid = _roleRepository.GetAll().Where(i => i.User.StravaId == athleteId.ToString()).Select(i => i.OrganizationId);
            var locations = _locationRepository.GetAll().Where(id => orgid.Contains(id.OrganizationId));
            int radius = 100;
            Guid id = Guid.Empty;
            foreach (var location in locations)
            {
                var distance = GetDistance(Convert.ToDouble(location.Coordinates.X), Convert.ToDouble(location.Coordinates.Y), coordinates.Lat, coordinates.Lng);

                if (radius > distance)
                {
                    id = location.OrganizationId;
                }
            }
            return id;
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c * 1000; //distane in m 
            return d;
        }
        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private async Task<StravaTokens> RefreshToken(int athleteId)
        {
            string client_id = "?client_id=" + _config["Strava:client_id"];
            string client_secret = "&client_secret=" + _config["Strava:client_secret"];
            string grant_type = "&grant_type=refresh_token";
            string url = "https://www.strava.com/api/v3/oauth/token";
            var refreshToken = _stravaRepository.GetAll().Where(i => i.AthleteId == athleteId).FirstOrDefault();

            HttpResponseMessage response = await client.PostAsync(url + client_id + client_secret + "&refresh_token=" + refreshToken.RefreshToken + grant_type, null);
            var responseString = await response.Content.ReadAsStringAsync();
            string newAccessToken = responseString.Substring(responseString.IndexOf("\"access_token\":\"") + "\"access_token\":\"".Length).Split("\",".ToCharArray()).First();
            string newRefreshToken = responseString.Substring(responseString.IndexOf("\"refresh_token\":\"") + "\"refresh_token\":\"".Length).Split("\",".ToCharArray()).First();
            string expiresAt = responseString.Substring(responseString.IndexOf("\"expires_at\":") + "\"expires_at\":".Length).Split(",".ToCharArray()).First();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt));

            refreshToken.AccessToken = newAccessToken;
            refreshToken.RefreshToken = newRefreshToken;
            refreshToken.Expiration = dateTimeOffset.DateTime;
            _stravaRepository.Update(refreshToken);
            _save.SaveChangesAsync();
            return refreshToken;
        }

        public static IEnumerable<LocationDTO> DecodePolyline(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                throw new ArgumentNullException("encodedPoints is null");

            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new LocationDTO
                {
                    Lat = Convert.ToDouble(currentLat) / 1E5,
                    Lng = Convert.ToDouble(currentLng) / 1E5
                };
            }
        }
        public async Task SyncActivitesWithStrava(long afterTime)
        {
            if (_userFactory.IsSuperAdmin())
            {
                var userAccessTokens = _stravaRepository.GetAll().ToList();
                var url = "https://www.strava.com/api/v3/athlete/activities?";
                var timeBefore = "before=" + (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
                var timeAfter = "&after=" + afterTime;
                string requestUrl = url + timeBefore + timeAfter + "&page=1&per_page=30";

                foreach (var user in userAccessTokens)
                {
                    if (user.Expiration <= DateTime.Now)
                    {
                        await RefreshToken(user.AthleteId);
                    }
                    HttpClient client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 0, 30);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.AccessToken);
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    var content = JsonConvert.DeserializeObject<List<ActivitySyncDTO>>(await response.Content.ReadAsStringAsync());
                    SaveActivityList(content);
                }   
            }
            else { throw new UnauthorizedAccessException(); }
        }

        public void SaveActivityList(List<ActivitySyncDTO> activities)
        {
            double avgSpeed = Convert.ToDouble(_config["Strava:maxAverageSpeed"]);
            List<Activity> activitiesToAdd = new List<Activity>();
            foreach (var activity in activities)
            {
                var savedUserActivityIds = _activitiesRepository.GetAll()
                    .Where(id => id.AthleteId == activity.Athlete.Id.ToString())
                    .Select(aId => aId.ActivityId)
                    .ToList();
                var endLocation = DecodePolyline(activity.Map.summary_polyline);
                var organizationId = CheckOrganizationLocation(endLocation.Last(), activity.Athlete.Id);
                if (savedUserActivityIds.Contains(activity.Id)
                    || activity.Type != "Ride"
                    || Convert.ToDouble(activity.Average_Speed) <= avgSpeed
                    || organizationId == Guid.Empty
                    || _userFactory.UsersLastBan(activity.Athlete.Id, organizationId) <= activity.start_date_local)
                {
                    continue;
                }
                var unsyncedActivity = new Activity()
                {
                    ActivityId = activity.Id,
                    OrganizationId = organizationId,
                    Distance = activity.Distance,
                    Type = activity.Type,
                    AvgSpeed = (long)Convert.ToDouble(activity.Average_Speed),
                    EndLocation = new NetTopologySuite.Geometries.Point(
                                              endLocation.Last().Lat,
                                              endLocation.Last().Lng)
                                              { SRID = 4326 },
                    AthleteId = activity.Athlete.Id.ToString(),
                    Elapsed_time = Convert.ToInt32(activity.Elapsed_time),
                    Moving_time = Convert.ToInt32(activity.Moving_time),
                    Event_time = activity.start_date_local
                };
                _activitiesRepository.Insert(unsyncedActivity);
                //activitiesToAdd.Add(unsyncedActivity);
                _save.SaveChanges();
            }
            _activitiesRepository.AddRange(activitiesToAdd);
            //_save.SaveChanges();
        }
    }
}
    

