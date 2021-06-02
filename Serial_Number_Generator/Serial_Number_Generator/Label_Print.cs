using Seagull.BarTender.Print;
using Seagull.BarTender.Print.Database;

namespace Serial_Number_Generator
{
    internal class Label_Print
    {
        //SQL Table
        public const string SERVER = "PAS-SQL";

        public const string DATABASE = "PAS_Logs";

        public const string SQL_CONNECTION = "Server=" + SERVER +
                              ";Database=" + DATABASE +
                              ";Integrated Security=True;";

        private const string start_serial_letter = "P";

        private const string INSERT_SN_RANGE = "SP_Insert_SN_Range";

        private const string TRAVELER_PATH = @"\\pas-serv\Shared Docs\Production\TRAVELERS\CCA_TRAVELERS\SQL_CCA_Traveler\CCA_TRAVELER_FORM.btw";
        private const string TRAVELER_REPRINT_PATH = @"\\pas-serv\Shared Docs\Production\TRAVELERS\CCA_TRAVELERS\SQL_CCA_Traveler\CCA_TRAVELER_FORM_Reprint.btw";

        private string tin_lead = null;
        private string lead_free = null;
        private string halogen_free = null;
        private string no_clean = null;
        private string class_three = null;
        private string conformal_coated = null;
        private string Traveler_Path = null;

        public void print_traveler(bool reprint)
        {
            using (Engine btEngine = new Engine(true))

            {
                btEngine.Start();                               //Bob added 11-17-20, maybe not needed.

                if (SN_Data.Tin_Lead)
                {
                    tin_lead = "TIN LEAD";
                }
                else
                {
                    tin_lead = null;
                }

                if (SN_Data.Halogen_Free)
                {
                    halogen_free = "HALOGEN FREE";
                }
                else
                {
                    halogen_free = null;
                }

                if (SN_Data.No_Clean)
                {
                    no_clean = "NO CLEAN";
                }
                else
                {
                    no_clean = null;
                }

                if (SN_Data.Class_Three)
                {
                    class_three = "CLASS 3";
                }
                else
                {
                    class_three = null;
                }

                if (SN_Data.Lead_Free)
                {
                    lead_free = "LEAD FREE";
                }
                else
                {
                    lead_free = null;
                }

                if (SN_Data.Conformal_Coated)
                {
                    conformal_coated = "CONFORMAL COATED";
                }
                else
                {
                    conformal_coated = null;
                }

                if (reprint)
                {
                    Traveler_Path = TRAVELER_REPRINT_PATH;
                }
                else
                {
                    Traveler_Path = TRAVELER_PATH;
                }

                // Show BarTender UI for debugging purpose (Set "None" to hide; "All" to show)
                btEngine.Window.VisibleWindows = VisibleWindows.None;
                // Do something with the engine.
                LabelFormatDocument Label = btEngine.Documents.Open(Traveler_Path);

                if (Label.Status.ToString() == "Loaded")
                {
                    Label.Activate();
                    Label.Prompts.SetPrompt("inbox_revision", SN_Data.Rev);
                    Label.Prompts.SetPrompt("inbox_quantity", SN_Data.QTY.ToString());
                    Label.Prompts.SetPrompt("inbox_kitid", SN_Data.Kit_ID);
                    Label.Prompts.SetPrompt("inbox_customer", SN_Data.Customer);
                    Label.Prompts.SetPrompt("inbox_assembly", SN_Data.Assembly);
                    Label.Prompts.SetPrompt("inbox_PO", SN_Data.PO);

                    Label.Prompts.SetPrompt("inbox_PO", SN_Data.PO);
                    Label.Prompts.SetPrompt("check_halogen", halogen_free);
                    Label.Prompts.SetPrompt("check_noclean", no_clean);
                    Label.Prompts.SetPrompt("check_class3", class_three);
                    Label.Prompts.SetPrompt("check_noclean 2", tin_lead);
                    Label.Prompts.SetPrompt("check_ccoated", conformal_coated);
                    Label.Prompts.SetPrompt("check_leadfree", lead_free);

                    QueryPrompts queryprompts = Label.DatabaseConnections.QueryPrompts;

                    Label.PrintSetup.UseDatabase = true;

                    if (reprint)
                    {
                        queryprompts["Reprint"].Value = SN_Data.ID.ToString();
                    }
                    else
                    {
                        queryprompts["id"].Value = SN_Data.ID.ToString();
                    }

                    //Label.Print();
                }

                btEngine.Stop();
            }
        }
    }
}