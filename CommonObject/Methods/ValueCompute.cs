using System.Text;

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
    }
}
