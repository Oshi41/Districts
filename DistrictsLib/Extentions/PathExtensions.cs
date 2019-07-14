using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsLib.Extentions
{
    public static class PathExtensions
    {
        /// <summary>
        /// Выясняет является ли переданный путь корневым и возвращает имя папки/файла.
        /// <para>Если путь не является корневым, возвращает null</para>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string TryGetRootedPath(this string path)
        {
            var list = GetPathsList(path);

            return list.Count == 1
                ? list.LastOrDefault()
                : null;
        }

        private static List<string> GetPathsList(string path)
        {
            var paths = new List<string>();

            while (!string.IsNullOrWhiteSpace(path))
            {
                var file = Path.GetFileName(path);

                if (!string.IsNullOrEmpty(file))
                {
                    paths.Add(file);
                }

                path = Path.GetDirectoryName(path);

                //var folderName = Path.GetFileName(Path.GetDirectoryName(path));
                //if (!string.IsNullOrEmpty(folderName))
                //{
                //    paths.Add(folderName);
                //}
            }

            return paths;
        }
    }
}
