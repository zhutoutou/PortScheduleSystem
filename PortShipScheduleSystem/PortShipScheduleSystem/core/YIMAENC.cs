using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortShipScheduleSystem.core
{

    public enum LoadMapName
    {
        GoogleSatMap = 0,//谷歌卫星地图
        GoogleStrMap = 1,//谷歌街道地图 	
        TencentSatMap = 2,//腾讯卫星地图
        TencentStrMap = 3,//腾讯街道地图

        TianDiSatMap = 4,//天地图卫星地图
        TianDiStrMap = 5,//天地图街道地图

        //大于10为百度切割模式
        BaiduStaMap = 11, //百度
        BaiduStrMap = 12,
    };

    //测量操作图形
    public enum M_TOOl_TYPE
    {
        TYPE_NONE = 0,
        TYPE_EBL = 1,
        TYPE_MEASURE_DIS = 2,
        TYPE_MEASURE_AREA = 3,
        TYPE_AREA_ZOOM_IN = 4
    }
    public enum SPECIAL_LINE_TYPE
    {
        NO_SPECIAL_TYPE = 0,
        RECTANGLE_LN_TYPE = 10,
        CIRCLE_LN_TYPE = 20,
        ELLIPSE_LN_TYPE = 30,
        ARC_LN_TYPE = 40,
        PIE_LN_TYPE = 41,
        BOW_LN_TYPE = 42,
        SECTOR_LN_TYPE = 50,
        CURVE_LN_TYPE = 60,
        CURVE_LN_TYPE_WITH_HEAD_ARROW = 61,
        CURVE_LN_TYPE_WITH_HAED_TAIL_ARROW = 62,
        SINGLE_ARROW_LN_TYPE = 80,
    }


    public enum M_GEO_TYPE : int //物标的几何属性
    {
        TYPE_NULL = -1,
        TYPE_POINT = 0,     //点对象
        TYPE_LINE = 2,      //线对象
        TYPE_FACE = 3,      //面对象 
        TYPE_COMBINED_OBJECT = 10
    };

    public enum LAYER_GEO_TYPE //图层的属性，注意：在少数情况下，图层可能不是唯一的地理类型
    {
        LAYER_GEO_TYPE_NULL = 0,
        ALL_POINT = 1,
        ALL_LINE = 2,
        ALL_FACE = 3,
        MULTIPLE_GEO_TYPE = 5
    };

    public enum M_DATA_TYPE_ID
    {
        M_NULL = 0x0,
        M_INT = 0x1,
        M_BOOL = 0x2,
        M_FLOAT = 0x3,//
        M_STRING = 0x4,  //with a maxinum length of 65536 byte, and a Chinese character is counted as 2 bytes
                         //M_DATE=0x5,
                         //M_BOOL=0x6
        M_ENUM = 0x5,
        M_LIST = 0x6
    };

    public class M_COLOR
    {
        public byte r;
        public byte g;
        public byte b;
        public byte reserve;

        public M_COLOR()
        {
            r = g = b = reserve = 0;
        }
        public M_COLOR(int iColor)
        {
            int _r = iColor & 255;
            int _g = iColor >> 8 & 255;
            int _b = iColor >> 16 & 255;
            r = Convert.ToByte(_r);
            g = Convert.ToByte(_g);
            b = Convert.ToByte(_b);

        }
        public M_COLOR(byte R, byte G, byte B)
        {
            r = R;
            g = G;
            b = B;
            reserve = 0;
        }

        public M_COLOR(byte R, byte G, byte B, byte rsv)
        {
            r = R;
            g = G;
            b = B;
            reserve = rsv;
        }

        public int ToInt()
        {
            byte[] buffer = new byte[4];
            buffer[0] = r;
            buffer[1] = g;
            buffer[2] = b;
            buffer[3] = reserve;
            return BitConverter.ToInt32(buffer, 0);
        }
    };

    public class M_POINT
    {
        public int x;
        public int y;

        public M_POINT()
        {
            x = y = 0;
        }

        public M_POINT(int poX, int poY)
        {
            x = poX;
            y = poY;
        }

        public static M_POINT operator +(M_POINT left, M_POINT right)
        {
            return new M_POINT(left.x + right.x, left.y + right.y);
        }

        public static M_POINT operator -(M_POINT left, M_POINT right)
        {
            return new M_POINT(left.x - right.x, left.y - right.y);
        }

        public M_POINT(byte[] strPoint)
        {
            x = BitConverter.ToInt32(strPoint, 0);
            y = BitConverter.ToInt32(strPoint, 4);
        }

        public byte[] ToBytes()
        {
            byte[] buffer = new byte[8];
            byte[] buf1 = BitConverter.GetBytes(x);
            Array.Copy(buf1, 0, buffer, 0, 4);
            byte[] buf2 = BitConverter.GetBytes(y);
            Array.Copy(buf2, 0, buffer, 4, 4);

            return buffer;
        }
    };

    public class MEM_GEO_OBJ_POS //geo object position in memory-maps
    {
        public int memMapPos;
        public int layerPos;
        public int innerLayerPos;

        public MEM_GEO_OBJ_POS()
        {
            memMapPos = layerPos = innerLayerPos = -1;
        }

        public MEM_GEO_OBJ_POS(int mapPos, int lyrPos, int objPos)
        {
            memMapPos = mapPos;
            layerPos = lyrPos;
            innerLayerPos = objPos;
        }

        public MEM_GEO_OBJ_POS(byte[] strObjPos)
        {
            memMapPos = BitConverter.ToInt32(strObjPos, 0);
            layerPos = BitConverter.ToInt32(strObjPos, 4);
            innerLayerPos = BitConverter.ToInt32(strObjPos, 8);
        }

        public byte[] ToBytes()
        {
            byte[] buffer = new byte[12];
            byte[] buf1 = BitConverter.GetBytes(memMapPos);
            Array.Copy(buf1, 0, buffer, 0, 4);
            byte[] buf2 = BitConverter.GetBytes(layerPos);
            Array.Copy(buf2, 0, buffer, 4, 4);
            byte[] buf3 = BitConverter.GetBytes(innerLayerPos);
            Array.Copy(buf3, 0, buffer, 8, 4);

            return buffer;
        }
    };

    public class M_GEO_OBJ_POS //geo object position in a specific map
    {
        public int layerPos;
        public int innerLayerObjectPos;

        public M_GEO_OBJ_POS()
        {
            layerPos = -1;
            innerLayerObjectPos = -1;
        }

        public M_GEO_OBJ_POS(int lyrPos, int inLyrPos)
        {
            layerPos = lyrPos;
            innerLayerObjectPos = inLyrPos;
        }

        public M_GEO_OBJ_POS(byte[] strObjPos)
        {
            layerPos = BitConverter.ToInt32(strObjPos, 0);
            innerLayerObjectPos = BitConverter.ToInt32(strObjPos, 4);
        }

        public byte[] ToBytes()
        {
            byte[] buffer = new byte[8];
            byte[] buf1 = BitConverter.GetBytes(layerPos);
            Array.Copy(buf1, 0, buffer, 0, 4);
            byte[] buf2 = BitConverter.GetBytes(innerLayerObjectPos);
            Array.Copy(buf2, 0, buffer, 4, 4);

            return buffer;
        }
    };

    public class InteropEncDotNet
    {
        public static string GetDegreeStringFromGeoCoor(bool bLongOrLatiCoor, int coorVal, int coorMultiFactor)
        {
            string retDegreeString = "";
            double fArcByDegree = coorVal / (float)coorMultiFactor;

            if (bLongOrLatiCoor)
            {
                if (fArcByDegree >= 0)
                {
                    retDegreeString = ((int)fArcByDegree).ToString() + "度" + (60 * (fArcByDegree - (int)fArcByDegree)).ToString() + "分E";
                }
                else
                {
                    fArcByDegree = -fArcByDegree;
                    retDegreeString = ((int)fArcByDegree).ToString() + "度" + (60 * (fArcByDegree - (int)fArcByDegree)).ToString() + "分W";
                }
            }
            else
            {
                if (fArcByDegree >= 0)
                {
                    retDegreeString = ((int)fArcByDegree).ToString() + "度" + (60 * (fArcByDegree - (int)fArcByDegree)).ToString() + "分N";
                }
                else
                {
                    fArcByDegree = -fArcByDegree;
                    retDegreeString = ((int)fArcByDegree).ToString() + "度" + (60 * (fArcByDegree - (int)fArcByDegree)).ToString() + "分S";
                }
            }

            return retDegreeString;
        }

        private static string GetStringFromBytes(byte[] buffer, int bufLen)
        {
            int charCount = bufLen / 2;
            char[] ca = new char[charCount];
            for (int charNum = 0; charNum < charCount; charNum++)
            {
                ca[charNum] = BitConverter.ToChar(buffer, charNum * 2);
            }

            return (new string(ca, 0, charCount));
        }

        private static byte[] GetBytesFromString(string str, int charCount)
        {
            byte[] buffer = new byte[charCount * 2];
            for (int charNum = 0; charNum < charCount; charNum++)
            {
                byte[] elementBuf = BitConverter.GetBytes(str.ToCharArray()[charNum]);
                Array.Copy(elementBuf, 0, buffer, charNum * 2, 2);
            }

            return buffer;
        }

        public static M_POINT[] GetPointArrayFromString(string strPoints, int elementCount)
        {
            byte[] buffer = GetBytesFromString(strPoints, elementCount * 4);
            M_POINT[] retPoints = new M_POINT[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                byte[] newElementBuffer = new byte[8];
                Array.Copy(buffer, i * 8, newElementBuffer, 0, 8);
                retPoints[i] = new M_POINT(newElementBuffer);
            }

            return retPoints;
        }

        //获取M_Point[]的string
        public static string GetStringFromPointArray(M_POINT[] pointArray, int elementCount)
        {
            int bufLen = elementCount * 8 + 8;
            byte[] buffer = new byte[bufLen];
            for (int i = 0; i < elementCount; i++)
            {
                byte[] elemBuf = pointArray[i].ToBytes();
                Array.Copy(elemBuf, 0, buffer, i * 8, 8);
            }

            string str = GetStringFromBytes(buffer, bufLen);
            return str;
        }

        public static M_GEO_OBJ_POS[] GetObjPosArrayFromString(string strObjPoses, int elementCount)
        {
            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(strObjPoses);
            M_GEO_OBJ_POS[] retObjPoses = new M_GEO_OBJ_POS[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                byte[] newElementBuffer = new byte[8];
                Array.Copy(buffer, i * 8, newElementBuffer, 0, 8);
                retObjPoses[i] = new M_GEO_OBJ_POS(newElementBuffer);
            }

            return retObjPoses;
        }

        public static MEM_GEO_OBJ_POS[] GetMemObjPosArrayFromString(string strObjPoses, int elementCount)
        {
            byte[] buffer = new byte[elementCount * 12 + 2];
            buffer = UnicodeEncoding.Unicode.GetBytes(strObjPoses);
            MEM_GEO_OBJ_POS[] retObjPoses = new MEM_GEO_OBJ_POS[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                byte[] newElementBuffer = new byte[12];
                Array.Copy(buffer, i * 12, newElementBuffer, 0, 12);
                retObjPoses[i] = new MEM_GEO_OBJ_POS(newElementBuffer);
            }

            return retObjPoses;
        }

        public static string GetStringFromMemGeoObjPos(MEM_GEO_OBJ_POS objPos)
        {
            return UnicodeEncoding.Unicode.GetString(objPos.ToBytes());
        }

        public static string GetStringFromIntArray(int[] intArray, int elementCount)
        {
            byte[] buffer = new byte[elementCount * 4];
            for (int i = 0; i < elementCount; i++)
            {
                byte[] elemBuf = BitConverter.GetBytes(intArray[i]);
                Array.Copy(elemBuf, 0, buffer, i * 4, 4);
            }
            return UnicodeEncoding.Unicode.GetString(buffer);
        }

        public static int[] GetIntArrayFromString(string str, int elementCount)
        {
            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(str.ToCharArray(), 0, elementCount * 2);
            int[] retIntArray = new int[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                retIntArray[i] = BitConverter.ToInt32(buffer, i * 4);
            }

            return retIntArray;
        }

        static DateTime m_dt = new DateTime(2010, 1, 1, 0, 0, 0);
        public static int DataTime2Int(DateTime curDateTime)
        {
            TimeSpan span = curDateTime - m_dt;
            int iResult = (int)span.TotalSeconds;
            return iResult;
        }
    };
}

