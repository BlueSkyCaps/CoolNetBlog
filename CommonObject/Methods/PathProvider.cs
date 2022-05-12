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

        /// <summary>
        /// 复制文件夹到一个位置，原文件夹不存在则忽略
        /// </summary>
        /// <param name="srcPath">要复制的文件夹路径，不存在则不执行复制操作</param>
        /// <param name="disPath">目标路径</param>
        public static void CopyDir(string srcPath, string disPath)
        {
            var srcDir = new DirectoryInfo(srcPath);
            var disDir = new DirectoryInfo(disPath);
            if (srcDir.Exists)
            {
                CopyDirCall(srcDir, disDir);
            }
        } 

        /// <summary>
        /// 复制文件夹到指定目录，核心递归方法
        /// </summary>
        /// <param name="srcDir">要复制的目录</param>
        /// <param name="disDir">目录的目标同名路径</param>
        private static void CopyDirCall(DirectoryInfo srcDir, DirectoryInfo disDir)
        {
            Directory.CreateDirectory(disDir.FullName);
            foreach (FileInfo fi in srcDir.GetFiles())
            {
                fi.CopyTo(Path.Combine(disDir.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo theSubD in srcDir.GetDirectories())
            {
                DirectoryInfo theSameDisSubDir = disDir.CreateSubdirectory(theSubD.Name);
                CopyDirCall(theSubD, theSameDisSubDir);
            }
        }
    }
}
