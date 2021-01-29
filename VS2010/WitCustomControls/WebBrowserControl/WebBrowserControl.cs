using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections.Specialized;

namespace CodePlex.WitCustomControls.Web
{
    public partial class WebBrowserControl : WitCustomControlBase
    {
        #region Constructors 

        public WebBrowserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region IWorkItemControl Overrides

        public override void InvalidateDatasource()
        {
            UpdateBrowser();
        }

        #endregion

        #region Private Members

        private void UpdateBrowser()
        {
            if (_properties != null)
            {
                string strURL = buildUrlFromProperty(_properties);

                if (strURL != "")
                    MainWebBrowser.Navigate(strURL);
            }
        }

        private string buildUrlFromProperty(StringDictionary propertyDictionary)
        {
            if (propertyDictionary.ContainsKey("URL"))
            {
                string strURL = propertyDictionary["URL"].ToString();
                string strParams;

                if (propertyDictionary.ContainsKey("Params"))
                {
                    strParams = propertyDictionary["Params"].ToString();
                }
                else
                {
                    //No Params provided, assume none needed and just return the URL
                    return strURL.ToString();
                }

                List<string> paramArray = new List<string>();
                try
                {
                    foreach (string urlParam in strParams.Split(",".ToCharArray()))
                    {
                        string fieldValue = "";

                        if (_workItem.Fields[urlParam].Value == null)
                            fieldValue = "";
                        else
                            fieldValue = _workItem.Fields[urlParam].Value.ToString();

                        paramArray.Add(fieldValue);
                    }
                    return string.Format(strURL, paramArray.ToArray());
                }
                catch (Exception e)
                {
                    showError(e.Message);
                    return "";
                }
            }
            else
            {
                showError("URL property not found in WIT definition, please contact your administrator.");
                return "";
            }
        }

        private void showError(string errorText)
        {
            StringBuilder strHTML = new StringBuilder();

            strHTML.Append("<HTML><BODY bgcolor=#FF867F><font color=black>");
            strHTML.Append(errorText);
            strHTML.Append("</font></BODY></HTML>");

            MainWebBrowser.DocumentText = strHTML.ToString();
        }

        #endregion

    }
}
