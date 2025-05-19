using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FineUICore.EmptyProject.RazorForms
{
    public class BaseModel : PageModel
    {
        #region IsPostBack

        /// <summary>
        /// 是否页面回发
        /// </summary>
        public bool IsPostBack
        {
            get
            {
                return FineUICore.PageContext.IsFineUIAjaxPostBack();
            }
        }

        #endregion

        #region RegisterStartupScript

        /// <summary>
        /// 注册客户端脚本
        /// </summary>
        /// <param name="scripts"></param>
        public void RegisterStartupScript(string scripts)
        {
            FineUICore.PageContext.RegisterStartupScript(scripts);
        }

        #endregion

        #region ViewBag

        private DynamicViewData _viewBag;

        /// <summary>
        /// Add ViewBag to PageModel
        /// https://forums.asp.net/t/2128012.aspx?Razor+Pages+ViewBag+has+gone+
        /// https://github.com/aspnet/Mvc/issues/6754
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                if (_viewBag == null)
                {
                    _viewBag = new DynamicViewData(ViewData);
                }
                return _viewBag;
            }
        } 
        #endregion

        #region ShowNotify

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public virtual void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public virtual void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify();
            n.Target = target;
            n.Message = message;
            n.MessageBoxIcon = messageIcon;
            n.PositionX = Position.Center;
            n.PositionY = Position.Top;
            n.DisplayMilliseconds = 3000;
            n.ShowHeader = false;

            n.Show();
        }

        #endregion


        #region GetResource

        private IHtmlLocalizer _localizer;

        /// <summary>
        /// 页面模型的多语言资源
        /// </summary>
        public IHtmlLocalizer Localizer
        {
            get
            {
                if (_localizer == null)
                {
                    _localizer = FineUICore.PageContext.GetLocalizer(this.GetType());
                }
                return _localizer;
            }
        }


        /// <summary>
        /// 获取页面模型的多语言资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string GetResource(string name, params object[] arguments)
        {
            if (arguments.Length == 0)
            {
                return Localizer.GetString(name);
            }
            else
            {
                return Localizer.GetString(name, arguments);
            }
        }


        /// <summary>
        /// 获取页面模型的多语言资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string _R(string name, params object[] arguments)
        {
            return GetResource(name, arguments);
        }

        public string _L(string name, params object[] arguments)
        {
            return GetResource_L(name, arguments);
        }
          public string GetResource_L(string name, params object[] arguments)
        {
            if (arguments.Length == 0)
            {
                return SharedLocalizer.GetString(name);
            }
            else
            {
                return SharedLocalizer.GetString(name, arguments);
            }
        }

       
     private IHtmlLocalizer _sharedLocalizer;
       public IHtmlLocalizer SharedLocalizer
        {
            get
            {
                if (_sharedLocalizer == null)
                {
                    _sharedLocalizer = FineUICore.PageContext.GetLocalizer<SharedResources>();
                }
                return _sharedLocalizer;
            }
        }
         #endregion
         protected string HowManyRowsAreSelected(Grid grid)
        {
            StringBuilder sb = new StringBuilder();
            int selectedCount = grid.SelectedRowIndexArray.Length;
            if (selectedCount > 0)
            {
                sb.Append("<p><strong>"+ SharedLocalizer.GetString("GridSelectedRowsMessage", selectedCount) + "</strong></p>");
                //sb.AppendFormat("<p><strong>" + SharedLocalizer.GetString("GridSelectedRowsMessage") + "</strong></p>", selectedCount);

                sb.Append("<table class=\"result\">");

                sb.Append("<tr><th>"+ SharedLocalizer.GetString("GridRowNumber") + "</th>");
                foreach (string datakey in grid.DataKeyNames)
                {
                    sb.AppendFormat("<th>{0}</th>", datakey);
                }
                sb.Append("</tr>");


                for (int i = 0; i < selectedCount; i++)
                {
                    int rowIndex = grid.SelectedRowIndexArray[i];
                    sb.Append("<tr>");

                    int rownumber = rowIndex + 1;
                    if (grid.AllowPaging)
                    {
                        rownumber += grid.PageIndex * grid.PageSize;
                    }
                    sb.AppendFormat("<td>{0}</td>", rownumber);


                    // 如果是内存分页，所有分页的数据都存在，rowIndex 就是在全部数据中的顺序，而不是当前页的顺序
                    if (grid.AllowPaging && !grid.IsDatabasePaging)
                    {
                        rowIndex = grid.PageIndex * grid.PageSize + rowIndex;
                    }

                    object[] dataKeys = grid.DataKeys[rowIndex];
                    for (int j = 0; j < dataKeys.Length; j++)
                    {
                        sb.AppendFormat("<td>{0}</td>", dataKeys[j]);
                    }

                    sb.Append("</tr>");
                }
                sb.Append("</table>");
            }
            else
            {
                sb.Append("<strong>"+ SharedLocalizer.GetString("GridNoSelectionMessage") + "</strong>");
            }

            return sb.ToString();
        }

        

    }
}