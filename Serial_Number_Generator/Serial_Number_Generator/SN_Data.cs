using System;

namespace Serial_Number_Generator
{
    public class SN_Data
    {
        public SN_Data()
        {
        }

        public void Set_Data(Guid id, DateTime date, string kitid, int qty, string sn_start,
            string sn_finish, int week, string rev, string customer, string assembly, string po,
            bool tin_lead, bool lead_free, bool halogen_free, bool no_clean, bool conformal_coated,
            bool class_three, string full_sn_finish, string full_sn_start, bool print_traveler, bool print_label,
            bool customer_sn, string prefix)
        {
            ID = id;
            Date = date;
            Kit_ID = kitid;
            QTY = qty;
            SN_Start = sn_start;
            SN_Finish = sn_finish;
            Week = week;
            Rev = rev;
            Customer = customer;
            Assembly = assembly;
            PO = po;
            Tin_Lead = tin_lead;
            Lead_Free = lead_free;
            Halogen_Free = halogen_free;
            No_Clean = no_clean;
            Conformal_Coated = conformal_coated;
            Class_Three = class_three;
            Full_SN_Finish = full_sn_finish;
            Full_SN_Start = full_sn_start;
            Print_Label = print_label;
            Print_Traveler = print_traveler;
            Customer_SN = customer_sn;
            Prefix = prefix;
        }

        public static bool Print_Traveler { get; set; }
        public static bool Print_Label { get; set; }
        public static DateTime Date { get; set; }
        public static Guid ID { get; set; }
        public static string Kit_ID { get; set; }
        public static int QTY { get; set; }
        public static string SN_Finish { get; set; }
        public static string SN_Start { get; set; }
        public static int Week { get; set; }
        public static string Rev { get; set; }
        public static string Customer { get; set; }
        public static string Assembly { get; set; }
        public static string PO { get; set; }
        public static bool Tin_Lead { get; set; }
        public static bool Lead_Free { get; set; }
        public static bool Halogen_Free { get; set; }
        public static bool Conformal_Coated { get; set; }
        public static bool Class_Three { get; set; }
        public static bool No_Clean { get; set; }
        public static string Full_SN_Finish { get; set; }
        public static string Full_SN_Start { get; set; }
        public static bool Customer_SN { get; set; }
        public static string Prefix { get; set; }
    }
}