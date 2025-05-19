using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FineUICore.EmptyProject.RazorForms.Pages
{
    public partial class IndexModel : BaseModel
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack){
                LoadData();
            }

        }

          #region LoadData

        // 框架页风格
        private string _framePageStyle = "f-dark-left";
        // 是否仅显示社区版示例
        private bool _showOnlyBase = false;
        // 显示模式
        private string _displayMode = "normal";
        // 主选项卡标签
        private string _mainTabs = "multi";
        // 语言
        private string _lang = "zh_CN";
        // 搜索文本
        private string _searchText = "";
        // 示例数
        private int _examplesCount = 0;

        private void LoadData()
        {
            string cookie = String.Empty;

            // 从Cookie中读取 - 框架页风格
            cookie = Request.Cookies["FramePageStyle"];
            if (cookie != null)
            {
                _framePageStyle = cookie;
            }

         

            // 从Cookie中读取 - 显示模式
            cookie = Request.Cookies["DisplayMode"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _displayMode = cookie;
            }

            // 从Cookie中读取 - 语言
            cookie = Request.Cookies["Language"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _lang = cookie;
            }

            // 从Cookie中读取 - 搜索文本
            cookie = Request.Cookies["SearchText"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _searchText = HttpUtility.UrlDecode(cookie);
            }

            // 从Cookie中读取 - 主选项卡标签
            cookie = Request.Cookies["MainTabs"];
            if (!String.IsNullOrEmpty(cookie))
            {
                _mainTabs = cookie;
            }


            // 初始化设置 - 框架页风格
            SetCheckedMenuItem(MenuFramePageStyle, _framePageStyle);

            // 初始化设置 - 显示模式
            SetCheckedMenuItem(MenuDisplayMode, _displayMode);

            // 初始化设置 - 主选项卡标签
            SetCheckedMenuItem(MenuMainTabs, _mainTabs);

            // 初始化设置 - 语言
            SetCheckedMenuItem(MenuLang, _lang);


   
       


            InitTreeMenu();

       
        }

        #endregion

        #region InitTreeMenu

        private void InitTreeMenu()
        {
            string xmlPath = FineUICore.PageContext.MapWebPath("~/res/menu.xml");

            string xmlContent = String.Empty;
            using (StreamReader sr = new StreamReader(xmlPath))
            {
                xmlContent = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);


            // 返回全部的叶子节点个数
            _examplesCount = ResolveXmlNodeList(treeMenu.Nodes, doc.DocumentElement.ChildNodes);

        }


        private int _nodeIndex = 0;

        private int ResolveXmlNodeList(IList<TreeNode> nodes, XmlNodeList xmlNodes)
        {
            // nodes 中渲染到页面上的节点个数
            int nodeVisibleCount = 0;

            foreach (XmlNode xmlNode in xmlNodes)
            {
                if (xmlNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                TreeNode node = new TreeNode();

                // 是否叶子节点
                bool isLeaf = xmlNode.ChildNodes.Count == 0;

                bool currentNodeIsVisible = true;

                string nodeText = "";
                bool nodeIsCorp = false;

                XmlAttribute textAttr = xmlNode.Attributes["Text"];
                if (textAttr != null)
                {
                    nodeText = SharedLocalizer.GetString(textAttr.Value);//_R(textAttr.Value);
                }

                // 是否企业版
                XmlAttribute isCorpAttr = xmlNode.Attributes["IsCorp"];
                if (isCorpAttr != null)
                {
                    nodeIsCorp = isCorpAttr.Value.ToLower() == "true";
                }


                int childVisibleCount = 0;
                if (isLeaf)
                {
                    // 仅显示社区版示例
                    if (_showOnlyBase && nodeIsCorp)
                    {
                        currentNodeIsVisible = false;
                    }

                    // 存在搜索文本
                    if (!String.IsNullOrEmpty(_searchText))
                    {
                        if (!nodeText.Contains(_searchText))
                        {
                            currentNodeIsVisible = false;
                        }
                    }
                }
                else
                {
                    // 递归
                    childVisibleCount = ResolveXmlNodeList(node.Nodes, xmlNode.ChildNodes);

                    nodeVisibleCount += childVisibleCount;

                    if (childVisibleCount == 0)
                    {
                        currentNodeIsVisible = false;
                    }
                    else
                    {
                        // 存在搜索文本
                        if (!String.IsNullOrEmpty(_searchText))
                        {
                            // 展开节点
                            node.Expanded = true;
                        }
                    }
					
					// 目录节点不可选择
                    node.Selectable = false;
                }

                if (currentNodeIsVisible)
                {
                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        string name = attribute.Name;
                        string value = attribute.Value;

                        if (name == "Text")
                        {
                            // Text需要特殊处理
                            if (isLeaf)
                            {
                                // 设置节点的提示信息
                                node.ToolTip = nodeText;
                            }

                            // 存在 IsCorp=True 属性，则改变 Text 的值
                            if (nodeIsCorp)
                            {
                                node.IconFont = IconFont._Pro;
                                //nodeText += "&nbsp;<span class=\"iscorp\">Corp.</span>";
                            }


                            if (childVisibleCount > 0)
                            {
                                //node.Attributes["data-child-count"] = childVisibleCount;
                                nodeText = String.Format("<raw>{0}<span class=\"menu-child-count\">{1}</span></raw>", nodeText, childVisibleCount);
                            }

                            node.Text = _R(nodeText);
                        }
                        else
                        {
                            node.SetPropertyValue(name, _R(value));
                        }
                    }

                    // 为每个节点分配一个ID
                    node.NodeID = String.Format("tn_{0}", _nodeIndex++);

                    nodes.Add(node);



                    // 示例数只计算叶子节点
                    if (isLeaf)
                    {
                        nodeVisibleCount++;
                    }

                }

            }

            return nodeVisibleCount;
        }

        #endregion

        #region SetCheckedMenuItem

        private void SetCheckedMenuItem(MenuButton menuButton, string checkedValue)
        {
            foreach (MenuItem item in menuButton.Menu.Items)
            {
                MenuCheckBox checkBox = (item as MenuCheckBox);
                if (checkBox != null)
                {
                    checkBox.Checked = checkBox.AttributeDataTag == checkedValue;
                }
            }
        }

        #endregion
    
    }
}