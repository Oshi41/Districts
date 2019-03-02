using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.New.ViewModel;

namespace Districts.Parser.v2
{
    internal interface IDoorStatusParser
    {
        string Parse(IEnumerable<iDoor> doors, DoorStatus status);
        void Populate(string text, IEnumerable<iDoor> doors, DoorStatus status);
    }

    class DoorStatusParser : IDoorStatusParser
    {
        public string Parse(IEnumerable<iDoor> doors, DoorStatus status)
        {
            if (!doors.IsNullOrEmpty())
            {
                var numbers = doors.Where(x => x.Status == status)
                    .Select(x => x.DoorNumber)
                    .ToList();

                var result = "";

                for (int i = 0; i < numbers.Count(); i++)
                {
                    var startIndex = i;

                    while (i < numbers.Count() && numbers[i + 1] + 1 == numbers[i])
                    {
                        i++;
                    }

                    if (startIndex + 2 <= i)
                    {
                        result = string.Join(", ", result, $"{numbers[startIndex]}-{numbers[i]}");
                    }
                    else
                    {
                        var vals = new[] { startIndex, i }.Distinct();
                        result = string.Join(", ", result, vals);
                    }
                }

                return result;
            }

            return string.Empty;
        }

        public void Populate(string text, IEnumerable<iDoor> doors, DoorStatus status)
        {
            if (!doors.IsNullOrEmpty() && !string.IsNullOrWhiteSpace(text))
            {
                var splitted = text.Replace(" ", "").Split(',');

                foreach (var range in splitted)
                {
                    if (range.Contains("-"))
                    {
                        var splittedRange = range.Split('-');

                        int.TryParse(splittedRange.FirstOrDefault(), out var first);
                        int.TryParse(splittedRange.LastOrDefault(), out var last);

                        SetDoors(doors, status, Enumerable.Range(first, last - first - 1).ToArray());
                    }
                    else
                    {
                        int.TryParse(range, out var number);
                        SetDoors(doors, status, number);
                    }
                }
            }
        }

        private void SetDoors(IEnumerable<iDoor> doors, DoorStatus status, params int[] numbers)
        {
            foreach (var i in numbers)
            {
                if (doors.FirstOrDefault(x => x.DoorNumber == i) is DoorVm find)
                {
                    find.Status = status;
                }
            }
        }
    }
}
