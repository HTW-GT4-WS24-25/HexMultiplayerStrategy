using UnityEngine;
using System.Collections;
using System.IO;

namespace Pinwheel.Jupiter
{
    public class JTakeScreenshot : MonoBehaviour
    {
        [SerializeField]
        private KeyCode hotKey;
        public KeyCode HotKey
        {
            get
            {
                return hotKey;
            }
            set
            {
                hotKey = value;
            }
        }

        [SerializeField]
        private string fileNamePrefix;
        public string FileNamePrefix
        {
            get
            {
                return fileNamePrefix;
            }
            set
            {
                fileNamePrefix = value;
            }
        }

        private void Reset()
        {
            HotKey = KeyCode.F9;
            FileNamePrefix = "Screenshot";
        }
    }
}