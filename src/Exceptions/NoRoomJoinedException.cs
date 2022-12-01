namespace ProjectDeception.Exceptions
{
	/// <summary>
	/// Thrown when a player requests to leave a room
	/// but isn't currently in a room
	/// </summary>
	[Serializable]
	public class NoRoomJoinedException : Exception
	{
		public NoRoomJoinedException() { }
		public NoRoomJoinedException(string message) : base(message) { }
		public NoRoomJoinedException(string message, Exception inner) : base(message, inner) { }
		protected NoRoomJoinedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
