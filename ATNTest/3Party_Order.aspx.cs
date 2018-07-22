using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS2
{
    public partial class _3Party_Order : System.Web.UI.Page
    {
        public string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            "0123456789ABCDEF".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }


        private VPCRequest conn;

        public static string Version
        {
            get
            {
                // Return the Example Code Version
                return "MasterCard 2.0";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                conn = new VPCRequest();
                pnlError.Visible = false;
                pnlRequest.Visible = true;
                if (!String.IsNullOrEmpty(conn.MerchantID))
                {
                    Label_MerchantID.Text = conn.MerchantID;
                }
                else
                {
                    Label_MerchantID.Text = "Not configured";
                }
                if (!String.IsNullOrEmpty(conn.AccessCode))
                {
                    Label_AccessCode.Text = conn.AccessCode;
                }
                else
                {
                    Label_AccessCode.Text = "Not configured";
                }

                vpc_MerchTxnRef.Text = "ord_" + GetUniqueKey(5);
                vpc_OrderInfo.Text = "ordinfo_" + GetUniqueKey(5);
            }
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            pnlRequest.Visible = false;
            try
            {

                // Connect to the Payment Client
                VPCRequest conn = new VPCRequest();
                // Add the Digital Order Fields for the functionality you wish to use
                // Core Transaction Fields
                conn.AddDigitalOrderField("vpc_Version", conn.Version);
                conn.AddDigitalOrderField("vpc_Command", conn.Command);
                conn.AddDigitalOrderField("vpc_AccessCode", conn.AccessCode);
                conn.AddDigitalOrderField("vpc_Merchant", conn.MerchantID);
                conn.AddDigitalOrderField("vpc_ReturnURL", conn.FormatReturnURL(Request.Url.Scheme, Request.Url.Host, Request.Url.Port, Request.ApplicationPath));
                conn.AddDigitalOrderField("vpc_MerchTxnRef", vpc_MerchTxnRef.Text);
                conn.AddDigitalOrderField("vpc_OrderInfo", vpc_OrderInfo.Text);
                conn.AddDigitalOrderField("vpc_Amount", vpc_Amount.Text);
                conn.AddDigitalOrderField("vpc_Currency", Currency_List.Text);
                conn.AddDigitalOrderField("vpc_Locale", vpc_Locale.Text);
                // Perform the transaction
                String url = conn.Create3PartyQueryString();
                Page.Response.Redirect(url);

            }
            catch (Exception ex)
            {
                // Capture and Display the error information
                lblErrorMessage.Text = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "");
                pnlError.Visible = true;
                try
                {

                }
                catch (Exception ex2)
                {
                    // Do Nothing
                }
            }
        }
    }
}