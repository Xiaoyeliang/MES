using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using dotNetCoreDll;




namespace FineUICore.EmptyProject.RazorForms.Pages.OPBasic
{
    public partial class OpSettingModel : BaseModel
    {
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AutoBindGrid();

              //  btnCheckSelection.OnClientClick = Grid1.GetNoSelectionAlertReference("请至少选择一项！");
              // btnPopupWindow.OnClientClick = Window1.GetShowReference(Url.Content("~/Grid/IFrameWindow")) + "return false;";

              //  btnConfirmButton.OnClientClick = Grid1.GetNoSelectionAlertReference("请至少选择一项！");
                btnDelete.ConfirmText = String.Format("<raw>你确定要删除选中的&nbsp;<b><script>{0}</script></b>&nbsp;项吗？</raw>", OPGrid.GetSelectedCountReference());

            }

            Panel7.Title = "表格 - 页面加载时间：" + DateTime.Now.ToLongTimeString();
        }

        #region BindGrid

        private void AutoBindGrid()
        {
            var sourceKey = OPGrid.Attributes.Value<string>("data-source-key");
            if (String.IsNullOrEmpty(sourceKey) || sourceKey == "table2")
            {
                BindGrid();
                sourceKey = "table1";
            }
            else
            {
                BindGrid();
                sourceKey = "table2";
            }

            OPGrid.Attributes["data-source-key"] = sourceKey;
        }

        private void BindGrid()
        {
            string itemStr=string.Empty;
            if(dllApproval.SelectedValue == "Approval")
            {
               itemStr=" and issuestate='0'";
            }
            else if(dllApproval.SelectedValue == "Approvaling")
            {
                itemStr=" and issuestate='1'";
            }else
            {
               itemStr=" and issuestate='2'";
            }

            if(ttbSearch.Text!=string.Empty)
            {
                itemStr += string.Format(" and (opname like '%{0}%' or opno like '%{0}%')", ttbSearch.Text);
            }
            
           // DataTable table = new DBUtil().SqlQuery(string.Format(@"select * from TBLOPBASIS where 2>1 {0}",item),"MESDB");
           var dotNetCoreDll = new dotNetCoreDll.dotNetCoreDll();
           DataTable dt = dotNetCoreDll.SqlQuery(string.Format(@"select * from TBLOPBASIS where 2>1 {0} and rownum <=2000",itemStr),"MESDB");
          // dotNetCoreDll.SendEmail("Core 測試 ",table,"測試","測試2",true);
            OPGrid.DataSource = dt;
            OPGrid.DataBind();
        }

        private void BindGrid2()
        {
            
        }

        #endregion

        protected void dllApproval_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Grid1_Sort(object sender, GridSortEventArgs e)
        {
            ShowNotify(e.SortField);
        }


        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            AutoBindGrid();
        }


        protected void ttbSearch_Trigger1Click(object sender, EventArgs e)
        {
            AutoBindGrid();

            ttbSearch.Text = String.Empty;
            ttbSearch.ShowTrigger1 = false;

        }

        protected void ttbSearch_Trigger2Click(object sender, EventArgs e)
        {
            AutoBindGrid();

            ttbSearch.ShowTrigger1 = true;
        }

        protected void btnApproval_Click(object sender, EventArgs e)
        {
           string itemStr=string.Empty;
            if(dllApproval.SelectedValue == "Approval")
            {
               itemStr=" and issuestate='0'";
            }
            else if(dllApproval.SelectedValue == "Approvaling")
            {
                itemStr=" and issuestate='1'";
            }else
            {
               itemStr=" and issuestate='2'";
            }

            if(ttbSearch.Text!=string.Empty)
            {
                itemStr += string.Format(" and (opname like '%{0}%' or opno like '%{0}%')", ttbSearch.Text);
            }
            
           // DataTable table = new DBUtil().SqlQuery(string.Format(@"select * from TBLOPBASIS where 2>1 {0}",item),"MESDB");
           var dotNetCoreDll = new dotNetCoreDll.dotNetCoreDll();
           DataTable dt = dotNetCoreDll.SqlQuery(string.Format(@"select * from TBLOPBASIS where 2>1 {0} and rownum <=2000",itemStr),"MESDB");
           dotNetCoreDll.SendEmail("Core 測試 ",dt,"測試","appsettings.json",true);
             
        }

    }
}