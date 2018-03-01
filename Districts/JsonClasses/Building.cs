using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    /// Информация о здании
    /// </summary>
    public class Building : BaseFindableObject
    {
        /// <summary>
        /// Кол-во квартир
        /// </summary>
        public int Doors { get; set; }

        /// <summary>
        /// Кол-во этажей
        /// </summary>
        public int Floors { get; set; }

        /// <summary>
        /// Кол-во лифтов
        /// </summary>
        public int Elevators { get; set; }

        /// <summary>
        /// Кол-во подъездов
        /// </summary>
        public int Entrances { get; set; }

        /// <summary>
        /// Есть ли консьерж
        /// </summary>
        public bool HasConcierge { get; set; }

        /// <summary>
        /// ссылка на дом
        /// </summary>
        public string Uri { get; set; }

        public Building(BaseFindableObject obj) : base(obj)
        {
        }

        public Building(string street, string home) : base(street, home)
        {
            
        }

        public Building()
        {
            
        }
    }
}
