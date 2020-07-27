using AxYIMAENCLib;
using PortShipScheduleSystem.core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PortShipScheduleSystem.mapdata
{
    //public class DataShipMan
    //{
    //    public AxYimaEnc m_pYimaEnc = null;
    //    private List<Ais_Type_Style> m_arrTypeStyle;
    //    public List<Ship> m_arrShips;
    //    public DataShipMan(AxYimaEnc _pYimaEnc)
    //    {
    //        m_pYimaEnc = _pYimaEnc;
    //        InitShipAttributes();
    //        m_arrShips = new List<Ship>();
    //    }

    //    //初始化船舶属性
    //    public void InitShipAttributes()
    //    {
    //        m_pYimaEnc.SetDrawOwnShipSpecialOptions(true, false, 1, 255);//设置本船当前轨迹样式 
    //        //m_pYimaEnc.SetDrawShipOptions(false, true, true, true, 1, true, 1, 500, 200, 60);  //设置目标船舶显示样式
    //        m_pYimaEnc.SetDrawShipOptions(false, true, true, true, 1, true, 1, 0, 0, 0);  //设置目标船舶显示样式 
    //        m_pYimaEnc.SetShipTrackStyle(true, 0, true, new M_COLOR(255, 0, 0).ToInt(), 2, false, 4, new M_COLOR(0, 255, 0).ToInt());
    //        m_pYimaEnc.SetShipTrackShowOrNot(true, false, -1, true);
    //        m_pYimaEnc.SetShipTrackShowOrNot(false, true, -1, true);//轨迹线的全局设置

    //        //----船舶样式初始化 
    //        m_arrTypeStyle = new List<Ais_Type_Style>(3);
    //        //----------创建CLASS_A船舶样式 
    //        Ais_Type_Style curAisTypeStyle = new Ais_Type_Style();
    //        SHIP_TYPE iShipType = SHIP_TYPE.CLASS_A;
    //        int iOnLineId = m_pYimaEnc.AddAisType(iShipType.ToString()); //添加AIS类型(AIS TYPE)(在线)
    //        m_pYimaEnc.SetAisTypeInfo(iOnLineId, iShipType.ToString(), false, 1, 1, false, 0, 2, 5000000, true, false, 200000, true);//设置该AIS 类型的显示风格
    //        int iOffLineId = m_pYimaEnc.AddAisType(iShipType.ToString()); //添加AIS类型(AIS TYPE)(离线)
    //        m_pYimaEnc.SetAisTypeInfo(iOffLineId, iShipType.ToString(), false, 3, 1, false, 0, 7, 5000000, true, false, 200000, true);//设置该AIS 类型的显示风格
    //        curAisTypeStyle.SetAisTypeInfo(iShipType, iOnLineId, iOffLineId);
    //        m_arrTypeStyle.Add(curAisTypeStyle);

    //        //-------------创建CLASS_B船舶样式
    //        Ais_Type_Style curAisTypeStyle1 = new Ais_Type_Style();
    //        SHIP_TYPE iShipType1 = SHIP_TYPE.CLASS_B;
    //        int iOnLineId1 = m_pYimaEnc.AddAisType(iShipType1.ToString()); //添加AIS类型(AIS TYPE)(在线)
    //        m_pYimaEnc.SetAisTypeInfo(iOnLineId1, iShipType1.ToString(), false, 4, 1, false, 0, 6, 5000000, true, false, 0, true);//设置该AIS 类型的显示风格
    //        int iOffLineId1 = m_pYimaEnc.AddAisType(iShipType1.ToString()); //添加AIS类型(AIS TYPE)(在线)
    //        m_pYimaEnc.SetAisTypeInfo(iOffLineId1, iShipType1.ToString(), false, 5, 1, false, 0, 7, 5000000, true, false, 0, true);//设置该AIS 类型的显示风格
    //        curAisTypeStyle1.SetAisTypeInfo(iShipType1, iOnLineId1, iOffLineId1);
    //        m_arrTypeStyle.Add(curAisTypeStyle1);

    //        //------------创建北斗船舶样式
    //        Ais_Type_Style curAisTypeStyle2 = new Ais_Type_Style();
    //        SHIP_TYPE iShipType2 = SHIP_TYPE.BeiDou;
    //        int iOnLineId2 = m_pYimaEnc.AddAisType(iShipType2.ToString()); //添加AIS类型(AIS TYPE)(在线)
    //        m_pYimaEnc.SetAisTypeInfo(iOnLineId2, iShipType2.ToString(), false, 19, 1, false, 0, 2, 5000000, true, false, 0, false);//设置该AIS 类型的显示风格
    //        int iOffLineId2 = m_pYimaEnc.AddAisType(iShipType2.ToString()); //添加AIS类型(AIS TYPE)(在线)
    //        m_pYimaEnc.SetAisTypeInfo(iOffLineId2, iShipType2.ToString(), false, 20, 1, false, 0, 7, 5000000, true, false, 0, true);//设置该AIS 类型的显示风格
    //        curAisTypeStyle2.SetAisTypeInfo(iShipType2, iOnLineId2, iOffLineId2);
    //        m_arrTypeStyle.Add(curAisTypeStyle2);

    //    }

    //    public void InitShipDataLater()
    //    {
    //        //从服务器下载船舶 

    //        Random rd = new Random();
    //        for (int i = 0; i < 20; i++)
    //        {
    //            Ship shipNew = new Ship();

    //            shipNew.bOnline = true;
    //            //shipNew.shipType = (SHIP_TYPE)rd.Next(1, 4);
    //            shipNew.shipType = (SHIP_TYPE)3;

    //            shipNew.currentGeoPoX = 1210000000 + rd.Next(10000000, 50000000);
    //            shipNew.currentGeoPoY = 300000000 + rd.Next(-15000000, 15000000);
    //            shipNew.heading = rd.Next(0, 360);
    //            shipNew.speed = rd.Next(0, 30);
    //            shipNew.course = shipNew.heading;
    //            shipNew.shipLength = rd.Next(30, 100);
    //            shipNew.shipBreath = rd.Next(20, 50);
    //            shipNew.mmsi = rd.Next(423456789, 912345678);
    //            shipNew.strShipName = "船舶" + shipNew.mmsi.ToString();
    //            shipNew.shipDbId = 20 + i;
    //            m_arrShips.Add(shipNew);
    //        }

    //        int shipCount = m_arrShips.Count();
    //        string strTmpatr = "";
    //        M_COLOR mTextColor = new M_COLOR(0, 0, 0);
    //        for (int i = 0; i < shipCount; i++)
    //        {
    //            Ship curShip = m_arrShips[i];
    //            int iShipId = m_pYimaEnc.AddOtherVessel(false, curShip.currentGeoPoX, curShip.currentGeoPoY, curShip.heading, curShip.course, 0, curShip.speed, 0);
    //            int iShipPos = m_pYimaEnc.GetOtherVesselCount() - 1;
    //            m_pYimaEnc.SetOtherVesselBasicInfo(iShipPos, curShip.shipLength, curShip.shipBreath, curShip.strShipName, curShip.mmsi, ref strTmpatr, 0);
    //            int selTypeId = GetShipAisTypeId(curShip.shipType, curShip.bOnline);
    //            m_pYimaEnc.SetAisTargetType(iShipPos, selTypeId);
    //            m_pYimaEnc.SetOtherVesselShowText(iShipPos, true, curShip.strShipName, false, 12, true, mTextColor.ToInt());
    //            m_pYimaEnc.SetShipTrackShowOrNot(false, false, iShipId, false);

    //            m_arrShips[i].shipSdkId = iShipId;
    //        }
    //    }

    //    //获取船舶数据表
    //    public DataTable GetShipInfoTable(int maxSelCount = 100)
    //    {
    //        DataTable dtShipInfo = new DataTable("DTShipInfo");
    //        DataColumn[] columns = new DataColumn[]
    //        {
    //            new DataColumn("序号"),
    //            new DataColumn("Id"), new DataColumn("船名"), new DataColumn("类型"), new DataColumn("经度"), new DataColumn("纬度"),
    //            new DataColumn("船长"), new DataColumn("船宽"), new DataColumn("mmsi"),new DataColumn("在线状态"), new DataColumn("船首向"),
    //            new DataColumn("航速"),  new DataColumn("航向")
    //        };
    //        dtShipInfo.Columns.AddRange(columns);//添加列，即属性名

    //        int allShipCount = m_arrShips.Count();
    //        int selShippCount = 0;
    //        for (int i = 0; i < allShipCount; i++)
    //        {
    //            //添加行，即属性值
    //            DataRow dr = dtShipInfo.NewRow();
    //            Ship curShip = m_arrShips[i];
    //            dr[0] = i + 1;
    //            dr[1] = curShip.shipDbId;
    //            dr[2] = curShip.strShipName;
    //            dr[3] = curShip.shipType;
    //            dr[4] = curShip.currentGeoPoX;
    //            dr[5] = curShip.currentGeoPoY;
    //            dr[6] = curShip.shipLength;
    //            dr[7] = curShip.shipBreath;
    //            dr[8] = curShip.mmsi;
    //            dr[9] = curShip.bOnline;
    //            dr[10] = curShip.heading + "°";
    //            dr[11] = curShip.speed;
    //            dr[12] = curShip.course;
    //            dtShipInfo.Rows.Add(dr);
    //            selShippCount++;
    //            if (selShippCount == maxSelCount)
    //            {
    //                break;
    //            }
    //        }
    //        return dtShipInfo;
    //    }

    //    public void UpdateShipPosion()
    //    {
    //        //从服务器更新船位  
    //    }

    //    //获取船舶应该显示的AisType
    //    public int GetShipAisTypeId(SHIP_TYPE shipType, bool bOnLine)
    //    {
    //        int iTypeCount = m_arrTypeStyle.Count();
    //        for (int i = 0; i < iTypeCount; i++)
    //        {
    //            if (m_arrTypeStyle[i].iShipType == shipType)
    //            {
    //                if (bOnLine)
    //                    return m_arrTypeStyle[i].iOnlineAisTypeId;
    //                else
    //                    return m_arrTypeStyle[i].iOnlineAisTypeId;
    //            }
    //        }
    //        return 0;
    //    }
    //}


    ////船舶属性
    //public class Ship
    //{
    //    public Ship()
    //    {

    //    }

    //    public int shipSdkId { get; set; }  //组件ID
    //    public int shipDbId { get; set; } //数据库ID, 表ship_StaticInfo

    //    //船舶静态信息
    //    public string strShipName { get; set; }
    //    public string strShipEnglishName { get; set; }
    //    public int companyId { get; set; } //所属公司，表CompanyInfo
    //    public int mmsi { get; set; }
    //    public int cdma { get; set; }
    //    public int beidouId { get; set; }
    //    public int bdAddr { get; set; }
    //    public SHIP_TYPE shipType { get; set; }
    //    public int Owner { get; set; }//船东id，表ship_SailorInfo
    //    public int driver { get; set; }//驾驶员id，表CompanyInfo
    //    public string fishingPermit { get; set; }
    //    public int IMO { get; set; }
    //    public string callSign { get; set; }
    //    public float shipLength { get; set; }
    //    public float shipBreath { get; set; }
    //    public float shipWeight { get; set; }
    //    public int draftDepth { get; set; }
    //    public string textture { get; set; }
    //    public int windProofLevel { get; set; }
    //    public string nativePlace { get; set; }
    //    public float power { get; set; }
    //    public int buildTime { get; set; }
    //    public int addTime { get; set; }


    //    //船舶动态信息
    //    public int currentGeoPoX { get; set; }
    //    public int currentGeoPoY { get; set; }
    //    public float heading { get; set; }
    //    public float course { get; set; }
    //    public float speed { get; set; }
    //    public int reportTime { get; set; }
    //    public string arrivalPlace { get; set; }
    //    public int arrivalTime { get; set; }
    //    public int fleetId { get; set; }
    //    public bool bInPort { get; set; }
    //    public int portId { get; set; }
    //    public bool bOnline { get; set; }

    //}
}
