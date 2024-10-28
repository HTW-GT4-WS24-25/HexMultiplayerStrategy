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
    }
}
