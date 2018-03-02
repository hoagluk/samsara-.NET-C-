using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

/*General Comments
 * 
 * This is a WPF application to test the Samsara API.  User clicks on the one button on the form to run the method MakeCalls().  
 * 
 * Nothing is done with the results of the calls.  You'll have to set a breakpoint and then examine the results with a Watch feature to see what comes back.
 * 
 * 
 */

namespace TestMakeAPICall
{

	//Internal classes

	internal class LogEntry
	{
		public Int64 groupId;
		public Int64 vehicleId;
		public Int64 driverId;
		public Int64 logStartMs;
		public string hosStatusType;
		public string locCity;
		public string locState;
		public float locLat;
		public float locLng;
		public string locName;
	}
	internal class FleetHOS_logs
	{
		public LogEntry[] logs;
	}

	internal class VehLocInfo
	{

		public Int64 id;
		public string name;
		public Int64 time;
		public float latitude;
		public float longitude;
		public float heading;
		public float speed;
		public string location;
		public bool onTrip;
		public string vin;
	}

	internal class FleetLocations {
		public VehLocInfo[] vehicles;
	}


	//Window logic

		public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			var res = MakeCalls();
		}

		static async Task<List<LogEntry>> MakeCalls()
		{

			//set up method's constants and values
			const string basePath = "https://us2.api.samsara.com/v1";
			const string accessToken = "YOUR TOKEN HERE";
			const int group_id = 999999; //YOUR GROUPID HERE

			//Query string (with access_token)
			var dicQueryParams = new Dictionary<String, String>();
			dicQueryParams["access_token"] = accessToken;
			var strQueryString = string.Join(";", dicQueryParams.Select(x => x.Key + "=" + x.Value).ToArray());
			var subPath = "";

			//Initialize HttpClient
			using (var client = new HttpClient { BaseAddress = new Uri(basePath) })
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


				// ---------------------------------------------------------------------------------------------------------------------------------------
				// POST: /fleet/locations ----------------------------------------------------------------------------------------------------------------
				subPath = "/fleet/locations";
				var uriBuilder = new UriBuilder(basePath + subPath);
				uriBuilder.Query = strQueryString;
				var strFullURL = uriBuilder.ToString();
				var objReqDataGroupID = new { groupId = group_id };

				try
				{
					HttpResponseMessage response = await client.PostAsJsonAsync(strFullURL, objReqDataGroupID);
					if (response.IsSuccessStatusCode)
					{
						var strContents = await response.Content.ReadAsStringAsync();
						var lstLocations = JsonConvert.DeserializeObject<FleetLocations>(strContents); //This method is in the Newtonsoft.Json library, available in Nuget
					} // <---------- When testing, you'll want a breakpoint here, and etc. down below.
					else
					{
						MessageBox.Show("Error making API call.", "Test Application");
					}// <---------- When testing, you'll want a breakpoint here.

				}
				catch (Exception e)
				{
				}// <---------- When testing, you'll want a breakpoint here.


				// ---------------------------------------------------------------------------------------------------------------------------------------
				// POST: /fleet/hos_logs ----------------------------------------------------------------------------------------------------------------
				subPath = "/fleet/hos_logs";
				uriBuilder = new UriBuilder(basePath + subPath);
				uriBuilder.Query = strQueryString;
				strFullURL = uriBuilder.ToString();
				string MyString = "12 June 2008";
				DateTime MyDateTime = DateTime.Parse(MyString);
				var objReqDataHOSLogs = new
				{
					groupId = group_id,
					driverId = 51084,
					startMs = new DateTimeOffset(2018, 2, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds(), //get Unix time in milliseconds, Feb 1, 2018
					endMs = new DateTimeOffset(2018, 2, 7, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds(), //, Feb 7, 2018
				};

				try
				{
					HttpResponseMessage response = await client.PostAsJsonAsync(strFullURL, objReqDataHOSLogs);
					if (response.IsSuccessStatusCode)
					{
						var strContents = await response.Content.ReadAsStringAsync();
						var lstLogs = JsonConvert.DeserializeObject<FleetHOS_logs>(strContents);
					}
					else
					{
						MessageBox.Show("Error making API call.", "Test Application");
					}
				}
				catch (Exception e)
				{
				}

				// ---------------------------------------------------------------------------------------------------------------------------------------
				// GET: /fleet/maintenance/dvirs ----------------------------------------------------------------------------------------------------------------
				subPath = "/fleet/maintenance/dvirs";
				uriBuilder = new UriBuilder(basePath + subPath);

				try
				{

					dicQueryParams = new Dictionary<String, String>();
					dicQueryParams["access_token"] = accessToken;
					dicQueryParams["end_ms"] = new DateTimeOffset(2018, 2, 12, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds().ToString(); //get Unix time in milliseconds, Feb 12, 2018;
					dicQueryParams["duration_ms"] = ((8.64e+7) * 3).ToString() ; //get Unix ms duration of 3 days
					dicQueryParams["group_id"] = group_id.ToString();
					strQueryString = string.Join(";", dicQueryParams.Select(x => x.Key + "=" + x.Value).ToArray());
				}
				catch (Exception e)
				{

				}

				uriBuilder.Query = strQueryString;
				strFullURL = uriBuilder.ToString();

				try
				{
					HttpResponseMessage response = await client.GetAsync(strFullURL);
					if (response.IsSuccessStatusCode)
					{
						var strContents = await response.Content.ReadAsStringAsync();
						//var lstLogs = JsonConvert.DeserializeObject....
						//Types necessary for deserialization are not defined for this, since they're complex.  But wanted to demonstrate a GET call.
					}
					else
					{
						MessageBox.Show("Error making API call.", "Test Application");
					}
				}
				catch (Exception e)
				{
				}

			} //using

			return new List<LogEntry>();
		}

	}
}

