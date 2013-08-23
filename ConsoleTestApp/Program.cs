using System;
using System.Collections.Generic;
using System.IO;
using WaterButt;

namespace ConsoleTestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime dtStartTime = DateTime.Now;
			string sJSON = "";

			//RainwaveClient rw = new RainwaveClient(23994, "1382eab9d0");

			//RainwaveChannel rwC = rw.Channels[1];
			//var o = rwC.Schedule_Current;

			//RainwaveListener rwL = rwC.getListenerByID(23994);
			//Console.WriteLine(rwL.sAvatar);

			//RainwaveAlbum rwA = rwC.getAlbumByID(470);
			//Debug.Print(rwA.Songs[1].Artists[0].Songs.ToString());

			//foreach (RainwaveChannel rwChannel in rw.Channels)
			//    sJSON += rwChannel.iID + " - " + rwChannel.sDescription + "\n";

			//sJSON = rw.Call("async/2/listeners_current", true);

			//dynamic oArgs = new DynamicDictionary();
			//oArgs.listener_uid = 23994;
			//sJSON = rw.Call(string.Format("async/2/listener_detail"), oArgs, true);

			//sJSON = rw.Call("async/2/artist_list", true);

			//dynamic oArgs = new DynamicDictionary();
			//oArgs.album_id = 470;
			//sJSON = rw.Call("async/2/album", oArgs, true);

			//dynamic oArgs = new DynamicDictionary();
			//oArgs.song_id = 3666626;
			//sJSON = rw.Call("async/2/song_detail", oArgs, true);

			//sJSON = rw.Call("async/2/get", null, true);

			GetFavsAndHighlyRated();

			Console.WriteLine("==================================================");
			Console.WriteLine(GetTimingFormated(DateTime.Now - dtStartTime, false));
			Console.WriteLine("==================================================");
			Console.WriteLine(sJSON);
			Console.ReadKey();
		}

		static string GetTimingFormated(TimeSpan p_tsValue, bool p_bIncludeMilliseconds)
		{
			int iTotalHours = (p_tsValue.Hours + (p_tsValue.Days * 24));
			return iTotalHours.ToString("00") + ":" + p_tsValue.Minutes.ToString("00") + ":" + p_tsValue.Seconds.ToString("00") + ":" + p_tsValue.Milliseconds.ToString("000");
		}

		static void GetFavsAndHighlyRated()
		{
			RainwaveClient rw = new RainwaveClient(23994, "1382eab9d0");
			RainwaveChannel rwChan = rw.Channels[1]; // OCRemix

			SortedSet<string> sortedSet = new SortedSet<string>();

			int iAlbumCountDown = rwChan.Albums.Count;
			string sAlbum = "";
			foreach (RainwaveAlbum rwAlbum in rwChan.Albums)
			{
				sAlbum = string.Format("{0} [{1}]{2}", rwAlbum.sName, rwAlbum.fRating.ToString(), (rwAlbum.bFavourite ? " {*}" : ""));
				Console.WriteLine("{" + iAlbumCountDown.ToString() + "} " + sAlbum);

				foreach (RainwaveSong rwSong in rwAlbum.Songs)
					if (rwSong.bFavourite || rwSong.fRating >= 3)
						sortedSet.Add(sAlbum + string.Format(" - {0} [{1}]{2} : {3}", rwSong.sTitle, rwSong.fRating.ToString(), (rwSong.bFavourite ? " {*}" : ""), rwSong.sArtistString));

				iAlbumCountDown--;
			}

			StreamWriter swList = File.CreateText(@"C:\AW Workspace\SkyDrive\Rainwave\API\List_Fav+Rated(C#).txt");
			//StreamWriter swList = File.CreateText(@"C:\SkyDrive\Rainwave\API\List_Fav+Rated(C#).txt");

			foreach (string sEntry in sortedSet)
				swList.WriteLine(sEntry);

			swList.Close();
		}
	}
}
