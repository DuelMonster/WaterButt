using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace WaterButt
{
    /// <summary>
    /// A RainwaveAlbum object represents one album.
    /// Note:
    /// 	You should not instantiate an object of this class directly, but rather obtain one from RainwaveChannel.albums.
    /// </summary>
    public class RainwaveAlbum
    {
        public RainwaveAlbum() { }
        public RainwaveAlbum(RainwaveChannel p_rwChannel) { Channel = p_rwChannel; }

        #region Album Raw Data:

        private string sJSON = "";
        private dynamic _oJSON = null;
        private dynamic oJSON
        {
            get
            {
                if (_oJSON == null)
                {
                    dynamic oArgs = Channel.oInitalArgs;
                    oArgs.id = iID;

                    sJSON = Channel.Client.Call("album", oArgs);
                    if (!sJSON.ToLower().Contains("invalid album id"))
                        _oJSON = Channel.Client.JSON.ToDynamic(sJSON).playlist_album;
                    else
                        throw new KeyNotFoundException("A RainwaveAlbum for the given album ID does not exist.");
                }
                return _oJSON;
            }
        }

        #endregion

        #region Album Collections:

        private List<RainwaveSong> _Songs = null;
        /// <summary>A list of RainwaveSong objects in the album.</summary>
        public List<RainwaveSong> Songs
        {
            get
            {
                if (_Songs == null)
                {
                    _Songs = Channel.Client.JSON.ToObject<List<RainwaveSong>>(Channel.Client.JSON.ToJSON(oJSON.songs));
                    // Now that we have the RainwaveSong objects we need to attach them to the RainwaveAlbum object and RainwaveChannel object.
                    foreach (RainwaveSong rwSong in _Songs)
                    {
                        rwSong.Channel = Channel;
                        rwSong.Albums.Add(this);
                        foreach (RainwaveArtist rwArtist in rwSong.Artists) rwArtist.Channel = Channel;
                    }
                }
                return _Songs;
            }
        }

        private List<RainwaveCooldownGroup> _CooldownGroups = null;
        /// <summary>A list of RainwaveCooldownGroup objects representing the cooldown groups the songs in the album belong to.</summary>
        public List<RainwaveCooldownGroup> CooldownGroups
        {
            get
            {
                if (_CooldownGroups == null)
                {
                    _CooldownGroups = Channel.Client.JSON.ToObject<List<RainwaveCooldownGroup>>(Channel.Client.JSON.ToJSON(oJSON.genres));
                    // Now that we have the RainwaveCooldownGroup objects we need to attach them to the RainwaveChannel object.
                    foreach (RainwaveCooldownGroup rwCG in _CooldownGroups) rwCG.Channel = Channel;
                }
                return _CooldownGroups;
            }
        }

        [DataMember(Name = "rating_histogram")]
        private SortedList<double, int> _RatingHistogram = null;
        /// <summary>
        /// A dictionary representing the distribution of ratings given to all songs in the album by all listeners. For example:
        ///		RatingHistogram = { 1.0 : 4, 1.5 : 4, ..., 4.5 : 46, 5.0 : 26} 
        ///	</summary>
        public SortedList<double, int> RatingHistogram
        {
            get
            {
                if (_RatingHistogram == null)
                {
                    _RatingHistogram = new SortedList<double, int>();

                    // NOTE:  I totally hate the way this is done and will attempt to find a better way of doing it.
                    //		  But for now it will have to suffice.

                    object oTmp = oJSON;
                    Dictionary<string, object> vTmp = new Dictionary<string, object>(
                        ((((new JavaScriptSerializer()).DeserializeObject(sJSON) as Dictionary<string, object>
                          )["album"] as Dictionary<string, object>
                         )["rating_histogram"] as Dictionary<string, object>
                        ), StringComparer.OrdinalIgnoreCase);

                    foreach (KeyValuePair<string, object> oKVP in vTmp)
                        _RatingHistogram.Add(Convert.ToDouble(oKVP.Key), Convert.ToInt32(oKVP.Value));
                }
                return _RatingHistogram;
            }
        }

        #endregion

        #region Album Variables:

        /// <summary>The RainwaveChannel object the album belongs to.</summary>
        public RainwaveChannel Channel { get; set; }

        /// <summary>The URL of the cover art for the album.</summary>
        [DataMember(Name = "art")]
        public string sArt { get { return oJSON.art; } }

        /// <summary>The number of listeners who have marked the album as a favourite.</summary>
        [DataMember(Name = "fave_count")]
        public int iFavCount { get { return (int)oJSON.fave_count; } }

        [DataMember(Name = "fave")]
        public bool _bFavourite = false;
        /// <summary>A Boolean representing whether the album is marked as a favourite or not. Change whether the album is a favourite by assigning a boolean value to this attribute.</summary>
        public bool bFavourite
        {
            get { return _bFavourite; }
            set
            {
                if (value != _bFavourite)
                {
                    // Set the private variable and send a call to the API
                    _bFavourite = value;
                    Channel.favAlbum(iID, value);
                }
            }
        }

        /// <summary>The ID of the album.</summary>
        [DataMember(Name = "id")]
        public int iID { get; set; }

        [DataMember(Name = "played_last")]
        private long lLastPlayed = 0;
        /// <summary>A TimeSpan specifying the most recent date and time when a song in the album was played.</summary>
        public TimeSpan tsLastPlayed { get { return TimeSpan.FromTicks(lLastPlayed); } }

        [DataMember(Name = "added_on")]
        private long lAddedOn = 0;
        /// <summary>A TimeSpan specifying the date and time when the album was created.</summary>
        public TimeSpan tsAddedOn { get { return TimeSpan.FromTicks(lAddedOn); } }

        /// <summary>A Boolean representing whether the album is currently in Cooldown.</summary>
        [DataMember(Name = "cool")]
        public bool bCool { get; set; }

        [DataMember(Name = "cool_lowest")]
        private long lLowestCD = 0;
        /// <summary>A DateTime specifying the earliest date and time a song in the album will be out of cooldown and available to play. If any song in the album is already available, dtLowestCD will be in the past.</summary>
        public TimeSpan tsLowestCD { get { return TimeSpan.FromTicks(lLowestCD); } }

        /// <summary>The multiple of time which the album will be placed in Cooldown for.</summary>
        [DataMember(Name = "cool_multiply")]
        public float fcool_multiply { get; set; }

        //??cool_override??

        /// <summary>The name of the album.</summary>
        [DataMember(Name = "name")]
        public string sName { get; set; }

        /// <summary>The average of all ratings given to songs in the album by all listeners.</summary>
        [DataMember(Name = "rating")]
        public float fRatingGlobal { get; set; }

        /// <summary>The average of all ratings given to songs in the album by only the listener authenticating to the API.</summary>
        [DataMember(Name = "rating_user")]
        public float fRating { get; set; }

        /// <summary>The total number of ratings given to songs in the album by all listeners.</summary>
        [DataMember(Name = "rating_count")]
        public int iRatingCount { get; set; }

        /// <summary>The position of the album when albums on the channel are ranked by rating. The highest-rated album will have rating_rank == 1.</summary>
        [DataMember(Name = "rating_rank")]
        public int iRatingRank { get; set; }

        /// <summary>The position of the album when albums on the channel are ranked by how often they are requested. The most-requested album will have request_rank == 1.</summary>
        [DataMember(Name = "request_rank")]
        public int iRequestRank { get; set; }

        /// <summary>The number of times a song from the album has been requested.</summary>
        [DataMember(Name = "request_count")]
        public int iRequestCount { get; set; }

        /// <summary>The total number of election votes songs in the album have received.</summary>
        [DataMember(Name = "vote_total")]
        public int iTotalVotes { get; set; }

        /// <summary>The position of the album when albums on the channel are ranked by how many votes they received. The most-voted-for album will have vote_rank == 1.</summary>
        [DataMember(Name = "vote_rank")]
        public int iVoteRank { get; set; }

        #endregion

        #region Album Methods:

        /// <summary>
        /// Raises an IndexError if there is no song with the given ID in the album.
        /// </summary>
        /// <param name="p_iID">The ID of the desired song.</param>
        /// <returns>A RainwaveSong for the given song ID.</returns>
        public RainwaveSong getSongByID(int p_iID)
        {
            RainwaveSong rwSong = null;
            return rwSong;
        }

        #endregion
    }
}
