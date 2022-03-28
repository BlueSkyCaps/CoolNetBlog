using System.Text;
using System.Text.RegularExpressions;

namespace CommonObject.Methods
{
    /// <summary>
    /// 公共计算方法类
    /// </summary>
    public class ValueCompute
    {
        /// <summary>
        /// 返回一个加密了的MD5字符串表现形式(32个字符长度)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MakeMD5(string str)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }

        }

        public static string NewSaveString(string input)
        {
            return input.Replace("<", "").Replace(">", "")
                .Replace("javascript:", "javascript：", true, null)
                .Replace("delete", "删除", true, null)
                .Replace("update", "更新", true, null)
                .Replace("insert", "插入", true, null)
                .Replace("show", "显示", true, null)
                .Replace("select", "查询", true, null)
                .Replace("drop", "删除", true, null)
                .Replace("alter", "修改", true, null);
        }

        public static bool CheckNotNullAndWhiteValue(object newO)
        {
            if (newO is null)
            {
                return false;
            }
            var type = newO.GetType();
            if (type.FullName.ToLower().EndsWith("string"))
            {
                if (string.IsNullOrWhiteSpace(newO.ToString()))
                {
                    return false;
                }
            }
            foreach (var p in type.GetProperties())
            {
                if (p != null)
                {
                    if (p.GetValue(newO) == null)
                    {
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(p.GetValue(newO)?.ToString()))
                    {
                        return false;

                    }
                }
            }
            return true;
        }
        public static void SetNotNullForObj(object newO)
        {
            try
            {
                var type = newO?.GetType();

                if (type != null)
                {
                    foreach (var p in type.GetProperties())
                    {
                        if (p.GetValue(newO) == null)
                        {
                            if (p.GetType().IsValueType)
                            {
                                newO = 0;
                            }
                            else
                            {
                                p.SetValue(newO, "");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static bool CheckHtmlLabelContains(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            Regex regex = new Regex(@"<\w+>", RegexOptions.IgnoreCase);
            var b = regex.IsMatch(input);
            if (b == false)
            {
                b = input.ToLower().Contains("javascript:");
            }
            return b;
        }

        /// <summary>
        /// 改成.Replace("<", "《").Replace(">", "》")效果一样
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceHtmlLabelContains(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            //改为中文冒号，避免"javascript:"字符
            input = input.Replace("javascript:", "javascript：", true, null);
            //匹配以<开头以>结尾，中间是任意字符包含空白的标签元素，且?让它尽可能少匹配，这样不会只匹配最外层的一个
            Regex regex = new Regex(@"<(.|\s)*?>", RegexOptions.IgnoreCase);
            var matcheds = regex.Matches(input);
            if (matcheds.Count>0)
            {
                foreach (Match mh in matcheds)
                {
                    var mhv = mh.Value;
                    string? filterInput = input.Replace(mhv, $"《{mhv.Trim('<').Trim('>')}》");
                    input = filterInput;
                }
            }
            return input;
        }

        /// <summary>
        /// 字符串是否是邮箱形式
        /// </summary>
        /// <returns></returns>
        public static bool IsEmail(string inpu)
        {
            Regex regex = new Regex(@"^[\w-]+([\.-]?\w+)*@[\w-]+([\.-]?\w+)*(\.\w{2,10})+$");
            return regex.IsMatch(inpu);
        }
    }
}
