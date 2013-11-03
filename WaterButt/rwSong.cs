using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WaterButt
{
    /// <summary>
    /// A RainwaveSong object represents one song.
    /// Note:
    ///		You should not instantiate an object of this class directly, but rather obtain one from RainwaveAlbum.songs, RainwaveArtist.songs, or some other object.
    /// </summary>
    public class RainwaveSong
    {
        public RainwaveSong() { }

        #region Song Collections:

        /// <summary>A list of RainwaveArtist objects the song is attributed to.</summary>
        [DataMember(Name = "artists")]
        public List<RainwaveArtist> Artists { get; set; }

        #endregion

        #region Song Variables:

        /// <summary>The RainwaveChannel object the album belongs to.</summary>
        public RainwaveChannel Channel { get; set; }

        /// <summary>The RainwaveAlbum object the song belongs to.</summary>
        public List<RainwaveAlbum> Albums = new List<RainwaveAlbum>();

        [DataMember(Name = "added_on")]
        public long lAddedOn = 0;
        /// <summary>The UTC date and time the song was added to the playlist (a datetime.datetime object).</summary>
        public DateTime dtAddedOn { get { return DateTime.FromBinary(lAddedOn); } }

        /// <summary>A single string with the names of all artists for the song.</summary>
        public string sArtistString
        {
            get
            {
                string sReturn = "";
                foreach (RainwaveArtist rwA in Artists)
                    sReturn += (sReturn == "" ? "" : ", ") + rwA.sName;
                return sReturn;
            }
        }

        /// <summary>A Boolean representing whether the song is available to play or not.</summary>
        [DataMember(Name = "song_available")]
        public bool bAvailable { get; set; }

        [DataMember(Name = "fave")]
        public bool _bFavourite { get; set; }
        /// <summary>A Boolean representing whether the song is marked as a favourite or not. Change whether the song is a favourite by assigning a boolean value to this attribute.</summary>
        public bool bFavourite
        {
            get { return _bFavourite; }
            set
            {
                if (value != _bFavourite)
                {
                    // Set the private variable and send a call to the API
                    _bFavourite = value;
                    Channel.favSong(iID, value);
                }
            }
        }

        /// <summary>The ID of the song.</summary>
        [DataMember(Name = "id")]
        public int iID { get; set; }

        [DataMember(Name = "song_lastplayed")]
        public long lLastPlayed = 0;
        /// <summary>The UTC date and time the song last played (a datetime.datetime object).</summary>
        public DateTime dtLastPlayed { get { return DateTime.FromBinary(lLastPlayed); } }

        /// <summary>The rating given to the song by the listener authenticating to the API. Change the rating by assigning a new value to this attribute.</summary>
        [DataMember(Name = "rating_user")]
        public float fRating { get; set; }

        /// <summary>The average of all ratings given to the song by all listeners.</summary>
        [DataMember(Name = "song_rating_avg")]
        public float fRating_avg { get; set; }

        /// <summary>The RainwaveChannel.id of the home channel for the song. This could be different from channel_id if the song is in the playlist of multiple channels.</summary>
        [DataMember(Name = "song_rating_sid")]
        public int iRatingChannelID { get; set; }

        /// <summary>The total number of ratings given to the song by all listeners.</summary>
        [DataMember(Name = "song_rating_count")]
        public int iRatingCount { get; set; }

        /// <summary>The rating ID of the song. If a song appears on multiple channels, the RainwaveSong objects will share a common rating_id so that when the song is rated on one channel the rating will be linked to the song on other channels.</summary>
        [DataMember(Name = "song_rating_id")]
        public int iRatingID { get; set; }

        [DataMember(Name = "song_releasetime")]
        public long lReleaseTime = 0;
        /// <summary>The UTC date and time the song will be out of cooldown and available to play. If the song is already available, dtReleaseTime will be in the past.</summary>
        public DateTime dtReleaseTime { get { return DateTime.FromBinary(lReleaseTime); } }

        /// <summary>The length of the song in seconds.</summary>
        [DataMember(Name = "song_secondslong")]
        public double dSecondsLong { get; set; }

        /// <summary>The length of the song as a TimeSpan.</summary>
        public TimeSpan tsLength { get { return TimeSpan.FromSeconds(dSecondsLong); } }

        /// <summary>The number of times the song lost an election.</summary>
        [DataMember(Name = "song_timesdefeated")]
        public int iTimesDefeated { get; set; }

        /// <summary>The number of times the song has played.</summary>
        [DataMember(Name = "song_timesplayed")]
        public int iTimesPlayed { get; set; }

        /// <summary>The number of times the song won an election.</summary>
        [DataMember(Name = "song_timeswon")]
        public int iTimesWon { get; set; }

        /// <summary>The title of the song.</summary>
        [DataMember(Name = "song_title")]
        public string sTitle { get; set; }

        /// <summary>The total number of times the song has been requested by any listener.</summary>
        [DataMember(Name = "song_totalrequests")]
        public int iTotalRequests { get; set; }

        /// <summary>The total number of election votes the song has received.</summary>
        [DataMember(Name = "song_totalvotes")]
        public int iTotalVotes { get; set; }

        /// <summary>The URL of more information about the song.</summary>
        [DataMember(Name = "song_url")]
        public string sURL { get; set; }

        /// <summary>The link text that corresponds with URL.</summary>
        [DataMember(Name = "song_urltext")]
        public string sURLText { get; set; }

        #endregion

        #region Song Methods:

        /// <summary>
        /// Add the song to the authenticating listener’s request queue.
        /// </summary>
        /// <returns>True if Request was successful.</returns>
        public bool Request()
        {
            return false;
        }

        #endregion
    }

    /// <summary>A RainwaveCandidate object is a subclass of RainwaveSong representing a song that is a candidate in an election.</summary>
    public class RainwaveCandidate : RainwaveSong
    {
        public RainwaveCandidate() { }

        #region Candidate Variables:

        /// <summary>The RainwaveListener who has a conflicting request, if the candidate is a conflict. Otherwise Null.</summary>
        public RainwaveListener ConflictsWith = null;

        /// <summary>The election entry ID for this candidate. Used for voting.</summary>
        public int iElecEntryID = 0;

        /// <summary>A boolean representing whether the candidate conflicts with a listener’s request.</summary>
        public bool bIsConflict = false;

        /// <summary>A boolean representing whether the candidate is a listener request or not.</summary>
        public bool bIsRequest = false;

        /// <summary>The RainwaveListener who requested the candidate, if the candidate is a request. :code:`None` otherwise.</summary>
        public RainwaveListener RequestedBy = null;

        /// <summary>The number of votes this candidate received in the election.</summary>
        public int iVotes = 0;

        #endregion

        #region Candidate Methods:

        /// <summary>
        /// Cast a vote for the candidate.
        /// </summary>
        /// <returns>True if Vote was successful.</returns>
        public bool Vote()
        {
            return false;
        }

        #endregion
    }

    /// <summary>A RainwaveRequest object is a subclass of RainwaveSong representing a song that has been requested to play on the radio.</summary>
    public class RainwaveRequest : RainwaveSong
    {
        public RainwaveRequest() { }

        #region Request Variables:

        /// <summary>The ID of the request.</summary>
        public int iRequestID = 0;

        /// <summary>The RainwaveListener who made the request.</summary>
        public RainwaveListener RequestedBy = null;

        #endregion
    }

    /// <summary>A RainwaveUserRequest object is a subclass of RainwaveSong representing a song in the authenticating listener’s requests queue.</summary>
    public class RainwaveUserRequest : RainwaveSong
    {
        public RainwaveUserRequest() { }

        #region UserRequest Variables:

        /// <summary>The request queue ID of the song in the authenticating listener’s request queue. Used to change, reorder, or delete a request.</summary>
        [DataMember(Name = "requestq_id")]
        public int iRequestQueue_ID { get; set; }

        /// <summary>The request queue order of the song in the authenticating listener’s request queue. Used to reorder a request.</summary>
        [DataMember(Name = "requestq_order")]
        public int iRequestQueue_Order { get; set; }

        /// <summary>The request queue order of the song in the authenticating listener’s request queue. Used to reorder a request.</summary>
        [DataMember(Name = "group_electionblock")]
        public bool bElectionBlock_Group { get; set; }

        /// <summary>The request queue order of the song in the authenticating listener’s request queue. Used to reorder a request.</summary>
        [DataMember(Name = "album_electionblock")]
        public bool bElectionBlock_Album { get; set; }

        #endregion

        #region UserRequest Methods:

        /// <summary>
        /// Remove the requested song from the authenticating listener’s request queue.
        /// </summary>
        /// <returns>True if Delete was successful.</returns>
        public bool Delete()
        {
            return false;
        }

        // Change the order of the requests in the queue.
        // Parameters:
        //	order (iterable) – the indices of the requests in the new order.
        // Example usage:
        //	If you have four songs in your request queue and you want to move the last song to the top of the queue:
        //		>>> from gutter import RainwaveClient
        //		>>> rw = RainwaveClient(5049, u'abcde12345')
        //		>>> game = rw.channels[0]
        //		>>> rq = game.user_requests
        //		>>> rq.reorder([3, 0, 1, 2])
        //	To randomly shuffle your request queue:
        //		>>> import random
        //		>>> indices = range(len(game.user_requests))
        //		>>> random.shuffle(indices)
        //		>>> rq.reorder(indices)
        //	All indices must appear in :code:`order` and each index must only appear once.
        public void Reorder(int[] p_iaOrder) { }

        #endregion
    }
}
