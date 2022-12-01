namespace ProjectDeception.Exceptions
{
	/// <summary>
	/// Thrown when a player attempts to join another room
	/// when the player is already within another room
	/// </summary>
	[Serializable]
	public class AlreadyInRoomException : Exception
	{
		public AlreadyInRoomException() { }
		public AlreadyInRoomException(string message) : base(message) { }
		public AlreadyInRoomException(string message, Exception inner) : base(message, inner) { }
		protected AlreadyInRoomException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
