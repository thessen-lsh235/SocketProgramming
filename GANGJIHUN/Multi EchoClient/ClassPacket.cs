using System.Runtime.InteropServices;

class ClassPacket
{
    struct sendPacket
    {
        public int trackNumber;
        public string id;
        public float latitude;
        public float longitude;
        public string activity;
        public string platformType;
    }

    struct recvPacket
    {
        public int trackNumber;
        public string id;
        public float latitude;
        public float longitude;
        public string activity;
        public string platformType;
    }
}