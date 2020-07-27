using AxYIMAENCLib;
using PortShipScheduleSystem.core;
using PortShipScheduleSystem.mapdata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PortShipScheduleSystem
{   
    //一个升压器对应多个风车的设计
    //风车结构
    public struct StructWind
    {
        public M_POINT Position;    //位置信息
        public int LineColor;       //线的颜色
        public StructWind(int GeoPoX, int GeoPoY, int curLineColor)
        {
            this.Position = new M_POINT(GeoPoX, GeoPoY);
            this.LineColor = curLineColor;
        }

        //public string Power;    //功率
        //public string WindSpeed;//起动风速
        //public string AxisType; //轴类型
    }

    //升压器结构
    public struct StructVoltage
    {
        public M_POINT Position;    //位置信息
        public int WindPos;         //链接的风车pos
        //public string Winding;      //绕组
        //public string PhaseNum;     //相数
        //public string CoolingType;  //冷却方式
        public StructVoltage(int GeoPoX, int GeoPoY, int curWindPos)
        {
            this.Position = new M_POINT(GeoPoX, GeoPoY);
            this.WindPos = curWindPos;
        }
    }

    //轨迹点结构
    public struct StructTrackPointInfo
    {
        //public M_POINT trackGeoPo; //轨迹点地理坐标
        public DateTime noteTime;       //记录该点的时间
        public float speed;        //航行速度
        public float course;       //航行方向
        public StructTrackPointInfo(DateTime tNoteTime, float fSpeed, float fCourse)
        {
            //trackGeoPo = new M_POINT(geoPoX, geoPoY);
            noteTime = tNoteTime;
            speed = fSpeed;
            course = fCourse;
        }

    }

    //轨迹线结构
    public struct StructTrackInfo
    {
        public int mmsi;
        public string shipName;
        public List<StructTrackPointInfo> shipTrackPoInfo;  //轨迹点其他信息，考虑到要取轨迹点数组，将地理坐标信息分离
        public List<M_POINT> shipTrackPo;                   //轨迹点地理坐标
        public int trackCount;              //轨迹点数量 
        public int trackColor;     //轨迹线颜色
        public int lineSize;       //轨迹线宽

        //设置回放轨迹的基本信息
        public void SetBackingShipBasicInfo(string strShipName, int iShipMmsi)
        {
            shipName = strShipName;
            mmsi = iShipMmsi;
        }

        //设置轨迹线样式
        public void SetBackingTrackStyle(M_COLOR mColor, int iLineSize)
        {
            trackColor = mColor.ToInt();
            lineSize = iLineSize;
        }

        //添加一个轨迹点
        public void AppendTrackPoInfo(int geoPoX, int geoPoY, DateTime tNoteTime, float fSpeed, float fCourse)
        {
            shipTrackPo.Add(new M_POINT(geoPoX, geoPoY));
            shipTrackPoInfo.Add(new StructTrackPointInfo(tNoteTime, fSpeed, fCourse));
            trackCount += 1;
        }

        //模拟数据
        public void GetTrackPo()
        {
            if (shipTrackPoInfo == null)
            {
                shipTrackPoInfo = new List<StructTrackPointInfo>();
            }
            if (shipTrackPo == null)
            {
                shipTrackPo = new List<M_POINT>();
            }

            if (shipTrackPo.Count > 0)
                return;

            //模拟轨迹点其他信息 
            DateTime tNoteTime = DateTime.Now;
            tNoteTime = tNoteTime.AddMinutes(-540);
            float fSpeed = 20;
            AppendTrackPoInfo(1209887416, 304457949, tNoteTime.AddMinutes(30), fSpeed, 85.3f);
            AppendTrackPoInfo(1211157993, 304555079, tNoteTime.AddMinutes(60), fSpeed, 85.3f);
            AppendTrackPoInfo(1212615420, 304846410, tNoteTime.AddMinutes(130), fSpeed, 76.6f);
            AppendTrackPoInfo(1214371806, 305396463, tNoteTime.AddMinutes(150), fSpeed, 69.4f);
            AppendTrackPoInfo(1216090822, 306075505, tNoteTime.AddMinutes(180), fSpeed, 66.3f);
            AppendTrackPoInfo(1217398769, 306560240, tNoteTime.AddMinutes(230), fSpeed, 66.8f);
            AppendTrackPoInfo(1218744086, 306980146, tNoteTime.AddMinutes(280), fSpeed, 70.1f);
            AppendTrackPoInfo(1220089403, 307851669, tNoteTime.AddMinutes(300), fSpeed, 53.0f);
            AppendTrackPoInfo(1221359980, 309076907, tNoteTime.AddMinutes(330), fSpeed, 48.9f);
            AppendTrackPoInfo(1222443708, 309882122, tNoteTime.AddMinutes(360), fSpeed, 52.1f);
            AppendTrackPoInfo(1223789025, 310783148, tNoteTime.AddMinutes(390), fSpeed, 52.1f);
            AppendTrackPoInfo(1224125354, 312357869, tNoteTime.AddMinutes(420), fSpeed, 10.6f);
            AppendTrackPoInfo(1223265846, 313769651, tNoteTime.AddMinutes(430), fSpeed, 332.3f);
            AppendTrackPoInfo(1223116366, 314891133, tNoteTime.AddMinutes(470), fSpeed, 353.9f);
            AppendTrackPoInfo(1221658940, 315787341, tNoteTime.AddMinutes(490), fSpeed, 306.2f);
            AppendTrackPoInfo(1220350992, 316011257, tNoteTime.AddMinutes(530), fSpeed, 306.2f);
        }
    }

    public struct StructTrackScrnInfo
    {
        public int ScrnPoX;     //屏幕的x坐标
        public int ScrnPoY;     //屏幕的y坐标 
        public int TrackPointPos;       //轨迹点的Pos 
        public StructTrackScrnInfo(int scrnX, int scrnY, int pos)
        {
            ScrnPoX = scrnX;
            ScrnPoY = scrnY;
            TrackPointPos = pos;
        }
    }
    public partial class FromMain : Form
    {
        public List<string> m_strWarnLyNames; //需要标记的图层名称
        public List<int> m_warnLyPoses;       //需要标记的图层索引
        public List<MEM_GEO_OBJ_POS> m_closedObjPos;//检测到的靠近本船的物标
        public List<M_POINT> m_closedShipGeoPo;     //检测到的靠近本船的船舶的地理坐标
        public double m_judgeDisByMeter;      //判距，单位米

        private StructTrackInfo m_shipBackingTrack;
        private int curPlayStep;    //回放轨迹的时间戳
        private List<StructTrackScrnInfo> m_TrackScrnPoInPlaying;//轨迹回放时轨迹点的屏幕坐标

        private int m_windmillLayerPos, m_voltageLayerPos; //风车图层、升压器图层索引
        private List<StructVoltage> m_VoltageLink;  //升压器
        private List<StructWind> m_allWindmill;      //风车
        private bool m_bAddVolWindByMouse;         //是否使用鼠标添加风车组 

        private bool bPointScaled = true;
        private bool bAnimationScaled = false;
        private bool bRealTimeDrag = false;
        public int m_testLayerPos = 0; //自定义标绘测试图层 
        public const int MAX_SEL_OBJS_COUNT = 255;
        public bool m_bLoaded = false;

        Thread m_thInitValue = null;
        public string strUserMapFilePath;  //自定义物标文件路径

        private int getX = 0;
        private int getY = 0;

        public FromMain()
        {
            InitializeComponent();
            m_strWarnLyNames = new List<string>() { "海岸线", "灯标", "沉船", "障碍物" };
            m_warnLyPoses = new List<int>();
            m_closedObjPos = new List<MEM_GEO_OBJ_POS>();
            m_closedShipGeoPo = new List<M_POINT>();
            m_judgeDisByMeter = 50000;
            curPlayStep = -1;
            m_allWindmill = new List<StructWind>();
            m_VoltageLink = new List<StructVoltage>();
            m_bAddVolWindByMouse = false;
            m_TrackScrnPoInPlaying = new List<StructTrackScrnInfo>();
            m_windmillLayerPos = -1;
            m_voltageLayerPos = -1;

        }

        #region 窗体事件
        /// <summary>
        /// 初始化
        /// </summary> 
        private void FromMain_Load(object sender, EventArgs e)
        {
            this.treeView1.ExpandAll();
            yimaEncCtrl.Dock = DockStyle.Fill;
            yimaEncCtrl.SendToBack();   //将海图控件置于最低层，为了在海图上悬浮其他控件
            menuStrip1.SendToBack();    //海图控件置底后，应将菜单栏、工具栏、状态栏等重新放在海图底下

            string strExecutePath = System.Windows.Forms.Application.ExecutablePath;
            int dotPos = strExecutePath.LastIndexOf('\\');
            int dotPos2 = strExecutePath.LastIndexOf('/');
            dotPos = dotPos2 > dotPos ? dotPos2 : dotPos;
            strExecutePath = strExecutePath.Substring(0, dotPos);
            bool bInitSuc = yimaEncCtrl.Init(strExecutePath);
            if (!bInitSuc)
            {
                MessageBox.Show("组件初始化失败！");
            }
            yimaEncCtrl.RefreshDrawer((int)this.Handle, this.yimaEncCtrl.Width, this.yimaEncCtrl.Height, 0, 0);
            yimaEncCtrl.OverViewLibMap(0);
            yimaEncCtrl.SetCurrentScale(2000000);
            yimaEncCtrl.ToolSetMapScaledMode(false, false);
            strUserMapFilePath = strExecutePath + "\\UserLayerInfo.ymc";
            yimaEncCtrl.tmOpenMapDataFile(strUserMapFilePath);
            GetWarnLayerPoses();
            AddShip(10);

            AddUserLayer();
            GetWindAndVoltagePoint();
            AddAllWindAndVoltageToMap();

            快速居中ToolStripMenuItem_Click(null, null);
            timer1.Enabled = true;
            timer1.Interval = 1500;
            m_bLoaded = true;

            //m_thInitValue = new Thread(AddRadarInfoTest); //显示雷达
            //m_thInitValue.Start();


        }

        /// <summary>
        /// 为了能在多线程加载图完成后自动刷新，需调用Paint事件绘制海图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromMain_Paint(object sender, PaintEventArgs e)
        {
            RedrawMapScreen();
        }

        /// <summary>
        /// 当窗体大小发生改变
        /// </summary> 
        private void FromMain_Resize(object sender, EventArgs e)
        {
            if (m_bLoaded == false) return;
            int imapScrnWidth = this.yimaEncCtrl.Width;
            int imapScrnHeight = this.yimaEncCtrl.Height;
            if (imapScrnWidth > 0 && imapScrnHeight > 0)
            {
                yimaEncCtrl.RefreshDrawer((int)this.Handle, imapScrnWidth, imapScrnHeight, 0, 0);
                RedrawMapScreen();
            }
        }

        private void FromMain_Deactivate(object sender, EventArgs e)
        {
            toolTip1.Active = false;
        }

        private void FromMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_thInitValue != null && m_thInitValue.ThreadState == ThreadState.Running)
            {
                m_thInitValue.Abort();
            }
            m_thInitValue = null;
        }

        private void FromMain_Activated(object sender, EventArgs e)
        {
            toolTip1.Active = false;
        }
        #endregion

        /// <summary>
        /// 刷新海图
        /// </summary>
        /// <param name="bForcyRefreash">true = 强制刷新,一般为false</param>
        public void RedrawMapScreen(bool bForcyRefreash = false)
        {
            try
            {
                if (bForcyRefreash)
                {
                    yimaEncCtrl.ForcelyRefreshMap();//强制刷新海图 
                }
                yimaEncCtrl.DrawMapsInScreen(0);
            }
            catch (Exception e)
            {
#if DEBUG
                MessageBox.Show("RedrawMapScreen Fail：" + e.Message);
#endif
            }
        }

        /// <summary>
        /// 定时器触发事件
        /// </summary>  
        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreashShipPosition();
            PlayBacking();
            if (yimaEncCtrl.ToolIsNoOperate())
            {
                RedrawMapScreen();
            }
        }

        #region 海图SDK事件
        private void yimaEncCtrl_OnBeginLButtonDown(object sender, AxYIMAENCLib._DYimaEncEvents_OnBeginLButtonDownEvent e)
        {
            if (m_bAddVolWindByMouse)
            {
                int GeoPoX = 0, GeoPoY = 0;
                yimaEncCtrl.GetGeoPoFromScrnPo(e.pointX, e.pointY, ref GeoPoX, ref GeoPoY);
                Random rd = new Random();
                byte r = (byte)rd.Next(0, 255);
                byte g = (byte)rd.Next(0, 255);
                byte b = (byte)rd.Next(0, 255);

                m_allWindmill.Add(new StructWind(GeoPoX + 1000000, GeoPoY + 1000000, new M_COLOR(r, g, b).ToInt()));
                int curWindPos = m_allWindmill.Count - 1;
                m_VoltageLink.Add(new StructVoltage(GeoPoX, GeoPoY, curWindPos));
                int curVolPos = m_VoltageLink.Count - 1;
                AddOneCoupleVolWind(curVolPos);
                RedrawMapScreen();
            }
            else if (yimaEncCtrl.ToolIsNoOperate())
            {
                GetUserObjTips(e.pointX, e.pointY);
                //GetTrackPointTips(e.pointX, e.pointY);
                yimaEncCtrl.SetPointSelectJudgeDist(30, 30);
                int selShipId = yimaEncCtrl.SelectOtherVesselByScrnPoint(e.pointX, e.pointY);
                if (selShipId >= 1)
                {
                    int selShipPos = yimaEncCtrl.GetOtherVesselPosOfID(selShipId);
                    float retShipLength = 0;
                    float retShipBreath = 0;
                    string retStrShipName = new string('\0', 50);
                    int retMmsi = 0;
                    string pExtAttrs = "";
                    yimaEncCtrl.GetOtherVesselBasicInfo(selShipPos, ref retShipLength, ref retShipBreath, ref retStrShipName, ref retMmsi, ref pExtAttrs);
                    string strTip = "船名：" + retStrShipName.Replace("\0", "") + "\n";
                    strTip += "MMSI：" + retMmsi + "\n";
                    strTip += "长度：" + retShipLength + "米" + "\n";
                    strTip += "宽度：" + retShipBreath + "米" + "\n";
                    int curGeoPoX = 0;
                    int curGeoPoY = 0;
                    bool bArpaOrAis = false;
                    int pTime = 0;
                    float heading = 0;
                    float courseOverGround = 0;
                    float courseThrghWater = 0;
                    float speedOverGround = 0;
                    float speedThrghWater = 0;
                    yimaEncCtrl.GetOtherVesselCurrentInfo(selShipPos, ref bArpaOrAis, ref curGeoPoX, ref curGeoPoY, ref heading, ref courseOverGround, ref courseThrghWater, ref speedOverGround, ref speedThrghWater, ref pTime, ref pExtAttrs);
                    strTip += "经度：" + curGeoPoX + "\n";
                    strTip += "纬度：" + curGeoPoY + "\n";
                    strTip += "船首向：" + heading + "\n";
                    strTip += "航速：" + courseOverGround + "\n";
                    strTip += "航向：" + speedOverGround + "\n";
                    toolTip1.SetToolTip(this.yimaEncCtrl, strTip);
                }
            }

        }

        private void yimaEncCtrl_OnEndLButtonDown(object sender, AxYIMAENCLib._DYimaEncEvents_OnEndLButtonDownEvent e)
        {

        }

        private void yimaEncCtrl_OnEndLButtonUp(object sender, AxYIMAENCLib._DYimaEncEvents_OnEndLButtonUpEvent e)
        {

        }

        private void yimaEncCtrl_OnBeginRButtonDown(object sender, AxYIMAENCLib._DYimaEncEvents_OnBeginRButtonDownEvent e)
        {
            yimaEncCtrl.GetGeoPoFromScrnPo(e.pointX, e.pointY, ref getX, ref getY);

            bool canOpenRightMenu = true;
            //bool bIsInDeleteState = true; //是否为删除物标状态
            if (yimaEncCtrl.ToolIsNoOperate()) //&& false == bIsInDeleteState)
            {
                string strRetObjPos = new string(' ', sizeof(int) * 3 * MAX_SEL_OBJS_COUNT + 2);
                int selObjCount = yimaEncCtrl.SelectObjectsByScrnPoint(ref strRetObjPos, e.pointX, e.pointY);
                if (selObjCount > 0)
                {
                    MEM_GEO_OBJ_POS[] arrRetObjPos = new MEM_GEO_OBJ_POS[selObjCount];
                    arrRetObjPos = InteropEncDotNet.GetMemObjPosArrayFromString(strRetObjPos, selObjCount);
                    MEM_GEO_OBJ_POS frstSelectObjectPos = arrRetObjPos[0];
                    if (frstSelectObjectPos.memMapPos == yimaEncCtrl.GetMemMapCount() - 1)
                    {
                        //风车和升压器不支持移动
                        if (frstSelectObjectPos.layerPos != m_voltageLayerPos && frstSelectObjectPos.layerPos != m_windmillLayerPos)
                        {
                            //如果选中的是自定义海图对象, 则进入编辑
                            bool bRetOk = yimaEncCtrl.ToolEditingUserObject(frstSelectObjectPos.layerPos, frstSelectObjectPos.innerLayerPos);
                            if (bRetOk)
                            {
                                canOpenRightMenu = false;
                                RedrawMapScreen();
                            }
                            else
                            {
                                MessageBox.Show("请确认物标是否存在！");
                            }
                        }
                    }
                }
                else
                {
                }
            }
            else
                canOpenRightMenu = false;
            if (canOpenRightMenu)
            {
                //没有其他右键操作，弹出右键演示菜单
                //rightMenuDemo.Parent = yimaEncCtrl;
                rightMenuDemo.Show(yimaEncCtrl, e.pointX, e.pointY);
            }

        }

        M_POINT lastShowHintPo = new M_POINT(-100, -100);

        private void yimaEncCtrl_OnBeginMouseMove(object sender, AxYIMAENCLib._DYimaEncEvents_OnBeginMouseMoveEvent e)
        {
            int GeoPoX = 0;
            int GeoPoY = 0;
            yimaEncCtrl.GetGeoPoFromScrnPo(e.pointX, e.pointY, ref GeoPoX, ref GeoPoY);
            this.toolStripStatusLabel2.Text = GeoPoX + "," + GeoPoY;

            if (yimaEncCtrl.ToolIsNoOperate())
            {
                if (Math.Abs(e.pointX - lastShowHintPo.x) >= 10 || Math.Abs(e.pointY - lastShowHintPo.y) >= 10)
                {
                    GetTrackPointTips(e.pointX, e.pointY);
                }
            }
        }
        private void yimaEncCtrl_OnEndMouseMove(object sender, AxYIMAENCLib._DYimaEncEvents_OnEndMouseMoveEvent e)
        {

        }
        /// <summary>
        /// 组件绘制完海图后自动调用
        /// </summary>
        private void OnAfterDrawMap(object sender, AxYIMAENCLib._DYimaEncEvents_AfterDrawMapEvent e)
        {

            //Graphics gdiplus = Graphics.FromHdc((IntPtr)e.hdc);
            //Image pic = Image.FromFile("E:\\Picture\\素材\\allScreen.png");
            //gdiplus.DrawImage(pic, new Point(100, 100));
            //gdiplus.Dispose();//释放gdiPlus


            //yimaEncCtrl.TextOutAtPoint("文字位置测试", 0, 0);
            //以本船为中心，绘制扫描区，橙色
            float iScrnJudgeLen = yimaEncCtrl.GetScrnLenFromGeoLen((float)m_judgeDisByMeter * 1000);
            M_POINT curOwnShipGeoPo = new M_POINT();
            float ftmp = 0;
            yimaEncCtrl.GetOwnShipCurrentInfo(ref curOwnShipGeoPo.x, ref curOwnShipGeoPo.y, ref ftmp, ref ftmp, ref ftmp, ref ftmp, ref ftmp);
            M_POINT curOwnShipScrnPo = new M_POINT();
            yimaEncCtrl.GetScrnPoFromGeoPo(curOwnShipGeoPo.x, curOwnShipGeoPo.y, ref curOwnShipScrnPo.x, ref curOwnShipScrnPo.y);

            M_COLOR penColor = new M_COLOR(230, 120, 56);
            yimaEncCtrl.SetCurrentPen(2, penColor.ToInt());
            yimaEncCtrl.DrawCircle(curOwnShipScrnPo.x, curOwnShipScrnPo.y, (int)iScrnJudgeLen, false, true);

            //高亮物标,红色
            M_COLOR highLightCol = new M_COLOR(255, 0, 0);
            for (int k = 0; k < m_closedObjPos.Count; k++)
            {
                string pstrObjPos = InteropEncDotNet.GetStringFromMemGeoObjPos(m_closedObjPos[k]);
                yimaEncCtrl.HighLightSelectedObject(ref pstrObjPos, highLightCol.ToInt(), highLightCol.ToInt(), 1, highLightCol.ToInt(), 20, false, false, 1);
            }



            //高亮船舶,蓝色
            penColor = new M_COLOR(20, 145, 250);
            yimaEncCtrl.SetCurrentPen(2, penColor.ToInt());
            for (int i = 0; i < m_closedShipGeoPo.Count; i++)
            {
                int curScrnPoX = 0;
                int curScrnPoY = 0;
                yimaEncCtrl.GetScrnPoFromGeoPo(m_closedShipGeoPo[i].x, m_closedShipGeoPo[i].y, ref curScrnPoX, ref curScrnPoY);
                yimaEncCtrl.DrawCircle(curScrnPoX, curScrnPoY, 12, false, true);
            }

            //绘制回放轨迹线，红色
            if (curPlayStep != -1)
            {
                M_POINT[] shipTrackScrnPo = new M_POINT[curPlayStep];
                if (curPlayStep >= 2)
                {
                    yimaEncCtrl.SetCurrentPen(m_shipBackingTrack.lineSize, m_shipBackingTrack.trackColor);
                    string strShipGeoPo = InteropEncDotNet.GetStringFromPointArray(m_shipBackingTrack.shipTrackPo.ToArray(), curPlayStep);
                    string strShipScrnPo = new string('1', (curPlayStep) * 4 + 4);
                    yimaEncCtrl.GetScrnPointsFromGeoPoints(curPlayStep, ref strShipGeoPo, ref strShipScrnPo);
                    yimaEncCtrl.DrawPolyline(ref strShipScrnPo, curPlayStep);
                    shipTrackScrnPo = InteropEncDotNet.GetPointArrayFromString(strShipScrnPo, curPlayStep);
                }

                M_COLOR brushColor = new M_COLOR(20, 145, 250);
                yimaEncCtrl.SetCurrentBrush(brushColor.ToInt());
                int curScrnPoX = 0;
                int curScrnPoY = 0;
                List<M_POINT> shipTrackPo = m_shipBackingTrack.shipTrackPo;
                m_TrackScrnPoInPlaying.Clear();
                M_POINT lastDrawPoint = new M_POINT(-100, -100);
                if (BtnTrackStyle1.Checked)
                {
                    for (int i = 0; i < curPlayStep; i++)
                    {
                        //yimaEncCtrl.GetScrnPoFromGeoPo(shipTrackPo[i].x, shipTrackPo[i].y, ref curScrnPoX, ref curScrnPoY);
                        curScrnPoX = shipTrackScrnPo[i].x;
                        curScrnPoY = shipTrackScrnPo[i].y;
                        m_TrackScrnPoInPlaying.Add(new StructTrackScrnInfo(curScrnPoX, curScrnPoY, i));
                        if (Math.Abs(lastDrawPoint.x - curScrnPoX) > 12 || Math.Abs(lastDrawPoint.y - curScrnPoY) > 12)
                        {
                            yimaEncCtrl.DrawPointWithLibSymbol(23, curScrnPoX, curScrnPoY, false, 0, 1, 0);
                            lastDrawPoint.x = curScrnPoX;
                            lastDrawPoint.y = curScrnPoY;
                        }
                    }
                }
                else if (BtnTrackStyle2.Checked)
                {
                    for (int i = 0; i < curPlayStep; i++)
                    {
                        if (shipTrackScrnPo==null|| shipTrackScrnPo.Count()==0|| shipTrackScrnPo[0]==null)
                        {
                            continue;
                        }
                        //yimaEncCtrl.GetScrnPoFromGeoPo(shipTrackPo[i].x, shipTrackPo[i].y, ref curScrnPoX, ref curScrnPoY);
                        curScrnPoX = shipTrackScrnPo[i].x;
                        curScrnPoY = shipTrackScrnPo[i].y;
                        m_TrackScrnPoInPlaying.Add(new StructTrackScrnInfo(curScrnPoX, curScrnPoY, i));
                        if (Math.Abs(lastDrawPoint.x - curScrnPoX) > 12 || Math.Abs(lastDrawPoint.y - curScrnPoY) > 12)
                        {
                            //Random rd = new Random();
                            //for (int k = 0; k < 15; k++)
                            //{
                            //    int ofx = rd.Next(0, 90);
                            //    int ofy = rd.Next(0, 93);
                            yimaEncCtrl.DrawCircle(curScrnPoX, curScrnPoY, 4, true, true);
                            //}
                            lastDrawPoint.x = curScrnPoX;
                            lastDrawPoint.y = curScrnPoY;
                        }
                    }
                }
                if (curPlayStep - 1 > 0)
                    yimaEncCtrl.DrawPointWithLibSymbol(1, curScrnPoX, curScrnPoY, false, 0, 1, m_shipBackingTrack.shipTrackPoInfo[curPlayStep - 1].course);
            }

            //绘制升压器和风车的连接线, 蓝色
            int iVoltageCount = m_VoltageLink.Count;
            for (int i = 0; i < iVoltageCount; i++)
            {
                int startScrnX = 0, startScrnY = 0, endScrnX = 0, endScrnY = 0;
                yimaEncCtrl.GetScrnPoFromGeoPo(m_VoltageLink[i].Position.x, m_VoltageLink[i].Position.y, ref startScrnX, ref startScrnY);
                int iWindPos = m_VoltageLink[i].WindPos;  //获取对应的风车索引
                yimaEncCtrl.GetScrnPoFromGeoPo(m_allWindmill[iWindPos].Position.x, m_allWindmill[iWindPos].Position.y, ref endScrnX, ref endScrnY);

                yimaEncCtrl.SetCurrentPen(2, m_allWindmill[iWindPos].LineColor);
                yimaEncCtrl.DrawLine(startScrnX, startScrnY, endScrnX, endScrnY);
            }
        }

        /// <summary>
        /// 使用Tool...接口添加物标操作结束后调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yimaEncCtrl_OnEndAddObjectByTool(object sender, AxYIMAENCLib._DYimaEncEvents_OnEndAddObjectByToolEvent e)
        {
            //--------此事件中无需重绘海图，事件结束后将自动调用------// 
            M_COLOR textColor = new M_COLOR(0, 0, 0);
            M_COLOR lineColor = new M_COLOR(255, 0, 0);
            if (e.geoType == (int)M_GEO_TYPE.TYPE_POINT)
            {
                yimaEncCtrl.tmSetPointObjectStyle(e.layerPos, e.objPos, 1, false, 0, 0, true, "点标绘", "宋体", 12, textColor.ToInt(), true, false, 0, 0, 18, -5, 0);
                yimaEncCtrl.tmSetObjectTextRotate(e.layerPos, e.objPos, 0);
            }
            else if (e.geoType == (int)M_GEO_TYPE.TYPE_LINE)
            {
                string strShowText = "线标绘";
                switch ((SPECIAL_LINE_TYPE)e.specialType)
                {
                    case SPECIAL_LINE_TYPE.NO_SPECIAL_TYPE: break;
                    case SPECIAL_LINE_TYPE.RECTANGLE_LN_TYPE: strShowText += "（矩形）"; break;
                    case SPECIAL_LINE_TYPE.CIRCLE_LN_TYPE: strShowText += "（圆）"; break;
                    case SPECIAL_LINE_TYPE.ELLIPSE_LN_TYPE: strShowText += "（椭圆）"; break;
                    case SPECIAL_LINE_TYPE.ARC_LN_TYPE: strShowText += "（弧形）"; break;
                    default: strShowText += "（其他线形）"; break;
                }
                yimaEncCtrl.tmSetLineObjectStyleRefLib(e.layerPos, e.objPos, 1, true, lineColor.ToInt(), true, 2);
                yimaEncCtrl.tmSetLineObjectStyle(e.layerPos, e.objPos, true, true, 0, 0, 2, lineColor.ToInt(), 0, 0, strShowText, "宋体", 14, textColor.ToInt(), true, false, 0, 0, true);
                yimaEncCtrl.tmSetObjectTextRotate(e.layerPos, e.objPos, 30);
            }
            else if (e.geoType == (int)M_GEO_TYPE.TYPE_FACE)
            {
                M_COLOR faceFillColor = new M_COLOR(125, 125, 125);
                string strShowText = "面标绘";
                switch ((SPECIAL_LINE_TYPE)e.specialType)
                {
                    case SPECIAL_LINE_TYPE.NO_SPECIAL_TYPE: break;
                    case SPECIAL_LINE_TYPE.RECTANGLE_LN_TYPE: strShowText += "（矩形）"; break;
                    case SPECIAL_LINE_TYPE.CIRCLE_LN_TYPE: strShowText += "（圆）"; break;
                    case SPECIAL_LINE_TYPE.ELLIPSE_LN_TYPE: strShowText += "（椭圆）"; break;
                }
                yimaEncCtrl.tmSetFaceObjectStyle(e.layerPos, e.objPos, true, faceFillColor.ToInt(), 20, -1, 0, 0, strShowText, "宋体", 14, textColor.ToInt(), true, false, true, 0, 0);
                yimaEncCtrl.tmSetFaceObjectStyleRefLib(e.layerPos, e.objPos, 1, true, faceFillColor.ToInt(), 1, 80, false, 0);
                yimaEncCtrl.tmSetLineObjectStyleRefLib(e.layerPos, e.objPos, 1, true, lineColor.ToInt(), true, 2);   //边框样式
                yimaEncCtrl.tmSetObjectTextRotate(e.layerPos, e.objPos, -30);
            }
        }

        private void yimaEncCtrl_OnAddingNewNodeByTool(object sender, AxYIMAENCLib._DYimaEncEvents_OnAddingNewNodeByToolEvent e)
        {
            //在添加物标操作时，添加一个新节点触发该事件
        }

        private void yimaEncCtrl_OnBeginRButtonUp(object sender, AxYIMAENCLib._DYimaEncEvents_OnBeginRButtonUpEvent e)
        {
            /* //删除物标
            string strObjPos = new string('\0', 255*8);
            yimaEncCtrl.SetPointSelectJudgeDist(20, 15);//设置判距
            int count = yimaEncCtrl.tmSelectObjectsByScrnPo(e.pointX, e.pointY, ref strObjPos);
            //MessageBox.Show("tmSelectObjectsByScrnPo" + count.ToString()); 
            M_GEO_OBJ_POS[] arrObjPos = InteropEncDotNet.GetObjPosArrayFromString(strObjPos, count);  
            M_GEO_OBJ_POS[] arrObjId = new M_GEO_OBJ_POS[count];
            for (int i = 0; i < count; i++)
            {
                int layerId = -1;
                int objId = -1;
                yimaEncCtrl.tmGetObjectIDFromPos(arrObjPos[i].layerPos, arrObjPos[i].innerLayerObjectPos, ref layerId, ref objId);
                arrObjId[i] = new M_GEO_OBJ_POS(layerId, objId); 
            }
            for(int i = 0; i < count; i++)
            {
                int layerPos = -1; 
                int objPos = -1;
                yimaEncCtrl.tmGetObjectPosFromID(arrObjId[i].layerPos, arrObjId[i].innerLayerObjectPos, ref layerPos, ref objPos);
                yimaEncCtrl.tmDeleteGeoObject(layerPos, objPos);
            }
            RedrawMapScreen();*/
        }

        private void yimaEncCtrl_OnMapScaleChanged(object sender, AxYIMAENCLib._DYimaEncEvents_OnMapScaleChangedEvent e)
        {

        }

        private void yimaEncCtrl_OnMapWorking(object sender, AxYIMAENCLib._DYimaEncEvents_OnMapWorkingEvent e)
        {

        }

        private void yimaEncCtrl_OnBeginMouseWheel(object sender, AxYIMAENCLib._DYimaEncEvents_OnBeginMouseWheelEvent e)
        {

        }

        private void yimaEncCtrl_OnLButtonDoubleClick(object sender, AxYIMAENCLib._DYimaEncEvents_OnLButtonDoubleClickEvent e)
        {

        }

        #endregion

        #region 本船报警演示、添加船舶
        /// <summary>
        /// 添加本船和目标船
        /// </summary>
        /// <param name="OtherShipCount">添加目标船的数量</param>
        public void AddShip(int OtherShipCount)
        {
            yimaEncCtrl.SetDrawOwnShipSpecialOptions(true, false, 1, 255);//设置本船当前轨迹样式 
            yimaEncCtrl.SetDrawShipOptions(true, false, false, false, 1, true, 1, 500, 200, 60);//设置本船指向线配置

            yimaEncCtrl.SetDrawShipOptions(false, true, true, true, 1, true, 1, 500, 200, 60);  //设置目标船舶显示样式，有轨迹线
            //yimaEncCtrl.SetDrawShipOptions(false, true, true, true, 1, true, 1, 0, 0, 0);  //设置目标船舶显示样式，无轨迹线

            yimaEncCtrl.SetOwnShipBasicInfo(100, 40, "探测号", "1105");//初始化本船
            yimaEncCtrl.SetOwnShipCurrentInfo(1240000000, 310000000, 230, 230, 0, 70, 30);
            yimaEncCtrl.SetShipTrackStyle(true, 0, true, new M_COLOR(255, 0, 0).ToInt(), 2, false, 4, new M_COLOR(0, 255, 0).ToInt());

            string strAisTypeName = "AisTypeOne";
            int typeIdOne = yimaEncCtrl.AddAisType(strAisTypeName); //添加一种船舶样式(AIS TYPE)
            yimaEncCtrl.SetAisTypeInfo(typeIdOne, strAisTypeName, false, 1, 1, false, 0, 2, 5000000, false, false, 200000, true);//设置该AIS 类型的显示风格

            strAisTypeName = "AisTypeTwo";
            int typeIdTwo = yimaEncCtrl.AddAisType(strAisTypeName); //添加一种船舶样式(AIS TYPE)
            yimaEncCtrl.SetAisTypeInfo(typeIdTwo, strAisTypeName, false, 4, 1, false, 0, 6, 5000000, false, false, 200000, true);//设置该AIS 类型的显示风格

            Random ra = new Random();
            M_COLOR trackLineColor = new M_COLOR(0, 0, 255);
            M_COLOR trackLineColorBat = new M_COLOR(255, 60, 45);
            M_COLOR trackPointColor = new M_COLOR(0, 255, 255);
            for (int i = 0; i < OtherShipCount; i++)
            {
                int iraNum = ra.Next(10, 200);
                int currentGeoPoX = 1260000000 + ra.Next(10, 280) * 100000 - 40000000;
                int currentGeoPoY = 290000000 + ra.Next(10, 300) * 100000;
                float heading = 360 * iraNum / 200;
                float courseOverGround = heading;
                float courseThrghWater = iraNum;
                float speedOverGround = 0;
                float speedThrghWater = iraNum * 1 / 3;
                int iShipID = yimaEncCtrl.AddOtherVessel(false, currentGeoPoX, currentGeoPoY, heading, courseOverGround, courseThrghWater, speedOverGround, speedThrghWater);
                int iShipPos = yimaEncCtrl.GetOtherVesselPosOfID(iShipID); //获取船舶Pos值
                int iShipLength = 100 * iraNum / 200 + 30;// 30~230长 
                string strShipName = "渔船" + (i + 1).ToString() + "号";
                string strAttr = "";
                yimaEncCtrl.SetOtherVesselBasicInfo(iShipPos, iShipLength, iShipLength * 0.5f, strShipName, iraNum * 100, ref strAttr, 0);
                yimaEncCtrl.SetOtherVesselShowText(iShipPos, true, strShipName, true, 14, true, trackLineColor.ToInt()); int iTypeId = iraNum < 100 ? typeIdTwo : typeIdTwo;
                yimaEncCtrl.SetAisTargetType(iShipPos, iTypeId);   //设置船舶的显示样式 
                if (i < OtherShipCount / 2)
                {
                    yimaEncCtrl.SetShipTrackStyle(false, iShipPos, true, trackLineColor.ToInt(), 2, false, 4, trackPointColor.ToInt());
                }
                else
                {
                    yimaEncCtrl.SetShipTrackStyle(false, iShipPos, true, trackLineColorBat.ToInt(), 2, true, 4, trackPointColor.ToInt());
                }
            }
        }

        /// <summary>
        /// 定时刷新本船和目标船位置
        /// </summary>
        public void RefreashShipPosition()
        {
            float heading = 0;
            float courseOverGround = 0;
            float courseThrghWater = 0;
            float speedOverGround = 0;
            float speedThrghWater = 0;
            int curOwnShipGeoPoX = 0;
            int curOwnShipGeoPoY = 0;

            Random ra = new Random();
            int iRaMaxNum = 32767;
            //本船信息更新
            yimaEncCtrl.GetOwnShipCurrentInfo(ref curOwnShipGeoPoX, ref curOwnShipGeoPoY, ref heading, ref courseOverGround, ref courseThrghWater, ref speedOverGround, ref speedThrghWater);
            double curOwnHeading = heading;
            float iOwnAddGeoLen = (float)(ra.Next(0, iRaMaxNum)) / iRaMaxNum * 100000;

            float iOwnRandNum = (float)(ra.Next(0, iRaMaxNum)) / iRaMaxNum * 10;//计算出航向

            if (iOwnRandNum % 4 == 0)
            {
                curOwnHeading += 1;
            }
            else
            {
                curOwnHeading -= 1;
            }

            float angleOwn = (float)((90 - heading) * Math.PI / 180);
            curOwnShipGeoPoX = (int)(curOwnShipGeoPoX + Math.Cos(angleOwn) * iOwnAddGeoLen);
            curOwnShipGeoPoY = (int)(curOwnShipGeoPoY + Math.Sin(angleOwn) * iOwnAddGeoLen);
            yimaEncCtrl.SetOwnShipCurrentInfo(curOwnShipGeoPoX, curOwnShipGeoPoY, heading, courseOverGround, courseThrghWater, speedOverGround, speedThrghWater);


            //目标船坐标更新
            m_closedShipGeoPo.Clear();//先清空记录
            int curGeoPoX = 0;
            int curGeoPoY = 0;
            bool bArpaOrAis = false;
            int otherVesselCount = yimaEncCtrl.GetOtherVesselCount();
            string pExtAttrs = "";
            int pTime = 0;
            for (int ishipPos = 0; ishipPos < otherVesselCount; ishipPos++)//模拟其它的船舶
            {
                yimaEncCtrl.GetOtherVesselCurrentInfo(ishipPos, ref bArpaOrAis, ref curGeoPoX, ref curGeoPoY, ref heading, ref courseOverGround, ref courseThrghWater, ref speedOverGround, ref speedThrghWater, ref pTime, ref pExtAttrs);
                //计算模拟数据 
                float curHeading = heading;
                float iAddGeoLen = ((float)(ra.Next(0, iRaMaxNum)) / iRaMaxNum * 100000) / 2;
                int iRandNum = (int)(ra.Next(0, iRaMaxNum)) / iRaMaxNum * 10;
                if (iRandNum % 2 == 0)
                {
                    curHeading += 2;
                }
                else
                {
                    curHeading -= 1;
                }

                float angle = (float)((90 - curHeading) * Math.PI / 180);
                curGeoPoX = (int)(curGeoPoX + Math.Cos(angle) * iAddGeoLen);
                curGeoPoY = (int)(curGeoPoY + Math.Sin(angle) * iAddGeoLen);

                if (curGeoPoX < 1100000000 || curGeoPoX > 1500000000 || curGeoPoY < 100000000 || curGeoPoY > 500000000)
                {
                    curHeading = (float)(ra.Next(0, iRaMaxNum)) / iRaMaxNum * 360;
                }
                pTime = InteropEncDotNet.DataTime2Int(DateTime.Now);
                yimaEncCtrl.SetOtherVesselCurrentInfo(ishipPos, curGeoPoX, curGeoPoY, curHeading, courseOverGround, courseThrghWater, speedOverGround, speedThrghWater, ref pTime, ref pExtAttrs, 0);

                if (yimaEncCtrl.ToolIsNoOperate())
                {
                    //检测该船舶是否靠近本船  
                    GetNearShip(curGeoPoX, curGeoPoY, curOwnShipGeoPoX, curOwnShipGeoPoY);
                }
            }
            if (yimaEncCtrl.ToolIsNoOperate())
            {
                //检测本船附近的警告物标
                CheckOwnShipWarnInfo(curOwnShipGeoPoX, curOwnShipGeoPoY, m_judgeDisByMeter);
            }
        }


        /// <summary>
        /// 获取需要标记的图层索引
        /// </summary>
        public void GetWarnLayerPoses()
        {
            m_warnLyPoses.Clear();
            int iLayerCount = yimaEncCtrl.GetLayerCount(-1);
            for (int iLayerPos = 0; iLayerPos < iLayerCount; iLayerPos++)
            {
                char[] cc = new char[100];
                string strLayerName = new string(cc);
                string strLayerNameToken = new string('1', 20);
                int itmp = 0;
                yimaEncCtrl.GetLayerInfo(-1, iLayerPos, ref strLayerName, ref strLayerNameToken, ref itmp);
                strLayerName = strLayerName.Replace("\0", "");
                for (int i = 0; i < m_strWarnLyNames.Count; i++)
                {
                    if (strLayerName == m_strWarnLyNames[i])
                    {
                        m_warnLyPoses.Add(iLayerPos);
                    }
                }
            }
        }

        /// <summary>
        /// 检测本船与其他船舶是否靠近
        /// </summary>  
        public void GetNearShip(int curGeoPoX, int curGeoPoY, int ownShipGeoPoX, int ownShipGeoPoY)
        {
            double Dis = yimaEncCtrl.GetDistBetwTwoPoint(curGeoPoX, curGeoPoY, ownShipGeoPoX, ownShipGeoPoY);
            Dis = Dis * 1852; //海里转化成米
            if (Dis <= m_judgeDisByMeter)
            {
                m_closedShipGeoPo.Add(new M_POINT(curGeoPoX, curGeoPoY));
            }
        }

        /// <summary>
        /// 检测本船附近的物标警告信息
        /// </summary>
        /// <param name="curGeoPoX">本船地理经度</param>
        /// <param name="curGeoPoY">本船地理纬度</param>
        /// <param name="nearJudeByMeter">判距，单位米</param>
        public void CheckOwnShipWarnInfo(int curGeoPoX, int curGeoPoY, double nearJudeByMeter)
        {
            m_closedObjPos.Clear();
            int imemMapCount = yimaEncCtrl.GetMemMapCount();
            for (int imemMapPos = 0; imemMapPos < imemMapCount; imemMapPos++)   //检测的图幅
            {
                //string strLayerName = new string('\0', 255);
                //string strLayerToken= new string('\0', 255);
                //int tmp = 0;
                //yimaEncCtrl.GetLayerInfo(imemMapPos, 10, ref strLayerName, ref strLayerToken, ref tmp);
                //if (strLayerName.TrimEnd('\0') != "航道测试")//航道测试：根据实际图层名称修改
                //{
                //    continue;   //保证该图幅是具备航道数据的ymp图幅
                //}
                for (int i = 0; i < m_warnLyPoses.Count; i++)
                {
                    int warnLayerPos = m_warnLyPoses[i]; //检测的图层
                    int iObjCount = yimaEncCtrl.GetLayerObjectCountOfMap(imemMapPos, warnLayerPos);
                    for (int iobjPos = 0; iobjPos < iObjCount; iobjPos++)
                    {
                        bool bretNear = false;
                        yimaEncCtrl.IsGeoObjectNearPoint(imemMapPos, warnLayerPos, iobjPos, curGeoPoX, curGeoPoY, (int)nearJudeByMeter, ref bretNear);
                        if (bretNear)
                        {
                            //检测到curGeoPoX, curGeoPoY附近有需要检测图层的物标
                            MEM_GEO_OBJ_POS tmpObj = new MEM_GEO_OBJ_POS(imemMapPos, warnLayerPos, iobjPos);
                            m_closedObjPos.Add(tmpObj);
                        }
                    }
                }
            }

        }

        #endregion 本船报警演示

        #region 菜单栏按钮
        /// <summary>
        /// 打印预览
        /// </summary>
        private void PrintPreView_Click(object sender, EventArgs e)
        {

            int wide = yimaEncCtrl.GetDrawerScreenWidth();
            int height = yimaEncCtrl.GetDrawerScreenHeight();
            //string strMapImg = new string('1', wide*height*3);
            //yimaEncCtrl.GetDrawDCBits(ref strMapImg);
            bool bSaved = yimaEncCtrl.SaveScrnToBmpFile(0, 0, wide, height, "tmp.bmp");
            //int hwnd = yimaEncCtrl.GetDrawerHWnd();

            printDocument1.DefaultPageSettings.Landscape = true;
            printPreviewDialog1.Document = this.printDocument1;

            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int wide = yimaEncCtrl.GetDrawerScreenWidth();
            int height = yimaEncCtrl.GetDrawerScreenHeight();
            Graphics myGraphics = this.CreateGraphics();
            Bitmap memoryImage = new Bitmap(wide, height, myGraphics);//打印预览图片数据
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            Image mapImg = Image.FromFile("tmp.bmp");
            memoryGraphics.DrawImage(mapImg, 0, 0);
            e.Graphics.DrawImage(memoryImage, 0, 0);

            mapImg.Dispose();
            myGraphics.Dispose();
        }


        /// <summary>
        /// 轨迹回放
        /// </summary> 
        private void BtnTrackPlayback_Click(object sender, EventArgs e)
        {
            m_shipBackingTrack.SetBackingShipBasicInfo("轨迹回放测试", 123456789);
            m_shipBackingTrack.SetBackingTrackStyle(new M_COLOR(255, 0, 0), 2);
            m_shipBackingTrack.GetTrackPo(); //模拟船舶数据获取
            curPlayStep = 0;
        }
        private void PlayBacking()
        {
            if (curPlayStep == -1)
                return;
            if (curPlayStep + 1 > m_shipBackingTrack.trackCount - 1)
            {
                return;
            }

            curPlayStep += 1;
        }

        /// <summary>
        /// 添加风车组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWinVol_Click(object sender, EventArgs e)
        {
            m_bAddVolWindByMouse = !m_bAddVolWindByMouse;
            if (m_bAddVolWindByMouse)
            {
                yimaEncCtrl.ToolUserDefinedOperator();  //使海图画面不移动
                AddWinVol.Text = "取消添加风车组";
            }
            else
            {
                yimaEncCtrl.ToolEndUserDefinedOperator();
                AddWinVol.Text = "添加风车组";
            }
        }

        #endregion 菜单栏按钮        /// 组件绘制事件，在海图绘制后自动调用

        #region 风车

        /// <summary>
        /// 获取风车和变压器的坐标位置，模拟/数据库
        /// </summary>
        private void GetWindAndVoltagePoint()
        {
            m_allWindmill.Add(new StructWind(1220000000, 312000000, new M_COLOR(255, 0, 0).ToInt()));
            int curWindPos = m_allWindmill.Count - 1;
            m_VoltageLink.Add(new StructVoltage(1218000000, 310000000, curWindPos));

            m_allWindmill.Add(new StructWind(1219000000, 311500000, new M_COLOR(0, 0, 255).ToInt()));
            curWindPos = m_allWindmill.Count - 1;
            m_VoltageLink.Add(new StructVoltage(1217000000, 310000000, curWindPos));
        }


        /// <summary>
        /// 添加风车和升压器图层
        /// </summary>
        private void AddUserLayer()
        {
            int count = yimaEncCtrl.tmGetLayerCount();
            for (int layerPos = 0; layerPos < count; layerPos++)
            {
                string strName = new string('\0', 50);
                yimaEncCtrl.tmGetLayerName(layerPos, ref strName);
                if (strName.Contains("升压器"))
                {
                    m_voltageLayerPos = layerPos;
                }
                if (strName.Contains("风车"))
                {
                    m_windmillLayerPos = layerPos;
                }
            }
            if (m_voltageLayerPos == -1)
            {
                yimaEncCtrl.tmAppendLayer((int)LAYER_GEO_TYPE.ALL_POINT);//升压器图层
                m_voltageLayerPos = yimaEncCtrl.tmGetLayerCount() - 1;
                yimaEncCtrl.tmSetLayerName(m_voltageLayerPos, "升压器");
                yimaEncCtrl.tmAddLayerAttribute(m_voltageLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "绕组");
                yimaEncCtrl.tmAddLayerAttribute(m_voltageLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "相数");
                yimaEncCtrl.tmAddLayerAttribute(m_voltageLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "冷却方式");
            }
            if (m_windmillLayerPos == -1)
            {
                yimaEncCtrl.tmAppendLayer((int)LAYER_GEO_TYPE.ALL_POINT);//风车图层 
                m_windmillLayerPos = yimaEncCtrl.tmGetLayerCount() - 1;
                yimaEncCtrl.tmSetLayerName(m_windmillLayerPos, "风车");
                yimaEncCtrl.tmAddLayerAttribute(m_windmillLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "功率");
                yimaEncCtrl.tmAddLayerAttribute(m_windmillLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "起动风速");
                yimaEncCtrl.tmAddLayerAttribute(m_windmillLayerPos, (int)M_DATA_TYPE_ID.M_STRING, "轴类型");
                //toolTip1.SetToolTip(this, "提示语\n换行");
            }

        }


        /// <summary>
        /// 添加风车和变压器到海图
        /// </summary>
        private void AddAllWindAndVoltageToMap()
        {
            int ivoltageCount = m_VoltageLink.Count;
            for (int i = 0; i < ivoltageCount; i++)
            {
                AddOneCoupleVolWind(i);
            }
        }

        private void AddOneCoupleVolWind(int curVoltagePos)
        {
            StructVoltage curVoltage = m_VoltageLink[curVoltagePos];  //升压器
            yimaEncCtrl.tmAppendObjectInLayer(m_voltageLayerPos, (int)M_GEO_TYPE.TYPE_POINT);
            int objVolPos = yimaEncCtrl.tmGetLayerObjectCount(m_voltageLayerPos) - 1;
            yimaEncCtrl.tmSetPointObjectCoor(m_voltageLayerPos, objVolPos, curVoltage.Position.x, curVoltage.Position.y);
            int curVolSymId = 30 + (curVoltagePos % 4);
            yimaEncCtrl.tmSetPointObjectStyleRefLib(m_voltageLayerPos, objVolPos, curVolSymId, false, 0, 1, 0);

            string strAttr = (curVoltagePos % 2 == 0) ? "双绕组" : "三绕组";
            yimaEncCtrl.tmSetObjectAttrValueString(m_voltageLayerPos, objVolPos, 0, strAttr);
            strAttr = (curVoltagePos % 2 == 0) ? "单相" : "三相";
            yimaEncCtrl.tmSetObjectAttrValueString(m_voltageLayerPos, objVolPos, 1, strAttr);
            strAttr = (curVoltagePos % 2 == 0) ? "干式" : "油浸式";
            yimaEncCtrl.tmSetObjectAttrValueString(m_voltageLayerPos, objVolPos, 2, strAttr);



            StructWind curWind = m_allWindmill[curVoltage.WindPos];  //风车
            yimaEncCtrl.tmAppendObjectInLayer(m_windmillLayerPos, (int)M_GEO_TYPE.TYPE_POINT);
            int objWindPos = yimaEncCtrl.tmGetLayerObjectCount(m_windmillLayerPos) - 1;
            yimaEncCtrl.tmSetPointObjectCoor(m_windmillLayerPos, objWindPos, curWind.Position.x, curWind.Position.y);
            int curwindSymId = 25 + (curVoltagePos % 5);
            yimaEncCtrl.tmSetPointObjectStyleRefLib(m_windmillLayerPos, objWindPos, curwindSymId, false, 0, 1, 0);

            strAttr = (curVoltagePos % 2 == 0) ? "12500kW" : "800kW";
            yimaEncCtrl.tmSetObjectAttrValueString(m_windmillLayerPos, objWindPos, 0, strAttr);
            strAttr = (curVoltagePos % 2 == 0) ? "3.0m/s" : "10.0m/s";
            yimaEncCtrl.tmSetObjectAttrValueString(m_windmillLayerPos, objWindPos, 1, strAttr);
            strAttr = (curVoltagePos % 2 == 0) ? "水平轴" : "垂直轴";
            yimaEncCtrl.tmSetObjectAttrValueString(m_windmillLayerPos, objWindPos, 2, strAttr);
        }



        #endregion 风车

        /// <summary>
        /// 获取自定义物标信息
        /// </summary>
        /// <param name="curScrnX"></param>
        /// <param name="curScrnY"></param>
        private void GetUserObjTips(int curScrnX, int curScrnY)
        {
            int ilayerCount = yimaEncCtrl.tmGetLayerCount();
            int objScrnX = 0, objScrnY = 0, objGeoX = 0, objGeoY = 0;
            for (int layerPos = 0; layerPos < ilayerCount; layerPos++)
            {
                int iobjCount = yimaEncCtrl.tmGetLayerObjectCount(layerPos);
                for (int objPos = 0; objPos < iobjCount; objPos++)
                {
                    if (yimaEncCtrl.tmGetGeoObjectType(layerPos, objPos) == 0)
                    {
                        yimaEncCtrl.tmGetPointObjectCoor(layerPos, objPos, ref objGeoX, ref objGeoY);
                        yimaEncCtrl.GetScrnPoFromGeoPo(objGeoX, objGeoY, ref objScrnX, ref objScrnY);
                        int iscrnDis = Math.Abs(objScrnX - curScrnX) + Math.Abs(objScrnY - curScrnY);
                        if (iscrnDis < 50)
                        {

                            string strTmp = new string('\0', 50);
                            yimaEncCtrl.tmGetLayerName(layerPos, ref strTmp);
                            string strTip = strTmp.Replace("\0", "") + "\n";
                            int iAttrCount = yimaEncCtrl.tmGetLayerObjectAttrCount(layerPos);
                            for (int attrPos = 0; attrPos < iAttrCount; attrPos++)
                            {
                                strTmp = new string('\0', 50);
                                yimaEncCtrl.tmGetLayerObjectAttrName(layerPos, attrPos, ref strTmp);
                                strTip += strTmp.Replace("\0", "") + ": ";
                                strTmp = new string('\0', 50);
                                yimaEncCtrl.tmGetObjectAttrValueString(layerPos, objPos, attrPos, ref strTmp);
                                strTip += strTmp.Replace("\0", "") + "\n";

                            }
                            toolTip1.SetToolTip(this.yimaEncCtrl, strTip);
                            break;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 获取回放轨迹点信息
        /// </summary>
        /// <param name="curScrnX"></param>
        /// <param name="curScrnY"></param>
        private void GetTrackPointTips(int curScrnX, int curScrnY)
        {
            if (curPlayStep != -1)
            {
                int iPlayingCount = m_TrackScrnPoInPlaying.Count;
                for (int i = 0; i < iPlayingCount; i++)
                {
                    if ((Math.Abs(m_TrackScrnPoInPlaying[i].ScrnPoX - curScrnX) < 10)
                      && (Math.Abs(m_TrackScrnPoInPlaying[i].ScrnPoY - curScrnY) < 10))
                    {
                        int selPointPos = m_TrackScrnPoInPlaying[i].TrackPointPos;
                        string strTip = new string('\0', 1024);
                        strTip = "船名：" + m_shipBackingTrack.shipName + "\n";
                        strTip += "MMSI：" + m_shipBackingTrack.mmsi + "\n";
                        strTip += "经度：" + m_shipBackingTrack.shipTrackPo[selPointPos].x + "\n";
                        strTip += "纬度：" + m_shipBackingTrack.shipTrackPo[selPointPos].y + "\n";
                        strTip += "航速：" + m_shipBackingTrack.shipTrackPoInfo[selPointPos].speed + "\n";
                        strTip += "航向：" + m_shipBackingTrack.shipTrackPoInfo[selPointPos].course + "\n";
                        strTip += "记录时间：" + m_shipBackingTrack.shipTrackPoInfo[selPointPos].noteTime;
                        toolTip1.Active = true;
                        toolTip1.SetToolTip(this.yimaEncCtrl, strTip);
                        lastShowHintPo.x = curScrnX;
                        lastShowHintPo.y = curScrnY;
                        break;
                    }
                }
            }
        }

        //切换轨迹样式
        private void TrackStyleChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            if (selItem.Checked == false)
            {
                BtnTrackStyle1.Checked = !BtnTrackStyle1.Checked;
                BtnTrackStyle2.Checked = !BtnTrackStyle2.Checked;
                RedrawMapScreen();
            }
        }

        //陆图叠加切换
        private void OnOverlayLandMap(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            switch (selItem.Text)
            {
                case "显示海图":
                    yimaEncCtrl.SetIfOverlaySeaMap(true);
                    不显示海图ToolStripMenuItem.Checked = false;
                    break;
                case "不显示海图":
                    yimaEncCtrl.SetIfOverlaySeaMap(false);
                    显示海图ToolStripMenuItem.Checked = false;
                    break;
                case "不显示陆图":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(false, 0);
                    break;
                case "谷歌街道":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.GoogleSatMap);
                    break;
                case "谷歌卫星":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.GoogleSatMap);
                    break;
                case "百度街道":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.BaiduStrMap);
                    break;
                case "百度卫星":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.BaiduStaMap);
                    break;
                case "天地图街道":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.TianDiStrMap);
                    break;
                case "天地图卫星":
                    ChangedCheckState();
                    yimaEncCtrl.SetIfOverlayLandMap(true, (int)LoadMapName.TianDiSatMap);
                    break;

                default: break;
            }
            selItem.Checked = true;
            RedrawMapScreen(true);
        }
        private void ChangedCheckState()
        {
            不显示陆图ToolStripMenuItem.Checked = false;
            谷歌街道ToolStripMenuItem.Checked = false;
            谷歌卫星ToolStripMenuItem.Checked = false;
            百度街道ToolStripMenuItem.Checked = false;
            百度卫星ToolStripMenuItem.Checked = false;
            天地图街道ToolStripMenuItem.Checked = false;
            天地图卫星ToolStripMenuItem.Checked = false;
        }

        /// <summary>
        /// 设置海图操作方式
        /// </summary>
        private void OnSetDragOrScaledMapMode(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            bool bAimCheck = !selItem.Checked;
            switch (selItem.Text)
            {
                case "定点缩放":
                    bPointScaled = bAimCheck;
                    yimaEncCtrl.ToolSetMapScaledMode(bAnimationScaled, !bPointScaled);
                    break;
                case "动画缩放":
                    bAnimationScaled = bAimCheck;
                    yimaEncCtrl.ToolSetMapScaledMode(bAnimationScaled, !bPointScaled);
                    break;
                case "无缝漫游":
                    bRealTimeDrag = bAimCheck;
                    yimaEncCtrl.ToolSetMapDragMode(bRealTimeDrag);
                    break;
                default: break;
            }
            selItem.Checked = bAimCheck;
        }

        /// <summary>
        /// 海图工具
        /// </summary>
        private void OnToolSelClick(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            switch (selItem.Text)
            {
                case "距离测算":
                    yimaEncCtrl.ToolMapMeasure((int)(M_TOOl_TYPE.TYPE_MEASURE_DIS));
                    break;
                case "面积测算":
                    yimaEncCtrl.ToolMapMeasure((int)(M_TOOl_TYPE.TYPE_MEASURE_AREA));
                    break;
                case "截图打印":
                    PrintPreView_Click(null, null);
                    break;
                case "电子方位线":
                    yimaEncCtrl.ToolMapMeasure((int)(M_TOOl_TYPE.TYPE_EBL));
                    break;
                case "区域放大":
                    yimaEncCtrl.ToolMapMeasure((int)(M_TOOl_TYPE.TYPE_AREA_ZOOM_IN));
                    break;
                case "自定义操作：禁止拖动海图":
                    yimaEncCtrl.ToolUserDefinedOperator();
                    break;
                case "结束自定义操作":
                    yimaEncCtrl.ToolEndUserDefinedOperator();
                    break;
                case "空心":
                    yimaEncCtrl.ToolMapEffect(true);
                    break;
                case "延迟":
                    yimaEncCtrl.ToolMapEffect(false);
                    break;
                case "取消效果":
                    yimaEncCtrl.CancelMapEffect();
                    RedrawMapScreen(true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddPointClick(object sender, EventArgs e)
        {
            m_testLayerPos = 1;//要预先设置颜色必须使用点图层
            int symbolId = 4;
            yimaEncCtrl.tmSetPointLayerStyleReferLib(m_testLayerPos, "", symbolId, false, 0, 1, 0, false, false, "", "", 14, 0, false, false, 0, 0, 0, 0);

            if (yimaEncCtrl.ToolIsNoOperate() == false)
            {
                MessageBox.Show("请先退出其他操作");
                return;
            }
            bool bCanAdd = yimaEncCtrl.ToolAddPointObj(m_testLayerPos);
            if (bCanAdd == false)
            {
                MessageBox.Show("请确认图层类型是否匹配！");
            }
        }

        private void OnAddLineObjClick(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            bool bCanAdd = false;

            m_testLayerPos = 2; //要预先设置颜色必须使用线图层
            int lineColor = new M_COLOR(125, 125, 0).ToInt();
            int lineWidth = 3;
            yimaEncCtrl.tmSetLineLayerStyleReferLib("", m_testLayerPos, 1, 0, 0, 1, true, lineColor, lineWidth, false, false, "", "", 0, 0, false, false, false, 0, 0);

            switch (selItem.Text)
            {
                case "常规":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.NO_SPECIAL_TYPE));
                    break;
                case "矩形":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.RECTANGLE_LN_TYPE));
                    break;
                case "圆":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CIRCLE_LN_TYPE));
                    break;
                case "椭圆":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.ELLIPSE_LN_TYPE));
                    break;
                case "弧线":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.ARC_LN_TYPE));
                    break;
                case "扇形":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.PIE_LN_TYPE));
                    break;
                case "弓形":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.BOW_LN_TYPE));
                    break;
                case "圆滑曲线":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CURVE_LN_TYPE));
                    break;
                case "单箭头曲线":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CURVE_LN_TYPE_WITH_HEAD_ARROW));
                    break;
                case "双箭头曲线":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CURVE_LN_TYPE_WITH_HAED_TAIL_ARROW));
                    break;
                case "单箭头":
                    bCanAdd = yimaEncCtrl.ToolAddLineObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.SINGLE_ARROW_LN_TYPE));
                    break;
                default:
                    break;
            }
            if (bCanAdd == false)
            {
                MessageBox.Show("请先退出其他操作或确认图层类型是否匹配！");
            }
        }

        private void OnAddFaceObjClick(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = (ToolStripMenuItem)sender;
            bool bCanAdd = false;

            m_testLayerPos = 3;//要预先设置填充色，必须使用面图层 
            int lineWidth = 3;
            int lineColor = new M_COLOR(125, 125, 0).ToInt();
            int faceColor = new M_COLOR(125, 125, 125).ToInt();
            int transparent = 60;
            yimaEncCtrl.tmSetFaceLayerStyleReferLib("", m_testLayerPos, true, 1, true, faceColor, transparent, false, 0, true, 1, 0, 0, 1, true, lineColor,
                lineWidth, false, false, "", "", 14, 0, false, false, true, 0, 0);

            switch (selItem.Text)
            {
                case "常规":
                    bCanAdd = yimaEncCtrl.ToolAddFaceObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.NO_SPECIAL_TYPE));
                    break;
                case "矩形":
                    bCanAdd = yimaEncCtrl.ToolAddFaceObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.RECTANGLE_LN_TYPE));
                    break;
                case "圆":
                    bCanAdd = yimaEncCtrl.ToolAddFaceObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CIRCLE_LN_TYPE));
                    break;
                case "椭圆":
                    bCanAdd = yimaEncCtrl.ToolAddFaceObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.ELLIPSE_LN_TYPE));
                    break;
                case "圆滑曲面":
                    bCanAdd = yimaEncCtrl.ToolAddFaceObj(m_testLayerPos, (int)(SPECIAL_LINE_TYPE.CURVE_LN_TYPE));
                    break;
                default:
                    break;
            }
            if (bCanAdd == false)
            {
                MessageBox.Show("请先退出其他操作或确认图层类型是否匹配！");
            }
        }

        //使用说明
        private void OnReadMeClick(object sender, EventArgs e)
        {
            long encProjector = yimaEncCtrl.GetEncMapProject();
            string strMsg = "当前投影：";
            switch (encProjector)
            {
                case 3: strMsg += "经纬投影"; break;
                case 2: strMsg += "谷歌墨卡托投影"; break;
                case 1: strMsg += "标准墨卡托"; break;
                default: break;
            }
            strMsg += "\n\r当前版本：";
            string strVersion = new string('\0', 50);
            string strCreatTime = new string('\0', 50);
            yimaEncCtrl.GetYimaSdkVersion(ref strVersion, ref strCreatTime);
            strMsg += strVersion.TrimEnd('\0') + "    " + strCreatTime.TrimEnd('\0') + "\n\r\n\r";
            MessageBox.Show(strMsg + "1、左键船舶或风车组可查看详细信息标签！\n\r2、右键自定义标绘可编辑！\n\r3、陆图叠加需要配置文件LandImage！\n\r");
        }

        private void 选择编辑图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (m_formLayerMan == null || m_formLayerMan.IsDisposed)
            //{
            //    m_formLayerMan = new FormLayerMan(yimaEncCtrl, this);

            //}
            //m_formLayerMan.Show();
        }

        private void 保存标绘文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            yimaEncCtrl.tmSaveToMapDataFile(strUserMapFilePath);
        }

        private void 当前船舶数量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            long lCurShipCount = yimaEncCtrl.GetOtherVesselCount();
            MessageBox.Show("当前船舶数量:" + lCurShipCount);
        }

        private void 添加3000船舶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            yimaEncCtrl.SetDrawShipOptions(false, false, false, false, 0, false, 0, 0, 0, 0);

            yimaEncCtrl.SetOtherVesselShowTextStartScale(50000);

            int iAisType1 = yimaEncCtrl.AddAisType("a");
            yimaEncCtrl.SetAisTypeInfo(iAisType1, "a", false, 1, 1, false, 0, 2, 500000, true, false, 1, true);

            int iAisType2 = yimaEncCtrl.AddAisType("b");
            yimaEncCtrl.SetAisTypeInfo(iAisType2, "b", false, 4, 1, false, 0, 6, 500000, true, false, 1, true);

            int iAisType3 = yimaEncCtrl.AddAisType("c");
            yimaEncCtrl.SetAisTypeInfo(iAisType3, "c", false, 3, 1, false, 0, 7, 500000, true, false, 1, true);

            Random rd = new Random();　//随机数
            for (int i = 0; i < 300; i++)
            {
                int iShipGeoX = (int)(((double)rd.Next(1, 32767) / 32767) * (1350000000 - 1210000000) + 1210000000);
                int iShipGeoY = (int)(((double)rd.Next(1, 32767) / 32767) * (380000000 - 250000000) + 250000000);
                int course = rd.Next(0, 360);
                int speed = rd.Next(0, 50);
                int shipId = yimaEncCtrl.AddOtherVessel(false, iShipGeoX, iShipGeoY, course, course, course, speed, speed);
                int iShipPos = yimaEncCtrl.GetOtherVesselCount() - 1;
                string strShipName = "渔船" + iShipPos + "号";

                if (i % 4 > 2)
                {
                    yimaEncCtrl.SetAisTargetType(iShipPos, iAisType1);
                }
                else
                {
                    yimaEncCtrl.SetAisTargetType(iShipPos, iAisType3);
                }

                if (i % 100 == 0)
                {
                    yimaEncCtrl.SetAisTargetType(iShipPos, iAisType2);
                    strShipName = "商船" + iShipPos + "号";
                }
                int mmsi = 120000000 + shipId;
                string tmp = "";
                yimaEncCtrl.SetOtherVesselBasicInfo(iShipPos, 100, 50, strShipName, mmsi, ref tmp, 0);
                yimaEncCtrl.SetOtherVesselShowText(iShipPos, true, strShipName, false, 0, false, 0);

            }

            RedrawMapScreen();
        }

        private void 删除船舶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            yimaEncCtrl.ClearOtherVessels();
            RedrawMapScreen();
        }

        private void 快速居中ToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            yimaEncCtrl.CenterMap(1240000000, 310000000);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            rightMenuDemo.Hide();
            Clipboard.Clear();//清空剪切板内容
            Clipboard.SetData(DataFormats.Text, getX + "," + getY);//复制内容到剪切板
            MessageBox.Show("位置已复制到粘贴板，当前位置：" + getX+","+getY);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            toolStripMenuItem8.Checked = !toolStripMenuItem8.Checked;
        }

        private void 轨迹导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewToExcel();
        }

        private void DataGridViewToExcel()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Execl files (*.xls)|*.xls";
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.FilterIndex = 0;
            dlg.RestoreDirectory = true;
            dlg.CreatePrompt = true;
            dlg.Title = "保存为Excel文件";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Stream myStream;    
                myStream = dlg.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
                string columnTitle = "";

                try

                {
                    //写入列标题   
                    for (int i = 0; i < 1; i++)
                    {
                        if (i > 0)
                        {
                            columnTitle += "\t";
                        }

                        columnTitle +="标题";
                    }

                    sw.WriteLine(columnTitle);

                    //写入列内容   
                    for (int j = 0; j < 5; j++)

                    {
                        string columnValue = "";

                        for (int k = 0; k < 1; k++)

                        {
                            if (k > 0)

                            {
                                columnValue += "\t";
                            }
                                columnValue += "Value";
                        }

                        sw.WriteLine(columnValue);
                    }

                    sw.Close();
                    myStream.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    sw.Close();
                    myStream.Close();
                }
            }
        }

        private void 船舶聚合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bCluster = yimaEncCtrl.GetIfShipsShowAsCluster();
            yimaEncCtrl.SetIfShipsShowAsCluster(!bCluster);
            船舶聚合ToolStripMenuItem.Checked = !bCluster;
            RedrawMapScreen();

        }
    }
}
