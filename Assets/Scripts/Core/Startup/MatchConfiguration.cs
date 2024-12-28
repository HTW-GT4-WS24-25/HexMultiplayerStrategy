using Unity.Netcode;
using UnityEngine;

namespace Core.Startup
{
    [CreateAssetMenu(fileName = "New Match Configuration", menuName = "Match Configuration")]
    public class MatchConfiguration : ScriptableObject, INetworkSerializable
    {
        public int nightsPerMatch;
        public float dayDuration;
        public float nightDuration;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref nightsPerMatch);
            serializer.SerializeValue(ref dayDuration);
            serializer.SerializeValue(ref nightDuration);
        }
    }
}