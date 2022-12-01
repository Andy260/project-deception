namespace ProjectDeception.Exceptions
{
	/// <summary>
	/// Thrown when a player attempts to join a room
	/// with an invalid room code
	/// </summary>
	[Serializable]
	public class InvalidRoomCodeException : Exception
	{
		public InvalidRoomCodeException() { }
		public InvalidRoomCodeException(string message) : base(message) { }
		public InvalidRoomCodeException(string message, Exception inner) : base(message, inner) { }
		protected InvalidRoomCodeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
