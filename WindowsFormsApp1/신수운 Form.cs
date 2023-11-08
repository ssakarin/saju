using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics.Eventing.Reader;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        public int[,] sjGanzi = new int[4, 2]; // 년월일시 간지
        public Goong[] goong = new Goong[9]; // 9궁
        public int eunboksu1, eunboksu2; // 은복수 
        public bool ly = false; // 윤년 
        public DateTime[] terms = new DateTime[24]; //  24절기
        public bool direction = false; // 양둔(true), 음둔(false)
        public int sisunsoo; //시순수
        public string name; //이름
        public int gender;  //성별
        public DateTime dt; //입력받는 날짜
        public int lunar_year, lunar_month, lunar_day;   // 음력
        public DateTime solar_dt; // 양력
        public DateTime real_dt;
        private PrintDocument printDocument1 = new PrintDocument();
        Bitmap memoryImage;
        bool bProcess = false;
        bool bTongi = false;
        bool bYoonyun = false;
        bool bSinsoo = false;
        DateTime dtTemp; // 신수운 - 행년도 저장용
        public int Hyear; // 행년도
        SolidBrush myBrush;

        public class Goong
        {
            public int[] hongNum = new int[2];      //천반수 지반수
            public bool[] b_dong = new bool[4];     //사지
            public bool[] b_gan = new bool[4];      //사간
            public int[] yoo_age = new int[2];      //유년
            public int[] six_sin = new int[2];      //육신
            public int[] hongNumlvl = new int[2];   //홍국수 강약
            public int[] yooksam = new int[2];      //육의삼기
            public int eightmun;                    //팔문
            public int eightgoe;                    //팔괴
            public int goosung;                     //구성
            public string eightjang;                //팔장
            public int cheoneul;                    //천을귀인
            public int cheonma;                     //천마
            public int ilrok;                       //일록
            public string eunsung;                  //12운성
            public string gongmang;                 //공망
            public string sinsal;                   //신살
            public string kyukkuk;                  //격국
            public int taeulgusung;              //태을구성
            public string josang;                    //조객상문
        }

        Form1 f1;

        public DateTime dateAdjust(DateTime dt)
        {
            string[,] summertime = {{"194806010000","194809130000"},{"194904030000","194909110000"},{"195004010000","195009100000"},{"195105060000","195109090000"},
                {"195505050000","195509090000"},{"195605200000","195609300000"},{"195705050000","195709220000"},{"195805040000","195809210000"},
                {"195905030000","195909200000"},{"196005010000","196009180000"},{"198705100200","198710110300"},{"198805080200","198810090300"}};
            string[,] seoultime = { { "190802010000", "191112312359" }, { "195403210000", "196108090000" } };
            DateTime[] t = new DateTime[2];

            for (int i = 0; i < 12; i++)
            {
                t[0] = DateTime.ParseExact(summertime[i, 0], "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                t[1] = DateTime.ParseExact(summertime[i, 1], "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                if (dt >= t[0] && dt < t[1]) dt = dt.AddHours(-1);
            }

            for (int i = 0; i < 2; i++)
            {
                t[0] = DateTime.ParseExact(seoultime[i, 0], "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                t[1] = DateTime.ParseExact(seoultime[i, 1], "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                if (dt >= t[0] && dt < t[1]) dt = dt.AddMinutes(30);
            }
            //textBox5.AppendText(dt.ToString() + Environment.NewLine);

            return dt;
        } // 서머타임 127.5서울시 조정

        public string toGan(int num)  // 숫자 12간 출력
        {
            if (num == 1) return "甲";
            else if (num == 2) return "乙";
            else if (num == 3) return "丙";
            else if (num == 4) return "丁";
            else if (num == 5) return "戊";
            else if (num == 6) return "己";
            else if (num == 7) return "庚";
            else if (num == 8) return "辛";
            else if (num == 9) return "壬";
            else if (num == 10) return "癸";
            else return "";
        }

        public string toZi(int num)  // 숫자 12지 출력
        {
            if (num == 1) return "子";
            else if (num == 2) return "丑";
            else if (num == 3) return "寅";
            else if (num == 4) return "卯";
            else if (num == 5) return "辰";
            else if (num == 6) return "巳";
            else if (num == 7) return "午";
            else if (num == 8) return "未";
            else if (num == 9) return "申";
            else if (num == 10) return "酉";
            else if (num == 11) return "戌";
            else if (num == 12) return "亥";
            else return "";
        }

        public string toNum(int num)  // 숫자 한자로 출력 
        {
            if (num == 1) return "一";
            else if (num == 2) return "二";
            else if (num == 3) return "三";
            else if (num == 4) return "四";
            else if (num == 5) return "五";
            else if (num == 6) return "六";
            else if (num == 7) return "七";
            else if (num == 8) return "八";
            else if (num == 9) return "九";
            else if (num == 10) return "十";
            else return "";
        }

        public string toOhaeng(int num)  // 오행
        {
            if (num == 0) return "金";
            else if (num == 1) return "水";
            else if (num == 2) return "木";
            else if (num == 3) return "火";
            else if (num == 4) return "土";
            else return "";
        }

        public string toOhaeng_1(int num)  // 오행
        {
            if (num == 0) return "水";
            else if (num == 1) return "木";
            else if (num == 2) return "火";
            else if (num == 3) return "土";
            else if (num == 4) return "金";
            else return "";
        }

        public string toSixSin(int num)  // 육신
        {
            if (num == 1) return "世";
            else if (num == 2) return "兄";
            else if (num == 3) return "孫";
            else if (num == 4) return "財";
            else if (num == 5) return "鬼";
            else if (num == 6) return "官";
            else if (num == 7) return "父";
            else return "";
        }

        public string toHongNumLvl(int num)  // 홍국수 강약
        {
            if (num == 0) return "XXX";
            else if (num == 1) return "XXO";
            else if (num == 2) return "XOX";
            else if (num == 3) return "XOO";
            else if (num == 4) return "OXX";
            else if (num == 5) return "OXO";
            else if (num == 6) return "OOX";
            else if (num == 7) return "OOO";
            else return "";
        }

        public string toYookSam(int num)  // 육의삼기
        {
            if (num == 0) return "戊";
            else if (num == 1) return "己";
            else if (num == 2) return "庚";
            else if (num == 3) return "辛";
            else if (num == 4) return "壬";
            else if (num == 5) return "癸";
            else if (num == 6) return "丁";
            else if (num == 7) return "丙";
            else if (num == 8) return "乙";
            else return " ";
        }

        public string toJeolGi(int num)
        {
            if (num == 0) return "소한";
            else if (num == 1) return "대한";
            else if (num == 2) return "입춘";
            else if (num == 3) return "우수";
            else if (num == 4) return "경칩";
            else if (num == 5) return "춘분";
            else if (num == 6) return "청명";
            else if (num == 7) return "곡우";
            else if (num == 8) return "입하";
            else if (num == 9) return "소만";
            else if (num == 10) return "망종";
            else if (num == 11) return "하지";
            else if (num == 12) return "소서";
            else if (num == 13) return "대서";
            else if (num == 14) return "입추";
            else if (num == 15) return "처서";
            else if (num == 16) return "백로";
            else if (num == 17) return "추분";
            else if (num == 18) return "한로";
            else if (num == 19) return "상강";
            else if (num == 20) return "입동";
            else if (num == 21) return "소설";
            else if (num == 22) return "대설";
            else if (num == 23) return "동지";
            else return "";
        } // 절기

        public string to8Mun(int num)
        {
            if (num == 0) return "生";
            else if (num == 1) return "傷";
            else if (num == 2) return "杜";
            else if (num == 3) return "景";
            else if (num == 4) return "死";
            else if (num == 5) return "驚";
            else if (num == 6) return "開";
            else if (num == 7) return "休";
            else return "";
        } // 팔문

        public string to8Goe(int num)  //팔괘
        {
            if (num == 0) return "氣";       //생기
            else if (num == 1) return "宜";  //천의
            else if (num == 2) return "體";  //절체
            else if (num == 3) return "魂";  //유혼
            else if (num == 4) return "害";  //화해
            else if (num == 5) return "德";  //복덕
            else if (num == 6) return "命";  //절명
            else if (num == 7) return "歸";  //귀혼
            else return "";
        }

        public string toMunWang(int num)  //팔괘
        {
            if (num == 0) return "坎";
            else if (num == 1) return "坤";
            else if (num == 2) return "震";
            else if (num == 3) return "巽";
            else if (num == 4) return "中";
            else if (num == 5) return "乾";
            else if (num == 6) return "兌";
            else if (num == 7) return "艮";
            else if (num == 8) return "離";
            else return "";
        }

        public string toGooSung(int num)    //구성
        {
            if (num == 0) return "輔";
            else if (num == 1) return "英";
            else if (num == 2) return "芮";
            else if (num == 3) return "柱";
            else if (num == 4) return "心";
            else if (num == 5) return "蓬";
            else if (num == 6) return "任";
            else if (num == 7) return "沖";
            else if (num == 8) return "禽";
            else return "";
        }

        public string toCheonMaRok(int cheoneul, int cheonma, int ilrok)
        {
            string text = " ";
            if (cheoneul == 1) text += "天乙 ";
            else if (cheoneul == 2) text += "伏天乙 ";
            if (cheonma == 1) text += "天馬 ";
            else if (cheonma == 2) text += "伏天馬 ";
            if (ilrok == 1) text += "日祿 ";
            else if (ilrok == 2) text += "伏日祿 ";
            return text;
        } // 천을 천마 일록

        public string toEunSung(int num)    //12은성
        {
            if (num == 0) return "胞";
            else if (num == 1) return "胎";
            else if (num == 2) return "養";
            else if (num == 3) return "生";
            else if (num == 4) return "浴";
            else if (num == 5) return "帶";
            else if (num == 6) return "祿";
            else if (num == 7) return "旺";
            else if (num == 8) return "衰";
            else if (num == 9) return "病";
            else if (num == 10) return "死";
            else if (num == 11) return "墓";
            else return "";
        }

        public string toFourgan(int num)
        {
            String[,] day = { { "子", "戊" }, { "戌", "己" }, { "申", "庚" }, { "午", "辛" }, { "辰", "壬" }, { "寅", "癸" } };
            string cmp = "";
            string temp = "";
            int i, j;

            for (j = 0; j < 4; j++)
            {
                if (toGan(sjGanzi[j, 0]) == "甲")
                {
                    for (i = 0; i < 6; i++)
                    {
                        if (toZi(sjGanzi[j, 1]) == day[i, 0]) cmp = day[i, 1];
                    }
                }
                else cmp = toGan(sjGanzi[j, 0]);
                if (j == 0 && toYookSam(goong[num].yooksam[1]) == cmp) temp += " 年干";
                if (j == 1 && toYookSam(goong[num].yooksam[1]) == cmp) temp += " 月干";
                if (j == 2 && toYookSam(goong[num].yooksam[1]) == cmp) temp += " 日干";
                if (j == 3 && toYookSam(goong[num].yooksam[1]) == cmp) temp += " 時干";
            }

            return temp;
        }   // 사간

        public string bokgankyuk(int num)  //복간격
        {
            string cmp = "";
            string temp = "";
            cmp = toGan(sjGanzi[2, 0]);
            if (toYookSam(goong[num].yooksam[1]) == cmp && toYookSam(goong[num].yooksam[0]) == "庚") temp += "伏干格";
            else if (toYookSam(goong[num].yooksam[0]) == cmp && toYookSam(goong[num].yooksam[1]) == "庚") temp += "伏干格";
            else if (toYookSam(goong[num].yooksam[0]) == "庚" && toYookSam(goong[num].yooksam[1]) == "庚") temp += "伏干格";
            return temp;
        }

        public string toBirthJeolgi(int i, int j, int k)
        {
            string str = "";
            String[] hterms =
                {"小寒", "大寒", "立春", "雨水", "驚蟄", "春分", "淸明", "穀雨","立夏", "小滿", "芒種", "夏至",
                "小署", "大暑", "立秋", "處暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"};// 24절기 이름
            if (i == -1)
            {
                DateTime[] terms1 = new DateTime[24];
                terms1 = get24Terms(real_dt.AddYears(-1));
                str += hterms[23] + " " + terms1[23].ToString("yyyy년 MM월 dd일 HH:mm") + Environment.NewLine + Environment.NewLine;
                i = 23;
            }
            else
            {
                str += hterms[i] + " " + terms[i].ToString("yyyy년 MM월 dd일 HH:mm") + Environment.NewLine + Environment.NewLine;
                //textBox5.AppendText(hterms[i] + $"{terms[i]}" + Environment.NewLine);
            }
            str += hterms[i];
            if (j/20 == 0) str += " 上元";
            else if (j/20 == 1) str += " 中元";
            else str += " 下元";
            str += " " + k + "局";
            if (i == 23 || i <= 10) str += " 陽遁";
            else str += " 陰遁";
            return str;
        }

        public string toSaji(bool[] b_dong, int i)
        {
            if (b_dong[0] == true) return "年";
            else if (b_dong[1] == true) return "月";
            else if (b_dong[2] == true) return "日";
            else if (b_dong[3] == true) return "時";
            else if (i == 4) return "中";
            else return "";

        }   //년월일시중 

        public void ToLunarDate(DateTime SolarDate, out bool ly, out int year, out int month, out int day)     // 양력 -> 음력
                                                                                                               //public DateTime ToLunarDate(DateTime SolarDate, out bool ly)     // 양력 -> 음력
        {
            KoreanLunisolarCalendar klc = new KoreanLunisolarCalendar();

            year = klc.GetYear(SolarDate);
            month = klc.GetMonth(SolarDate);
            day = klc.GetDayOfMonth(SolarDate);
            ly = klc.IsLeapMonth(year, month);

            if (klc.GetMonthsInYear(year) > 12)
            {
                int leapMonth = klc.GetLeapMonth(year);

                if (month >= leapMonth)
                {
                    month--;
                }
            }

            //return new DateTime(year, month, day);
        }

        public DateTime ToSolarDate(DateTime dt, bool isLeapMonth)  // 음력 -> 양력
        {
            KoreanLunisolarCalendar klc = new KoreanLunisolarCalendar();

            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;

            if (klc.GetMonthsInYear(year) > 12)
            {
                int leapMonth = klc.GetLeapMonth(year);

                if (month > leapMonth - 1)
                {
                    month++;
                }
                else if (month == leapMonth - 1 && isLeapMonth)
                {
                    month++;
                }
            }

            return klc.ToDateTime(year, month, day, dt.Hour, dt.Minute, 0, 0);
        }

        public void ToSajuYear(DateTime dt, DateTime term, out int sjGanYear, out int sjZiyear)  // 년주 천간 지지
        {
            int year;
            if (dt.CompareTo(term) >= 0) year = dt.Year;
            else year = dt.Year - 1;
            sjGanYear = (year + 6) % 10 + 1;
            sjZiyear = (year + 8) % 12 + 1;
        }

        public void ToSajuMonth(DateTime dt, DateTime[] terms, int year_gan, out int sjGanMonth, out int sjZiMonth)  // 월주 천간 지지
        {
            int _year_gan = year_gan;
            int month;// = dt.Month;
            int i;

            for (i = 0; i < 12 && dt.CompareTo(terms[2 * i]) >= 0; i++) ;
            if (i == 0) month = 11;
            else if (i == 1) month = 12;
            else month = i - 1;
            sjZiMonth = (month + 1) % 12 + 1;
            if (_year_gan % 5 == 1) month += 1;
            else if (_year_gan % 5 == 2) month += 13;
            else if (_year_gan % 5 == 3) month += 25;
            else if (_year_gan % 5 == 4) month += 37;
            else month += 49;

            sjGanMonth = month % 10 + 1;
        }

        public void ToSajuDay(DateTime dt, out int sjGanDay, out int sjZiDay)  // 일주 천간 지지
        {
            int year = dt.Year;

            if (dt.Hour == 23 && dt.Minute >= 30)
                dt = dt.AddHours(1);

            year -= 1900;
            int day = year * 5 + (int)((year - 1) / 4) + dt.DayOfYear - 1;

            sjGanDay = day % 10 + 1;
            sjZiDay = (day + 10) % 12 + 1;
        }

        public void ToSajuTime(DateTime dt, int day_gan, out int sjGanTime, out int sjZiTime)  // 시주 천간 지지
        {
            int _day_gan = day_gan;
            double time = dt.TimeOfDay.TotalMinutes; // 서울 표준시  127.5 기준임 
            sjGanTime = 0;

            sjZiTime = ((int)(time + 30) / 120) % 12 + 1;

            if (_day_gan % 5 == 1) sjGanTime += 0;
            else if (_day_gan % 5 == 2) sjGanTime += 12;
            else if (_day_gan % 5 == 3) sjGanTime += 24;
            else if (_day_gan % 5 == 4) sjGanTime += 36;
            else sjGanTime += 48;

            sjGanTime = (sjGanTime + sjZiTime - 1) % 10 + 1;
        }

        public string toTaeulGusung(int i)
        {
            if (i == 0) return "太乙";
            else if (i == 1) return "攝提";
            else if (i == 2) return "軒轅";
            else if (i == 3) return "招搖";
            else if (i == 4) return "天符";
            else if (i == 5) return "靑龍";
            else if (i == 6) return "咸池";
            else if (i == 7) return "太陰";
            else if (i == 8) return "天乙";
            else return "에러";
        }

        public int getPakjehwaeui(int mun, int goong)
        {
            int[] eightmun_ohaeng = { 3, 1, 1, 2, 3, 4, 4, 0, 99 };
            int[] goong_ohaeng = { 0, 3, 1, 1, -99, 4, 4, 3, 2 };

            if (goong_ohaeng[goong] - eightmun_ohaeng[mun] == 1 || goong_ohaeng[goong] - eightmun_ohaeng[mun] == -4) return 1;
            else if (eightmun_ohaeng[mun] - goong_ohaeng[goong] == 1 || eightmun_ohaeng[mun] - goong_ohaeng[goong] == -4) return 2;
            else if (goong_ohaeng[goong] - eightmun_ohaeng[mun] == 2 || goong_ohaeng[goong] - eightmun_ohaeng[mun] == -3) return 3;
            else if (eightmun_ohaeng[mun] - goong_ohaeng[goong] == 2 || eightmun_ohaeng[mun] - goong_ohaeng[goong] == -3) return 4;
            else return 0;
        }

        public Color getGancolor(int gan) //천간 오행 색상
        {
            if (gan == 1 || gan == 2) return Color.LightGreen; //목
            else if (gan == 3 || gan == 4) return Color.Pink; //화
            else if (gan == 5 || gan == 6) return Color.Wheat;  //토
            else if (gan == 7 || gan == 8) return Color.White;  //금
            else return Color.Gray; //수
        }

        public Color getZicolor(int gan) // 지간 오행 색깔
        {
            if (gan == 3 || gan == 4) return Color.LightGreen;
            else if (gan == 6 || gan == 7) return Color.Pink;
            else if (gan == 2 || gan == 5 || gan == 8 || gan == 11) return Color.Wheat;
            else if (gan == 9 || gan == 10) return Color.White;
            else return Color.Gray;
        }

        public DateTime[] get24Terms(DateTime datetime) // 24절기 계산
        {
            DateTime solar_start = new DateTime(2000, 3, 20, 16, 35, 15); // 2000년 춘분점 기준 2000년 3월 20일 16시 35분 15초
            double solar_tyear = 31556940; // 평균 태양년을 초로 표시
            double solar_byear = 2000; // 기준 년도
            String[] hterms =
                {"소한", "대한", "입춘", "우수", "경칩", "춘분", "청명", "곡우","입하", "소만", "망종", "하지",
                "소서", "대서", "입추", "처서", "백로", "추분", "한로", "상강", "입동", "소설", "대설", "동지"};// 24절기 이름
            double[] tterms =
                {-6418939, -5146737, -3871136, -2589569, -1299777, 0, 1310827, 2633103, 3966413, 5309605, 6660762, 8017383,
                9376511, 10735018, 12089855, 13438199, 14777792, 16107008, 17424841, 18731368, 20027093, 21313452, 22592403, 23866369};// 소서~ 동지까지 초단위 시간
            double[,] addstime =
                {{1902, 1545} ,{1903, 1734} ,{1904, 1740} ,{1906,  475} ,{1907,  432}
                ,{1908,  480} ,{1909,  462} ,{1915, -370} ,{1916, -332} ,{1918, -335}
                ,{1919, -263} ,{1925,  340} ,{1927,  344} ,{1928, 2133} ,{1929, 2112}
                ,{1930, 2100} ,{1931, 1858} ,{1936, -400} ,{1937, -400} ,{1938, -342}
                ,{1939, -300} ,{1944,  365} ,{1945,  380} ,{1946,  400} ,{1947,  200}
                ,{1948,  244} ,{1953, -266} ,{1954, 2600} ,{1955, 3168} ,{1956, 3218}
                ,{1957, 3366} ,{1958, 3300} ,{1959, 3483} ,{1960, 2386} ,{1961, 3015}
                ,{1962, 2090} ,{1963, 2090} ,{1964, 2264} ,{1965, 2370} ,{1966, 2185}
                ,{1967, 2144} ,{1968, 1526} ,{1971, -393} ,{1972, -430} ,{1973, -445}
                ,{1974, -543} ,{1975, -393} ,{1980,  300} ,{1981,  490} ,{1982,  400}
                ,{1983,  445} ,{1984,  393} ,{1987,-1530} ,{1988,-1600} ,{1990, -362}
                ,{1991, -366} ,{1992, -400} ,{1993, -449} ,{1994, -321} ,{1995, -344}
                ,{1999,  356} ,{2000,  480} ,{2001,  483} ,{2002,  504} ,{2003,  294}
                ,{2007, -206} ,{2008, -314} ,{2009, -466} ,{2010, -416} ,{2011, -457}
                ,{2012, -313} ,{2018,  347} ,{2020,  257} ,{2021,  351} ,{2022,  159}
                ,{2023,  177} ,{2026, -134} ,{2027, -340} ,{2028, -382} ,{2029, -320}
                ,{2030, -470} ,{2031, -370} ,{2032, -373} ,{2036,  349} ,{2037,  523}};// 1902년 부터 년도별 오차 보정 테이블
            double[,] addttime =
                {{1919, 14,-160}, {1939, 10, -508}
                ,{1953, 0, 220}, {1954, 1,-2973}
                ,{1982, 18, 241} ,{1988, 13,-2455}
                ,{2013, 6, 356}, {2031, 20, 411}
                ,{2023, 0, 399}, {2023, 11,-571}};//1902년부터 절기 보정 테이블
            double time;
            DateTime[] terms = new DateTime[24];

            int kk = 0;
            for (int i = 0; i < 24; i++)
            {
                terms[i] = solar_start;  // 2000년도 춘분일
                time = (datetime.Year - solar_byear) * solar_tyear; // 계산하고자하는 년도까지 차이를 초단위로 
                time += tterms[i];  // 각 절기의 차이 더해주기

                double D = (time + 6809779) / 86400; // 기준일 기준 날짜
                double JD = D + 2451545;
                double J = 2000 + (JD - 2451545) / 365.25;
                double g = 357.529 + (0.98560028 * D);
                double q = 280.459 + (0.98564736 * D);

                g = g % 360;
                if (g < 0) g += 360;
                q = q % 360;
                if (q < 0) q += 360;

                double L = q + 1.915 * Math.Sin(g * Math.PI / 180) + 0.020 * Math.Sin(2 * g * Math.PI / 180);

                L = L % 360;
                if (L < 0) L += 360;

                double aTIME = (Math.Round(L) - L) * 87658.1256;
                time += aTIME;

                for (int j = 0; j < 85; j++)
                {
                    if (datetime.Year == addstime[j, 0]) time += addstime[j, 1];
                }
                for (int j = 0; j < 10; j++)
                {
                    if (datetime.Year == addttime[j, 0] && i == addttime[j, 1]) time += addttime[j, 2];
                }
                terms[i] = terms[i].Add(TimeSpan.FromSeconds(time));
                //Console.WriteLine(hterms[i] + $"{terms[i]}");
                //textBox5.AppendText(hterms[i] + $"{terms[i]}" + Environment.NewLine);
            }
            //textBox5.AppendText(Environment.NewLine);
            return terms;

            //    $time += $atime + $addstime + $addttime[$i]; // re-fixed 년도 보정
            //    $terms[calendar::_date('nd',$time)] = &$hterms[$i];  
            //    $times[] = $time; // fixed utime
            //}


            //if ($GMT) $utime += 32400; // force GMT to static KST, see 946727936  // 그리니치 시간 -> 한국시간 (9시간(32400초)더함)

            //$D = sprintf('%.8f',$D / 86400); // float, number of days  시간을 하루 24시간(86400초)로 나눔
            //$JD = sprintf('%.8f',$D + 2451545.0); // float, Julian Day  2000년 1월 1일(2351545)을 더해줌
            //$J = sprintf('%.4f', 2000.0 + ($JD - 2451545.0) / 365.25); // Jxxxx.xxxx format 2000.각도 로 표시

            //$g = 357.529 + (0.98560028 *$D);
            //$q = 280.459 + (0.98564736 *$D);

            //## fixed
            //##
            //$g = calendar::deg2valid($g); // to valid degress
            //$q = calendar::deg2valid($q); // to valid degress

            //## convert
            //##
            //$deg2rad = array();
            //$deg2rad['g'] = deg2rad($g); // radian
            //$deg2rad['2g'] = deg2rad($g * 2); // radian

            //$sing = sin($deg2rad['g']); // degress
            //$sin2g = sin($deg2rad['2g']); // degress

            //## L is an approximation to the Sun's geocentric apparent ecliptic longitude
            //##
            //$L = $q + (1.915 *$sing) +(0.020 *$sin2g);
            //$L = calendar::deg2valid($L); // degress
            //$atime = calendar::deg2solartime(round($L) -$L); // float

            //        return array($L,$atime); // array, float degress, float seconds
        }

        public bool getDirection(DateTime dt, DateTime[] terms)
        {
            int i;
            bool direction = false;

            //String temp = toGan(sjGanzi[2, 0]) + toZi(sjGanzi[2, 1]);
            for (i = 0; i < 24 && dt.CompareTo(terms[i]) >= 0; i++) ;
            if (i == 24 || i <= 11) direction = true;

            return direction;
        } // 양둔/음둔 계산

        public void setHongNum(Goong[] goong, int[,] sjGanzi, out int eunboksu1, out int eunboksu2) // 홍국수 계산
        {
            int t1, t2;

            t1 = (sjGanzi[0, 1] + sjGanzi[1, 1] + sjGanzi[2, 1] + sjGanzi[3, 1]) % 9;
            t2 = (sjGanzi[0, 0] + sjGanzi[1, 0] + sjGanzi[2, 0] + sjGanzi[3, 0]) % 9;

            if (t1 == 0) t1 = 9;
            if (t2 == 0) t2 = 9;

            if (t1 == 5)
            {
                goong[8].hongNum[0] = t2 % 10 + 1;

                for (int i = 0; i < 9; i++) goong[i].hongNum[1] = i + 1;
                for (int i = 7; i >= 0; i--) goong[i].hongNum[0] = goong[i + 1].hongNum[0] % 10 + 1;

                eunboksu1 = goong[4].hongNum[0];
                eunboksu2 = 5;

                goong[4].hongNum[0] = t2;
            }

            else
            {
                goong[0].hongNum[1] = t1 % 10 + 1;
                goong[8].hongNum[0] = t2 % 10 + 1;

                for (int i = 1; i < 9; i++) goong[i].hongNum[1] = goong[i - 1].hongNum[1] % 10 + 1;
                for (int i = 7; i >= 0; i--) goong[i].hongNum[0] = goong[i + 1].hongNum[0] % 10 + 1;

                eunboksu1 = goong[4].hongNum[0];
                eunboksu2 = goong[4].hongNum[1];
                goong[4].hongNum[1] = t1;
                goong[4].hongNum[0] = t2;
            }
        }

        public void setDongcheo(Goong[] goong, int[,] sjGanzi) // 동처 계산
        {
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 1) goong[0].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 8 || sjGanzi[i, 1] == 9) goong[1].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 4) goong[2].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 5 || sjGanzi[i, 1] == 6) goong[3].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 11 || sjGanzi[i, 1] == 12) goong[5].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 10) goong[6].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 2 || sjGanzi[i, 1] == 3) goong[7].b_dong[i] = true; }
            for (int i = 0; i < 4; i++) { if (sjGanzi[i, 1] == 7) goong[8].b_dong[i] = true; }
        }

        public void setSixSin(Goong[] goong, int[,] sjGanzi) // 육신 계산
        {
            int i;
            for (i = 0; goong[i].b_dong[2] != true; i++) ;

            for (int j = 0; j < 9; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    if (goong[i].hongNum[1] == 1 || goong[i].hongNum[1] == 6)
                    {
                        if (goong[(i + j) % 9].hongNum[k] == 1 || goong[(i + j) % 9].hongNum[k] == 6) goong[(i + j) % 9].six_sin[k] = 2;        //형
                        else if (goong[(i + j) % 9].hongNum[k] == 3 || goong[(i + j) % 9].hongNum[k] == 8) goong[(i + j) % 9].six_sin[k] = 3; //손
                        else if (goong[(i + j) % 9].hongNum[k] == 2 || goong[(i + j) % 9].hongNum[k] == 7) goong[(i + j) % 9].six_sin[k] = 4; //재
                        else if ((goong[i].hongNum[1] == 1 && goong[(i + j) % 9].hongNum[k] == 5) || (goong[i].hongNum[1] == 6 && goong[(i + j) % 9].hongNum[k] == 10)) goong[(i + j) % 9].six_sin[k] = 5; //귀
                        else if ((goong[i].hongNum[1] == 1 && goong[(i + j) % 9].hongNum[k] == 10) || (goong[i].hongNum[1] == 6 && goong[(i + j) % 9].hongNum[k] == 5)) goong[(i + j) % 9].six_sin[k] = 6; //관
                        else goong[(i + j) % 9].six_sin[k] = 7; //부
                    }
                    else if (goong[i].hongNum[1] == 3 || goong[i].hongNum[1] == 8)
                    {
                        if (goong[(i + j) % 9].hongNum[k] == 3 || goong[(i + j) % 9].hongNum[k] == 8) goong[(i + j) % 9].six_sin[k] = 2;        //형
                        else if (goong[(i + j) % 9].hongNum[k] == 2 || goong[(i + j) % 9].hongNum[k] == 7) goong[(i + j) % 9].six_sin[k] = 3; //손
                        else if (goong[(i + j) % 9].hongNum[k] == 5 || goong[(i + j) % 9].hongNum[k] == 10) goong[(i + j) % 9].six_sin[k] = 4; //재
                        else if ((goong[i].hongNum[1] == 3 && goong[(i + j) % 9].hongNum[k] == 4) || (goong[i].hongNum[1] == 8 && goong[(i + j) % 9].hongNum[k] == 9)) goong[(i + j) % 9].six_sin[k] = 6; //관
                        else if ((goong[i].hongNum[1] == 3 && goong[(i + j) % 9].hongNum[k] == 9) || (goong[i].hongNum[1] == 8 && goong[(i + j) % 9].hongNum[k] == 4)) goong[(i + j) % 9].six_sin[k] = 5; //귀
                        else goong[(i + j) % 9].six_sin[k] = 7; //부
                    }
                    else if (goong[i].hongNum[1] == 2 || goong[i].hongNum[1] == 7)
                    {
                        if (goong[(i + j) % 9].hongNum[k] == 2 || goong[(i + j) % 9].hongNum[k] == 7) goong[(i + j) % 9].six_sin[k] = 2;        //형
                        else if (goong[(i + j) % 9].hongNum[k] == 5 || goong[(i + j) % 9].hongNum[k] == 10) goong[(i + j) % 9].six_sin[k] = 3; //손
                        else if (goong[(i + j) % 9].hongNum[k] == 4 || goong[(i + j) % 9].hongNum[k] == 9) goong[(i + j) % 9].six_sin[k] = 4; //재
                        else if ((goong[i].hongNum[1] == 2 && goong[(i + j) % 9].hongNum[k] == 1) || (goong[i].hongNum[1] == 7 && goong[(i + j) % 9].hongNum[k] == 6)) goong[(i + j) % 9].six_sin[k] = 6; //관
                        else if ((goong[i].hongNum[1] == 2 && goong[(i + j) % 9].hongNum[k] == 6) || (goong[i].hongNum[1] == 7 && goong[(i + j) % 9].hongNum[k] == 1)) goong[(i + j) % 9].six_sin[k] = 5; //귀
                        else goong[(i + j) % 9].six_sin[k] = 7; //부
                    }
                    else if (goong[i].hongNum[1] == 5 || goong[i].hongNum[1] == 10)
                    {
                        if (goong[(i + j) % 9].hongNum[k] == 5 || goong[(i + j) % 9].hongNum[k] == 10) goong[(i + j) % 9].six_sin[k] = 2;        //형
                        else if (goong[(i + j) % 9].hongNum[k] == 4 || goong[(i + j) % 9].hongNum[k] == 9) goong[(i + j) % 9].six_sin[k] = 3; //손
                        else if (goong[(i + j) % 9].hongNum[k] == 1 || goong[(i + j) % 9].hongNum[k] == 6) goong[(i + j) % 9].six_sin[k] = 4; //재
                        else if ((goong[i].hongNum[1] == 5 && goong[(i + j) % 9].hongNum[k] == 3) || (goong[i].hongNum[1] == 10 && goong[(i + j) % 9].hongNum[k] == 8)) goong[(i + j) % 9].six_sin[k] = 5; //귀
                        else if ((goong[i].hongNum[1] == 5 && goong[(i + j) % 9].hongNum[k] == 8) || (goong[i].hongNum[1] == 10 && goong[(i + j) % 9].hongNum[k] == 3)) goong[(i + j) % 9].six_sin[k] = 6; //귀
                        else goong[(i + j) % 9].six_sin[k] = 7; //부
                    }
                    else
                    {
                        if (goong[(i + j) % 9].hongNum[k] == 4 || goong[(i + j) % 9].hongNum[k] == 9) goong[(i + j) % 9].six_sin[k] = 2;        //형
                        else if (goong[(i + j) % 9].hongNum[k] == 1 || goong[(i + j) % 9].hongNum[k] == 6) goong[(i + j) % 9].six_sin[k] = 3; //손
                        else if (goong[(i + j) % 9].hongNum[k] == 3 || goong[(i + j) % 9].hongNum[k] == 8) goong[(i + j) % 9].six_sin[k] = 4; //재
                        else if ((goong[i].hongNum[1] == 4 && goong[(i + j) % 9].hongNum[k] == 2) || (goong[i].hongNum[1] == 9 && goong[(i + j) % 9].hongNum[k] == 7)) goong[(i + j) % 9].six_sin[k] = 5; //귀
                        else if ((goong[i].hongNum[1] == 4 && goong[(i + j) % 9].hongNum[k] == 7) || (goong[i].hongNum[1] == 9 && goong[(i + j) % 9].hongNum[k] == 2)) goong[(i + j) % 9].six_sin[k] = 6; //귀
                        else goong[(i + j) % 9].six_sin[k] = 7; //부
                    }

                }
            }
            goong[i].six_sin[1] = 1; // 세 표시
        }

        public void setHonglvl(Goong[] goong, int month, DateTime dt) // 육신 강약 계산
        {
            for (int i = 0; i < 9; i++)
            {
                {
                    int[] temp = { 0, 0, 0 };
                    if (getohaeng(goong[i].hongNum[1], 0) == getohaeng(i + 1, 3) || getohaeng(goong[i].hongNum[1], 0) == (getohaeng(i + 1, 3) + 1) % 5)
                        temp[0] = 1;
                    if (getohaeng(goong[i].hongNum[1], 0) == getohaeng(goong[i].hongNum[0], 0) || getohaeng(goong[i].hongNum[1], 0) == (getohaeng(goong[i].hongNum[0], 0) + 1) % 5)
                        temp[1] = 1;
                    //if (getohaeng(goong[i].hongNum[1], 0) == getohaeng(month, 2) || getohaeng(goong[i].hongNum[1], 0) == (getohaeng(month, 2) + 1) % 5)
                    if (getohaeng(goong[i].hongNum[1], 0) == getohaeng_dt(dt) || getohaeng(goong[i].hongNum[1], 0) == (getohaeng_dt(dt) + 1) % 5)
                        temp[2] = 1;
                    goong[i].hongNumlvl[1] = 4 * temp[0] + 2 * temp[1] + temp[2];

                    temp[0] = temp[1] = temp[2] = 0;
                    if (getohaeng(goong[i].hongNum[0], 0) == getohaeng(i + 1, 3) || getohaeng(goong[i].hongNum[0], 0) == (getohaeng(i + 1, 3) + 1) % 5)
                        temp[0] = 1;
                    if (getohaeng(goong[i].hongNum[0], 0) == getohaeng(goong[i].hongNum[1], 0) || getohaeng(goong[i].hongNum[0], 0) == (getohaeng(goong[i].hongNum[1], 0) + 1) % 5)
                        temp[1] = 1;
                    //if (getohaeng(goong[i].hongNum[0], 0) == getohaeng(month, 2) || getohaeng(goong[i].hongNum[0], 0) == (getohaeng(month, 2) + 1) % 5)
                    if (getohaeng(goong[i].hongNum[0], 0) == getohaeng_dt(dt) || getohaeng(goong[i].hongNum[0], 0) == (getohaeng_dt(dt) + 1) % 5)
                        temp[2] = 1;
                    goong[i].hongNumlvl[0] = 4 * temp[0] + 2 * temp[1] + temp[2];
                }
            }
        }

        public int getohaeng(int num, int type) // 오행 받아오기
        {
            //type 0 : 숫자 1~10
            //type 1 : 천간 1~10
            //tyep 2 : 지지 1~12  , 이거 안쓰고 아래  getohaeng_day 사용
            //type 3 : 각 국의 바탕오행
            int ohaeng; // 0:수, 1:목, 2:화, 3:토, 4:금
            if (type == 0)
            {
                if (num == 1 || num == 6) ohaeng = 0;
                else if (num == 3 || num == 8) ohaeng = 1;
                else if (num == 2 || num == 7) ohaeng = 2;
                else if (num == 5 || num == 10) ohaeng = 3;
                else ohaeng = 4;
            }
            else if (type == 1)
            {
                if (num == 1 || num == 2) ohaeng = 1;
                else if (num == 3 || num == 4) ohaeng = 2;
                else if (num == 5 || num == 6) ohaeng = 3;
                else if (num == 7 || num == 8) ohaeng = 4;
                else ohaeng = 0;
            }
            else if (type == 2)
            {
                if (num == 1 || num == 12) ohaeng = 0;
                else if (num == 3 || num == 4) ohaeng = 1;
                else if (num == 6 || num == 7) ohaeng = 2;
                else if (num == 2 || num == 5 || num == 8 || num == 11) ohaeng = 3;
                else ohaeng = 4;
            }
            else
            {
                if (num == 1 || num == 6) ohaeng = 0;  // 수
                else if (num == 3 || num == 8) ohaeng = 1;  // 목
                else if (num == 4 || num == 9) ohaeng = 2;  // 화
                else if (num == 5) ohaeng = 3; //토
                else ohaeng = 4;  //금
            }
            return ohaeng;
        }

        public int getohaeng_dt(DateTime dt) // 날짜의 오행 받아오기
        {
            int ohaeng; // 0:수, 1:목, 2:화, 3:토, 4:금
            if ((dt.Month == 2 && dt.Day >= 4) || dt.Month == 3 || (dt.Month == 4 && dt.Day < 17)) ohaeng = 1;
            else if ((dt.Month == 5 && dt.Day >= 5) || dt.Month == 6 || (dt.Month == 7 && dt.Day < 20)) ohaeng = 2;
            else if ((dt.Month == 8 && dt.Day >= 7) || dt.Month == 9 || (dt.Month == 10 && dt.Day < 20)) ohaeng = 4;
            else if ((dt.Month == 11 && dt.Day >= 7) || dt.Month == 12 || (dt.Month == 1 && dt.Day < 17)) ohaeng = 0;
            else ohaeng = 3;
            return ohaeng;
        }

            public void setYooAge(Goong[] goong, int t1, int t2) // 유년 계산
        {
            int i;

            for (i = 0; goong[i].b_dong[2] != true; i++) ;
            goong[i].yoo_age[1] = 1;

            for (int j = 1; j < 9; j++)
            {
                if (goong[(j + i - 1) % 9].hongNum[1] == 10) goong[(j + i) % 9].yoo_age[1] = t2 + goong[(j + i - 1) % 9].yoo_age[1];
                else goong[(j + i) % 9].yoo_age[1] = goong[(j + i - 1) % 9].hongNum[1] + goong[(j + i - 1) % 9].yoo_age[1];
            }
            if (goong[(i + 8) % 9].hongNum[1] == 10) goong[i].yoo_age[0] = goong[(i + 8) % 9].yoo_age[1] + t2;
            else
                goong[i].yoo_age[0] = goong[(i + 8) % 9].yoo_age[1] + goong[(i + 8) % 9].hongNum[1];
            for (int j = 1; j < 9; j++)
            {
                if (goong[(i - j + 10) % 9].hongNum[0] == 10) goong[(i - j + 9) % 9].yoo_age[0] = t1 + goong[(i - j + 10) % 9].yoo_age[0];
                else goong[(i - j + 9) % 9].yoo_age[0] = goong[(i - j + 10) % 9].hongNum[0] + goong[(i - j + 10) % 9].yoo_age[0];
            }
        }

        public int setYookSam(Goong[] goong, int[,] sjGanzi, DateTime dt, DateTime[] terms, bool direction) // 육의삼기 설정
        {
            String[,] month = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "甲午", "乙未", "丙申", "丁酉", "戊戌", "己卯", "庚辰", "辛巳", "壬午", "癸未", "己酉", "庚戌", "辛亥", "壬子", "癸丑"},
                { "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己巳", "庚午", "辛未", "壬申", "癸酉", "己亥", "庚子", "辛丑", "壬寅", "癸卯"},
                { "甲辰", "乙巳", "丙午", "丁未", "戊申", "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己丑", "庚寅", "辛卯", "壬辰", "癸巳", "己未", "庚申", "辛酉", "壬戌", "癸亥" } };

            String[,] day = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉" },
                            { "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未" },
                            { "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳" },
                            { "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑", "壬寅", "癸卯" },
                            { "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子", "癸丑" },
                            { "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥" } };

            String[] gabja_order = {   "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉" ,
                                        "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未" ,
                                        "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳" ,
                                        "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑", "壬寅", "癸卯" ,
                                        "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子", "癸丑" ,
                                        "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥" };


            int[,] jeolgi = {{2,8,5}, {3,9,6}, {8,5,2}, {9,6,3}, {1,7,4}, {3,9,6}, {4,1,7}, {5,2,8}, {4,1,7}, {5,2,8}, {6,3,9}, {9,3,6},
                            {8,2,5}, {7,1,4}, {2,5,8}, {1,4,7}, {9,3,6}, {7,1,4}, {6,9,3}, {5,8,2}, {6,9,3}, {5,8,2}, {4,7,1}, {1,7,4}};

            int[] rr = { 4, 9, 2, 7, 6, 1, 8, 3 };

            int i, j, k, start;
            //bool direction = false;

            for (i = 0; i < 9; i++)
            {
                goong[i].yooksam[0] = 10;
                goong[i].yooksam[1] = 10;
            }

            //육의삼기 지반
            String temp = toGan(sjGanzi[2, 0]) + toZi(sjGanzi[2, 1]); // 日 의 간지

            for (i = 0; i < 60 && month[i / 20, i % 20] != temp; i++) ;// month는 60갑자 , 여기서 i는 위 temp(日의 간지)가 몇번째 순번인가 카운트

            for (j = 0; j < 24 && dt.CompareTo(terms[j]) >= 0; j++) ; // j는 절기가 어디에 해당하는가 카운트

            //생일 전 절기 日의 간지
            int j_1, j_2;
            string temp2;
            DateTime[] terms_previous_year = new DateTime[24];
            DateTime previous_jeolgi;

            if (j != 0) // 생일이 24절기 안에 들어갈때
            {
                ToSajuDay(terms[j - 1], out j_1, out j_2);
                temp2 = toGan(j_1) + toZi(j_2);
            }
            else // 생일이 각 해의 첫번째 절기(소한- 보통 1월 5일~6일) 보다 앞에 있을때, 즉 생일 전의 절기가 그 전해의 마지막 절기(동지) 일때
            {
                terms_previous_year = get24Terms(real_dt.AddYears(-1));
                ToSajuDay(terms_previous_year[23], out j_1, out j_2);
                previous_jeolgi = terms_previous_year[23];
                temp2 = toGan(j_1) + toZi(j_2);
            }

            //생일 후 절기 日의 간지
            DateTime[] terms_next_year = new DateTime[24];
            DateTime next_jeolgi;
            string temp3;

            if (j == 24)
            {
                terms_next_year = get24Terms(real_dt.AddYears(1));
                ToSajuDay(terms_next_year[0], out j_1, out j_2);
                next_jeolgi = terms_next_year[0];
                temp3 = toGan(j_1) + toZi(j_2);
            }
            else
            {
                ToSajuDay(terms[j], out j_1, out j_2);
                next_jeolgi = terms[j];
                temp3 = toGan(j_1) + toZi(j_2);
            }

            // 몇번째인지 카운트
            int l, m, n;
            for (l = 0; l < 60 && gabja_order[l] != temp; l++) ;  // 생일
            for (m = 0; m < 60 && gabja_order[m] != temp2; m++) ; // 생일전 절기입일
            for (n = 0; n < 60 && gabja_order[n] != temp3; n++) ; // 생일전 절기입일

            //MessageBox.Show(k+temp2 + terms[j-1].ToString());
            //MessageBox.Show("생일 "+ l + " 전절기 " + m + " 후절기 " + n + " 상원첫날 " + (int)(l/15)*15 + " 차이 " + (n - (int)(l / 15) * 15)) ;

            int diffHour = next_jeolgi.Hour - real_dt.Hour;
            int diffMin = next_jeolgi.Minute - real_dt.Minute;

            if (n - (int)(l / 15) * 15 < 10 && n - (int)(l / 15) * 15 > 0) // 생일의 절기입일이 생일 다음 절기와 비교해 10일 이내이면 생일은 다음 절기로 간주한다(초신)
                                                                           // 접기의 경우 기존 방식을 사용해도 결과가 같게 나오므로(보국 때문에) 별도로 처리하지 않는다
            {
                if ((l % 15) == 0)  // 생일이 절기 입일인 경우 시간을 따져서 절기 시간보다 빠르면 이전 절기로, 뒤에 오면 다음 절기로 간주
                {
                    if (diffHour < 0 || (diffHour == 0 && diffMin <= 0)) j++;
                }
                else j++;
            }


            //if (j == 24 || j <= 10) direction = true;
            start = jeolgi[(j + 23) % 24, i / 20];   // 지반 戊 시작하는 궁 위치

            label56.Text = toBirthJeolgi(j-1, i, start);
            //textBox5.AppendText(hterms[i] + $"{terms[i]}" + Environment.NewLine);

            label56.Text = label56.Text + " " + toOhaeng_1(getohaeng_dt(real_dt)) + "月令";

            //Text += toJeolGi((j+23)%24) + Environment.NewLine; 
            //Console.WriteLine("지반 戊 시작하는 궁 위치: " + (start));

            for (k = 0; k < 9; k++)
            {
                if (direction)
                    goong[(start + 8 + k) % 9].yooksam[1] = k;
                else
                    goong[(start + 8 - k) % 9].yooksam[1] = k;
            }

            //육의삼기 천반
            temp = toGan(sjGanzi[3, 0]) + toZi(sjGanzi[3, 1]);

            for (i = 0; i < 60 && day[i / 10, i % 10] != temp; i++) ;
            for (j = 0; j < 9 && toYookSam(goong[j].yooksam[1]) != toGan(sjGanzi[3, 0]); j++) ;

            start = i / 10;     //시순수 값
            //goong[j].yooksam[0] = start;

            //textBox5.Text += "지반 시간이 위치한 궁 위치: " + (j + 1) + Environment.NewLine;
            //textBox5.Text += "천반 시순수 값: " + toYookSam(start) + Environment.NewLine;

            for (i = 0; i < 8 && rr[i] != j + 1; i++) ;
            if (j == 9) //j 가 9면 시간이 甲 (복음국으로 취급)
            {
                i = j = 0;
                goto EXIT;
            }
            if (i == 8)  // 천반 순수가 있는 궁 위치가 중궁인 경우 곤궁(2번방)으로 변경
            {
                i = 2;
            }

            for (j = 0; j < 8 && goong[rr[j] - 1].yooksam[1] != start; j++) ;
            if (j == 8) // 시순수가 있는 위치가 중궁이면 곤궁으로 간주
            {
                j = 2;

                for (k = 0; k < 8; k++)
                    goong[rr[(i + k) % 8] - 1].yooksam[0] = goong[(rr[(k + j) % 8]) - 1].yooksam[1];

                goong[rr[i % 8] - 1].yooksam[0] = start;

                goto EXIT_2;
            }

        EXIT:
            for (k = 0; k < 8; k++)
                goong[rr[(i + k) % 8] - 1].yooksam[0] = goong[(rr[(k + j) % 8]) - 1].yooksam[1];

            EXIT_2:
            return start;
        }

        public int setYookSamSaJu(Goong[] goong, int[,] sjGanzi, int term, bool direction) // 육의삼기 설정
        {
            String[,] month = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "甲午", "乙未", "丙申", "丁酉", "戊戌", "己卯", "庚辰", "辛巳", "壬午", "癸未", "己酉", "庚戌", "辛亥", "壬子", "癸丑"},
                { "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己巳", "庚午", "辛未", "壬申", "癸酉", "己亥", "庚子", "辛丑", "壬寅", "癸卯"},
                { "甲辰", "乙巳", "丙午", "丁未", "戊申", "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己丑", "庚寅", "辛卯", "壬辰", "癸巳", "己未", "庚申", "辛酉", "壬戌", "癸亥" } };

            String[,] day = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉" },
                            { "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未" },
                            { "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳" },
                            { "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑", "壬寅", "癸卯" },
                            { "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子", "癸丑" },
                            { "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥" } };

            int[,] jeolgi = {{2,8,5}, {3,9,6}, {8,5,2}, {9,6,3}, {1,7,4}, {3,9,6}, {4,1,7}, {5,2,8}, {4,1,7}, {5,2,8}, {6,3,9}, {9,3,6},
                            {8,2,5}, {7,1,4}, {2,5,8}, {1,4,7}, {9,3,6}, {7,1,4}, {6,9,3}, {5,8,2}, {6,9,3}, {5,8,2}, {4,7,1}, {1,7,4}};

            int[] rr = { 4, 9, 2, 7, 6, 1, 8, 3 };

            int i, j, k, start;
            //bool direction = false;

            for (i = 0; i < 9; i++)
            {
                goong[i].yooksam[0] = 10;
                goong[i].yooksam[1] = 10;
            }

            //육의삼기 지반
            String temp = toGan(sjGanzi[2, 0]) + toZi(sjGanzi[2, 1]);

            for (i = 0; i < 60 && month[i / 20, i % 20] != temp; i++) ;
            //for (j = 0; j < 24 && dt.CompareTo(terms[j]) >= 0; j++) ;

            //if (j == 24 || j <= 10) direction = true;
            start = jeolgi[(term + 23) % 24, i / 20];   // 지반 戊 시작하는 궁 위치

            //if (comboBox9.SelectedIndex != -1)
                label56.Text = toBirthJeolgi((term + 23) % 24, i, start);
            //else label56.Text = "";
            //textBox5.AppendText(hterms[i] + $"{terms[i]}" + Environment.NewLine);


            //Text += toJeolGi((j+23)%24) + Environment.NewLine; 
            //Console.WriteLine("지반 戊 시작하는 궁 위치: " + (start));

            for (k = 0; k < 9; k++)
            {
                if (direction)
                    goong[(start + 8 + k) % 9].yooksam[1] = k;
                else
                    goong[(start + 8 - k) % 9].yooksam[1] = k;
            }

            //육의삼기 천반
            temp = toGan(sjGanzi[3, 0]) + toZi(sjGanzi[3, 1]);

            for (i = 0; i < 60 && day[i / 10, i % 10] != temp; i++) ;
            for (j = 0; j < 9 && toYookSam(goong[j].yooksam[1]) != toGan(sjGanzi[3, 0]); j++) ;

            start = i / 10;     //시순수 값
            //goong[j].yooksam[0] = start;

            //textBox5.Text += "지반 시간이 위치한 궁 위치: " + (j + 1) + Environment.NewLine;
            //textBox5.Text += "천반 시순수 값: " + toYookSam(start) + Environment.NewLine;

            for (i = 0; i < 8 && rr[i] != j + 1; i++) ;
            if (j == 9) //j 가 9면 시간이 甲 (복음국으로 취급)
            {
                i = j = 0;
                goto EXIT;
            }
            if (i == 8)  // 천반 순수가 있는 궁 위치가 중궁인 경우 곤궁(2번방)으로 변경
            {
                i = 2;
            }

            for (j = 0; j < 8 && goong[rr[j] - 1].yooksam[1] != start; j++) ;
            if (j == 8) // 시순수가 있는 위치가 중궁이면 곤궁으로 간주
            {
                j = 2;

                for (k = 0; k < 8; k++)
                    goong[rr[(i + k) % 8] - 1].yooksam[0] = goong[(rr[(k + j) % 8]) - 1].yooksam[1];

                goong[rr[i % 8] - 1].yooksam[0] = start;

                goto EXIT_2;
            }

        EXIT:
            for (k = 0; k < 8; k++)
                goong[rr[(i + k) % 8] - 1].yooksam[0] = goong[(rr[(k + j) % 8]) - 1].yooksam[1];

            EXIT_2:
            return start;
        }

        public void set8mun(Goong[] goong, int[,] sjGanzi, bool direction)
        {
            String[,] day = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉","甲戌", "乙亥"},
                            { "丙子", "丁丑", "戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未", "甲申", "乙酉", "丙戌", "丁亥"},
                            { "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳", "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥"},
                            { "庚子", "辛丑", "壬寅", "癸卯", "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥"},
                            { "壬子", "癸丑", "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥"}};
            int[] foward_direction = { 8, 7, 4, 9, 1, 6, 3, 2 };
            int[] backward_direction = { 8, 2, 3, 6, 1, 9, 4, 7 };

            int i = 0, j = 0, k = 0;
            string temp = toGan(sjGanzi[2, 0]) + toZi(sjGanzi[2, 1]); // 일주

            //일주 오자원 찾기
            for (i = 0; i < 5; i++)
                for (j = 0; j < 12; j++)
                {
                    if (day[i, j] == temp) goto EXIT_FOR;
                }
            EXIT_FOR:

            if (i == 1 || i == 3) j += 12;

            for (k = 0; k < 8; k++)
            {
                if (direction)
                {
                    goong[foward_direction[(j / 3 + k) % 8] - 1].eightmun = k;
                }
                else
                {
                    goong[backward_direction[(j / 3 + k) % 8] - 1].eightmun = k;
                }
            }
            goong[4].eightmun = 8;
        } // 팔문 붙이기

        public void set8goe(Goong[] goong) // 팔괘 붙이기
        {
            int[,] goe = { { 0, 1, 0 }, { 0, 0, 0 }, { 0, 0, 1 }, { 1, 1, 0 }, { 1, 1, 0 }, { 1, 1, 1 }, { 0, 1, 1 }, { 1, 0, 0 }, { 1, 0, 1 } };
            int[] temp = new int[3];
            int index = 0;
            int i, j;
            bool direction = false;

            for (i = 0; i < 3; i++)
                temp[i] = goe[goong[4].hongNum[1] - 1, i];
            //textBox5.Text += temp[0]+" "+temp[1]+" "+temp[2] + Environment.NewLine;

            for (i = 0; i < 8; i++)
            {
                if (temp[index] == 0) temp[index] = 1;
                else temp[index] = 0;

                if (index == 2)
                {
                    direction = true;
                    index -= 1;
                }
                else if (!direction) index += 1;

                else if (index == 0)
                {
                    direction = false;
                    index += 1;
                }
                else if (direction) index -= 1;

                for (j = 0; j < 9; j++)
                {
                    if (goe[j, 0] == temp[0] && goe[j, 1] == temp[1] && goe[j, 2] == temp[2])
                    {
                        goong[j].eightgoe = i;
                        //textBox5.Text += i + Environment.NewLine;
                    }
                }
                //textBox5.Text += temp[0] + " " + temp[1] + " " + temp[2] + Environment.NewLine;
            }

            goong[4].eightgoe = 10; // 중궁은 팔괘 X
        }

        public void setGooSung(Goong[] goong, int[,] sjGanzi, int sisunsoo)
        {
            int[] rr = { 4, 9, 2, 7, 6, 1, 8, 3 };
            string temp = toGan(sjGanzi[3, 0]) + toZi(sjGanzi[3, 1]);
            int i, j, k, l, m;
            int yooksam_temp = -1;
            int c = 0;

            for (i = 0; i < 9 && goong[i].yooksam[1] != sisunsoo; i++) ;
            for (j = 0; j < 9 && goong[j].yooksam[0] != sisunsoo; j++) ;

            //textBox5.Text += toYookSam(sisunsoo) + Environment.NewLine;
            //textBox5.Text += i + Environment.NewLine;
            //textBox5.Text += j + Environment.NewLine;

            if (i == 4) // 천반 시순수 위치가 중궁에 위치할 경우
            {
                i = 1;
                c = 1;
            }

            // 중궁에 시간이 배치되는 경우
            else if (toYookSam(goong[4].yooksam[1]) == toGan(sjGanzi[3, 0]))
            {
                yooksam_temp = goong[1].yooksam[1];
                c = 2;
            }

            for (k = 0; k < 8 && rr[k] != i + 1; k++) ; // 구궁도 상에서 시작 위치, 천반
            for (l = 0; l < 8 && rr[l] != j + 1; l++) ; // 구궁도 상에서 시작 위치, 지반

            if (c == 0)
            {
                for (m = 0; m < 8; m++)
                    goong[rr[(m + l - k + 8) % 8] - 1].goosung = m;
                goong[4].goosung = 8;
            }
            else if (c == 1)
            {
                for (m = 0; m < 8; m++)
                {
                    if (m == 2)
                        goong[rr[(m + l - k + 8) % 8] - 1].goosung = 8;
                    else
                        goong[rr[(m + l - k + 8) % 8] - 1].goosung = m;
                    goong[4].goosung = 2;
                }
            }
            else
            {
                for (m = 0; m < 8; m++)
                {
                    if (goong[rr[(m + l - k + 8) % 8] - 1].yooksam[0] == yooksam_temp)
                    {
                        goong[rr[(m + l - k + 8) % 8] - 1].goosung = 8;
                        goong[4].goosung = m;
                    }
                    else
                    {
                        goong[rr[(m + l - k + 8) % 8] - 1].goosung = m;
                    }
                }
            }
        } // 구성 붙이기

        public void setEightjang(Goong[] goong, int[,] sjGanzi, bool direction, int sisunsoo)
        {
            string[] eightjang1 = { "直", "蛇", "陰", "合", "陳", "雀", "地", "天" };
            string[] eightjang2 = { "直", "蛇", "陰", "合", "虎", "武", "地", "天" };
            int[] rr = { 4, 9, 2, 7, 6, 1, 8, 3 };
            int i, j;

            //textBox5.Text += toYookSam(sisunsoo) + Environment.NewLine;

            if (sjGanzi[3, 0] == 1)
                for (i = 0; i < 9 && goong[i].yooksam[1] != sisunsoo; i++) ;

            else
                for (i = 0; i < 9 && toYookSam(goong[i].yooksam[1]) != toGan(sjGanzi[3, 0]); i++) ;

            //textBox5.Text += i + Environment.NewLine;

            if (i == 4) i = 1;

            for (j = 0; j < 8 && rr[j] - 1 != i; j++) ;

            if (direction)
            {
                for (i = 0; i < 8; i++)
                    goong[rr[(i + j) % 8] - 1].eightjang = eightjang1[i];
            }
            else
            {
                for (i = 0; i < 8; i++)
                    goong[rr[(8 - i + j) % 8] - 1].eightjang = eightjang2[i];
            }
            goong[4].eightjang = " ";
        } // 팔장 붙이기

        public void setCheonMaRok(Goong[] goong, int[,] sjGanzi, bool direction)
        {
            int[] yang = { 10, 9, 4, 6, 10, 1, 10, 3, 8, 2 };
            int[] eum = { 10, 1, 6, 4, 10, 9, 10, 7, 2, 8 };
            int[] chma = { 3, 5, 7, 9, 5, 1, 3, 5, 7, 9, 5, 1 };
            int[] il = { 3, 8, 2, 7, 2, 7, 9, 4, 6, 1 };
            int cheoneol;

            if (direction)
                cheoneol = yang[sjGanzi[2, 0] - 1];

            else
                cheoneol = eum[sjGanzi[2, 0] - 1];

            for (int i = 0; i < 9; i++)
            {
                if (goong[i].hongNum[1] == cheoneol)  // 천을
                {
                    if (sjGanzi[2, 0] == 5 || sjGanzi[2, 0] == 7 || sjGanzi[2, 0] == 1)
                    {
                        if ((sjGanzi[2, 0] == 5 || sjGanzi[2, 0] == 7) && ((direction && sjGanzi[0, 0] % 2 == 1) || (!direction && sjGanzi[0, 0] % 2 == 0)))
                            goong[i].cheoneul = 1;
                        else if (sjGanzi[2, 0] == 1 && ((!direction && sjGanzi[0, 0] % 2 == 1) || (direction && sjGanzi[0, 0] % 2 == 0)))
                            goong[i].cheoneul = 1;
                    }
                    else goong[i].cheoneul = 1;
                    //textBox5.Text += (i + 1) + "궁 천을" + Environment.NewLine;
                }

                else if (i == 4 && (goong[i].hongNum[1] + 5) % 10 == cheoneol)
                {
                    if (sjGanzi[2, 0] == 5 || sjGanzi[2, 0] == 7 || sjGanzi[2, 0] == 2)
                    {
                        if ((sjGanzi[2, 0] == 5 || sjGanzi[2, 0] == 7) && ((direction && sjGanzi[0, 0] % 2 == 1) || (!direction && sjGanzi[0, 0] % 2 == 0)))
                            goong[i].cheoneul = 2;
                        else if (sjGanzi[2, 0] == 2 && ((!direction && sjGanzi[0, 0] % 2 == 1) || (direction && sjGanzi[0, 0] % 2 == 0)))
                            goong[i].cheoneul = 2;
                    }
                    else goong[i].cheoneul = 2;
                    //textBox5.Text += (i + 1) + "궁 복천을" + Environment.NewLine;
                }
            }

            for (int i = 0; i < 9; i++) //천마
            {
                if (goong[i].hongNum[1] == chma[sjGanzi[1, 1] - 1])
                    goong[i].cheonma = 1;

                else if (i == 4 && (goong[i].hongNum[1] + 5) % 10 == chma[sjGanzi[1, 1] - 1])
                    goong[i].cheonma = 2;
            }

            for (int i = 0; i < 9; i++) //일록
            {
                if (goong[i].hongNum[1] == il[sjGanzi[2, 0] - 1])
                    goong[i].ilrok = 1;

                else if (i == 4 && (goong[i].hongNum[1] + 5) % 10 == il[sjGanzi[2, 0] - 1])
                    goong[i].ilrok = 2;
            }
        }  // 천을 천마 일록

        public void swapstring(string str, out string ostr)
        {
            string temp = str;
            ostr = temp.Substring(1, 1) + temp.Substring(0, 1);
        }

        public void setEunsung(Goong[] goong, int[,] sjGanzi)
        {
            int i, j;
            int[] gipyo1 = { 1, 6, 7, 2, 5, 10, 3, 8, 9, 4 };
            int[] gipyo2 = { 6, 7, 12, 1, 12, 1, 9, 10, 3, 4 };
            //int[] gipyo1 = { 1, 6, 1, 6, 7, 2, 5, 10, 3, 8, 9, 4 };
            //int[] gipyo2 = { 2, 7, 10, 5, 6, 1, 6, 1, 9, 4, 3, 8 };
            int[] rr = { 1, 8, 8, 3, 4, 4, 9, 2, 2, 7, 6, 6 };

            for (i = 0; i < 9 && goong[i].b_dong[2] != true; i++) ;
            for (j = 0; j < 10 && goong[i].hongNum[1] != gipyo1[j]; j++) ;

            int start = gipyo2[j];

            for (i = 0; i < 12; i++)
            {
                if (j % 2 == 0) // 시계
                {
                    goong[rr[(i + start + 11) % 12] - 1].eunsung += toEunSung(i);
                }
                else // 반시계
                {
                    goong[rr[((-i + start + 11) % 12)] - 1].eunsung += toEunSung(i);
                }

            }
            if (j % 2 == 0) //시계
            {
                if (start == 3) // 8궁두번째
                {
                    swapstring(goong[5].eunsung, out goong[5].eunsung);
                }
                else if (start == 6) //4궁 두번째 시작
                {
                    swapstring(goong[3].eunsung, out goong[3].eunsung);
                    swapstring(goong[5].eunsung, out goong[5].eunsung);
                    swapstring(goong[7].eunsung, out goong[7].eunsung);
                }

                else if (start == 9)  //2궁 두번째
                {
                    swapstring(goong[1].eunsung, out goong[1].eunsung);
                    swapstring(goong[5].eunsung, out goong[5].eunsung);
                    swapstring(goong[7].eunsung, out goong[7].eunsung);
                }
                else if (start == 12)  //6궁 두번째
                {
                    swapstring(goong[7].eunsung, out goong[7].eunsung);
                }
                else
                {
                    swapstring(goong[5].eunsung, out goong[5].eunsung);
                    swapstring(goong[7].eunsung, out goong[7].eunsung);
                }
            }
            else //반시계
            {
                if (start == 2) // 8궁 첫번째
                {
                    swapstring(goong[1].eunsung, out goong[1].eunsung);
                    swapstring(goong[3].eunsung, out goong[3].eunsung);
                    swapstring(goong[7].eunsung, out goong[7].eunsung);
                }
                else if (start == 6) //4궁 두번째 시작
                {
                    swapstring(goong[1].eunsung, out goong[1].eunsung);
                }

                else if (start == 9)  //2궁 두번째
                {
                    swapstring(goong[3].eunsung, out goong[3].eunsung);
                }
                else if (start == 11)  //6궁 첫번째
                {
                    swapstring(goong[5].eunsung, out goong[5].eunsung);
                    swapstring(goong[1].eunsung, out goong[1].eunsung);
                    swapstring(goong[3].eunsung, out goong[3].eunsung);
                }
                else
                {
                    swapstring(goong[1].eunsung, out goong[1].eunsung);
                    swapstring(goong[3].eunsung, out goong[3].eunsung);
                }
            }
            //textBox5.AppendText(toZi(gipyo2[j]));  
        }// 12운성

        public void setGongMang(Goong[] goong, int[,] sjGanzi)
        {
            int i, j;
            i = sjGanzi[2, 1];
            j = sjGanzi[2, 0];

            if (i - j == 0)
                goong[5].gongmang = "◯";
            else if ((i - j + 12) % 12 == 10)
            {
                goong[1].gongmang = "◯";
                goong[6].gongmang = "◯";
            }
            else if ((i - j + 12) % 12 == 8)
            {
                goong[1].gongmang = "◯";
                goong[8].gongmang = "◯";
            }
            else if ((i - j + 12) % 12 == 6)
            {
                goong[3].gongmang = "◯";
            }
            else if ((i - j + 12) % 12 == 4)
            {
                goong[2].gongmang = "◯";
                goong[7].gongmang = "◯";
            }
            else if ((i - j + 12) % 12 == 2)
            {
                goong[0].gongmang = "◯";
                goong[7].gongmang = "◯";
            }
            if (goong[goong[4].hongNum[1] - 1].gongmang == "◯") goong[goong[4].hongNum[1] - 1].gongmang = "◎";
        }// 공망

        public void setSinsal(Goong[] goong, int[,] sjGanzi)
        {
            int[,] samhab = { { 11, 3, 7 }, { 2, 6, 10 }, { 5, 9, 1 }, { 8, 0, 4 } };
            int[] rr = { 0, 7, 7, 2, 3, 3, 8, 1, 1, 6, 5, 5 };
            int[] hong = { 1, 99, 3, 8, 99, 2, 7, 99, 9, 4, 99, 6 };
            int i, j;

            for (i = 0; i < 4; i++)
                for (j = 0; j < 3; j++)
                    if (samhab[i, j] == sjGanzi[0, 1] - 1) goto EXIT1;
                    EXIT1:
            for (j = 0; j < 9; j++)
            {
                if (goong[j].hongNum[1] == hong[(samhab[i, 0] + 6) % 12]) goong[j].sinsal += " 歲馬";
                if (goong[j].hongNum[1] == hong[(samhab[i, 1] + 11) % 12]) goong[j].sinsal += " 歲亡";
                if (goong[j].hongNum[1] == hong[(samhab[i, 2] + 1) % 12]) goong[j].sinsal += " 歲劫";
            }
            goong[rr[(samhab[i, 0] + 1) % 12]].sinsal += " 歲年";
            goong[rr[(samhab[i, 2]) % 12]].sinsal += " 歲華";

            for (i = 0; i < 4; i++)
                for (j = 0; j < 3; j++)
                    if (samhab[i, j] == sjGanzi[2, 1] - 1) goto EXIT2;
                    EXIT2:
            for (j = 0; j < 9; j++)
            {
                if (goong[j].hongNum[1] == hong[(samhab[i, 0] + 6) % 12]) goong[j].sinsal += " 日馬";
                if (goong[j].hongNum[1] == hong[(samhab[i, 1] + 11) % 12]) goong[j].sinsal += " 日亡";
                if (goong[j].hongNum[1] == hong[(samhab[i, 2] + 1) % 12]) goong[j].sinsal += " 日劫";
            }
            goong[rr[(samhab[i, 0] + 1) % 12]].sinsal += " 日年";
            goong[rr[(samhab[i, 2]) % 12]].sinsal += " 日華";
        }// 신살

        public void setKyukkuk(Goong[] goong, int[,] sjGanzi)
        {
            int i;
            for (i = 0; i < 9; i++)
            {
                if (i != 4)
                {
                    if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "靑龍合靈";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "靑龍回首";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "靑龍耀明";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "伏吟峻山";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "貴人入獄";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "直符飛宮";//;"吉事不吉";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "靑龍折足";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "山明水秀"; //隻帆漂洋";
                    else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "岩石浸蝕"; // "靑龍華蓋";

                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "伏吟雜草"; //"日奇伏吟";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "三奇順遂";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "三奇相佐";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "鮮花名甁";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "三奇得使 以一當十";// "日奇入霧";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "夫妻懷私"; // "日奇被刑";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "靑龍逃走";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "荷葉蓮花"; // 日奇入地";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "祿野朝露"; // "遁跡修道"; //  "華蓋逢星";

                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "日月並行";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "有勇無謨"; // "月奇孛師";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "三奇順遂";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "三奇得使 飛鳥跌穴";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "大地普照";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "滎入太白";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "謨事就成";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "是非頗多"; //  "火入天羅";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "黑雲遮日"; // "華蓋孛師";

                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "燒田種作";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "星隨月轉";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "兩火成炎";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐有火";// "平安壽福" "靑龍轉光";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "火入句陳";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "火煉眞金";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "朱雀入獄";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "三奇得使 五神互合";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "朱雀投江";

                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "柔情密意";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "火孛地戶";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "朱雀入墓"; //"先曲後直"
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "犬遇靑龍";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "百事不遂";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "活鬼廛身"; // "利格返名";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "濕泥汚玉"; // "遊魂入墓";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "反吟濁水"; // "地網高張"
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "好事必止";

                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "太白逢星";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "太白入熒";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "亨亨之格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐無火";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "刑格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "戰格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "車絶馬死";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "耗散小格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "反吟大格";

                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "白虎猖狂";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "干合孛師";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "獄神得奇";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "妄動禍殃"; // "有頭無尾";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "奴僕背主"; // "入獄自刑";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "白虎出力";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "白虎兩立"; // "伏吟相剋"
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "寒塘月影"; //表實內虛
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "誤入天網";

                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "逐水桃花";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "日落西海";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "干合星奇";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "小蛇化龍";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "凶蛇入獄";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "太白擒蛇";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "淘洗珠玉"; // "騰蛇相戰";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "伏吟地網";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "幼女奸淫";

                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "梨花春雨";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "日出霧散"; // "常人平安" "華蓋孛師";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "螣蛇妖嬌";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "困時得助"; // "天乙會合";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "音信皆阻"; // "華蓋地戶";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "太白入網";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "網蓋天牢"; // "陽衰陰盛";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "復見騰蛇";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "伏吟天羅";

                    //길격
                    if (i == 2 && goong[i].yooksam[0] == 8) goong[i].kyukkuk += " 乙奇陞殿";
                    if (i == 8 && goong[i].yooksam[0] == 7) goong[i].kyukkuk += " 丙奇陞殿";
                    if (i == 6 && goong[i].yooksam[0] == 6) goong[i].kyukkuk += " 丁奇陞殿";

                    if (goong[i].yooksam[0] == 8 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 乙奇上吉門";
                    if (goong[i].yooksam[0] == 7 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 丙奇上吉門";
                    if (goong[i].yooksam[0] == 6 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 丁奇上吉門";

                    if (goong[i].yooksam[0] == 6 && goong[i].eightmun == 0) goong[i].kyukkuk += " 玉女守門";

                    //흉격
                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == "直" && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";

                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";

                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (goong[i].eightjang == "直" && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";

                    if ((goong[i].b_gan[2] == true || goong[i].b_gan[3] == true) && goong[i].yooksam[0] == 2) goong[i].kyukkuk += " 伏干";
                    if ((goong[i].b_gan[2] == true || goong[i].b_gan[3] == true) && goong[i].yooksam[1] == 2) goong[i].kyukkuk += " 飛干";

                    if (i == 3 && (goong[i].yooksam[0] == 4 || goong[i].yooksam[0] == 5)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 8 && (goong[i].yooksam[0] == 3)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 1 && (goong[i].yooksam[0] == 1)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 2 && (goong[i].yooksam[0] == 0)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 7 && (goong[i].yooksam[0] == 2)) goong[i].kyukkuk += " 六儀擊形";

                    if (i == 5 && goong[i].yooksam[0] == 8) goong[i].kyukkuk += " 乙奇入墓";
                    if (i == 5 && goong[i].yooksam[0] == 7) goong[i].kyukkuk += " 丙奇入墓";
                    if (i == 7 && goong[i].yooksam[0] == 6) goong[i].kyukkuk += " 丁奇入墓";
                }
            }
        }// 격국

        public void setKyukkuk_old(Goong[] goong, int[,] sjGanzi)
        {
            int i;
            for (i = 0; i < 9; i++)
            {
                if (i != 4)
                {
                    //천간 지간 둘다 무 인 경우(무무)
                    if (sjGanzi[3, 0] == 1 && sjGanzi[3, 1] == 1)
                    {
                        if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "雙木盛林";
                    }
                    else if (sjGanzi[3, 0] == 1)
                    {
                        if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "禿山孤木";
                    }
                    else if (sjGanzi[3, 0] != 1 && sisunsoo != 0)
                    {
                        if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "伏吟峻山";
                    }
                    else if (sjGanzi[3, 0] != 1 && sisunsoo == 0)
                    {
                        if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "不平難伸";
                    }

                    // 천간 무, 지간 무인 경우 (무X, X무)
                    if ((sjGanzi[0, 0] == 1 && sjGanzi[0, 1] == 1) || (sjGanzi[1, 0] == 1 && sjGanzi[1, 1] == 1) || (sjGanzi[2, 0] == 1 && sjGanzi[2, 1] == 1) || (sjGanzi[3, 0] == 1 && sjGanzi[3, 1] == 1))
                    {

                        if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "永不發芽";
                        else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "官人失僞 商覆失位";
                        else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "懷才不運 越下松影";
                        else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "內外危險 速決爲主";
                        else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "困時得助";
                        else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐有火";// "平安壽福" "靑龍轉光";
                        else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "飛鳥跌穴";
                        else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "錦上添花";


                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "貴人提拔";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "靑龍回首";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "一拍卽合";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "共協互惠";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "直符飛宮";//"飛宮斫伐";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "木棍碎瓦";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "有去無歸";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "樹根露水";
                    }
                    else
                    {
                        if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "犬遇靑龍";
                        else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐無火";
                        else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "官司破財";
                        else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "男人發達 女座金與";
                        else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "天乙會合 財喜婚姻";
                        else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐有火";// "平安壽福" "靑龍轉光";
                        else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "丙奇得使 有利有益";
                        else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "鮮花名甁";

                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "靑龍合靈";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "日出東山";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "以小勝多";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "勿以類聚";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "直符飛宮";//"助紂爲虐";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "十事九敗";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "山明水秀";
                        else if (goong[i].yooksam[0] == 0 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "巖石浸蝕";
                    }

                    //else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "錦上添花";
                    if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "伏吟雜草";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "遷官進職";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "文書社吉";
                    //else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "鮮花名甁";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "以一當十";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "夫妻懷私"; // "爭訟財産";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "靑龍逃走";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "男遊天下";
                    else if (goong[i].yooksam[0] == 8 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "遁跡修道";

                    //else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "飛鳥跌穴";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "公私皆吉";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "破耗損失";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "貴人吉利";
                    //else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "丙奇得使";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "大地普照";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "滎入太白";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "謨事就成";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "是非頗多";
                    else if (goong[i].yooksam[0] == 7 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "黑雲遮日";

                    //else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐有火";// "平安壽福" "靑龍轉光";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "可官進祿";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "樂極生悲";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "文書卽至";
                    // else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐有火";// "平安壽福" "靑龍轉光";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "奸私仇寃";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "火煉眞金";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "燒毁珠玉";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "丁奇得使";
                    else if (goong[i].yooksam[0] == 6 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "朱雀投江";



                    //else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "永不發芽";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "柔情密意";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "陽人相害";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "朱雀入墓"; //"先曲後直"
                    //else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "犬遇靑龍";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "百事不遂";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "活鬼廛身";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "懷恨千秋";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "反吟濁水";
                    else if (goong[i].yooksam[0] == 1 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "好事必止";

                    //else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "官人失僞";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "退吉進凶";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "太白入熒";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "門吉卽吉";
                    //else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "有爐無火";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "刑格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "戰格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "車絶馬死";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "耗散小格";
                    else if (goong[i].yooksam[0] == 2 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "反吟大格";

                    //else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "懷才不運";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "白虎猖狂";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "雖有大利";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "一喜一悲";
                    //else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "官司破財";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "奴僕背主";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "白虎出力";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "白虎兩立";
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "寒塘月影"; //表實內虛
                    else if (goong[i].yooksam[0] == 3 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "誤入天網";

                    //else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "內外危險";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "逐水桃花";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "會光返照";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "文書順理";
                    //else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "南人發達";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "反吟泥漿";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "螣蛇相戰";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "淘洗珠玉";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "地羅占蔣";
                    else if (goong[i].yooksam[0] == 4 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "反福爲禍";

                    //else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "困時得助";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 8) goong[i].kyukkuk += "梨花春雨";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 7) goong[i].kyukkuk += "貴人祿位";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 6) goong[i].kyukkuk += "螣蛇妖嬌";
                    //else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 0) goong[i].kyukkuk += "困時得助"; // "天乙會合";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 1) goong[i].kyukkuk += "音信皆阻";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 2) goong[i].kyukkuk += "不能成鋼";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 3) goong[i].kyukkuk += "占病占訟";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 4) goong[i].kyukkuk += "急進誤事";
                    else if (goong[i].yooksam[0] == 5 && goong[i].yooksam[1] == 5) goong[i].kyukkuk += "伏吟天羅";

                    //길격
                    if (i == 2 && goong[i].yooksam[0] == 8) goong[i].kyukkuk += " 乙奇陞殿";
                    if (i == 8 && goong[i].yooksam[0] == 7) goong[i].kyukkuk += " 丙奇陞殿";
                    if (i == 6 && goong[i].yooksam[0] == 6) goong[i].kyukkuk += " 丁奇陞殿";

                    if (goong[i].yooksam[0] == 8 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 乙奇上吉門";
                    if (goong[i].yooksam[0] == 7 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 丙奇上吉門";
                    if (goong[i].yooksam[0] == 6 && (goong[i].eightmun == 6 || goong[i].eightmun == 7 || goong[i].eightmun == 0)) goong[i].kyukkuk += " 丁奇上吉門";

                    if (goong[i].yooksam[0] == 6 && goong[i].eightmun == 0) goong[i].kyukkuk += " 玉女守門";

                    //흉격
                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";
                    else if (toYookSam(goong[i].yooksam[1]) == "直" && (goong[i].yooksam[0] == 7 || goong[i].yooksam[1] == 7)) goong[i].kyukkuk += " 悖亂";

                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 5 || goong[i].yooksam[1] == 5)) goong[i].kyukkuk += " 天網四張";

                    if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[0, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[1, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[2, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (toYookSam(goong[i].yooksam[1]) == toGan(sjGanzi[3, 0]) && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";
                    else if (goong[i].eightjang == "直" && (goong[i].yooksam[0] == 4 || goong[i].yooksam[1] == 4)) goong[i].kyukkuk += " 地網遮蔽";

                    if ((goong[i].b_gan[2] == true || goong[i].b_gan[3] == true) && goong[i].yooksam[0] == 2) goong[i].kyukkuk += " 伏干";
                    if ((goong[i].b_gan[2] == true || goong[i].b_gan[3] == true) && goong[i].yooksam[1] == 2) goong[i].kyukkuk += " 飛干";

                    if (i == 3 && (goong[i].yooksam[0] == 4 || goong[i].yooksam[0] == 5)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 8 && (goong[i].yooksam[0] == 3)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 1 && (goong[i].yooksam[0] == 1)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 2 && (goong[i].yooksam[0] == 0)) goong[i].kyukkuk += " 六儀擊形";
                    else if (i == 7 && (goong[i].yooksam[0] == 2)) goong[i].kyukkuk += " 六儀擊形";

                    if (i == 5 && goong[i].yooksam[0] == 8) goong[i].kyukkuk += " 乙奇入墓";
                    if (i == 5 && goong[i].yooksam[0] == 7) goong[i].kyukkuk += " 丙奇入墓";
                    if (i == 7 && goong[i].yooksam[0] == 6) goong[i].kyukkuk += " 丁奇入墓";
                }
            }
        }// 격국 예전 버전

        public string setParkJeHwaUi(int i)
        {
            if (getPakjehwaeui(goong[i].eightmun, i) == 1) return "和";
            else if (getPakjehwaeui(goong[i].eightmun, i) == 2) return "義";
            else if (getPakjehwaeui(goong[i].eightmun, i) == 3) return "迫";
            else if (getPakjehwaeui(goong[i].eightmun, i) == 4) return "制";
            else return "";
        }// 격국 신버전

        public void setIsabangui(Goong[] goong, int[,] sjGanzi)
        {

        }//이사방위

        public string setBatangguk(Goong[] goong)
        {
            if (goong[4].hongNum[0] == 3 && goong[4].hongNum[1] == 5) return "戰局";
            else if (goong[4].hongNum[0] == 2 && goong[4].hongNum[1] == 5) return "沖局";
            else if (goong[4].hongNum[0] == 8 && goong[4].hongNum[1] == 5) return "沖局";
            else if (goong[4].hongNum[0] == 7 && goong[4].hongNum[1] == 5) return "怨嗔局";
            else if (goong[4].hongNum[0] == 5 && goong[4].hongNum[1] == 5) return "刑破害局";
            else if ((goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 4 || (goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 9 || (goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 0)
                return "和局";
            else if ((goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 1 || (goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 3 || (goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 6)
                return "戰局";
            else if ((goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 2 || (goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 8)
                return "沖局";
            else if ((goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 7)
                return "怨嗔局";
            else if ((goong[4].hongNum[0] + goong[4].hongNum[1]) % 10 == 5)
                return "刑破害局";
            else return "";
        }   // 바탕국

        public void setTaeulGusung(Goong[] goong, int[,] sjGanzi, bool direction)
        {
            int[] plus = { 7, 8, 0, 1, 2, 3 };
            int[] minus = { 1, 0, 8, 7, 6, 5 };

            int i, j, k, l;
            i = sjGanzi[2, 1];  //일지
            j = sjGanzi[2, 0];  //일간

            if (i - j == 0) k = 0;
            else if ((i - j + 12) % 12 == 10) k = 1;
            else if ((i - j + 12) % 12 == 8) k = 2;
            else if ((i - j + 12) % 12 == 6) k = 3;
            else if ((i - j + 12) % 12 == 4) k = 4;
            else k = 5;

            if (direction)
            {
                for (l = 0; l < 9; l++)
                {
                    goong[(plus[k] + (j - 1) + l) % 9].taeulgusung = l;
                }
            }
            else
            {
                for (l = 0; l < 9; l++)
                {
                    goong[(minus[k] - (j - 1) - l + 18) % 9].taeulgusung = l;
                }
            }
        }

        private void Yunyun()  //유년계거 출력
        {
            String[,] day = { { "甲子", "乙丑", "丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉" },
                        { "甲戌", "乙亥", "丙子", "丁丑", "戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未" },
                        { "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑", "庚寅", "辛卯", "壬辰", "癸巳" },
                        { "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑", "壬寅", "癸卯" },
                        { "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子", "癸丑" },
                        { "甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥" } };
            int i, j, start, age;
            string temp;
            int[] t_age = { 1924, 1984, 2044, 2104 };

            temp = toGan(sjGanzi[0, 0]) + toZi(sjGanzi[0, 1]);
            for (i = 0; i < 60 && day[i / 10, i % 10] != temp; i++) ;
            start = ((i / 10) + (i % 10)) % 9;
            //textBox5.AppendText(toYookSam(start));
            for (i = 0; i < 9 && goong[i].yooksam[1] != start; i++) ;
            start = i;

            for (i = 0; i < 4; i++) t_age[i] -= solar_dt.Year;
            for (i = 0, j = 0; i < 4; i++)
            {
                if (t_age[i] > 0 && t_age[i] < 120)
                {
                    t_age[j] = t_age[i];
                    j += 1;
                }
            }

            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if (direction)
                    {
                        String name = "richTextBox" + ((j + start) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = i * 9 + j % 9 + 1;
                        if (i != 0) c.Text += ", ";
                        c.Text += age.ToString();
                        if (age >= t_age[0]) goto EXIT1;
                    }
                    else
                    {
                        String name = "richTextBox" + ((start + 9 - j) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = i * 9 + j % 9 + 1;
                        if (i != 0) c.Text += ", ";
                        c.Text += age.ToString();
                        if (age >= t_age[0]) goto EXIT1;
                    }
                }
            }
        EXIT1:
            for (i = 0; i < 9 && goong[i].yooksam[1] != 0; i++) ;
            start = i;
            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if (direction)
                    {
                        String name = "richTextBox" + ((j + start) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = t_age[0] + i * 9 + j % 9 + 1;
                        c.Text += ", " + age.ToString();
                        if (age > 90) goto EXIT;
                        if (age >= t_age[1] + 1) goto EXIT2;
                    }
                    else
                    {
                        String name = "richTextBox" + ((start + 9 - j) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = t_age[0] + i * 9 + j % 9 + 1;
                        c.Text += ", " + age.ToString();
                        if (age > 90) goto EXIT;
                        if (age >= t_age[1] + 1) goto EXIT2;
                    }
                }
            }
        EXIT2:
            for (i = 0; i < 9 && goong[i].yooksam[1] != 0; i++) ;
            start = i;
            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    if (direction)
                    {
                        String name = "richTextBox" + ((j + start) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = t_age[1] + i * 9 + j % 9 + 1;
                        if (age > 90) goto EXIT;
                        c.Text += ", " + age.ToString();
                    }

                    else
                    {
                        String name = "richTextBox" + ((start + 9 - j) % 9 + 1);
                        Control ct = this.Controls[name];
                        RichTextBox c = ct as RichTextBox;
                        age = t_age[1] + i * 9 + j % 9 + 1;
                        if (age > 90) goto EXIT;
                        c.Text += ", " + age.ToString();
                    }
                }
            }
        EXIT:;
        }

        public void AlignText(RichTextBox c)
        {
            string[] Line = c.Text.Split(new char[] { '\n' });
            int start = 0;

            c.Select(start, 6);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 9, FontStyle.Regular);

            string[] subStr1 = Line[0].Split(' ');
            int l = 0;
            for (int i = 0; i < subStr1.Length; i++)
            {
                if (subStr1[i] == "日支" || subStr1[i] == "月支" || subStr1[i] == "時支" || subStr1[i] == "年支")
                {
                    c.Select(l, subStr1[i].Length);
                    c.SelectionBackColor = Color.LightSkyBlue;
                    c.SelectionFont = new Font(c.SelectionFont.FontFamily, 12, FontStyle.Regular);
                }
                else if (subStr1[i] == "日干" || subStr1[i] == "月干" || subStr1[i] == "時干" || subStr1[i] == "年干")
                {
                    c.Select(l, subStr1[i].Length);
                    c.SelectionBackColor = Color.LightGreen;
                    c.SelectionFont = new Font(c.SelectionFont.FontFamily, 12, FontStyle.Regular);
                }
                else
                {
                    c.Select(l, subStr1[i].Length);
                    c.SelectionFont = new Font(c.SelectionFont.FontFamily, 12, FontStyle.Regular);
                }
                l += subStr1[i].Length + 1;
            }
            start += Line[0].Length + 1;

            c.Select(7, Line[0].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 12, FontStyle.Regular);
            //start += Line[0].Length + 1;

            c.Select(start, Line[1].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 12, FontStyle.Regular);
            c.SelectionAlignment = HorizontalAlignment.Right;
            start += Line[1].Length + 1;

            c.Select(start, Line[2].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 9, FontStyle.Regular);
            start += Line[2].Length + 1;

            string[] subStr2 = Line[3].Split(' ');
            c.Select(start, subStr2[0].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 11, FontStyle.Regular);
            c.SelectionAlignment = HorizontalAlignment.Right;
            //start += Line[3].Length + 1;

            c.Select(start + subStr2[0].Length, Line[3].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 13, FontStyle.Regular);
            c.SelectionAlignment = HorizontalAlignment.Right;
            start += Line[3].Length + 1;

            c.Select(start - 15, 1);
            //c.SelectionColor = Color.Red;
            c.SelectionFont = new Font("맑은 고딕", 13, FontStyle.Bold);

            c.Select(start, Line[4].Length - 2);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 13, FontStyle.Regular);
            c.SelectionAlignment = HorizontalAlignment.Right;
            start += Line[4].Length + 1;

            c.Select(start - 15, 1);
            //c.SelectionColor = Color.Red;
            c.SelectionFont = new Font("맑은 고딕", 13, FontStyle.Bold);

            if (Line[4].Substring(Line[4].Length - 2, 1) == "世")
            {
                c.Select(start - 3, 2);
                c.SelectionColor = Color.Red;
                c.SelectionFont = new Font(c.SelectionFont.FontFamily, 13, FontStyle.Bold);
            }
            else if (Line[4].Substring(Line[4].Length - 8, 1) == "世")
            {
                c.Select(start - 9, 2);
                c.SelectionColor = Color.Red;
                c.SelectionFont = new Font(c.SelectionFont.FontFamily, 13, FontStyle.Bold);
            }
            else
            {
                c.Select(start - 3, 2);
                c.SelectionFont = new Font(c.SelectionFont.FontFamily, 13, FontStyle.Regular);
            }

            c.Select(start, Line[5].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 9, FontStyle.Regular);
            start += Line[5].Length + 1;

            c.Select(start, Line[6].Length);
            c.SelectionFont = new Font(c.SelectionFont.FontFamily, 11, FontStyle.Regular);
            c.SelectionAlignment = HorizontalAlignment.Right;
            start += Line[6].Length + 1;

        }       // 궁 출력결과 위치 폰트 조절

        public void showGoongLabelText(Goong[] goong, int t1, int t2)  //9궁에 결과 출력
        {
            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                c.BackColor = Color.White;

                //1줄  숫자, 속성, 사지, 사간, 천을, 천마, 일록
                c.Text = " " + (i + 1) + " (" + toOhaeng_1(getohaeng(i + 1, 3)) + ")";
                if (i == 4) c.BackColor = Color.Wheat;
                else
                {
                    if (goong[i].b_dong[0] == true) { c.Text += " 年支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[1] == true) { c.Text += " 月支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[2] == true) { c.Text += " 日支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[3] == true) { c.Text += " 時支"; c.BackColor = Color.LightGray; }
                }
                c.AppendText(toFourgan(i));
                c.AppendText(toCheonMaRok(goong[i].cheoneul, goong[i].cheonma, goong[i].ilrok) + Environment.NewLine);

                //2줄  신살
                c.SelectionAlignment = HorizontalAlignment.Right;
                c.AppendText(goong[i].sinsal + Environment.NewLine);

                //3줄 홍국수강약
                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[0]) + Environment.NewLine);

                //4줄 팔문, 천반 홍국수(강약), 천반천간, 구성, 유년, 육신 
                if (i == 4) c.AppendText(setBatangguk(goong));
                c.AppendText(setParkJeHwaUi(i) + " " + to8Mun(goong[i].eightmun) + " " + toNum(goong[i].hongNum[0]) + " " + toYookSam(goong[i].yooksam[0]) + " " + toGooSung(goong[i].goosung) + " " + goong[i].yoo_age[0] + "~");
                if (goong[i].hongNum[0] == 10) c.AppendText((goong[i].yoo_age[0] + t1 - 1).ToString());
                else c.AppendText((goong[i].yoo_age[0] + goong[i].hongNum[0] - 1).ToString());
                c.Text += " " + toSixSin(goong[i].six_sin[0]);
                c.Text += "ㅤ";
                c.Text += "\n";

                // 5줄 팔괘, 지반 홍국수(강약), 천간지반, 팔장, 유년, 육신 
                string fmt = "00.##";
                c.Text += " " + to8Goe(goong[i].eightgoe) + " " + toNum(goong[i].hongNum[1]) + " " + toYookSam(goong[i].yooksam[1]) + " " + goong[i].eightjang + " " + (goong[i].yoo_age[1]).ToString(fmt) + "~";
                if (goong[i].hongNum[1] == 10) c.Text += (goong[i].yoo_age[1] + t2 - 1).ToString(fmt);
                else c.Text += (goong[i].yoo_age[1] + goong[i].hongNum[1] - 1).ToString(fmt);
                c.Text += " " + toSixSin(goong[i].six_sin[1]);
                c.Text += "ㅤ";
                c.Text += "\n";

                //6줄 홍국수강약
                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[1]) + "          " + toTaeulGusung(goong[i].taeulgusung) + Environment.NewLine);

                //7줄 격국
                c.AppendText(goong[i].kyukkuk + Environment.NewLine);

                //8쭐 문왕, 공망, 운성, 유년
                c.Text += " (" + toMunWang(i) + ") " + goong[i].gongmang + " " + goong[i].eunsung + " ";

            }
            label19.Text = toNum(goong[4].hongNum[0]);
            label20.Text = toNum(goong[4].hongNum[1]);
            //Yunyun();

            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                AlignText(c);
            }
        }

        public void showGoongLabelTextSaJuBlank(Goong[] goong, int t1, int t2)  //9궁에 결과 출력
        {
            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                c.BackColor = Color.White;

                //1줄  숫자, 속성, 사지, 사간
                c.Text = " " + (i + 1) + " (" + toOhaeng_1(getohaeng(i + 1, 3)) + ")";
                if (i == 4) c.BackColor = Color.Wheat;
                else
                {
                    if (goong[i].b_dong[0] == true) { c.Text += " 年支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[1] == true) { c.Text += " 月支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[2] == true) { c.Text += " 日支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[3] == true) { c.Text += " 時支"; c.BackColor = Color.LightGray; }
                }
                c.AppendText(Environment.NewLine);
                //c.AppendText(toCheonMaRok(goong[i].cheoneul, goong[i].cheonma, goong[i].ilrok) + Environment.NewLine);

                //2줄  신살
                //c.SelectionAlignment = HorizontalAlignment.Right;
                c.AppendText(Environment.NewLine);

                //3줄 홍국수강약
                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[0]) + Environment.NewLine);

                //4줄 팔문, 천반 홍국수(강약), 천반천간, 구성, 유년, 육신 
                //if (i == 4) c.AppendText(setBatangguk(goong));
                c.AppendText(toNum(goong[i].hongNum[0]) + "     " + goong[i].yoo_age[0] + "~");
                if (goong[i].hongNum[0] == 10) c.AppendText((goong[i].yoo_age[0] + t1 - 1).ToString());
                else c.AppendText((goong[i].yoo_age[0] + goong[i].hongNum[0] - 1).ToString());
                c.Text += " " + toSixSin(goong[i].six_sin[0]);
                c.Text += "ㅤ";
                c.Text += "\n";

                // 5줄 팔괘, 지반 홍국수(강약), 천간지반, 팔장, 유년, 육신 
                string fmt = "00.##";
                c.Text += " " + toNum(goong[i].hongNum[1]) + "     " + (goong[i].yoo_age[1]).ToString(fmt) + "~";
                if (goong[i].hongNum[1] == 10) c.Text += (goong[i].yoo_age[1] + t2 - 1).ToString(fmt);
                else c.Text += (goong[i].yoo_age[1] + goong[i].hongNum[1] - 1).ToString(fmt);
                c.Text += " " + toSixSin(goong[i].six_sin[1]);
                c.Text += "ㅤ";
                c.Text += "\n";

                //6줄 홍국수강약
                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[1]) + "          " + Environment.NewLine);

                //7줄 격국
                c.AppendText(Environment.NewLine);

                //8쭐 문왕, 공망, 운성, 유년
                //c.Text += " (" + toMunWang(i) + ") " + goong[i].gongmang + " " + goong[i].eunsung + " ";

            }
            label19.Text = toNum(goong[4].hongNum[0]);
            label20.Text = toNum(goong[4].hongNum[1]);
            //Yunyun();

            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                AlignText(c);
            }
        }

        public void showGoongLabelText_Sinsoo(Goong[] goong, int t1, int t2)  //9궁에 결과 출력
        {
            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                c.BackColor = Color.White;

                //1줄  숫자, 속성, 사지, 사간, 천을, 천마, 일록
                c.Text = " " + (i + 1) + " (" + toOhaeng_1(getohaeng(i + 1, 3)) + ")";
                if (i == 4) c.BackColor = Color.Wheat;
                else
                {
                    if (goong[i].b_dong[0] == true) { c.Text += " 年支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[1] == true) { c.Text += " 月支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[2] == true) { c.Text += " 日支"; c.BackColor = Color.LightGray; }
                    if (goong[i].b_dong[3] == true) { c.Text += " 時支"; c.BackColor = Color.LightGray; }
                }
                c.AppendText(toFourgan(i));
                c.AppendText(toCheonMaRok(goong[i].cheoneul, goong[i].cheonma, goong[i].ilrok) + Environment.NewLine);

                //2줄  신살
                c.SelectionAlignment = HorizontalAlignment.Right;
                c.AppendText(bokgankyuk(i) + " " + goong[i].sinsal + " " + goong[i].josang + Environment.NewLine);

                //3줄 홍국수강약
                if (i == Hyear - 1)
                    c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[0]) + "     行年宮" + Environment.NewLine);
                else
                    c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[0]) + Environment.NewLine);
                //4줄 팔문, 천반 홍국수(강약), 천반천간, 구성, 유년, 육신 
                if (i == 4) c.AppendText(setBatangguk(goong));
                c.AppendText(setParkJeHwaUi(i) + " " + to8Mun(goong[i].eightmun) + " " + toNum(goong[i].hongNum[0]) + " " + toYookSam(goong[i].yooksam[0]) + " " + toGooSung(goong[i].goosung));
                c.Text += " " + toSixSin(goong[i].six_sin[0]);
                c.Text += "      ㅤ";
                c.Text += "\n";

                // 5줄 팔괘, 지반 홍국수(강약), 천간지반, 팔장, 유년, 육신 
                string fmt = "00.##";
                c.Text += " " + to8Goe(goong[i].eightgoe) + " " + toNum(goong[i].hongNum[1]) + " " + toYookSam(goong[i].yooksam[1]) + " " + goong[i].eightjang;
                c.Text += " " + toSixSin(goong[i].six_sin[1]);
                c.Text += "      ㅤ";
                c.Text += "\n";

                //6줄 홍국수강약
                //                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[1]) + Environment.NewLine);
                c.AppendText("               " + toHongNumLvl(goong[i].hongNumlvl[1]) + "          " + toTaeulGusung(goong[i].taeulgusung) + Environment.NewLine);


                //7줄 격국
                c.AppendText(goong[i].kyukkuk + Environment.NewLine);

                //8쭐 문왕, 공망, 운성, 유년
                c.Text += " (" + toMunWang(i) + ") " + goong[i].gongmang + " " + goong[i].eunsung + " ";

            }
            label19.Text = toNum(goong[4].hongNum[0]);
            label20.Text = toNum(goong[4].hongNum[1]);
            //Yunyun();

            for (int i = 0; i < 9; i++)
            {
                String name = "richTextBox" + (i + 1);
                Control ct = this.Controls[name];
                RichTextBox c = ct as RichTextBox;
                AlignText(c);
            }
        }

        public void showTongGido(Goong[] goong)
        {
            pictureBox3.Visible = false;
            //label88.Visible = false;
            label111.Visible = false;
            label110.Visible = false;
            label109.Visible = false;
            label108.Visible = false;
            label107.Visible = false;
            label106.Visible = false;
            label105.Visible = false;
            label104.Visible = false;
            label103.Visible = false;
            label112.Visible = false;
            label86.Visible = false;
            label85.Visible = false;
            label84.Visible = false;
            label83.Visible = false;
            label82.Visible = false;
            label81.Visible = false;
            label80.Visible = false;
            label79.Visible = false;
            label78.Visible = false;
            label87.Visible = false;
            label102.Visible = false;
            label101.Visible = false;
            label100.Visible = false;
            label90.Visible = false;
            label89.Visible = false;

            pictureBox1.Visible = true;
            label41.Visible = true;
            label42.Visible = true;
            label43.Visible = true;
            label44.Visible = true;
            label45.Visible = true;
            label46.Visible = true;
            label47.Visible = true;
            label48.Visible = true;
            label49.Visible = true;
            label50.Visible = true;
            label61.Visible = true;
            label62.Visible = true;
            label63.Visible = true;
            label64.Visible = true;
            label65.Visible = true;
            label66.Visible = true;
            label67.Visible = true;
            label68.Visible = true;
            label69.Visible = true;
            label70.Visible = true;
            label51.Visible = true;
            label59.Visible = true;
            label21.Visible = true;
            label23.Visible = true;
            label24.Visible = true;
            label25.Visible = true;
            label26.Visible = true;
            label27.Visible = true;
            label57.Visible = true;
            label58.Visible = true;

            bool[] idx = { false, false, false, false, false, false, false, false, false, false };
            int[] t = { 4, 9, 1, 6, 3, 8, 2, 7, 5, 10 };
            label41.Text = "";
            label42.Text = "";
            label43.Text = "";
            label44.Text = "";
            label45.Text = "";
            label46.Text = "";
            label47.Text = "";
            label48.Text = "";
            label49.Text = "";
            label50.Text = "";
            label61.Text = "";
            label62.Text = "";
            label63.Text = "";
            label64.Text = "";
            label65.Text = "";
            label66.Text = "";
            label67.Text = "";
            label68.Text = "";
            label69.Text = "";
            label70.Text = "";
            label59.Text = "";

            int i, j;
            string[,] labelname = { { "label41", "label43", "label45", "label47", "label49" }, { "label42", "label44", "label46", "label48", "label50" } };
            string[,] labelname2 = { {"label41", "label42", "label43", "label44", "label45", "label46", "label47", "label48", "label49", "label50" },
                                     {"label61", "label62", "label63", "label64", "label65", "label66", "label67", "label68", "label69", "label70" }};

            label41.BackColor = Color.FromArgb(255, 229, 229);
            label42.BackColor = Color.FromArgb(255, 229, 229);
            label43.BackColor = Color.FromArgb(255, 229, 229);
            label44.BackColor = Color.FromArgb(255, 229, 229);
            label45.BackColor = Color.FromArgb(255, 229, 229);
            label46.BackColor = Color.FromArgb(255, 229, 229);
            label47.BackColor = Color.FromArgb(255, 229, 229);
            label48.BackColor = Color.FromArgb(255, 229, 229);
            label49.BackColor = Color.FromArgb(255, 229, 229);
            label50.BackColor = Color.FromArgb(255, 229, 229);
            label61.BackColor = Color.FromArgb(255, 229, 229);
            label62.BackColor = Color.FromArgb(255, 229, 229);
            label63.BackColor = Color.FromArgb(255, 229, 229);
            label64.BackColor = Color.FromArgb(255, 229, 229);
            label65.BackColor = Color.FromArgb(255, 229, 229);
            label66.BackColor = Color.FromArgb(255, 229, 229);
            label67.BackColor = Color.FromArgb(255, 229, 229);
            label68.BackColor = Color.FromArgb(255, 229, 229);
            label69.BackColor = Color.FromArgb(255, 229, 229);
            label70.BackColor = Color.FromArgb(255, 229, 229);
            label51.BackColor = Color.FromArgb(255, 229, 229);

            // 숫자 표시
            for (i = 0; i < 9; i++)
            {
                if (i == 4) // "中"
                {
                    if (toSixSin(goong[i].six_sin[1]) == "兄") { idx[0] = true; label61.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "兄") { idx[1] = true; label42.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "孫") { idx[2] = true; label43.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "父") { idx[3] = true; label44.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "財") { idx[4] = true; label45.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "官" || toSixSin(goong[i].six_sin[0]) == "鬼") { idx[5] = true; label46.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "官" || toSixSin(goong[i].six_sin[1]) == "鬼") { idx[6] = true; label47.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "財") { idx[7] = true; label48.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "父") { idx[8] = true; label49.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "孫") { idx[9] = true; label50.Text = toNum(goong[i].hongNum[0]) + "中"; }
                }

                else
                {
                    if (toSixSin(goong[i].six_sin[1]) == "世")
                        label41.Text = toNum(goong[i].hongNum[1]) + "世";
                    label41.ForeColor = Color.Red;

                    if (toSixSin(goong[i].six_sin[1]) == "兄")
                    {
                        if (goong[i].b_dong[0] == true)
                            label61.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label61.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label61.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label61.Text = toNum(goong[i].hongNum[1]) + "時";
                    }

                    if (toSixSin(goong[i].six_sin[0]) == "兄")
                    {
                        if (idx[1] == false)
                        {
                            idx[1] = true;
                            if (goong[i].b_dong[0] == true)
                                label42.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label42.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label42.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label42.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[1] = false;
                        }
                        else if (label42.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label59.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label59.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label59.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label59.Text = toNum(goong[i].hongNum[0]) + "時";
                    }

                    if (toSixSin(goong[i].six_sin[1]) == "孫")
                    {
                        if (idx[2] == false)
                        {
                            idx[2] = true;
                            if (goong[i].b_dong[0] == true)
                                label43.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label43.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[3] == true)
                                label43.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[2] = false;
                        }
                        else if (label43.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label63.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label63.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[3] == true)
                            label63.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "父")
                    {
                        if (idx[3] == false)
                        {
                            idx[3] = true;
                            if (goong[i].b_dong[0] == true)
                                label44.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label44.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label44.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label44.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[3] = false;
                        }
                        else if (label44.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label64.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label64.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label64.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label64.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "財")
                    {
                        if (idx[4] == false)
                        {
                            idx[4] = true;
                            if (goong[i].b_dong[0] == true)
                                label45.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label45.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[3] == true)
                                label45.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[4] = false;
                        }
                        else if (label45.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label65.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label65.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[3] == true)
                            label65.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "官" || toSixSin(goong[i].six_sin[0]) == "鬼")
                    {
                        if (idx[5] == false)
                        {
                            idx[5] = true;
                            if (goong[i].b_dong[0] == true)
                                label46.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label46.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label46.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label46.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[5] = false;
                        }
                        else if (label46.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label66.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label66.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label66.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label66.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "官" || toSixSin(goong[i].six_sin[1]) == "鬼")
                    {
                        if (idx[6] == false)
                        {
                            idx[6] = true;

                            if (goong[i].b_dong[0] == true)
                                label47.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label47.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[3] == true)
                                label47.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[6] = false;
                        }
                        else if (label47.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label67.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label67.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[3] == true)
                            label67.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "財")
                    {
                        if (idx[7] == false)
                        {
                            idx[7] = true;

                            if (goong[i].b_dong[0] == true)
                                label48.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label48.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label48.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label48.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[7] = false;
                        }
                        else if (label48.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label68.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label68.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label68.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label68.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "父")
                    {
                        if (idx[8] == false)
                        {
                            idx[8] = true;

                            if (goong[i].b_dong[0] == true)
                                label49.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label49.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[3] == true)
                                label49.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[8] = false;
                        }
                        else if (label49.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label69.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label69.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[3] == true)
                            label69.Text = toNum(goong[i].hongNum[1]) + "時";

                    }
                    if (toSixSin(goong[i].six_sin[0]) == "孫")
                    {
                        if (idx[9] == false)
                        {
                            idx[9] = true;

                            if (goong[i].b_dong[0] == true)
                                label50.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label50.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label50.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label50.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[9] = false;
                        }
                        else if (label50.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label70.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label70.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label70.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label70.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                }
            }

            // 오행 설정
            for (i = 0; i < 9 && goong[i].b_dong[2] != true; i++) ;
            for (j = 0; j < 10 && goong[i].hongNum[1] != t[j]; j++) ;
            label51.Text = toOhaeng(j / 2);
            label52.Text = toOhaeng((j / 2 + 1) % 5);
            label53.Text = toOhaeng((j / 2 + 2) % 5);
            label54.Text = toOhaeng((j / 2 + 3) % 5);
            label55.Text = toOhaeng((j / 2 + 4) % 5);
            
            Color myColor;
            if (label51.Text == "金")
            {
                myColor = Color.FromArgb(229, 229, 229);
            }
            else if (label51.Text == "水")
            {
                myColor = Color.FromArgb(204, 204, 204);

            }
            else if (label51.Text == "木")
            {
                myColor = Color.FromArgb(178, 255, 178);
            }
            else if (label51.Text == "火")
            {
                myColor = Color.FromArgb(255, 203, 203);
            }
            else // 土
            {
                myColor = Color.FromArgb(255, 189, 101);
            }
            
            myBrush = new SolidBrush(myColor);

            label51.BackColor = myColor;
            label41.BackColor = myColor;
            label61.BackColor = myColor;
            label42.BackColor = myColor;
            label62.BackColor = myColor;
            label59.BackColor = myColor;

            //for (i = 0; i < 2; i++)
            //{
            //    for (j = 0; j < 5; j++)
            //    {
            //        Control ct1 = this.Controls[labelname[i, (j + 3) % 5]];
            //        Control ct2 = this.Controls[labelname[i, (j + 4) % 5]];
            //        Control ct3 = this.Controls[labelname[i, j]];
            //        Control ct4 = this.Controls[labelname[i, (j + 1) % 5]];
            //        Control ct5 = this.Controls[labelname[i, (j + 2) % 5]];
            //        if (ct1.Text.Length == 2) ct1.Text = ct1.Text.Substring(0, 1) + " " + ct1.Text.Substring(1, 1);
            //        if (ct2.Text.Length == 2) ct2.Text = ct2.Text.Substring(0, 1) + " " + ct2.Text.Substring(1, 1);
            //        if (ct3.Text.Length == 2) ct3.Text = ct3.Text.Substring(0, 1) + " " + ct3.Text.Substring(1, 1);
            //        if (ct4.Text.Length == 2) ct4.Text = ct4.Text.Substring(0, 1) + " " + ct4.Text.Substring(1, 1);
            //        if (ct5.Text.Length == 2) ct5.Text = ct5.Text.Substring(0, 1) + " " + ct5.Text.Substring(1, 1);
            //    }
            //}
            this.Refresh();
        }       //통기도 출력

        public void showTongGido_1(Goong[] goong)
        {
            pictureBox1.Visible = false;

            label41.Visible = false;
            label42.Visible = false;
            label43.Visible = false;
            label44.Visible = false;
            label45.Visible = false;
            label46.Visible = false;
            label47.Visible = false;
            label48.Visible = false;
            label49.Visible = false;
            label50.Visible = false;
            label61.Visible = false;
            label62.Visible = false;
            label63.Visible = false;
            label64.Visible = false;
            label65.Visible = false;
            label66.Visible = false;
            label67.Visible = false;
            label68.Visible = false;
            label69.Visible = false;
            label70.Visible = false;
            label51.Visible = false;
            label59.Visible = false;
            label21.Visible = false;
            label23.Visible = false;
            label24.Visible = false;
            label25.Visible = false;
            label26.Visible = false;
            label27.Visible = false;
            label57.Visible = false;
            label58.Visible = false;

            pictureBox3.Visible = true;
            pictureBox3.SendToBack();
            //label88.Visible = true;
            label111.Visible = true;
            label110.Visible = true;
            label109.Visible = true;
            label108.Visible = true;
            label107.Visible = true;
            label106.Visible = true;
            label105.Visible = true;
            label104.Visible = true;
            label103.Visible = true;
            label112.Visible = true;
            label86.Visible = true;
            label85.Visible = true;
            label84.Visible = true;
            label83.Visible = true;
            label82.Visible = true;
            label81.Visible = true;
            label80.Visible = true;
            label79.Visible = true;
            label78.Visible = true;
            label87.Visible = true;
            label102.Visible = true;
            label101.Visible = true;
            label100.Visible = true;
            label90.Visible = true;
            label89.Visible = true;

            label79.BackColor = Color.FromArgb(249, 237, 245);
            label81.BackColor = Color.FromArgb(249, 237, 245);
            label83.BackColor = Color.FromArgb(249, 237, 245);
            label85.BackColor = Color.FromArgb(249, 237, 245);
            label87.BackColor = Color.FromArgb(249, 237, 245);
            label112.BackColor = Color.FromArgb(249, 237, 245);
            label110.BackColor = Color.FromArgb(249, 237, 245);
            label108.BackColor = Color.FromArgb(249, 237, 245);
            label106.BackColor = Color.FromArgb(249, 237, 245);
            label104.BackColor = Color.FromArgb(249, 237, 245);

            label78.BackColor = Color.FromArgb(229, 229, 229);
            label80.BackColor = Color.FromArgb(229, 229, 229);
            label82.BackColor = Color.FromArgb(229, 229, 229);
            label84.BackColor = Color.FromArgb(229, 229, 229);
            label86.BackColor = Color.FromArgb(229, 229, 229);
            label111.BackColor = Color.FromArgb(229, 229, 229);
            label109.BackColor = Color.FromArgb(229, 229, 229);
            label107.BackColor = Color.FromArgb(229, 229, 229);
            label105.BackColor = Color.FromArgb(229, 229, 229);
            label103.BackColor = Color.FromArgb(229, 229, 229);

            label102.BackColor = Color.FromArgb(255, 255, 203);
            label101.BackColor = Color.FromArgb(255, 255, 203);
            label100.BackColor = Color.FromArgb(255, 255, 203);
            label90.BackColor = Color.FromArgb(255, 255, 203);
            label89.BackColor = Color.FromArgb(255, 255, 203);


            label86.ForeColor = Color.Black;
            label111.ForeColor = Color.Black;

            bool[] idx = { false, false, false, false, false, false, false, false, false, false };
            int[] t = { 4, 9, 1, 6, 3, 8, 2, 7, 5, 10 };
            label111.Text = "";
            label110.Text = "";
            label109.Text = "";
            label108.Text = "";
            label107.Text = "";
            label106.Text = "";
            label105.Text = "";
            label104.Text = "";
            label103.Text = "";
            label112.Text = "";
            label86.Text = "";
            label85.Text = "";
            label84.Text = "";
            label83.Text = "";
            label82.Text = "";
            label81.Text = "";
            label80.Text = "";
            label79.Text = "";
            label78.Text = "";
            label87.Text = "";
            int i, j;

            string[,] labelname = { { "label111", "label109", "label107", "label105", "label103" }, { "label110", "label108", "label106", "label104", "label112" } };
            string[,] labelname2 = { {"label111", "label110", "label109", "label108", "label107", "label106", "label105", "label104", "label103", "label112" },
                                     {"label86", "label85", "label84", "label83", "label82", "label81", "label80", "label79", "label78", "label87" }};

            // 숫자 표시
            for (i = 0; i < 9; i++)
            {
                if (i == 4) // "中"
                {
                    if (toSixSin(goong[i].six_sin[1]) == "兄") label86.Text = toNum(goong[i].hongNum[1]) + "中";
                    if (toSixSin(goong[i].six_sin[0]) == "兄") { idx[1] = true; label110.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "孫") { idx[2] = true; label109.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "孫") { idx[3] = true; label108.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "財") { idx[4] = true; label107.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "財") { idx[5] = true; label106.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "官" || toSixSin(goong[i].six_sin[1]) == "鬼") { idx[6] = true; label105.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "官" || toSixSin(goong[i].six_sin[0]) == "鬼") { idx[7] = true; label104.Text = toNum(goong[i].hongNum[0]) + "中"; }
                    if (toSixSin(goong[i].six_sin[1]) == "父") { idx[8] = true; label103.Text = toNum(goong[i].hongNum[1]) + "中"; }
                    if (toSixSin(goong[i].six_sin[0]) == "父") { idx[9] = true; label112.Text = toNum(goong[i].hongNum[0]) + "中"; }
                }

                else
                {
                    if (toSixSin(goong[i].six_sin[1]) == "世")
                    {
                        if (idx[0] == true)
                        {
                            label86.Text = toNum(goong[i].hongNum[1]) + "世";
                            label86.ForeColor = Color.Red;
                        }
                        else
                        {
                            label111.Text = toNum(goong[i].hongNum[1]) + "世";
                            label111.ForeColor = Color.Red;
                            idx[0] = true;
                        }
                    }

                    //        if (goong[i].b_dong[2] == true)
                    //    {
                    //        idx[0] = true; label111.Text = toNum(goong[i].hongNum[1]) + "世";
                    //        if (toSixSin(goong[i].six_sin[1]) == "兄") label86.Text = toNum(goong[i].hongNum[1]) + "日";
                    //        if (toSixSin(goong[i].six_sin[0]) == "兄") { idx[1] = true; label110.Text = toNum(goong[i].hongNum[0]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[1]) == "孫") { idx[2] = true; label109.Text = toNum(goong[i].hongNum[1]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[0]) == "孫") { idx[3] = true; label108.Text = toNum(goong[i].hongNum[0]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[1]) == "財") { idx[4] = true; label107.Text = toNum(goong[i].hongNum[1]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[0]) == "財") { idx[5] = true; label106.Text = toNum(goong[i].hongNum[0]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[1]) == "官" || toSixSin(goong[i].six_sin[1]) == "鬼") { idx[6] = true; label105.Text = toNum(goong[i].hongNum[1]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[0]) == "官" || toSixSin(goong[i].six_sin[0]) == "鬼") { idx[7] = true; label104.Text = toNum(goong[i].hongNum[0]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[1]) == "父") { idx[8] = true; label103.Text = toNum(goong[i].hongNum[1]) + "日"; }
                    //        if (toSixSin(goong[i].six_sin[0]) == "父") { idx[9] = true; label112.Text = toNum(goong[i].hongNum[0]) + "日"; }
                    //    }
                    //}// 일
                    //for (i = 0; i < 9; i++)
                    //{

                    //if (goong[i].b_dong[0] == true && goong[i].b_dong[2] != true)    // 세(년)
                    //{
                    if (toSixSin(goong[i].six_sin[1]) == "兄")
                    {
                        if (idx[0] == false)
                        {
                            idx[0] = true;
                            if (goong[i].b_dong[0] == true)
                                label111.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label111.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label111.Text = toNum(goong[i].hongNum[1]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label111.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[0] = false;
                        }
                        else if (label111.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label86.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label86.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label86.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label86.Text = toNum(goong[i].hongNum[1]) + "時";
                    }

                    if (toSixSin(goong[i].six_sin[0]) == "兄")
                    {
                        if (idx[1] == false)
                        {
                            idx[1] = true;
                            if (goong[i].b_dong[0] == true)
                                label110.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label110.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label110.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label110.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[1] = false;
                        }
                        else if (label110.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label85.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label85.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label85.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label85.Text = toNum(goong[i].hongNum[0]) + "時";
                    }

                    if (toSixSin(goong[i].six_sin[1]) == "孫")
                    {
                        if (idx[2] == false)
                        {
                            idx[2] = true;
                            if (goong[i].b_dong[0] == true)
                                label109.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label109.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label109.Text = toNum(goong[i].hongNum[1]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label109.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[2] = false;
                        }
                        else if (label109.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label84.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label84.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label84.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label84.Text = toNum(goong[i].hongNum[1]) + "時";
                    }

                    if (toSixSin(goong[i].six_sin[0]) == "孫")
                    {
                        if (idx[3] == false)
                        {
                            idx[3] = true;
                            if (goong[i].b_dong[0] == true)
                                label108.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label108.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label108.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label108.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[3] = false;
                        }
                        else if (label108.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label83.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label83.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label83.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label83.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "財")
                    {
                        if (idx[4] == false)
                        {
                            idx[4] = true;
                            if (goong[i].b_dong[0] == true)
                                label107.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label107.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label107.Text = toNum(goong[i].hongNum[1]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label107.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[4] = false;
                        }
                        else if (label107.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label82.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label82.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label82.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label82.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "財")
                    {
                        if (idx[5] == false)
                        {
                            idx[5] = true;
                            if (goong[i].b_dong[0] == true)
                                label106.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label106.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label106.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label106.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[5] = false;
                        }
                        else if (label106.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label81.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label81.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label81.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label81.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "官" || toSixSin(goong[i].six_sin[1]) == "鬼")
                    {
                        if (idx[6] == false)
                        {
                            idx[6] = true;
                            if (goong[i].b_dong[0] == true)
                                label105.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label105.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label105.Text = toNum(goong[i].hongNum[1]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label105.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[6] = false;
                        }
                        else if (label105.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label80.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label80.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label80.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label80.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "官" || toSixSin(goong[i].six_sin[0]) == "鬼")
                    {
                        if (idx[7] == false)
                        {
                            idx[7] = true;
                            if (goong[i].b_dong[0] == true)
                                label104.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label104.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label104.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label104.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[7] = false;
                        }
                        else if (label104.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label79.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label79.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label79.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label79.Text = toNum(goong[i].hongNum[0]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[1]) == "父")
                    {
                        if (idx[8] == false)
                        {
                            idx[8] = true;
                            if (goong[i].b_dong[0] == true)
                                label103.Text = toNum(goong[i].hongNum[1]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label103.Text = toNum(goong[i].hongNum[1]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label103.Text = toNum(goong[i].hongNum[1]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label103.Text = toNum(goong[i].hongNum[1]) + "時";
                            else idx[8] = false;
                        }
                        else if (label103.Text.Contains(toNum(goong[i].hongNum[1]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label78.Text = toNum(goong[i].hongNum[1]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label78.Text = toNum(goong[i].hongNum[1]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label78.Text = toNum(goong[i].hongNum[1]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label78.Text = toNum(goong[i].hongNum[1]) + "時";
                    }
                    if (toSixSin(goong[i].six_sin[0]) == "父")
                    {
                        if (idx[9] == false)
                        {
                            idx[9] = true;
                            if (goong[i].b_dong[0] == true)
                                label112.Text = toNum(goong[i].hongNum[0]) + "年";
                            else if (goong[i].b_dong[1] == true)
                                label112.Text = toNum(goong[i].hongNum[0]) + "月";
                            else if (goong[i].b_dong[2] == true)
                                label112.Text = toNum(goong[i].hongNum[0]) + "日";
                            else if (goong[i].b_dong[3] == true)
                                label112.Text = toNum(goong[i].hongNum[0]) + "時";
                            else idx[9] = false;
                        }
                        else if (label112.Text.Contains(toNum(goong[i].hongNum[0]))) ;
                        else if (goong[i].b_dong[0] == true)
                            label87.Text = toNum(goong[i].hongNum[0]) + "年";
                        else if (goong[i].b_dong[1] == true)
                            label87.Text = toNum(goong[i].hongNum[0]) + "月";
                        else if (goong[i].b_dong[2] == true)
                            label87.Text = toNum(goong[i].hongNum[0]) + "日";
                        else if (goong[i].b_dong[3] == true)
                            label87.Text = toNum(goong[i].hongNum[0]) + "時";

                    }
                }
            }// 년

            // 오행 설정
            for (i = 0; i < 9 && goong[i].b_dong[2] != true; i++) ;
            for (j = 0; j < 10 && goong[i].hongNum[1] != t[j]; j++) ;
            label102.Text = toOhaeng(j / 2);
            label101.Text = toOhaeng((j / 2 + 1) % 5);
            label100.Text = toOhaeng((j / 2 + 2) % 5);
            label90.Text = toOhaeng((j / 2 + 3) % 5);
            label89.Text = toOhaeng((j / 2 + 4) % 5);
        }

        public string getGanYooksin(int i, int j)
        {
            string text = "";

            int batang_ohaeng = (i - 1) / 2; // 0 목, 1 화, 2 토, 3 금, 4 수
            int temp_Ohaeng = (j - 1) / 2;

            if (batang_ohaeng == temp_Ohaeng)
            {
                if ((j % 2 == 1 && i % 2 == 1) || (j % 2 == 0 && i % 2 == 0)) text = "비견";
                else text = "겁재";
            }

            else if ((j % 2 == 1 && i % 2 == 1) || (j % 2 == 0 && i % 2 == 0)) // 음양이 같을때
            {
                if (temp_Ohaeng - batang_ohaeng == 1 || temp_Ohaeng - batang_ohaeng == -4) text = "식신";
                else if (batang_ohaeng - temp_Ohaeng == 1 || batang_ohaeng - temp_Ohaeng == -4) text = "편인";
                else if (temp_Ohaeng - batang_ohaeng == 2 || temp_Ohaeng - batang_ohaeng == -3) text = "편재";
                else text = "편관";
            }

            else //음양이 다를때
            {
                if (temp_Ohaeng - batang_ohaeng == 1 || temp_Ohaeng - batang_ohaeng == -4) text = "상관";
                else if (batang_ohaeng - temp_Ohaeng == 1 || batang_ohaeng - temp_Ohaeng == -4) text = "정인";
                else if (temp_Ohaeng - batang_ohaeng == 2 || temp_Ohaeng - batang_ohaeng == -3) text = "정재";
                else text = "정관";
            }
            return text;
        }

        public string getZiYooksin(int i, int j)
        {
            string text = "";

            int batang_ohaeng = (i - 1) / 2; // 0 목, 1 화, 2 토, 3 금, 4 수
            int temp_Ohaeng, b;

            if (j == 3 || j == 4) temp_Ohaeng = 0;
            else if (j == 6 || j == 7) temp_Ohaeng = 1;
            else if (j == 5 || j == 11 || j == 2 || j == 8) temp_Ohaeng = 2;
            else if (j == 9 || j == 10) temp_Ohaeng = 3;
            else temp_Ohaeng = 4; //12 , 1

            if (j == 3 || j == 6 || j == 5 || j == 11 || j == 9 || j == 12) b = 1;
            else b = 0;

            if (batang_ohaeng == temp_Ohaeng)
            {
                if ((b == 1 && i % 2 == 1) || (b == 0 && i % 2 == 0)) text = "비견";
                else text = "겁재";
            }

            else if ((b == 1 && i % 2 == 1) || (b == 0 && i % 2 == 0)) // 음양이 같을때
            {
                if (temp_Ohaeng - batang_ohaeng == 1 || temp_Ohaeng - batang_ohaeng == -4) text = "식신";
                else if (batang_ohaeng - temp_Ohaeng == 1 || batang_ohaeng - temp_Ohaeng == -4) text = "편인";
                else if (temp_Ohaeng - batang_ohaeng == 2 || temp_Ohaeng - batang_ohaeng == -3) text = "편재";
                else text = "편관";
            }

            else //음양이 다를때
            {
                if (temp_Ohaeng - batang_ohaeng == 1 || temp_Ohaeng - batang_ohaeng == -4) text = "상관";
                else if (batang_ohaeng - temp_Ohaeng == 1 || batang_ohaeng - temp_Ohaeng == -4) text = "정인";
                else if (temp_Ohaeng - batang_ohaeng == 2 || temp_Ohaeng - batang_ohaeng == -3) text = "정재";
                else text = "정관";
            }
            return text;
        }

        public void SaJuYooksin(int[,] sjGanzi)
        {
            label73.Text = getGanYooksin(sjGanzi[2, 0], sjGanzi[3, 0]);
            label75.Text = getGanYooksin(sjGanzi[2, 0], sjGanzi[1, 0]);
            label76.Text = getGanYooksin(sjGanzi[2, 0], sjGanzi[0, 0]);
            label77.Text = getZiYooksin(sjGanzi[2, 0], sjGanzi[3, 1]);
            label60.Text = getZiYooksin(sjGanzi[2, 0], sjGanzi[2, 1]);
            label71.Text = getZiYooksin(sjGanzi[2, 0], sjGanzi[1, 1]);
            label72.Text = getZiYooksin(sjGanzi[2, 0], sjGanzi[0, 1]);

        }
        public int zi2Goong(int zi)
        {
            int goong = -1;
            if (zi == 1) goong = 1;         //자, 1수
            else if (zi == 2) goong = 10;   //축, 10토
            else if (zi == 3) goong = 3;    //인, 3목
            else if (zi == 4) goong = 8;    //묘, 8목
            else if (zi == 5) goong = 5;    //진, 5토
            else if (zi == 6) goong = 2;    //사, 2화
            else if (zi == 7) goong = 7;    //유, 7화
            else if (zi == 8) goong = 10;   //미, 10토
            else if (zi == 9) goong = 9;    //신, 9금
            else if (zi == 10) goong = 4;   //유, 4금
            else if (zi == 11) goong = 5;   //술, 5토
            else goong = 6;                //해, 6수

            return goong;
        }

        public void setJoSang(Goong[] goong, int[,] sjGanzi)
        {
            int jogaek, sangmun;
            int jogaek_hongNumber, sangmun_hongNumber;

            jogaek = sjGanzi[0, 1] - 2;
            if (jogaek < 0) jogaek = 12 + jogaek;
            sangmun = sjGanzi[0, 1] + 2;
            if (sangmun > 11) sangmun = sangmun - 12;

            jogaek_hongNumber = zi2Goong(jogaek); // 오행 받아오기
            sangmun_hongNumber = zi2Goong(sangmun); // 오행 받아오기

            for (int i = 0; i < 9; i++)
            {
                if (goong[i].hongNum[1] == jogaek_hongNumber)
                    goong[i].josang = "弔客";  // 조객
                else if (goong[i].hongNum[1] == sangmun_hongNumber)
                    goong[i].josang = "喪門";  // 상문
                else goong[i].josang = "";  // 
            }
        }


        private void CaptureScreen()
        {
            Graphics myGraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap(s.Width - 10, s.Height - 40, myGraphics);

            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(this.Location.X + 10, this.Location.Y + 30, 0, 0, s);
        }   //프린트용

        private void printDocument1_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(memoryImage, 0, 0);
        }   //프린트용

        private void button4_Click(object sender, EventArgs e)
        {
            CaptureScreen();
            printDocument1.DefaultPageSettings.Landscape = true;
            printDocument1.Print();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public Form4()
        {
            InitializeComponent();
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
        }

        public Form4(Form1 form)
        {
            InitializeComponent();
            f1 = form;
            dateTimePicker1.Value = f1.dateTimePicker1.Value;
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            radioButton8.Checked = true;
            if (f1.radioButton4.Checked) radioButton4.Checked = true;
            if (f1.radioButton5.Checked) radioButton5.Checked = true;
            textBox6.Text = f1.textBox6.Text;
            //label2.Text = "생일: " + ;
            //label14.Text = f1.label14.Text;
            //label56.Text = f1.label56.Text;
        }

        private void gimundungab_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++) goong[i] = new Goong();

            name = f1.textBox6.Text;
            if (f1.radioButton4.Checked) gender = 1; else gender = 0;

            try
            {
                //dt = DateTime.ParseExact(date, "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture); //  생년월일
                real_dt = dateTimePicker1.Value;
                dt = dateTimePicker1.Value;

                int age = dt.Year - f1.dateTimePicker1.Value.Year + 1;
                if (gender == 1)
                {
                    int[] HyearRR = { 7, 6, 1, 8, 3, 4, 9, 2 };
                    if (age == 1) Hyear = 9;
                    else Hyear = HyearRR[(age + 6) % 8];
                }
                else
                {
                    int[] HyearRR = { 7, 2, 9, 4, 3, 8, 1, 6 };
                    if (age == 1) Hyear = 1;
                    else Hyear = HyearRR[(age + 6) % 8];
                }
                if (f1.radioButton1.Checked)
                {
                    solar_dt = dt;
                    ToLunarDate(dt, out ly, out lunar_year, out lunar_month, out lunar_day); // 양력->음력 변환
                }
                else if (f1.radioButton2.Checked)
                {
                    //lunar_dt = dt;
                    ly = false;
                    solar_dt = ToSolarDate(dt, false); // 음력->양력 변환
                }
                else //if (radioButton3.Checked)
                {
                    //lunar_dt = dt;
                    ly = true;
                    solar_dt = ToSolarDate(dt, true); // 음력->양력 변환
                }

                //양력 표시
                if (f1.radioButton2.Checked || f1.radioButton3.Checked)
                {
                    label14.Text = "양력 " + solar_dt.Year + "년 " + solar_dt.Month + "월 " + solar_dt.Day + "일 " + dt.Hour.ToString() + ":" + dt.Minute.ToString();
                    //syear.Text = solar_dt.Year + " 년";
                    //smonth.Text = solar_dt.Month + " 월";
                    //sday.Text = solar_dt.Day + " 일";
                    //shour.Text = dt.Hour.ToString() + ":"+dt.Minute.ToString() ;
                }
                else
                {
                    if (ly == false)
                        label14.Text = "음력 " + lunar_year + "년 " + lunar_month.ToString("00") + "월 " + lunar_day.ToString("00") + "일 " + dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00");
                    else
                        label14.Text = "음력 " + lunar_year + "년 " + lunar_month.ToString("00") + "월(윤달) " + lunar_day.ToString("00") + "일 " + dt.Hour.ToString() + ":" + dt.Minute.ToString("00"); ;
                    //syear.Text = lunar_year + " 년";
                    //if (ly == false) smonth.Text = lunar_month + " 월";
                    //else smonth.Text = lunar_month + "월(윤달)";
                    //sday.Text = lunar_day + " 일";
                    //shour.Text = dt.Hour.ToString() + ":" + dt.Minute.ToString();
                }

                real_dt = dateAdjust(solar_dt);  // 서머타임 동경시 등 보정

                // 24절기 계산
                terms = get24Terms(real_dt);

                //양둔, 음둔 계산
                direction = getDirection(real_dt, terms);

                //년월일시 간지 계산 
                ToSajuYear(real_dt, terms[2], out sjGanzi[0, 0], out sjGanzi[0, 1]);
                ToSajuMonth(real_dt, terms, sjGanzi[0, 0], out sjGanzi[1, 0], out sjGanzi[1, 1]);
                ToSajuDay(real_dt, out sjGanzi[2, 0], out sjGanzi[2, 1]);
                ToSajuTime(real_dt, sjGanzi[2, 0], out sjGanzi[3, 0], out sjGanzi[3, 1]);

                //년월일시 간지 표시
                label12.Text = toGan(sjGanzi[0, 0]); label12.BackColor = getGancolor(sjGanzi[0, 0]);
                label18.Text = toZi(sjGanzi[0, 1]); label18.BackColor = getZicolor(sjGanzi[0, 1]);
                label11.Text = toGan(sjGanzi[1, 0]); label11.BackColor = getGancolor(sjGanzi[1, 0]);
                label17.Text = toZi(sjGanzi[1, 1]); label17.BackColor = getZicolor(sjGanzi[1, 1]);
                label10.Text = toGan(sjGanzi[2, 0]); label10.BackColor = getGancolor(sjGanzi[2, 0]);
                label16.Text = toZi(sjGanzi[2, 1]); label16.BackColor = getZicolor(sjGanzi[2, 1]);
                label9.Text = toGan(sjGanzi[3, 0]); label9.BackColor = getGancolor(sjGanzi[3, 0]);
                label15.Text = toZi(sjGanzi[3, 1]); label15.BackColor = getZicolor(sjGanzi[3, 1]);

                //홍국수
                setHongNum(goong, sjGanzi, out eunboksu1, out eunboksu2);

                //동처 계산
                setDongcheo(goong, sjGanzi);

                //유년계산
                setYooAge(goong, eunboksu1, eunboksu2);

                //육신 계산
                setSixSin(goong, sjGanzi);

                //흥국수 강약
                setHonglvl(goong, sjGanzi[1, 1], real_dt);

                //육의삼기 붙이기
                sisunsoo = setYookSam(goong, sjGanzi, real_dt, terms, direction);

                //조객 상문 붙이기
                setJoSang(goong, sjGanzi);

                //팔문 붙이기
                set8mun(goong, sjGanzi, direction);

                //팔괴 붙이기
                set8goe(goong);

                //구성 붙이기
                setGooSung(goong, sjGanzi, sisunsoo);

                //팔장 붙이기
                setEightjang(goong, sjGanzi, direction, sisunsoo);

                //천을, 천마 일록
                setCheonMaRok(goong, sjGanzi, direction);

                //12운성
                setEunsung(goong, sjGanzi);

                //공망
                setGongMang(goong, sjGanzi);

                //신살
                setSinsal(goong, sjGanzi);

                //격국
                setKyukkuk(goong, sjGanzi);

                //이사방위
                setIsabangui(goong, sjGanzi);

                //태을구성법
                setTaeulGusung(goong, sjGanzi, direction);

                //결과물 구궁에 출력
                showGoongLabelText_Sinsoo(goong, eunboksu1, eunboksu2);

                //통기도 출력
                if (radioButton8.Checked)
                    showTongGido(goong);
                else
                    showTongGido_1(goong);

                //사주팔자 육신
                SaJuYooksin(sjGanzi);

                bYoonyun = false;
                bTongi = false;
                bProcess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("년,월,일,시를 정확히 입력하세요");
            }

            Graphics g = pictureBox1.CreateGraphics();
            if (radioButton8.Checked)
            {
                g.FillEllipse(myBrush, 121, 103, 90, 90);
                g.DrawEllipse(Pens.Black, 121, 103, 90, 90);
            }
                
        } //신수운 (1번 버튼에서 유년 부분제거)




    }
}
