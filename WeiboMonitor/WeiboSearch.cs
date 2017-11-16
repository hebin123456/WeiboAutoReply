using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeiboMonitor
{
    class WeiboSearch
    {
        public WeiboSearch(string name)
        {
            GetOid(name);
        }

        public string Oid { get; set; }

        private string Get(string url)
        {
            return HttpHelper.Get(url, true);
        }

        private void GetOid(string name)
        {
            string html =  Get("https://s.weibo.com/weibo/" + name);
            
            if (html.IndexOf("div class=\\\"pl_noresult\\\"") >= 0) Oid = "";
            else
            {
                //微博正文内容 在下面这个字符串所在的那一行
                string searchStr = "\"pid\":\"pl_weibo_directtop\"";
                //按换行符分割HTML
                string[] line = html.Split('\n');
                int i;
                for (i = 0; i < line.Length; i++)
                {
                    if (line[i].IndexOf(searchStr) >= 0)
                    {
                        break;
                    }
                }
                
                //取出<script>标签内的Json数据
                string htmlStr = line[i].Replace("<script>STK && STK.pageletM && STK.pageletM.view(", "").Replace(")</script>", "");
                int a = htmlStr.IndexOf("\"html\":\"");
                htmlStr = htmlStr.Substring(a + 8, htmlStr.Length - a - 10).Replace(@"weibo.com\/u\/", @"weibo.com\/");
                
                Regex regex = new Regex(@"weibo\.com\\/[0-9a-zA-Z]+\?re");
                string str = regex.Match(htmlStr).ToString();
                
                Oid = str.Substring(11, str.Length - 14);
            }
        }
    }
}
