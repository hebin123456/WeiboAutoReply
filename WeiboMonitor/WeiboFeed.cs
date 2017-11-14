using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;

namespace WeiboMonitor
{
    /// <summary>
    /// 用于Json解析
    /// </summary>
    public class ResponseJson
    {
        public string code { get; set; }
        public string msg { get; set; }
        public DataJson data { get; set; }

        public class DataJson
        {
            public int isDel { get; set; }
            public string html { get; set; }
        }
    }

    public class WeiboFeed
    {
        /// <summary>
        /// 微博ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 微博内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 点赞状态
        /// </summary>
        public bool IsLike { get; set; }

        public WeiboPage WBPage { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="content"></param>
        public WeiboFeed(WeiboPage fatherPage, string id, string username, string time, string content, bool isLike)
        {
            WBPage = fatherPage;
            ID = id;
            Username = username;
            Content = content;
            IsLike = isLike;

            Time = DateTime.Parse(time);
        }

        /// <summary>
        /// 点赞功能
        /// </summary>
        /// <param name="wbLogin"></param>
        /// <param name="yOrN">是点赞，还是取消赞</param>
        /// <returns></returns>
        public int Like(WeiboLogin wbLogin, bool yOrN)
        {
            if (IsLike == yOrN)
            {
                return -2;
            }

            try
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //这个是点赞的接口
                string url = "https://weibo.com/aj/v6/like/add?ajwvr=6&__rnd=" + (int)ts.TotalSeconds + "000";
                //这个是评论的接口
                //string url = "https://weibo.com/aj/v6/comment/add?ajwvr=6&__rnd=" + (int)ts.TotalSeconds + "000";
                
                //这个是点赞的提交字符串
                string postStr = "location=" + WBPage.Location + "&version=mini&qid=heart&mid=" + ID + "&loc=profile&cuslike=1&_t=0";
                //这个是评论的提交字符串,转发把forward改成1
                //string postStr = "act=post&mid=" + ID + "&uid=3166169725&forward=0&isroot=0&content=helloworld!&location=" + WBPage.Location + "&module=scommlist&group_source=&pdetail=1005056399610336&_t=0";
                string responseStr = HttpHelper.Post(url, WBPage.Url, wbLogin.MyCookies, postStr);

                //System.Windows.Forms.MessageBox.Show(url);
                //System.Windows.Forms.MessageBox.Show(postStr);
                //System.Windows.Forms.MessageBox.Show(responseStr);

                ResponseJson responseJson = JsonConvert.DeserializeObject<ResponseJson>(responseStr);
                return responseJson.data.isDel;
            }
            catch
            {
                return -1;
            }
        }

        private string urlencode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        public int Comment(WeiboLogin wbLogin, string comment, string Uid, bool forward)
        {
            try
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //这个是点赞的接口
                //string url = "https://weibo.com/aj/v6/like/add?ajwvr=6&__rnd=" + (int)ts.TotalSeconds + "000";
                //这个是评论的接口
                string url = "https://weibo.com/aj/v6/comment/add?ajwvr=6&__rnd=" + (int)ts.TotalSeconds + "000";

                //这个是点赞的提交字符串
                //string postStr = "location=" + WBPage.Location + "&version=mini&qid=heart&mid=" + ID + "&loc=profile&cuslike=1&_t=0";
                //这个是评论的提交字符串,转发把forward改成1
                string postStr = "act=post&mid=" + ID + "&uid=" + Uid + "&forward=" + (forward? 1 : 0) + "&isroot=0&content=" + urlencode(comment) + "&location=" + WBPage.Location + "&module=scommlist&group_source=&pdetail=1005056399610336&_t=0";
                string responseStr = HttpHelper.Post(url, WBPage.Url, wbLogin.MyCookies, postStr);

                //System.Windows.Forms.MessageBox.Show(url);
                //System.Windows.Forms.MessageBox.Show(postStr);
                //System.Windows.Forms.MessageBox.Show(responseStr);

                ResponseJson responseJson = JsonConvert.DeserializeObject<ResponseJson>(responseStr);
                return responseJson.data.isDel;
            }
            catch
            {
                return -1;
            }
        }
    }
}
