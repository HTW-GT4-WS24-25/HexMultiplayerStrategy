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
    }
}
