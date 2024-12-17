using Unity.Netcode;
using UnityEngine;

namespace Helper
{
    public static class HelperMethods
    {
        public static void CopyToClipboard(string stringToCopy) {
            var textEditor = new TextEditor { text = stringToCopy };
        
            textEditor.SelectAll();
            textEditor.Copy();
        }

        public static ClientRpcParams GetClientRpcParamsToSingleTarget(ulong targetId)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { targetId }
                }
            };
        }
        
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            // Ensure we don't divide by zero
            if (Mathf.Abs(b - a) < Mathf.Epsilon)
                return 0f;
            
            return (value - a) / (b - a);
        }
    }
}
