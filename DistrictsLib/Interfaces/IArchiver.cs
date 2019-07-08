namespace DistrictsLib.Interfaces
{
    public interface IArchiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool TryToZip(string source, string zip);

        /// <summary>
        /// Разархивирует файл в указанное место
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        bool TryUnzip(string zip, string destination);
    }
}
