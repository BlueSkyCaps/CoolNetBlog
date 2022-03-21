namespace CommonObject.Methods
{
    public class PathProvider
    {
        /// <summary>
        /// 验证是否字符串是合格的Url地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool TryCreateUrl(string url) {
            Uri? uriResult;
            bool passUri = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                    && (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps);
            return passUri;
        }
    }
}
